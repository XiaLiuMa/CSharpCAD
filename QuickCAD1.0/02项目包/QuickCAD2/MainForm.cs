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



                    // 加载 DWG 文件
                    using (Aspose.CAD.Image image = Aspose.CAD.Image.Load(dwgFilePath))
                    {
                        // 创建位图对象
                        Bitmap bitmap = new Bitmap(image.Width, image.Height);

                        // 将 DWG 文件渲染到位图
                        //CadRasterizationOptions rasterizationOptions = new CadRasterizationOptions();
                        //rasterizationOptions.PageWidth = image.Width;
                        //rasterizationOptions.PageHeight = image.Height;



                        // 在 PictureBox 中显示预览图像
                        pictureBox1.Image = bitmap;
                        this.Refresh();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("加载 DWG 文件出错：" + ex.Message);
                }
            }
        }
    }
}
