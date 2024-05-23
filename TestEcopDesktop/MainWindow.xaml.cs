using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TestEcopDesktop
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Inicio_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new PedidoPage());
        }

        private void Cliente_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ClientePage());
        }

        private void Producto_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(new ProductoPage());
        }
    }
}