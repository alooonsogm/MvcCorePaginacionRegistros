using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MvcCorePaginacionRegistros.Data;
using MvcCorePaginacionRegistros.Models;

#region vista
//create VIEW V_DEPARTAMENTOS_INDIVIDUAL
//AS
//SELECT cast(ROW_NUMBER() OVER (ORDER BY DEPT_NO) as int) AS POSICION, DEPT_NO, DNOMBRE, LOC FROM DEPT
//GO
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
    }
}
