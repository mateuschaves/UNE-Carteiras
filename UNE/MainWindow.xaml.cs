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

namespace UNE
{
    /// <summary>
    /// Interação lógica para MainWindow.xam
    /// </summary>
    public partial class MainWindow : Window
    {
       
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void ComboBoxItem_Selected(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var connString = "Server=localhost;Database=une-carteirinhas;Uid=root;Pwd=";
            var connection = new MySqlConnection(connString);
            var command = connection.CreateCommand();

            try
            {
                connection.Open();
                command.CommandText = "INSERT INTO `carteiras` (`name`, `cpf`, `rg`, `birthday`, `institution`, `image`, `course`, `coursetype`) VALUES ( @name, @cpf, @rg, @birthday, @institution, @image, @course, @coursetype);";
                command.Parameters.Add("@name", MySqlDbType.VarChar).Value= txtName.Text.Trim();
                command.Parameters.Add("@cpf", MySqlDbType.VarChar).Value= txtCpf.Text.Trim();
                command.Parameters.Add("@rg", MySqlDbType.VarChar).Value = txtRg.Text.Trim();
                command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = dtpDate.Text;
                command.Parameters.Add("@institution", MySqlDbType.VarChar).Value = txtInstitution.Text.Trim();
                command.Parameters.Add("@image", MySqlDbType.VarChar).Value = "eu ts";
                command.Parameters.Add("@course", MySqlDbType.VarChar).Value = txtCourse.Text.Trim();
                command.Parameters.Add("@coursetype", MySqlDbType.VarChar).Value = cmbCourse.Text;
                command.ExecuteNonQuery();
            }
            finally
            {
                if (connection.State == ConnectionState.Open)
                    connection.Close();
            }

        }
    }
}
