using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGraph
{
    class Vector2
    {
        public double X { get; set; }
        public double Y { get; set; }


        public Vector2()
        {
            X = 0;
            Y = 0;
        }
        public Vector2(Vector2 v)
        {
            X = v.X;
            Y = v.Y;
        }
        public Vector2(double x, double y)
        {
            X = x;
            Y = y;
        }
        ////////////////////////////

        public Vector2 Add(Vector2 v)
        {
            X += v.X;
            Y += v.Y;
            return this;
        }
        static public Vector2 Add(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X + v.X, u.Y + v.Y);
        }

        public Vector2 Subtract(Vector2 v)
        {
            X -= v.X;
            Y -= v.Y;
            return this;
        }
        static public Vector2 Subtract(Vector2 u, Vector2 v)
        {
            return new Vector2(u.X - v.X, u.Y - v.Y);
        }

        public Vector2 Divide(double k)
        {
            X /= k;
            Y /= k;
            return this;
        }
        static public Vector2 Divide(Vector2 u, double k)
        {
            return new Vector2(u.X / k, u.Y / k);
        }


        public Vector2 Multiply(double k)
        {
            X *= k;
            Y *= k;
            return this;
        }
        static public Vector2 Multiply(Vector2 u, double k)
        {
            return new Vector2(u.X * k, u.Y * k);
        }

        static public double Distance(Vector2 u, Vector2 v)
        {
            return Math.Sqrt(u.X - v.X) * (u.X - v.X) + (u.Y - v.Y) * (u.Y - v.Y);
        }

    }
}
