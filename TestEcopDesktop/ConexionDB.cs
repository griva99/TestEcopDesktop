using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestEcopDesktop
{
    public static class ConexionDB
    {
        //Data Source=Nombre de la conexion;Initial Catalog= base de datos;Integrated Security=True
        public static string ConnectionString { get; } = "Data Source=LAPTOP-OP2NKJ4L\\SQLEXPRESS;Initial Catalog=test;Integrated Security=True";

    }
}
