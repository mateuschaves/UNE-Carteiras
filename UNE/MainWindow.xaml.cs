using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Windows.Documents;
using System.Windows.Media.Imaging;
using System.Data;
using Microsoft.Win32;
using IronPdf;


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
        string uriImage = "";


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

        private void btnGerarQRCode_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Erro");
            }
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
                    string pathImage;
                    if (uriImage != "")
                    {
                        pathImage = uriImage;
                    }
                    else
                    {
                        pathImage = lblpathImage.Text;
                    }
                    connection.Open();
                    command.CommandText = "INSERT INTO `carteiras` (`name`, `cpf`, `rg`, `birthday`, `institution`, `image`, `course`, `coursetype`, `code`) VALUES ( @name, @cpf, @rg, @birthday, @institution, @image, @course, @coursetype, @code);";
                    command.Parameters.Add("@name", MySqlDbType.VarChar).Value = txtName.Text.Trim();
                    command.Parameters.Add("@cpf", MySqlDbType.VarChar).Value = txtCpf.Text.Trim();
                    command.Parameters.Add("@rg", MySqlDbType.VarChar).Value = txtRg.Text.Trim();
                    command.Parameters.Add("@birthday", MySqlDbType.VarChar).Value = dtpDate.Text;
                    command.Parameters.Add("@institution", MySqlDbType.VarChar).Value = txtInstitution.Text.Trim();
                    command.Parameters.Add("@image", MySqlDbType.VarChar).Value = pathImage;
                    command.Parameters.Add("@course", MySqlDbType.VarChar).Value = txtCourse.Text.Trim();
                    command.Parameters.Add("@coursetype", MySqlDbType.VarChar).Value = cmbCourse.Text;
                    command.Parameters.Add("@code", MySqlDbType.VarChar).Value = code;
                    command.ExecuteNonQuery();

                    IronPdf.HtmlToPdf Renderer = new IronPdf.HtmlToPdf();
                    // Render an HTML document or snippet as a string
                    //Renderer.RenderHtmlAsPdf("<h1>Hello World</h1>").SaveAs("C:/Users/mateu/OneDrive/Documents/html-string.pdf");
                    // Advanced: 
                    // Set a "base url" or file path so that images, javascript and CSS can be loaded  
                    //var PDF = Renderer.RenderHtmlAsPdf("<img src='/Documents/images1172019910.jpg'>", @"C:\Users\mateu\OneDrive\");
                    var PDF = Renderer.RenderHtmlAsPdf("<div><img style='margin-right: 10px' width='150' height='200' src='"+ pathImage + "'/><div style='margin-top: -12px; float: right; margin-right: 400px'><p style='font-size: 10pt'><b>Nome</b><br />"+txtName.Text+"</p><p style='font-size: 10pt'><b>Instituição de Ensino</b><br />"+txtInstitution.Text+"<br />Graduação <br />"+txtCourse.Text+"</p><p style='font-size: 10pt'><b>Dt. Nasc</b> 00/00/0000 </p><p style='font-size: 10pt'><b>RG</b> "+txtRg.Text+"</p><p style='font-size: 10pt'><b>CPF</b>"+txtCpf.Text+"</p></div></div>");
                    Renderer.PrintOptions.SetCustomPaperSizeInInches(3.93701, 1.9685);
                    PDF.SaveAs("C:/Users/mateu/OneDrive/Documents/html-with-assets.pdf");

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
                uriImage = openFileDialog.FileName;
                image.Width = 100;
                image.Height = 100;

            }
            else
            {
                WebCam cameraForm = new WebCam();
                cameraForm.Show();
                this.Hide();
            }
        }


        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
        }
    }
}
