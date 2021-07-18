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

        struct Nums
        {
            private int upperNum, upperRightNum, rightNum, downNum, downRightNum;
            public Nums(int _upperNum, int _upperRightNum, int _rightNum, int _downNum, int _downRightNum)
            {
                upperNum = _upperNum;
                upperRightNum = _upperRightNum;
                rightNum = _rightNum;
                downNum = _downNum;
                downRightNum = _downRightNum;
            }

            public void searchForNeighbors(int areaCount, int number)
            {
                upperNum = number - areaCount;
                upperRightNum = number - areaCount + 1;
                rightNum = number + 1;
                downNum = number + areaCount;
                downRightNum = number + areaCount + 1;
            }
        }
        public int Rotate(Point A, Point B, Point C)
        {
            return ((B.X - A.X) * (C.Y - B.Y) - (B.Y - A.Y) * (C.X - B.X));
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
            int pointsCount = 10;
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
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Point p in pointsList)
            {
                flagGraphics.FillEllipse(Brushes.Red, p.X-2, p.Y-2, 4, 4);
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
            if (pointsList.Count < 2)
            {
                throw new Exception("Точек меньше двух");
            }
            int indexStartPoint = 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].X < pointsList[indexStartPoint].X)
                {
                    indexStartPoint = i;
                }
            }
            int indexNextPoint = indexStartPoint == 0 ? 1 : 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if ((indexStartPoint!=indexNextPoint) && (Rotate(pointsList[indexStartPoint],pointsList[indexNextPoint],pointsList[i])>0))
                {
                    indexNextPoint = i;
                }
            }
            if (indexNextPoint==indexStartPoint)
            {
                throw new Exception("Ошибка алгоритма");
            }
            Pen blackPen = new Pen(Color.Green, 1);
            using (var graphics = Graphics.FromImage(bmp))
            {
                graphics.DrawLine(blackPen, pointsList[indexStartPoint].X, pointsList[indexStartPoint].Y,
                                            pointsList[indexNextPoint].X, pointsList[indexNextPoint].Y);
            }
            //нужные области для базовой линии
            int areaNumber1 = -1, areaNumber2 = -1;
            for (int i = 0; i < areasList.Count; i++)
            {
                if (areasList[i].rect.Contains(pointsList[indexStartPoint]))
                {
                    areaNumber1 = i;
                }
                if (areasList[i].rect.Contains(pointsList[indexNextPoint]))
                {
                    areaNumber2 = i;
                }
            }
            Nums numms1 = new Nums(-1, -1, -1, -1, -1);
            numms1.searchForNeighbors(3, areaNumber1);
            Nums numms2 = new Nums(-1, -1, -1, -1, -1);
            numms2.searchForNeighbors(3, areaNumber2);
        }


        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindBaseLine();
        }
    }
}
