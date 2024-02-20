using System;
using System.Drawing;
using CADImport.DWG;
using CADImport;

namespace QuickCAD2
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DWG Files|*.dwg";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string dwgFilePath = openFileDialog.FileName;

                try
                {
                    DWGImage dwgImage = (DWGImage)CADImage.LoadFromFile(dwgFilePath);
                    int width = (int)dwgImage.I;
                    int height = (int)dwgImage.ImageHeight;
                    Bitmap bitmap = new Bitmap(width, height);

                    using (Graphics graphics = Graphics.FromImage(bitmap))
                    {
                        dwgImage.Draw(graphics);
                    }



                    // ���� DWG �ļ�
                    using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(dwgFilePath))
                    {
                        // ����λͼ����
                        Bitmap bitmap = new Bitmap(image.Width, image.Height);

                        // �� DWG �ļ���Ⱦ��λͼ
                        //CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();
                        //rasterizationOptions.PageWidth = image.Width;
                        //rasterizationOptions.PageHeight = image.Height;



                        // �� PictureBox ����ʾԤ��ͼ��
                        pictureBox1.Image = bitmap;
                        this.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("���� DWG �ļ�����" + ex.Message);
                }
            }
        }
    }
}
