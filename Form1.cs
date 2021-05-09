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

        public static void Swap<T>(IList<T> list,  int indexA,  int indexB)
        {
            T tmp = list[indexA];
            list[indexA] = list[indexB];
            list[indexB] = tmp;
        }

        public int Rotate(Point A, Point B, Point C)
        {
            return ((B.X-A.X)*(C.Y-B.Y) - (B.Y-A.Y)*(C.X-B.X));
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
            int indexStartPoint = 0;
            int minX = 0, minY = 0;
            foreach (Point p in pointsList)
            {
                if (pointIterator == 0)
                {
                    minX = p.X;
                    minY = p.Y;
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
                }
                pointIterator++;
            }
            //поместила стартовую точку в начало списка
            Swap(pointsList, 0, indexStartPoint);

            bmp.SetPixel(pointsList[0].X, pointsList[0].Y, Color.Blue);
            //делаем стартовую вершину текущей, ищем самую правую точку относительно текущей вершины
            for (int i=2; i < pointsList.Count; i++)
            {
                int j = i;
                while (j>1 && Rotate(pointsList[0], pointsList[j-1], pointsList[j])<0)
                {
                    Swap(pointsList, j, j - 1);
                    j--;
                }
            }
            Pen blackPen = new Pen(Color.Black, 1);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, pointsList[0].X, pointsList[0].Y,
                                            pointsList[1].X, pointsList[1].Y);
            }
        }
        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
        }
    }
}
