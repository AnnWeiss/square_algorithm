using System;
using System.Collections.Generic;
using System.Collections;
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
        List<int> areasByPass = new List<int>();//сюда будет закидываться обход клеток
        Queue<Point> lineQueue = new Queue<Point>();
        HashSet<Pair> pairsSet = new HashSet<Pair>();
        HashSet<int> alreadyAddedAreas = new HashSet<int>();
        public int Rotate(Point A, Point B, Point C)
        {
            return ((B.X - A.X) * (C.Y - B.Y) - (B.Y - A.Y) * (C.X - B.X));
        }
        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        public void CreateBitmapAtRuntime()
        {
            int areasize = 65;
            int areasCount = Convert.ToInt32(textBox1.Text);
            if (areasCount < 2 || areasCount > 10)
            {
                MessageBox.Show("Введите число от 2 до 10");
            }
            mainPictureBox.Size = new Size(areasize * areasCount, areasize * areasCount);
            bmp = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            flagGraphics.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            mainPictureBox.Image = bmp;
            mainPictureBox.Invalidate();
        }

        public void GenerateAreas()
        {
            int areasCount = Convert.ToInt32(textBox1.Text);
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            for (int i = 0; i < bmp.Height; i = i + h)
            {
                for (int j = 0; j < bmp.Width; j = j + w)
                {
                    areasList.Add(new Area(new Rectangle(j, i, w, h)));
                }
            }
        }
        public void DrawAreas()
        {
            Pen blackPen = new Pen(Color.Black, 1);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Area a in areasList)
            {
                flagGraphics.DrawRectangle(blackPen, a.rect);
            }
        }

        public void GeneratePoints()
        {
            Random rnd = new Random();
            int maxXvalue = bmp.Size.Width;
            int maxYvalue = bmp.Size.Height;
            int pointsCount = Convert.ToInt32(textBox2.Text);
            if (pointsCount < 2 || pointsCount > 10000)
            {
                MessageBox.Show("Введите число от 2 до 10000");
            }
            int pointsIterator = 0;
            pointsList.Clear();
            while (pointsIterator < pointsCount)
            {
                int x = rnd.Next(0, maxXvalue);
                int y = rnd.Next(0, maxYvalue);
                pointsList.Add(new Point(x, y));
                pointsIterator++;
            }
        }

        public void DrawRandomPoints()
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            foreach (Point p in pointsList)
            {
                flagGraphics.FillEllipse(Brushes.Red, p.X-2, p.Y-2, 4, 4);
            }
        }

        public void PointBelongs()
        {
            int areasCount = Convert.ToInt32(textBox1.Text);
            int w = bmp.Width / areasCount;
            int h = bmp.Height / areasCount;
            //принадлежность точек к прямоугольникам
            foreach (Point p in pointsList)
            {
                int x1 = p.X / w;
                int y1 = p.Y / h;
                int areaNumber;
                y1 *= areasCount;
                areaNumber = x1 + y1;
                areasList[areaNumber].points.Add(p);
            }
            //принадлежность вершин к прямоугольникам
            for (int k = 0; k < areasList.Count; k++)
            {
                areasList[k].vertices.Add(new Point(areasList[k].rect.X, areasList[k].rect.Y));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X+w, areasList[k].rect.Y));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X, areasList[k].rect.Y+h));
                areasList[k].vertices.Add(new Point(areasList[k].rect.X+w, areasList[k].rect.Y+h));
            }
        }

        public void FindBaseLine()
        {
            if (pointsList.Count < 2)
            {
                MessageBox.Show("Точек меньше двух!");
            }
            int indexStartPoint = 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if (pointsList[i].X < pointsList[indexStartPoint].X)
                {
                    indexStartPoint = i;
                }
            }
            int indexEndPoint = indexStartPoint == 0 ? 1 : 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if ((indexStartPoint!=indexEndPoint) && (Rotate(pointsList[indexStartPoint],pointsList[indexEndPoint],pointsList[i])>0))
                {
                    indexEndPoint = i;
                }
            }
            if (indexEndPoint == indexStartPoint)
            {
                MessageBox.Show("Ошибка алгоритма");
            }
            Point SP = new Point();
            SP = pointsList[indexStartPoint];
            Point EP = new Point();
            EP = pointsList[indexEndPoint];
            int areasCount = Convert.ToInt32(textBox1.Text);
            lineQueue.Enqueue(SP);
            lineQueue.Enqueue(EP);
            pairsSet.Add(new Pair(SP, EP));
            TriangulationFull(lineQueue, areasCount);
        }
        public List<Nums> getNumsByBaseLine(Point SP, Point EP)
        {
            List<Nums> numsList = new List<Nums>();//лист клеток с первой базовой линией
            int areaNumberSP = -1, areaNumberEP = -1;
            for (int i = 0; i < areasList.Count; i++)
            {
                if (areasList[i].rect.Contains(SP))
                {
                    areaNumberSP = i;
                }
                if (areasList[i].rect.Contains(EP))
                {
                    areaNumberEP = i;
                }
            }
            //области iSP iEP
            int areasCount = Convert.ToInt32(textBox1.Text);
            int xI1 = -1, yJ1 = -1, xI2 = -1, yJ2 = -1;
            int[,] Matrix = new int[areasCount, areasCount];
            int k = 0;
            for (int i = 0; i < areasCount; i++)
            {
                for (int j = 0; j < areasCount; j++)
                {
                    Matrix[i, j] = k++;
                    if (Matrix[i, j] == areaNumberSP)
                    {
                        xI1 = i;
                        yJ1 = j;
                    }
                    if (Matrix[i, j] == areaNumberEP)
                    {
                        xI2 = i;
                        yJ2 = j;
                    }
                }
            }
            if (xI1 > xI2)//swap
            {
                xI1 = xI1 + xI2;
                xI2 = xI1 - xI2;
                xI1 = xI1 - xI2;
            }
            if (yJ1 > yJ2)
            {
                yJ1 = yJ1 + yJ2;
                yJ2 = yJ1 - yJ2;
                yJ1 = yJ1 - yJ2;
            }

            Point NVector = new Point();
            NVector = getAntiNormalVector(SP, EP); //вектор нормали к базовой линии

            for (int i = xI1; i <= xI2; i++)//рассматриваем в диапазоне и добавляем в лист
            {
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int area = Matrix[i, j];
                    addNums(NVector, areasCount, area, numsList);
                }
            }
            foreach (Nums n in numsList)
            {
                alreadyAddedAreas.Add(n.mainNum);
            }
            return numsList;
        }
        public void addNums(Point NVector, int areasCount, int area, List<Nums> numsList)
        {
            Nums nums = new Nums(areasCount*areasCount);
            if(area >= 0 && area < areasCount * areasCount)
            {
                if (NVector.X > 0 && NVector.Y > 0) //вправо вверх
                {
                    nums.setSquare_Right_Or_RightUp(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X > 0 && NVector.Y < 0) //вправо вниз
                {
                    nums.setSquare_Down_Or_RightDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X < 0 && NVector.Y < 0) //влево вниз
                {
                    nums.setSquare_Left_Or_LeftDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X < 0 && NVector.Y > 0) //влево вверх
                {
                    nums.setSquare_Up_Or_UpLeft(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X == 0 && NVector.Y < 0) //вниз
                {
                    nums.setSquare_Down_Or_RightDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X == 0 && NVector.Y > 0) //вверх
                {
                    nums.setSquare_Up_Or_UpLeft(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X > 0 && NVector.Y == 0) //вправо
                {
                    nums.setSquare_Right_Or_RightUp(areasCount, area);
                    numsList.Add(nums);
                }
                if (NVector.X < 0 && NVector.Y == 0) //влево
                {
                    nums.setSquare_Left_Or_LeftDown(areasCount, area);
                    numsList.Add(nums);
                }
            }
        }
        public Point getAntiNormalVector(Point sp, Point ep)
        {
            ep.X = ep.X - sp.X; ep.Y = ep.Y - sp.Y;//конец минус начало
            int a = -ep.Y; int b = ep.X;
            ep.X = a; ep.Y = b;
            return ep;//возврат координат конца вектора, повернутые на 90 градусов во внеш.сторону
        }
        public bool isAreaBehindBaseLine(Area area, Point SP, Point EP)
        {
            int one, two, three, four;
            one = Rotate(SP, EP, area.getVertice(0));
            two = Rotate(SP, EP, area.getVertice(1));
            three = Rotate(SP, EP, area.getVertice(2));
            four = Rotate(SP, EP, area.getVertice(3));
            if (one > 0 && two > 0 && three > 0 && four > 0)
            {
                return true;
            }
            return false;
        }
        public List<Nums> addAreasBypass(List<Nums> numslist, int areasCount, Point SP, Point EP, Point NVector)//задается нужный порядок обхода от базовой линии
        {
            List<Nums> numslist2 = new List<Nums>();
            for (int i = 0; i < numslist.Count; i++)
            {
                for (int j = 0; j < numslist[i].numsList.Count; j++)
                {
                    if (!alreadyAddedAreas.Contains(numslist[i].numsList[j]) && !isAreaBehindBaseLine(areasList[numslist[i].numsList[j]], SP, EP))
                    {
                        alreadyAddedAreas.Add(numslist[i].numsList[j]);
                        addNums(NVector, areasCount, numslist[i].numsList[j], numslist2);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return numslist2;
        }
        public void trySetTriangle(List<Nums> numsList, int areasCount, Point A, Point B, Point ANVector)
        {
            areasByPass.Clear();
            for (int i = 0; i < numsList.Count; i++)
            {
                areasByPass.Add(numsList[i].mainNum);
            }
            bool isTriangleSetted = setTriangle(A, B, areasByPass);
            if (!isTriangleSetted && areasByPass.Count > 0)
            {
                numsList = addAreasBypass(numsList, areasCount, A, B, ANVector);
                trySetTriangle(numsList, areasCount, A, B, ANVector);
            }
        }
        public void TriangulationFull(Queue<Point> lineQueue, int areasCount)
        {
            if (lineQueue.Count >= 2)
            {
                Point A = lineQueue.Dequeue();
                Point B = lineQueue.Dequeue();
                Point ANVector = new Point();
                ANVector = getAntiNormalVector(A, B);
                List<Nums> numsList = new List<Nums>();
                numsList = getNumsByBaseLine(A, B);//вызов поиска областей для базовой линии
                trySetTriangle(numsList, areasCount, A, B, ANVector);
                if (lineQueue.Count > 0)
                {
                    alreadyAddedAreas.Clear();
                    TriangulationFull(lineQueue, areasCount);
                }
                else
                {
                    areasByPass.Clear();
                    alreadyAddedAreas.Clear();
                }
            }
        }
        public double getLineLength(Point A, Point B)
        {
            double xVal = Math.Pow(B.X - A.X, 2);
            double yVal = Math.Pow(B.Y - A.Y, 2);
            double dist = Math.Sqrt(xVal + yVal);
            return dist;
        }
        public bool setTriangle(Point A, Point B, List<int> areasByPass)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            int a = 0, n = 0;
            List<Point> newListPoints = new List<Point>();
            Pair newPair1 = new Pair(A,A);
            Pair newPair2 = new Pair(B,B);
            for (int i = 0; i < areasByPass.Count; i++)
            {
                newListPoints = areasList[areasByPass[i]].getListPoints();
                for (int k = 0; k < newListPoints.Count; k++)//перебор листа поинтов num ректангла
                {
                    int val = Rotate(A, B, newListPoints[k]);
                    if (val < 0)
                    {
                        double AC = getLineLength(A, newListPoints[k]); //b
                        double CB = getLineLength(newListPoints[k], B); //a
                        double gamma = Math.Acos((CB * CB + AC * AC - AB * AB) / (2 * CB * AC)) * 180 / Math.PI;
                        Pair newPairVarite1 = new Pair(A, areasList[areasByPass[i]].points[k]);
                        Pair newPairVarite2 = new Pair(areasList[areasByPass[i]].points[k], B);
                        bool isnewPairVarite1Crosses = areCrossing(newPairVarite1.Point1, newPairVarite1.Point2, pairsSet);
                        bool isnewPairVariate2Crosses = areCrossing(newPairVarite2.Point1, newPairVarite2.Point2, pairsSet);

                        if (gamma > finalGamma && !isnewPairVarite1Crosses && !isnewPairVariate2Crosses)//тут условие на пересечение
                        {
                            finalGamma = gamma;
                            newPair1.Point2 = areasList[areasByPass[i]].points[k];
                            newPair2.Point1 = areasList[areasByPass[i]].points[k];
                            a = k;
                            n = i;
                        }
                    }
                }
            }
            if (finalGamma > 0)
            {
                Pair newPairInverted1 = new Pair(areasList[areasByPass[n]].points[a], A);
                Pair newPairInverted2 = new Pair(B, areasList[areasByPass[n]].points[a]);

                if (!pairsSet.Contains(newPair1) && !pairsSet.Contains(newPairInverted1))
                {
                    lineQueue.Enqueue(A);
                    lineQueue.Enqueue(areasList[areasByPass[n]].points[a]);
                    pairsSet.Add(newPair1);
                }
                if (!pairsSet.Contains(newPair2) && !pairsSet.Contains(newPairInverted2))
                {
                    lineQueue.Enqueue(areasList[areasByPass[n]].points[a]);
                    lineQueue.Enqueue(B);
                    pairsSet.Add(newPair2);
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        public int vectorMult(int ax, int ay, int bx, int by) //векторное произведение
        {
            return ax * by - bx * ay;
        }
        public bool areCrossing(Point p1, Point p2, HashSet<Pair> pairsSet)//проверка пересечения
        {
            foreach (Pair p in pairsSet)
            {
                long v1 = vectorMult(p.Point2.X - p.Point1.X, p.Point2.Y - p.Point1.Y, p1.X - p.Point1.X, p1.Y - p.Point1.Y);
                long v2 = vectorMult(p.Point2.X - p.Point1.X, p.Point2.Y - p.Point1.Y, p2.X - p.Point1.X, p2.Y - p.Point1.Y);
                long v3 = vectorMult(p2.X - p1.X, p2.Y - p1.Y, p.Point1.X - p1.X, p.Point1.Y - p1.Y);
                long v4 = vectorMult(p2.X - p1.X, p2.Y - p1.Y, p.Point2.X - p1.X, p.Point2.Y - p1.Y);
                if ((v1 * v2) < 0 && (v3 * v4) < 0)
                {
                    return true;
                }
            }
            return false;
        }
        public void drawLine(HashSet<Pair> pairsSet)
        {
            Random r = new Random();
            Pen br = new Pen(Color.FromArgb(r.Next(255), r.Next(100), r.Next(100), r.Next(255)));

            Graphics flagGraphics = Graphics.FromImage(bmp);
            Pen bluePen = new Pen(Color.Blue, 1);
            foreach (Pair p in pairsSet)
            {
                flagGraphics.DrawLine(bluePen, p.Point1, p.Point2);
            }
        }
        private void genButton_Click(object sender, EventArgs e)
        {
            CreateBitmapAtRuntime();
            GenerateAreas();
            DrawAreas();
            GeneratePoints();
            DrawRandomPoints();
            PointBelongs();
            FindBaseLine();
            drawLine(pairsSet);
            clearAll();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }
        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }
        public void clearAll()
        {
            areasList.Clear();
            pointsList.Clear();
            areasByPass.Clear();
            lineQueue.Clear();
            pairsSet.Clear();
            alreadyAddedAreas.Clear();
        }
    }
}
