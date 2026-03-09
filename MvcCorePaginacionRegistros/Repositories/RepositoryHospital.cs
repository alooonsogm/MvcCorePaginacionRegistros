using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;
using System.Data;
using System.Diagnostics.Metrics;

#region vista/procedure
//create VIEW V_DEPARTAMENTOS_INDIVIDUAL
//AS
//SELECT cast(ROW_NUMBER() OVER (ORDER BY DEPT_NO) as int) AS POSICION, DEPT_NO, DNOMBRE, LOC FROM DEPT
//GO

//create procedure SP_GRUPO_DEPT(@posicion int)
//as
//	select DEPT_NO, DNOMBRE, LOC FROM V_DEPARTAMENTOS_INDIVIDUAL
//	WHERE POSICION >= @posicion and POSICION < (@posicion + 2)
//go

//exec SP_GRUPO_DEPT 1

//create VIEW V_EMPLEADOS_INDIVIDUAL
//AS
//SELECT cast(ROW_NUMBER() OVER (ORDER BY APELLIDO) as int) AS POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//GO

//create procedure SP_GRUPO_EMP(@posicion int)
//as
//	select EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM V_EMPLEADOS_INDIVIDUAL
//	WHERE POSICION >= @posicion and POSICION < (@posicion + 3)
//go

//exec SP_GRUPO_EMP 1

//CREATE PROCEDURE SP_GRUPO_EMPLEADOS_OFICIO(@posicion int, @oficio nvarchar(50), @registros int out)
//AS
//select @registros = count(EMP_NO) FROM EMP WHERE OFICIO = @oficio
//SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
//(select cast(ROW_NUMBER() OVER (ORDER BY APELLIDO) as int) AS POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//WHERE OFICIO = @oficio) QUERY
//WHERE (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 3))
//GO

//DECLARE @registros int;
//exec SP_GRUPO_EMPLEADOS_OFICIO 1, 'EMPLEADO', @registros OUTPUT
//select @registros AS NumRegistros

//CREATE PROCEDURE SP_EMPLEADOS_DEPT(@posicion int, @dept_no int, @registros int out)
//AS
//select @registros = count(EMP_NO) FROM EMP WHERE DEPT_NO = @dept_no
//SELECT EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM
//(select cast(ROW_NUMBER() OVER (ORDER BY APELLIDO) as int) AS POSICION, EMP_NO, APELLIDO, OFICIO, SALARIO, DEPT_NO FROM EMP
//WHERE DEPT_NO = @dept_no) QUERY
//WHERE (QUERY.POSICION >= @posicion and QUERY.POSICION < (@posicion + 1))
//GO

//DECLARE @registros int;
//exec SP_EMPLEADOS_DEPT 3, 10, @registros OUTPUT
//select @registros AS NumRegistros
#endregion

namespace MvcCorePaginacionRegistros.Repositories
{
    public class RepositoryHospital
    {
        private HospitalContext context;

        public RepositoryHospital(HospitalContext context)
        {
            this.context = context;
        }

        public async Task<int> GetNumeroRegistrosVistaDepatamentoAsync()
        {
            return await this.context.VistaDepartamento.CountAsync();
        }

        public async Task<VistaDepartamento> GetVistaDepartamentoAsync(int posicion)
        {
            VistaDepartamento dept = await this.context.VistaDepartamento.Where(z => z.Posicion == posicion).FirstOrDefaultAsync();
            return dept;
        }

        public async Task<List<VistaDepartamento>> GetGrupoDepartamentoAsync(int posicion)
        {
            var consulta = from datos in this.context.VistaDepartamento
                           where datos.Posicion >= posicion && datos.Posicion < (posicion + 2)
                           select datos;

            return await consulta.ToListAsync();
        }

        public async Task<List<Departamento>> GetGrupoDepartamentoProcedureAsync(int posicion)
        {
            string sql = "SP_GRUPO_DEPT @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Departamentos.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumeroRegistrosEmpleadosAsync()
        {
            return await this.context.Empleados.CountAsync();
        }

        public async Task<List<Empleado>> GetGrupoEmpleadosProcedureAsync(int posicion)
        {
            string sql = "SP_GRUPO_EMP @posicion";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion);
            return await consulta.ToListAsync();
        }

        public async Task<int> GetNumeroRegistrosEmpleadosOficioAsync(string oficio)
        {
            return await this.context.Empleados.Where(z => z.Oficio == oficio).CountAsync();
        }

        public async Task<ModelEmpleadoOficio> GetGrupoEmpleadosProcedureOficioAsync(int posicion, string oficio)
        {
            string sql = "SP_GRUPO_EMPLEADOS_OFICIO @posicion, @oficio, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamOficio = new SqlParameter("@oficio", oficio);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.DbType = DbType.Int32;
            pamRegistros.Direction = ParameterDirection.Output;

            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamOficio, pamRegistros);
            List<Empleado> emp = await consulta.ToListAsync();
            int resgitros = (int)pamRegistros.Value;
            ModelEmpleadoOficio model = new ModelEmpleadoOficio();
            model.Empleados = emp;
            model.NumRegistros = resgitros;
            return model;
        }

        public async Task<List<Departamento>> GetDepartamentosAsync()
        {
            var consulta = from datos in this.context.Departamentos select datos;
            return await consulta.ToListAsync();
        }

        public async Task<Departamento> FindDepartamentoAsync(int idDepart)
        {
            var consulta = from datos in this.context.Departamentos where datos.IdDepartamento == idDepart select datos;
            return await consulta.FirstOrDefaultAsync();
        }

        public async Task<ModelEmpleadoDepart> GetEmpleadosProcedureDEPTAsync(int posicion, int idDepart)
        {
            string sql = "SP_EMPLEADOS_DEPT @posicion, @dept_no, @registros out";
            SqlParameter pamPosicion = new SqlParameter("@posicion", posicion);
            SqlParameter pamIdDepart = new SqlParameter("@dept_no", idDepart);
            SqlParameter pamRegistros = new SqlParameter("@registros", 0);
            pamRegistros.DbType = DbType.Int32;
            pamRegistros.Direction = ParameterDirection.Output;

            Departamento dept = await FindDepartamentoAsync(idDepart);
            var consulta = this.context.Empleados.FromSqlRaw(sql, pamPosicion, pamIdDepart, pamRegistros);
            List<Empleado> emp = await consulta.ToListAsync();
            int resgitros = (int)pamRegistros.Value;
            ModelEmpleadoDepart model = new ModelEmpleadoDepart();
            model.Empleados = emp;
            model.NumRegistros = resgitros;
            model.Departamento = dept;
            return model;
        }
    }
}
