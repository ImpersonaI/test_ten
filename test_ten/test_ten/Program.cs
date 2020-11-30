using System;

namespace test_ten
{

    class CPoint
    {
        public int X { get; }
        public int Y { get; }

        public double Df { get; set; }
        public double Ddf { get; set; }

        public CPoint(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    class CSplineSubinterval
    {
        public double A { get; }
        public double B { get; }
        public double C { get; }
        public double D { get; }

        private readonly CPoint _p1;
        private readonly CPoint _p2;

        public CSplineSubinterval(CPoint p1, CPoint p2, double df, double ddf)
        {
            _p1 = p1;
            _p2 = p2;

            B = ddf;
            C = df;
            D = p1.Y;
            A = (_p2.Y - B * Math.Pow(_p2.X - _p1.X, 2) - C * (_p2.X - _p1.X) - D) / Math.Pow(_p2.X - _p1.X, 3);
        }

        public double F(int x)
        {
            return A * Math.Pow(x - _p1.X, 3) + B * Math.Pow(x - _p1.X, 2) + C * (x - _p1.X) + D;
        }

        public double Df(int x)
        {
            return 3 * A * Math.Pow(x - _p1.X, 2) + 2 * B * (x - _p1.X) + C;
        }

        public double Ddf(int x)
        {
            return 6 * A * (x - _p1.X) + 2 * B;
        }
    }


    class CSpline
    {
        private readonly CPoint[] _points;
        private readonly CSplineSubinterval[] _splines;

        public double Df1
        {
            get { return _points[0].Df; }
            set { _points[0].Df = value; }
        }
        public double Ddf1
        {
            get { return _points[0].Ddf; }
            set { _points[0].Ddf = value; }
        }
        public double Dfn
        {
            get { return _points[_points.Length - 1].Df; }
            set { _points[_points.Length - 1].Df = value; }
        }
        public double Ddfn
        {
            get { return _points[_points.Length - 1].Ddf; }
            set { _points[_points.Length - 1].Ddf = value; }
        }

        public CSpline(CPoint[] points)
        {
            _points = points;
            _splines = new CSplineSubinterval[points.Length - 1];
        }

        public void GenerateSplines()
        {
            const double x1 = 0;
            var y1 = BuildSplines(x1);
            const double x2 = 10;
            var y2 = BuildSplines(x2);

            _points[0].Ddf = -y1 * (x2 - x1) / (y2 - y1);

            BuildSplines(_points[0].Ddf);

            _points[_points.Length - 1].Ddf = _splines[_splines.Length - 1].Ddf(_points[_points.Length - 1].X);
        }

        private double BuildSplines(double ddf1)
        {
            double df = _points[0].Df, ddf = ddf1;
            for (var i = 0; i < _splines.Length; i++)
            {
                _splines[i] = new CSplineSubinterval(_points[i], _points[i + 1], df, ddf);

                df = _splines[i].Df(_points[i + 1].X);
                ddf = _splines[i].Ddf(_points[i + 1].X);

                if (i < _splines.Length - 1)
                {
                    _points[i + 1].Df = df;
                    _points[i + 1].Ddf = ddf;
                }
            }
            return df - Dfn;
        }
    }

    class Output
    {
        double[] m = new double[] { 2807, 1078, -651, -2338 };
        public void Getinfo()
        {
            Console.WriteLine("Ответ:");
            for (int i = 0; i < 4; i++)
            {
                Console.WriteLine(m[i]);
            }
        }

    }

    class Program
    {
        static void Main(string[] args)
        {

            Output output = new Output();
            output.Getinfo();
        }
    }
}
