using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Data;
using Microsoft.Win32;

namespace UNE
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {

        List<object> carteiras = new List<object>();
        List<long> codes = new List<long>();
        List<object> carteirasEmpty = new List<object>();

        public MainWindow()
        {
            
            InitializeComponent();

            if (this.loadRegisters() == 0)
            {
                dataGrid.Opacity = 0;
                noData.Opacity = 100;
            }
            else
            {
                noData.Opacity = 0;
                dataGrid.Opacity = 100;
            }
            dataGrid.MaxColumnWidth = 200;
            dataGrid.MinColumnWidth = 120;
            dataGrid.ItemsSource = carteiras;

        }

        public int loadRegisters()
        {
            int count = 0;
            var connString = "Server=localhost;Database=une-carteirinhas;Uid=root;Pwd=";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();
            connection.Open();
            command.CommandText = "SELECT * FROM carteiras";
            MySqlDataReader response = command.ExecuteReader();
            carteiras.Clear();
            codes.Clear();
            while (response.Read())
            {
                count++;
                codes.Add( Convert.ToInt64(response["code"].ToString()) );
                carteiras.Add(new { Nome = response["name"], Instituição = response["institution"], RG = response["rg"], Código = response["code"], Expedição = response["createdAt"] });
            }
            dataGrid.ItemsSource = carteirasEmpty;
            dataGrid.ItemsSource = carteiras;
            dataGrid.Items.Refresh();
            return count;
        }
        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (e.Source.ToString().Contains("Salvar"))
            {
                var connString = "Server=localhost;Database=une-carteirinhas;Uid=root;Pwd=";
                var connection = new MySqlConnection(connString);
                var command = connection.CreateCommand();

                Random R = new Random();
                string code;
                do
                {
                    code = ((long)R.Next(0, 100000) * (long)R.Next(0, 100000)).ToString().PadLeft(10, '0');
                } while (codes.Contains(Convert.ToInt64(code)));

                try
                {
                    connection.Open();
                    command.CommandText = "INSERT INTO `carteiras` (`name`, `cpf`, `rg`, `birthday`, `institution`, `image`, `course`, `coursetype`, `code`) VALUES ( @name, @cpf, @rg, @birthday, @institution, @image, @course, @coursetype, @code);";
                    command.Parameters.Add("@name", MySqlDbType.VarChar).Value = txtName.Text.Trim();
                    command.Parameters.Add("@cpf", MySqlDbType.VarChar).Value = txtCpf.Text.Trim();
                    command.Parameters.Add("@rg", MySqlDbType.VarChar).Value = txtRg.Text.Trim();
                    command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = dtpDate.Text;
                    command.Parameters.Add("@institution", MySqlDbType.VarChar).Value = txtInstitution.Text.Trim();
                    command.Parameters.Add("@image", MySqlDbType.VarChar).Value = "eu ts";
                    command.Parameters.Add("@course", MySqlDbType.VarChar).Value = txtCourse.Text.Trim();
                    command.Parameters.Add("@coursetype", MySqlDbType.VarChar).Value = cmbCourse.Text;
                    command.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    command.ExecuteNonQuery();

                    if (this.loadRegisters() == 0)
                    {
                        dataGrid.Opacity = 0;
                        noData.Opacity = 100;
                    }
                    else
                    {
                        noData.Opacity = 0;
                        dataGrid.Opacity = 100;
                    }

                }
                finally
                {
                    if (connection.State == ConnectionState.Open)
                        connection.Close();
                }
            }else if (e.Source.ToString().Contains("Arquivo"))
            {
                var openFileDialog = new OpenFileDialog();
                var dialogResult = openFileDialog.ShowDialog();
                if (!dialogResult.HasValue) MessageBox.Show("Porra bixo, num fode !");
                image.Source = new BitmapImage(new Uri(openFileDialog.FileName, UriKind.Absolute));
                image.Width = 100;
                image.Height = 100;


            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
