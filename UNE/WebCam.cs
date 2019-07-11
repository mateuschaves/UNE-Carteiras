using System;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace UNE
{
    public partial class WebCam : Form
    {
        public DirectX.Capture.Filter Camera;
        public DirectX.Capture.Capture CaptureInfo;
        public DirectX.Capture.Filters CamContainer;
        Image capturaImagem;
        public string caminhoImagemSalva = null;
        public WebCam()
        {
            InitializeComponent();
        }

        public void AtualizaImagem(PictureBox frame)
        {
            try
            {
                capturaImagem = frame.Image;
                this.pictureBox2.Image = capturaImagem;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro " + ex.Message);
            }
        }

        private void WebCam_Load(object sender, EventArgs e)
        {
            CamContainer = new DirectX.Capture.Filters();
            try
            {
                int no_of_cam = CamContainer.VideoInputDevices.Count;

                for (int i = 0; i < no_of_cam; i++)
                {
                    try
                    {
                        // obtém o dispositivo de entrada do vídeo
                        Camera = CamContainer.VideoInputDevices[i];

                        // inicializa a Captura usando o dispositivo
                        CaptureInfo = new DirectX.Capture.Capture(Camera, null);

                        // Define a janela de visualização do vídeo
                        CaptureInfo.PreviewWindow = this.pictureBox1;

                        // Capturando o tratamento de evento
                        CaptureInfo.FrameCaptureComplete += AtualizaImagem;

                        // Captura o frame do dispositivo
                        CaptureInfo.CaptureFrame();

                        // Se o dispositivo foi encontrado e inicializado então sai sem checar o resto
                        break;
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }

        private void BtnCapture_Click(object sender, EventArgs e)
        {
            try
            {
                CaptureInfo.CaptureFrame();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro " + ex.Message);
            }
        }
    }
}
