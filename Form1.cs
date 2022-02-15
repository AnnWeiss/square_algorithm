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
        public class Nums
        {
            public int mainNum;
            public List<int> numsList = new List<int>();
            static public int sizeOfArea; //areasCount*areasCount
            public Nums(int size)
            {
                sizeOfArea = size;
            }
            public void setSquare_Down_Or_RightDown(int areaCount, int number)
            {
                mainNum = number;
                int down = number - areaCount;
                int up = number + areaCount;
                int sideRight = number + 1;
                int sideLeft = number - 1;
                var isOneRowRight = sideRight / areaCount == number / areaCount;
                var isOneRowLeft = sideLeft / areaCount == number / areaCount;
                if (isOneRowRight && down >= 0)
                {
                    numsList.Add(down + 1);
                }
                if (isOneRowRight)
                {
                    numsList.Add(sideRight);
                }
                if (isOneRowRight && up < sizeOfArea)
                {
                    numsList.Add(up + 1);
                }
                if (up < sizeOfArea)
                {
                    numsList.Add(up);
                }
                if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
                {
                    numsList.Add(up - 1);
                }
                if (isOneRowLeft && sideLeft >= 0)
                {
                    numsList.Add(sideLeft);
                }
                if (isOneRowLeft && down >= 0)
                {
                    numsList.Add(down - 1);
                }
                if (down >= 0)
                {
                    numsList.Add(down);
                }
            }
            public void setSquare_Left_Or_LeftDown(int areaCount, int number)
            {
                mainNum = number;
                int down = number - areaCount;
                int up = number + areaCount;
                int sideRight = number + 1;
                int sideLeft = number - 1;
                var isOneRowRight = sideRight / areaCount == number / areaCount;
                var isOneRowLeft = sideLeft / areaCount == number / areaCount;
                if (isOneRowLeft && down >= 0)
                {
                    numsList.Add(down - 1);
                }
                if (down >= 0)
                {
                    numsList.Add(down);
                }
                if (isOneRowRight && down >= 0)
                {
                    numsList.Add(down + 1);
                }
                if (isOneRowRight)
                {
                    numsList.Add(sideRight);
                }
                if (isOneRowRight && up < sizeOfArea)
                {
                    numsList.Add(up + 1);
                }
                if (up < sizeOfArea)
                {
                    numsList.Add(up);
                }
                if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
                {
                    numsList.Add(up - 1);
                }
                if (isOneRowLeft && sideLeft >= 0)
                {
                    numsList.Add(sideLeft);
                }
            }
            public void setSquare_Up_Or_UpLeft(int areaCount, int number)
            {
                mainNum = number;
                int down = number - areaCount;
                int up = number + areaCount;
                int sideRight = number + 1;
                int sideLeft = number - 1;
                var isOneRowRight = sideRight / areaCount == number / areaCount;
                var isOneRowLeft = sideLeft / areaCount == number / areaCount;
                if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
                {
                    numsList.Add(up - 1);
                }
                if (isOneRowLeft && sideLeft >= 0)
                {
                    numsList.Add(sideLeft);
                }
                if (isOneRowLeft && down >= 0)
                {
                    numsList.Add(down - 1);
                }
                if (down >= 0)
                {
                    numsList.Add(down);
                }
                if (isOneRowRight && down >= 0)
                {
                    numsList.Add(down + 1);
                }
                if (isOneRowRight)
                {
                    numsList.Add(sideRight);
                }
                if (isOneRowRight && up < sizeOfArea)
                {
                    numsList.Add(up + 1);
                }
                if (up < sizeOfArea)
                {
                    numsList.Add(up);
                }
            }
            public void setSquare_Right_Or_RightUp(int areaCount, int number)
            {
                mainNum = number;
                int down = number - areaCount;
                int up = number + areaCount;
                int sideRight = number + 1;
                int sideLeft = number - 1;
                var isOneRowRight = sideRight / areaCount == number / areaCount;
                var isOneRowLeft = sideLeft / areaCount == number / areaCount;
                if (isOneRowRight && up < sizeOfArea)
                {
                    numsList.Add(up + 1);
                }
                if (up < sizeOfArea)
                {
                    numsList.Add(up);
                }
                if (isOneRowLeft && up < sizeOfArea && sideLeft >= 0)
                {
                    numsList.Add(up - 1);
                }
                if (isOneRowLeft && sideLeft >= 0)
                {
                    numsList.Add(sideLeft);
                }
                if (isOneRowLeft && down >= 0)
                {
                    numsList.Add(down - 1);
                }
                if (down >= 0)
                {
                    numsList.Add(down);
                }
                if (isOneRowRight && down >= 0)
                {
                    numsList.Add(down + 1);
                }
                if (isOneRowRight)
                {
                    numsList.Add(sideRight);
                }
            }
        }
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
            mainPictureBox.Size = new Size(areasize*areasCount, areasize * areasCount);
            bmp = new Bitmap(mainPictureBox.Size.Width, mainPictureBox.Size.Height);
            Graphics flagGraphics = Graphics.FromImage(bmp);
            flagGraphics.FillRectangle(Brushes.White, 0, 0, bmp.Width, bmp.Height);
            mainPictureBox.Image = bmp;
            mainPictureBox.Invalidate();
        }

        public void GenerateAreas()
        {
            areasList.Clear();
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
            int indexNextPoint = indexStartPoint == 0 ? 1 : 0;
            for (int i = 0; i < pointsList.Count; i++)
            {
                if ((indexStartPoint!=indexNextPoint) && (Rotate(pointsList[indexStartPoint],pointsList[indexNextPoint],pointsList[i])>0))
                {
                    indexNextPoint = i;
                }
            }
            if (indexNextPoint == indexStartPoint)
            {
                MessageBox.Show("Ошибка алгоритма");
            }
            Point SP = new Point();
            SP = pointsList[indexStartPoint];
            Point NP = new Point();
            NP = pointsList[indexNextPoint];
            int areasCount = Convert.ToInt32(textBox1.Text);
            lineQueue.Enqueue(SP);
            lineQueue.Enqueue(NP);
            pairsSet.Add(new Pair(SP, NP));
            Triangulation(ref lineQueue, areasCount);
        }
        public List<Nums> getNumsByBaseLine(Point SP, Point NP)
        {
            List<Nums> numsList = new List<Nums>();//лист клеток с первой базовой линией
            int areaNumberSP = -1, areaNumberNP = -1;
            for (int i = 0; i < areasList.Count; i++)
            {
                if (areasList[i].rect.Contains(SP))
                {
                    areaNumberSP = i;
                }
                if (areasList[i].rect.Contains(NP))
                {
                    areaNumberNP = i;
                }
            }
            //вершины областей iSP iNP
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
                    if (Matrix[i, j] == areaNumberNP)
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
            NVector = getAntiNormalVector(SP, NP); //вектор нормали к базовой линии

            for (int i = xI1; i <= xI2; i++)//рассматриваем в диапазоне, пропускаем клетки, через которые прямая не проходит
            {
                int one, two, three, four;
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int val = Matrix[i, j];
                    one = Rotate(SP, NP, areasList[val].getVertice(0));
                    two = Rotate(SP, NP, areasList[val].getVertice(1));
                    three = Rotate(SP, NP, areasList[val].getVertice(2));
                    four = Rotate(SP, NP, areasList[val].getVertice(3));

                    if ((one < 0 && two < 0 && three < 0 && four < 0) || (one > 0 && two > 0 && three > 0 && four > 0))
                    {
                        continue;
                    }
                    else
                    {
                        int area = Matrix[i, j];
                        addNums(NVector, areasCount, area, ref numsList);
                    }
                }
            }
            foreach (Nums n in numsList)
            {
                alreadyAddedAreas.Add(n.mainNum);
            }
            return numsList;
        }
        public void addNums(Point NVector, int areasCount, int area, ref List<Nums> numsList)
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
        public Point getAntiNormalVector(Point sp, Point np)
        {
            np.X = np.X - sp.X; np.Y = np.Y - sp.Y;//конец минус начало
            int a = -np.Y; int b = np.X;
            np.X = a; np.Y = b;
            return np;//возврат координат конца вектора, повернутые на 90 градусов во внеш.сторону
        }
        public bool isAreaBehindBaseLine(Area area, Point SP, Point NP)
        {
            int one, two, three, four;
            one = Rotate(SP, NP, area.getVertice(0));
            two = Rotate(SP, NP, area.getVertice(1));
            three = Rotate(SP, NP, area.getVertice(2));
            four = Rotate(SP, NP, area.getVertice(3));
            if (one > 0 && two > 0 && three > 0 && four > 0)
            {
                return true;
            }
            return false;
        }
        public List<Nums> addAreasBypass(ref List<Nums> numslist, int areasCount, Point SP, Point NP, Point NVector)//задается нужный порядок обхода от базовой линии
        {
            List<Nums> numslist2 = new List<Nums>();
            for (int i = 0; i < numslist.Count; i++)
            {
                for (int j = 0; j < numslist[i].numsList.Count; j++)
                {
                    if (!alreadyAddedAreas.Contains(numslist[i].numsList[j]) && !isAreaBehindBaseLine(areasList[numslist[i].numsList[j]], SP, NP))
                    {
                        alreadyAddedAreas.Add(numslist[i].numsList[j]);
                        addNums(NVector, areasCount, numslist[i].numsList[j], ref numslist2);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return numslist2;
        }
        public void trySetTriangle(ref List<Nums> numsList, int areasCount, Point A, Point B, Point ANVector)
        {
            areasByPass.Clear();
            for (int i = 0; i < numsList.Count; i++)
            {
                areasByPass.Add(numsList[i].mainNum);
            }
            bool isTriangleSetted = setTriangle(A, B, ref areasByPass);
            if (!isTriangleSetted && areasByPass.Count > 0)
            {
                numsList = addAreasBypass(ref numsList, areasCount, A, B, ANVector);
                trySetTriangle(ref numsList, areasCount, A, B, ANVector);
            }
        }
        public void Triangulation(ref Queue<Point> lineQueue, int areasCount)
        {
            if (lineQueue.Count >= 2)
            {
                Point A = lineQueue.Dequeue();
                Point B = lineQueue.Dequeue();
                Point ANVector = new Point();
                ANVector = getAntiNormalVector(A, B);
                List<Nums> numsList = new List<Nums>();
                numsList = getNumsByBaseLine(A, B);//вызов поиска областей для базовой линии
                trySetTriangle(ref numsList, areasCount, A, B, ANVector);
                if (lineQueue.Count > 0)
                {
                    areasByPass.Clear();
                    alreadyAddedAreas.Clear();
                    Triangulation(ref lineQueue, areasCount);
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
        public bool setTriangle(Point A, Point B, ref List<int> areasByPass)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            int a = 0, n = 0;
            List<Point> newListPoints = new List<Point>();
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
                        if (gamma > finalGamma)
                        {
                            finalGamma = gamma;
                            a = k;
                            n = i;
                        }
                    }
                }
            }
            if (finalGamma > 0)
            {
                Pair newPair1 = new Pair(A, areasList[areasByPass[n]].points[a]);
                Pair newPair2 = new Pair(areasList[areasByPass[n]].points[a], B);
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
        public void drawLine(ref HashSet<Pair> pairsSet)
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            Pen bluePen = new Pen(Color.Blue, 1);
            foreach (Pair p in pairsSet)
            {
                flagGraphics.DrawLine(bluePen, p.Point1, p.Point2);
            }
            pairsSet.Clear();
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
            drawLine(ref pairsSet);
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            FindBaseLine();
        }
    }
}
