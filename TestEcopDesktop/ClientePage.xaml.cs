using System;
using System.Collections.Generic;
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
    /// Lógica de interacción para ClientePage.xaml
    /// </summary>
    public partial class ClientePage : Page
    {
        private string connectionString = ConexionDB.ConnectionString;
        private List<Cliente> clientesEnGrilla;
        Cliente cliente = new Cliente();
        public ClientePage()
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
                string nombre = txtNombre.Text;
                string apellido = txtApellido.Text;
                string tipoDoc = (cbIdDoc.SelectedItem as ComboBoxItem).Content.ToString();
                int idDoc = tipoDoc != "CI" ? 2 : 1;
                string documento = txtDocumneto.Text;

                // Insertar el cliente en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "INSERT INTO [clientes] (codigo, nombre,apellido, id_doc, nrodoc) VALUES (@codigo, @nombre,@apellido, @id_doc, @nrodoc)";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@id_doc", idDoc);
                    cmd.Parameters.AddWithValue("@nrodoc", documento);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Cliente guardado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCodigo.Text = "";
                txtNombre.Text = "";
                txtApellido.Text = "";
                txtDocumneto.Text = "";
                cbIdDoc.Text = "";
                // Recargar la grilla
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al guardar el cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Ver_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            int id = (int)btn.Tag;
            cliente = ((List<Cliente>)dataGridCliente.ItemsSource).FirstOrDefault(p => p.Id == id);

            if (cliente != null)
            {
                txtCodigo.Text = cliente.Codigo.ToString();
                txtNombre.Text = cliente.Nombre;
                txtApellido.Text = cliente.Apellido;
                cbIdDoc.Text = cliente.DocDes.ToString();
                txtDocumneto.Text = cliente.NroDoc.ToString();
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
                EliminarCliente(id);

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Cliente eliminado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al eliminar el cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EliminarCliente(int id)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                string query = "DELETE FROM [clientes] WHERE id = @id";
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
                string nombre = txtNombre.Text;
                string apellido = txtApellido.Text;
                string tipoDoc = (cbIdDoc.SelectedItem as ComboBoxItem).Content.ToString();
                int idDoc = tipoDoc != "CI" ? 2 : 1;
                string documento = txtDocumneto.Text;

                // Actualizar el cliente en la base de datos
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "UPDATE [clientes] SET codigo=@codigo, nombre = @nombre,apellido = @apellido, id_doc = @id_doc, nrodoc = @nrodoc WHERE id = @id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    cmd.Parameters.AddWithValue("@id", cliente.Id);
                    cmd.Parameters.AddWithValue("@codigo", codigo);
                    cmd.Parameters.AddWithValue("@nombre", nombre);
                    cmd.Parameters.AddWithValue("@apellido", apellido);
                    cmd.Parameters.AddWithValue("@id_doc", idDoc);
                    cmd.Parameters.AddWithValue("@nrodoc", documento);

                    connection.Open();
                    cmd.ExecuteNonQuery();
                }

                // Mostrar un mensaje de confirmación
                MessageBox.Show("Cliente actualizado exitosamente", "Éxito", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCodigo.Text = "";
                txtNombre.Text = "";
                txtApellido.Text = "";
                txtDocumneto.Text = "";
                cbIdDoc.Text = "";
                btnGuardar.IsEnabled = true;
                btnActualizar.IsEnabled = false;
                // Recargar la grilla
                LoadDataGrid();
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al actualizar el cliente: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadDataGrid()
        {
            try
            {
                clientesEnGrilla = new List<Cliente>();

                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    string query = "SELECT c.id,c.codigo, c.nombre, c.apellido, c.id_doc, c.nrodoc, e.descripcion AS destipo " +
            "FROM [clientes] AS c " +
            "JOIN [tipodoc] AS e ON c.id_doc = e.id";
                    SqlCommand cmd = new SqlCommand(query, connection);
                    connection.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        Cliente cliente = new Cliente
                        {

                            Id = reader.GetInt32(0),
                            Codigo = reader.GetInt32(1),
                            Nombre = reader.GetString(2),
                            Apellido = reader.GetString(3),
                            IdDoc = reader.GetInt32(4),
                            NroDoc = reader.GetString(5),
                            DocDes = reader.GetString(6)
                        };
                        clientesEnGrilla.Add(cliente);
                    }
                }

                dataGridCliente.ItemsSource = clientesEnGrilla;
            }
            catch (Exception ex)
            {
                // Manejar errores y mostrar un mensaje de error
                MessageBox.Show("Error al cargar los clientes: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
