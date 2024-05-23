using System;
using System.Collections.Generic;
using System.Data.Common;
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
using System.Data;
using System.Data.SqlClient;


namespace TestEcopDesktop
{
    /// <summary>
    /// Lógica de interacción para ProductoPage.xaml
    /// </summary>
    public partial class ProductoPage : Page
    {
        private string connectionString = ConexionDB.ConnectionString;
        private List<Producto> productosEnGrilla;
        Producto producto1 = new Producto();
        public ProductoPage()
        {
            InitializeComponent();
            btnActualizar.IsEnabled = false;
            LoadDataGrid();
        }
        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar y obtener valores del formulario
                int codigo = int.Parse(txtCodigo.Text);
                string descripcion = txtDescripcion.Text;
                string tipoUnd = (cbIdUnd.SelectedItem as ComboBoxItem).Content.ToString();
                int idUnd = 0;
                switch (tipoUnd)
                {
                    case "KILO":
                        idUnd = 1;
                        break;
                    case "LITRO":
                        idUnd = 2;
                        break;
                    case "UNIDAD":
                        idUnd = 3;
                        break;
                    default:
                        break;
                }
                float precio = float.Parse(txtPrecio.Text);

                // Insertar el producto en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO [productos] (codigo, descripcion, id_und, precio) VALUES (@codigo, @descripcion, @id_und, @precio)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@id_und", idUnd);
                    cmd.Parameters.AddWithValue("@precio", precio);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Producto guardado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCodigo.Text = "";
                txtDescripcion.Text = "";
                txtPrecio.Text = "";
                cbIdUnd.Text = "";
                // Recargar la grilla
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al guardar el producto: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Ver_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = (int)btn.Tag;
            producto1 = ((List<Producto>)dataGridProductos.ItemsSource).FirstOrDefault(p => p.Id == id);

            if (producto1 != null)
            {
                txtCodigo.Text = producto1.Codigo.ToString();
                txtDescripcion.Text = producto1.Descripcion;
                cbIdUnd.Text = producto1.DesUnd.ToString();
                txtPrecio.Text = producto1.Precio.ToString();
                btnGuardar.IsEnabled = false;
                btnActualizar.IsEnabled = true;
            }
        }

        private void Eliminar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Button btn = sender as Button;
                int id = (int)btn.Tag;
                EliminarProducto(id);

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Producto eliminado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al eliminar el producto: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarProducto(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [productos] WHERE id = @id";
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@id", id);
                    connection.Open();
                    command.ExecuteNonQuery();
                }
            }
        }
        private void Actualizar_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Validar y obtener valores del formulario
                int codigo = int.Parse(txtCodigo.Text);
                string descripcion = txtDescripcion.Text;
                string tipoUnd = (cbIdUnd.SelectedItem as ComboBoxItem).Content.ToString();
                int idUnd = 0;
                switch (tipoUnd)
                {
                    case "KILO":
                        idUnd = 1;
                        break;
                    case "LITRO":
                        idUnd = 2;
                        break;
                    case "UNIDAD":
                        idUnd = 3;
                        break;
                    default:
                        break;
                }
                float precio = float.Parse(txtPrecio.Text);

                // Actualizar el producto en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE [productos] SET codigo=@codigo, descripcion = @descripcion, id_und = @id_und, precio = @precio WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", producto1.Id);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@descripcion", descripcion);
                    cmd.Parameters.AddWithValue("@id_und", idUnd);
                    cmd.Parameters.AddWithValue("@precio", precio);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Producto actualizado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCodigo.Text = "";
                txtDescripcion.Text = "";
                txtPrecio.Text = "";
                cbIdUnd.Text = "";
                btnGuardar.IsEnabled = true;
                btnActualizar.IsEnabled = false;
                // Recargar la grilla
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al actualizar el producto: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                productosEnGrilla = new List<Producto>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT p.id, p.codigo, p.descripcion, p.precio, p.id_und, e.descripcion AS desunidad FROM [productos] AS p JOIN [undme] AS e ON p.id_und = e.id;";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Producto producto = new Producto
                        {

                            Id = reader.GetInt32(0),
                            Codigo = reader.GetInt32(1),
                            Descripcion = reader.GetString(2),
                            Precio = (float)reader.GetDouble(3),
                            IdUnd = reader.GetInt32(4),
                            DesUnd = reader.GetString(5)
                        };
                        productosEnGrilla.Add(producto);
                    }
                }

                dataGridProductos.ItemsSource = productosEnGrilla;
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al cargar los productos: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
