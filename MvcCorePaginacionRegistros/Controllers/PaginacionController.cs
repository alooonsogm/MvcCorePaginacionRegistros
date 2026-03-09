using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.Controllers
{
    public class PaginacionController : Controller
    {
        private RepositoryHospital repo;

        public PaginacionController(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IActionResult> RegistroVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepatamentoAsync();

            int siguiente = posicion.Value + 1;
            if(siguiente > numeroRegistros)
            {
                siguiente = numeroRegistros;
            }

            int anterior = posicion.Value - 1;
            if(anterior < 1)
            {
                anterior = 1;
            }

            ViewData["ULTIMO"] = numeroRegistros;
            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;

            VistaDepartamento depart = await this.repo.GetVistaDepartamentoAsync(posicion.Value);
            return View(depart);
        }

        public async Task<IActionResult> GrupoVistaDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepatamentoAsync();
            ViewData["REGISTROS"] = numeroRegistros;

            List<VistaDepartamento> departs = await this.repo.GetGrupoDepartamentoAsync(posicion.Value);
            return View(departs);
        }

        public async Task<IActionResult> GrupoDepartamento(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepatamentoAsync();
            ViewData["REGISTROS"] = numeroRegistros;

            List<Departamento> departs = await this.repo.GetGrupoDepartamentoProcedureAsync(posicion.Value);
            return View(departs);
        }

        public async Task<IActionResult> GrupoEmpleado(int? posicion)
        {
            if(posicion == null)
            {
                posicion = 1;
            }

            int numeroRegistros = await this.repo.GetNumeroRegistrosEmpleadosAsync();
            ViewData["REGISTROS"] = numeroRegistros;

            List<Empleado> emp = await this.repo.GetGrupoEmpleadosProcedureAsync(posicion.Value);
            return View(emp);
        }

        public async Task<IActionResult> GrupoEmpleadoOficio(int? posicion, string oficio)
        {
            if(posicion == null)
            {
                posicion = 1;
                return View();
            }
            else
            {
                ModelEmpleadoOficio model = await this.repo.GetGrupoEmpleadosProcedureOficioAsync(posicion.Value, oficio);
                ViewData["REGISTROS"] = model.NumRegistros;
                ViewData["OFICIO"] = oficio;
                return View(model.Empleados);
            }
        }

        [HttpPost]
        public async Task<IActionResult> GrupoEmpleadoOficio(string oficio)
        {
            ModelEmpleadoOficio model = await this.repo.GetGrupoEmpleadosProcedureOficioAsync(1, oficio);
            ViewData["REGISTROS"] = model.NumRegistros;
            ViewData["OFICIO"] = oficio;
            return View(model.Empleados);
        }

        public async Task<IActionResult> Details(int idDepart, int? posicion)
        {
            if (posicion == null)
            {
                posicion = 1;
            }

            ModelEmpleadoDepart model = await this.repo.GetEmpleadosProcedureDEPTAsync(posicion.Value, idDepart);

            int siguiente = posicion.Value + 1;
            if (siguiente > model.NumRegistros)
            {
                siguiente = model.NumRegistros;
            }

            int anterior = posicion.Value - 1;
            if (anterior < 1)
            {
                anterior = 1;
            }

            ViewData["SIGUIENTE"] = siguiente;
            ViewData["ANTERIOR"] = anterior;
            ViewData["POSICION"] = posicion;
            return View(model);
        }
    }
}
