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

            int numeroPagina = 1;
            int numeroRegistros = await this.repo.GetNumeroRegistrosVistaDepatamentoAsync();

            ViewData["REGISTROS"] = numeroRegistros;
            ViewData["NUMEROPAGINA"] = numeroPagina;

            List<VistaDepartamento> departs = await this.repo.GetGrupoDepartamentoAsync(posicion.Value);
            return View(departs);
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
