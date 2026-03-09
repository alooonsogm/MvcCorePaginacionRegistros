using Microsoft.AspNetCore.Mvc;
using MvcCorePaginacionRegistros.Models;
using MvcCorePaginacionRegistros.Repositories;

namespace MvcCorePaginacionRegistros.ViewComponents
{
    public class MenuDepartViewComponent: ViewComponent
    {
        private RepositoryHospital repo;

        public MenuDepartViewComponent(RepositoryHospital repo)
        {
            this.repo = repo;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<Departamento> departs = await this.repo.GetDepartamentosAsync();
            return View(departs);
        }
    }
}
