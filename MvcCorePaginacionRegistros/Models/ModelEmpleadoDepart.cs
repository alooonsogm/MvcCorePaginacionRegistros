namespace MvcCorePaginacionRegistros.Models
{
    public class ModelEmpleadoDepart
    {
        public List<Empleado> Empleados { get; set; }
        public int NumRegistros { get; set; }
        public Departamento Departamento { get; set; }
    }
}
