using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TestEcopDesktop.Clases;

namespace TestEcopDesktop
{
    /// <summary>
    /// Lógica de interacción para PedidoPage.xaml
    /// </summary>
    public partial class PedidoPage : Page
    {
        private ObservableCollection<Producto> productosEnGrilla;
        private float precioTotal;
        private string connectionString = ConexionDB.ConnectionString;
        public PedidoPage()
        {
            InitializeComponent();
            productosEnGrilla = new ObservableCollection<Producto>();
            dataGridPedido.ItemsSource = productosEnGrilla;
            LoadClientes();
            LoadProductos();
        }

        private void LoadClientes()
        {
            ObservableCollection<Cliente> clientes = new ObservableCollection<Cliente>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, nombre + ' ' + apellido AS nombre FROM [clientes]";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        clientes.Add(new Cliente
                        {
                            Id = reader.GetInt32(0),
                            Nombre = reader.GetString(1)
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los clientes: " + ex.Message);
                }
            }

            cbCliente.ItemsSource = clientes;
            cbCliente.DisplayMemberPath = "Nombre";
            cbCliente.SelectedValuePath = "Id";
        }

        private void LoadProductos()
        {
            ObservableCollection<Producto> productos = new ObservableCollection<Producto>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "SELECT id, descripcion, precio,codigo FROM [productos]";
                SqlCommand command = new SqlCommand(query, connection);

                try
                {
                    connection.Open();
                    SqlDataReader reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        productos.Add(new Producto
                        {
                            Id = reader.GetInt32(0),
                            Descripcion = reader.GetString(1),
                            Precio = (float)reader.GetDouble(2),
                            Codigo = reader.GetInt32(3)
                        });
                    }
                    reader.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al cargar los productos: " + ex.Message);
                }
            }

            cbProducto.ItemsSource = productos;
            cbProducto.DisplayMemberPath = "Descripcion";
            cbProducto.SelectedValuePath = "Id";
        }

        private void Agregar_Click(object sender, RoutedEventArgs e)
        {
            // Obtiene el producto seleccionado
            Producto producto = cbProducto.SelectedItem as Producto;
            if (producto != null)
            {
                productosEnGrilla.Add(producto);
                precioTotal += producto.Precio;
                lblPrecioTotal.Content = precioTotal.ToString();
                cbCliente.IsEnabled = false;
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is int codigo)
            {
                Producto producto = productosEnGrilla.FirstOrDefault(p => p.Codigo == codigo);
                if (producto != null)
                {
                    productosEnGrilla.Remove(producto);
                    precioTotal -= producto.Precio;
                    lblPrecioTotal.Content = precioTotal.ToString();
                }
            }
        }

        public int GuardarPedido(float precioTotal, int cantidad, ObservableCollection<Producto> productosEnGrilla, Cliente cliente)
        {
            int nro = 0;
            int idPedido = 0;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                SqlTransaction transaction = connection.BeginTransaction();

                try
                {
                    // Paso 1: Insertar el pedido y obtener el ID generado
                    string queryPedido = "INSERT INTO pedidos (preciototal, cantidad, fecha) OUTPUT INSERTED.id VALUES (@preciototal, @cantidad, GETDATE())";

                    using (SqlCommand cmdPedido = new SqlCommand(queryPedido, connection, transaction))
                    {
                        cmdPedido.Parameters.AddWithValue("@preciototal", precioTotal);
                        cmdPedido.Parameters.AddWithValue("@cantidad", cantidad);

                        idPedido = (int)cmdPedido.ExecuteScalar();
                    }

                    // Paso 2: Insertar los detalles del pedido
                    foreach (var producto in productosEnGrilla)
                    {
                        string queryDetalle = "INSERT INTO detallepedido (id_pedido, id_producto, id_cliente, precio, preciototal) VALUES (@id_pedido, @id_producto, @id_cliente, @precio, @preciototal)";

                        using (SqlCommand cmdDetalle = new SqlCommand(queryDetalle, connection, transaction))
                        {
                            cmdDetalle.Parameters.AddWithValue("@id_pedido", idPedido);
                            cmdDetalle.Parameters.AddWithValue("@id_producto", producto.Id);
                            cmdDetalle.Parameters.AddWithValue("@id_cliente", cliente.Id);
                            cmdDetalle.Parameters.AddWithValue("@precio", producto.Precio);
                            cmdDetalle.Parameters.AddWithValue("@preciototal", precioTotal);

                            cmdDetalle.ExecuteNonQuery();
                        }
                    }

                    // Si todo es correcto, confirma la transacción
                    transaction.Commit();
                    nro = 1; // Indica éxito
                }
                catch (Exception ex)
                {
                    // Si hay algún error, revierte la transacción
                    transaction.Rollback();
                    Console.WriteLine("Error al insertar el pedido y sus detalles: " + ex.Message);
                    nro = -1; // Indica fallo
                }
            }

            return nro;
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            Cliente clienteSeleccionado = cbCliente.SelectedItem as Cliente;
            if (clienteSeleccionado != null && productosEnGrilla.Count > 0)
            {
                int resultado = GuardarPedido(precioTotal, productosEnGrilla.Count, productosEnGrilla, clienteSeleccionado);

                if (resultado > 0)
                {
                    MessageBox.Show("Pedido guardado correctamente.");
                    cbCliente.IsEnabled = true;
                    cbCliente.Text = "";
                    cbProducto.Text = "";
                    productosEnGrilla.Clear();

                }
                else
                {
                    MessageBox.Show("Error al guardar el pedido.");
                }
            }
            else
            {
                MessageBox.Show("Seleccione un cliente y agregue al menos un producto al pedido.");
            }
        }
    }
}
