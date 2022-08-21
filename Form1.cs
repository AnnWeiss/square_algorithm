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
using System.Diagnostics;
using System.IO;

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
        Stopwatch stopwatch = new Stopwatch();
        int areasCount;
        int pointsCount;
        StreamWriter sw = new StreamWriter("C:\\Users\\Анна\\Desktop\\algorithm.txt");
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
            if (areasCount < 2 || areasCount > 100)
            {
                MessageBox.Show("Введите число от 2 до 100");
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
            /*if (pointsCount < 10 || pointsCount > 1000000)
            {
                MessageBox.Show("Введите число от 1000 до 1000000");
            }*/
            int pointsIterator = 0;
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
            lineQueue.Enqueue(SP);
            lineQueue.Enqueue(EP);
            pairsSet.Add(new Pair(SP, EP));
        }
        public void getNumsByBaseLine(Point SP, Point EP, List<Nums> numsList)
        {
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

            Point ANVector = new Point();
            ANVector = getAntiNormalVector(SP, EP); //вектор антинормали к базовой линии

            for (int i = xI1; i <= xI2; i++)//рассматриваем в диапазоне и добавляем в лист
            {
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int area = Matrix[i, j];
                    addNums(ANVector, areasCount, area, numsList);
                }
            }
            foreach (Nums n in numsList)
            {
                alreadyAddedAreas.Add(n.mainNum);
            }
        }
        public void addPairToAreas(Point SP, Point EP)
        {
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
            for (int i = xI1; i <= xI2; i++)//рассматриваем в диапазоне и добавляем в лист
            {
                for (int j = yJ1; j <= yJ2; j++)
                {
                    int area = Matrix[i, j];
                    bool isArea = isAreaNotContainsBaseLine(areasList[area], SP, EP);
                    if (!isArea)
                    {
                        Pair baseLine = new Pair(SP, EP);
                        areasList[area].pairsList.Add(baseLine);
                    }
                }
            }
        }
        public void addNums(Point ANVector, int areasCount, int area, List<Nums> numsList)
        {
            Nums nums = new Nums(areasCount*areasCount);
            if(area >= 0 && area < areasCount * areasCount)
            {
                if (ANVector.X > 0 && ANVector.Y > 0) //вправо вверх
                {
                    nums.setSquare_Right_Or_RightUp(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X > 0 && ANVector.Y < 0) //вправо вниз
                {
                    nums.setSquare_Down_Or_RightDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X < 0 && ANVector.Y < 0) //влево вниз
                {
                    nums.setSquare_Left_Or_LeftDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X < 0 && ANVector.Y > 0) //влево вверх
                {
                    nums.setSquare_Up_Or_UpLeft(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X == 0 && ANVector.Y < 0) //вниз
                {
                    nums.setSquare_Down_Or_RightDown(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X == 0 && ANVector.Y > 0) //вверх
                {
                    nums.setSquare_Up_Or_UpLeft(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X > 0 && ANVector.Y == 0) //вправо
                {
                    nums.setSquare_Right_Or_RightUp(areasCount, area);
                    numsList.Add(nums);
                }
                if (ANVector.X < 0 && ANVector.Y == 0) //влево
                {
                    nums.setSquare_Left_Or_LeftDown(areasCount, area);
                    numsList.Add(nums);
                }
            }
        }
        public Point getAntiNormalVector(Point SP, Point EP)
        {
            EP.X = EP.X - SP.X; EP.Y = EP.Y - SP.Y;//конец минус начало
            int a = -EP.Y; int b = EP.X;
            EP.X = a; EP.Y = b;
            return EP;//возврат координат конца вектора, повернутые на 90 градусов во внеш.сторону
        }
        public bool isAreaNotContainsBaseLine(Area area, Point SP, Point EP)
        {
            int one, two, three, four;
            one = Rotate(SP, EP, area.getVertice(0));
            two = Rotate(SP, EP, area.getVertice(1));
            three = Rotate(SP, EP, area.getVertice(2));
            four = Rotate(SP, EP, area.getVertice(3));
            if ((one > 0 && two > 0 && three > 0 && four > 0) ||
                (one < 0 && two < 0 && three < 0 && four < 0))
            {
                return true;
            }
            return false;
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
        public List<Nums> addAreasBypass(List<Nums> numslist, int areasCount, Point SP, Point EP, Point ANVector)//задается нужный порядок обхода от базовой линии
        {
            List<Nums> numslist2 = new List<Nums>();
            for (int i = 0; i < numslist.Count; i++)
            {
                for (int j = 0; j < numslist[i].numsList.Count; j++)
                {
                    if (!alreadyAddedAreas.Contains(numslist[i].numsList[j]) && !isAreaBehindBaseLine(areasList[numslist[i].numsList[j]], SP, EP))
                    {
                        alreadyAddedAreas.Add(numslist[i].numsList[j]);
                        addNums(ANVector, areasCount, numslist[i].numsList[j], numslist2);
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            return numslist2;
        }
        public bool trySetTriangle(List<Nums> numsList, Point A, Point B)
        {
            areasByPass.Clear();
            for (int i = 0; i < numsList.Count; i++)
            {
                areasByPass.Add(numsList[i].mainNum);
            }
            bool isTriangleSetted = setTriangle(A, B, areasByPass, numsList);
            return isTriangleSetted;
        }
        public void TriangulationFull(Queue<Point> lineQueue, int areasCount)
        {
            if (lineQueue.Count >= 2)
            {
                while (lineQueue.Count > 0)
                {
                    alreadyAddedAreas.Clear();
                    Point A = lineQueue.Dequeue();
                    Point B = lineQueue.Dequeue();
                    Point ANVector = new Point();
                    ANVector = getAntiNormalVector(A, B);
                    List<Nums> numsList = new List<Nums>();
                    getNumsByBaseLine(A, B, numsList);//вызов поиска областей для базовой линии
                    bool isTriangleSetted = trySetTriangle(numsList, A, B);
                    while (isTriangleSetted == false && areasByPass.Count > 0)
                    {
                        numsList = addAreasBypass(numsList, areasCount, A, B, ANVector);
                        isTriangleSetted = trySetTriangle(numsList, A, B);
                    }
                }
                areasByPass.Clear();
                alreadyAddedAreas.Clear();
            }
        }
        public double getLineLength(Point A, Point B)
        {
            double xVal = Math.Pow(B.X - A.X, 2);
            double yVal = Math.Pow(B.Y - A.Y, 2);
            double dist = Math.Sqrt(xVal + yVal);
            return dist;
        }
        public bool setTriangle(Point A, Point B, List<int> areasByPass, List<Nums> numsList)
        {
            double AB = getLineLength(A, B); //c
            double finalGamma = 0;
            List<Point> newListPoints = new List<Point>();
            //формируем лучших кандидатов на проверку
            List<PointAndAngle> bestListPoints = new List<PointAndAngle>();
            //задаем дефолтные pairs для которых будем искать лучшую точку и менять их поле
            Pair newPair1 = new Pair(A, A);
            Pair newPair2 = new Pair(B, B);
            //заполняем лист лучших точек
            for (int i = 0; i < areasByPass.Count; i++)
            {
                newListPoints = areasList[areasByPass[i]].getListPoints();
                for (int k = 0; k < newListPoints.Count; k++)//перебор листа поинтов area
                {
                    //проверяем, нужна ли нам эта точка
                    int val = Rotate(A, B, newListPoints[k]);
                    if (val < 0)
                    {
                        PointAndAngle pointAndAngle = new PointAndAngle(newListPoints[k]);
                        //вычисляем для каждой нужной точки угол
                        double AC = getLineLength(A, newListPoints[k]); //b
                        double CB = getLineLength(newListPoints[k], B); //a
                        double gamma = Math.Acos((CB * CB + AC * AC - AB * AB) / (2 * CB * AC)) * 180 / Math.PI;
                        pointAndAngle.setAngle(gamma);
                        bestListPoints.Add(pointAndAngle);
                        if (gamma > finalGamma)
                        {
                            finalGamma = gamma;
                        }
                    }
                }
            }
            if (finalGamma > 0)
            {
                //сортируем список по углу
                AngleDescendingComparer angleDescComparer = new AngleDescendingComparer();
                bestListPoints.Sort(angleDescComparer);
                //перебераем отсортированный список
                foreach (PointAndAngle p in bestListPoints)
                {
                    //варьируем поля для pair и проверяем на пересечение
                    Pair newPairVarite1 = new Pair(A, p.point);
                    Pair newPairVarite2 = new Pair(p.point, B);
                    bool isnewPairVarite1Crosses = areCrossing(newPairVarite1.Point1, newPairVarite1.Point2, numsList);
                    bool isnewPairVariate2Crosses = areCrossing(newPairVarite2.Point1, newPairVarite2.Point2, numsList);
                    //если нет пересечения, делаем проверку на наличие в сете и добавляем в очередь
                    if (!isnewPairVarite1Crosses && !isnewPairVariate2Crosses)
                    {
                        //проверяем наличие pair. Pair должен присутствовать единожды в множестве, поэтому пользуемся инвертированием
                        Pair newPairInverted1 = new Pair(p.point, A);
                        Pair newPairInverted2 = new Pair(B, p.point);
                        //изменяем дефолтные pairs
                        newPair1.Point2 = p.point;
                        newPair2.Point1 = p.point;
                        if (!pairsSet.Contains(newPair1) && !pairsSet.Contains(newPairInverted1))
                        {
                            lineQueue.Enqueue(A);
                            lineQueue.Enqueue(p.point);
                            pairsSet.Add(newPair1);
                            addPairToAreas(newPair1.Point1, newPair1.Point2);
                        }
                        if (!pairsSet.Contains(newPair2) && !pairsSet.Contains(newPairInverted2))
                        {
                            lineQueue.Enqueue(p.point);
                            lineQueue.Enqueue(B);
                            pairsSet.Add(newPair2);
                            addPairToAreas(newPair2.Point1, newPair2.Point2);
                        }
                        return true;
                    }
                    else
                    {
                        continue;
                    }
                }
                return false;
            }
            else
            {
                return false;
            }
        }
        public long vectorMult(int ax, int ay, int bx, int by) //векторное произведение
        {
            long result = ax * by - bx * ay;
            if (result > 0)
            {
                return 1;
            }
            if (result < 0)
            {
                return -1;
            }
            return 0;
        }
        public bool areCrossing(Point p1, Point p2, List<Nums> numsList)//проверка пересечения
        {
            for (int i = 0; i < numsList.Count; i++)
            {
                for (int j = 0; j < areasList[numsList[i].mainNum].pairsList.Count; j++)
                {
                    Pair p = areasList[numsList[i].mainNum].pairsList[j];

                    long v1 = vectorMult(p.Point2.X - p.Point1.X, p.Point2.Y - p.Point1.Y, p1.X - p.Point1.X, p1.Y - p.Point1.Y);
                    long v2 = vectorMult(p.Point2.X - p.Point1.X, p.Point2.Y - p.Point1.Y, p2.X - p.Point1.X, p2.Y - p.Point1.Y);
                    long v3 = vectorMult(p2.X - p1.X, p2.Y - p1.Y, p.Point1.X - p1.X, p.Point1.Y - p1.Y);
                    long v4 = vectorMult(p2.X - p1.X, p2.Y - p1.Y, p.Point2.X - p1.X, p.Point2.Y - p1.Y);
                    if ((v1 * v2) < 0 && (v3 * v4) < 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        public void drawLine()
        {
            Graphics flagGraphics = Graphics.FromImage(bmp);
            Pen bluePen = new Pen(Color.Blue, 1);
            foreach (Pair p in pairsSet)
            {
                flagGraphics.DrawLine(bluePen, p.Point1, p.Point2);
            }
        }
        private void genButton_Click(object sender, EventArgs e)
        {
            areasCount = Convert.ToInt32(textBox1.Text);
            pointsCount = Convert.ToInt32(textBox2.Text);
            for(int i = 0; i < 1; i++)
            {
                CreateBitmapAtRuntime();
                GenerateAreas();
                DrawAreas();
                GeneratePoints();
                DrawRandomPoints();
                PointBelongs();
                FindBaseLine();
                stopwatch.Start();
                TriangulationFull(lineQueue, areasCount);
                stopwatch.Stop();
                drawLine();
                clearAll();
                TimeSpan ts = stopwatch.Elapsed;
                string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
                sw.WriteLine(elapsedTime);
                //MessageBox.Show(elapsedTime);
                stopwatch.Reset();
            }
            sw.Close();
            MessageBox.Show("!");
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
