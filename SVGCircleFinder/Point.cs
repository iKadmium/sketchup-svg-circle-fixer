using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace SVGCircleFinder
{
    class Point
    {
        public double X { get; set; }
        public double Y { get; set; }

        public Point(double x, double y)
        {
            X = x;
            Y = y;
        }
        
        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + X.GetHashCode();
            hash = (hash * 7) + Y.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Point other = obj as Point;
            if ((object)other == null)
            {
                return false;
            }

            return (other.X.ApproxEquals(X) && other.Y.ApproxEquals(Y));
        }

        public static bool operator==(Point first, Point other)
        {
            if (ReferenceEquals(first, other))
            {
                return true;
            }

            if (((object)first == null) || ((object)other == null))
            {
                return false;
            }
            
            return first.Equals(other);
        }

        public double Distance(Point other)
        {
            double xSquared = Math.Pow(other.X - X, 2);
            double ySquared = Math.Pow(other.Y - Y, 2);

            double totalSquared = xSquared + ySquared;
            return Math.Sqrt(totalSquared);
        }

        public static bool operator!=(Point first, Point other)
        {
            return !(first == other);
        }

        public System.Windows.Point GetShape()
        {
            return new System.Windows.Point(X, Y);
        }
    }

    public static class Extensions
    {
        public static bool ApproxEquals(this double first, double second, double errorMargin = 0.0001)
        {
            return Math.Abs(second - first) < errorMargin;
        }
    }
}
