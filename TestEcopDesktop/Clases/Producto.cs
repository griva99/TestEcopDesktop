using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEcopDesktop.Clases
{
    public class Producto
    {
        public int Id { get; set; }
        public int Codigo { get; set; }
        public string Descripcion { get; set; }
        public int IdUnd { get; set; }
        public float Precio { get; set; }
        public string DesUnd { get; set; }
    }
}
