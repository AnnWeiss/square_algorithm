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
        List<Point> pointsList = new List<Point>();


        public int Rotate(int x1, int y1, int x2, int y2, int x3, int y3)
        {
            return ((x2 - x1) * (y3 - y2) - (y2 - y1) * (x3 - x2));
        }
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
            flagGraphics.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            mainPictureBox.Image = bmp;
            mainPictureBox.Invalidate();
            
        }

        public void GenerateAreas()
        {
            areasList.Clear();
            int areasCount = 3;
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            for (int i = 0; i < bmp.Height; i = i + h)
            {
                for (int j = 0; j < bmp.Width; j = j + w)
                {
                    areasList.Add(new Area(new Rectangle(j, i, w, h)));
                }
            }
            DrawAreas();
        }
        public void DrawAreas()
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Area a in areasList)
            {
                flagGraphics.DrawRectangle(blackPen, a.rect);

            }
            GeneratePoints();
        }

        public void GeneratePoints()
        {
            Random rnd = new Random();
            int maxXvalue = bmp.Size.Width;
            int maxYvalue = bmp.Size.Height;
            int pointsCount = 20;
            int pointsIterator = 0;
            pointsList.Clear();
            while (pointsIterator < pointsCount)
            {
                int x = rnd.Next(0, maxXvalue);
                int y = rnd.Next(0, maxYvalue);
                pointsList.Add(new Point(x, y));
                pointsIterator++;
            }
            DrawRandomPoints();
        }

        public void DrawRandomPoints()
        {
            foreach (Point p in pointsList)
            {
                bmp.SetPixel(p.X, p.Y, Color.Red);
            }
            PointBelongs();
        }

        public void PointBelongs()
        {
            int areasCount = 3;
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            foreach (Point p in pointsList)
            {
                int x1 = p.X / w;
                int y1 = p.Y / h;
                int areaNumber;
                y1 *= areasCount;
                areaNumber = x1 + y1;
                areasList[areaNumber].points.Add(p);
            }
            FindBaseLine();
        }

        public void FindBaseLine()
        {
            //нам нужна стартовая точка, которая гарантированно входит в МВО, берем самую левую точку (по Х самое маленькое число)
            int pointIterator = 0;
            int indexStartPoint = 0, indexNextPoint = 0;
            int minX = 0, minY = 0, maxX = 0;
            foreach (Point p in pointsList)
            {
                if (pointIterator == 0)
                {
                    minX = p.X;
                    minY = p.Y;
                    maxX = p.X;
                    indexStartPoint = pointIterator;
                }
                else
                {
                    if (minX > p.X)
                    {
                        minX = p.X;
                        minY = p.Y;
                        indexStartPoint = pointIterator;
                    }
                    if (minX == p.X && minY > p.Y)
                    {
                        indexStartPoint = pointIterator;
                    }

                    if (maxX < p.X)
                    {
                        maxX = p.X;
                        minY = p.Y;
                        indexNextPoint = pointIterator;
                    }
                    if (minX == p.X && minY > p.Y)
                    {
                        indexNextPoint = pointIterator;
                    }
                }
                pointIterator++;
            }

            for (int i = 0; i<pointsList.Count; i++)
            {
                if (pointsList[i] == pointsList[indexNextPoint])
                {
                    continue;
                }
                if (pointsList[i] != pointsList[indexNextPoint])
                {
                    int result = Rotate(pointsList[indexStartPoint].X, pointsList[indexStartPoint].Y,
                                        pointsList[indexNextPoint].X, pointsList[indexNextPoint].Y, pointsList[i].X, pointsList[i].Y);
                    if (result > 0)
                    {
                        indexNextPoint = i;
                    }

                }
            }
            Pen blackPen = new Pen(Color.Green, 1);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, pointsList[indexStartPoint].X, pointsList[indexStartPoint].Y,
                                            pointsList[indexNextPoint].X, pointsList[indexNextPoint].Y);
            }
        }
        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
        }
    }
}
