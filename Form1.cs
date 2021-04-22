using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace square_algorithm
{
    public partial class Form1 : Form
    {
        Bitmap bmp;
        List<Area> areasList = new List<Area>();
        public Form1()
        {
            areasList = new List<Area>();
            InitializeComponent();
            CreateBitmapAtRuntime();
            GenerateAreas();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void CreateBitmapAtRuntime()
        {
            mainPictureBox.Size = new Size(207, 207);
            bmp = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            //flagGraphics.FillRectangle(Brushes.Black, 0, 0, bmp.Width, bmp.Height);
            mainPictureBox.Image = bmp;
            mainPictureBox.Invalidate();
            
        }

        public void GenerateAreas()
        {
            areasList.Clear();
            int areasCount = 9;
            int areasIterator = 0;
            while (areasIterator < areasCount)
            {
                areasList.Add(new Area(new Rectangle(0, 0, bmp.Size.Width / areasCount * 3, bmp.Size.Height / areasCount * 3)));
                areasIterator++;
            }

            DrawAreas();
        }
        public void DrawAreas()
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            for (int i = 0; i < bmp.Height; i++) //Y
            { 
                for (int j = 0; j < bmp.Width; j++) //X
                {
                    foreach (Area a in areasList)
                    {
                        if (j < a.rect.Width && i < a.rect.Height)
                        {
                           bmp.SetPixel(j, i, Color.Black);
                        }
                    }
                }
            }
            
        }
        
    }
}
