using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    class Line
    {
        public static double MarginForError = 0.1;

        public Point Start { get; set; }
        public Point End { get; set; }

        public Line(XElement element)
        {
            Start = new Point(double.Parse(element.Attribute("x1").Value), double.Parse(element.Attribute("y1").Value));
            End = new Point(double.Parse(element.Attribute("x2").Value), double.Parse(element.Attribute("y2").Value));
        }

        public override string ToString()
        {
            return Start.ToString() + " - " + End.ToString();
        }

        public override int GetHashCode()
        {
            int hash = 13;
            hash = (hash * 7) + Start.GetHashCode();
            hash = (hash * 7) + End.GetHashCode();
            return hash;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
            {
                return false;
            }
            Line other = obj as Line;
            if ((object)other == null)
            {
                return false;
            }

            return (other.Start == Start && other.End == End);
        }

        public double GetLength()
        {
            return Start.Distance(End);
        }

        public static bool operator ==(Line first, Line other)
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

        public XElement Serialize()
        {
            XNamespace svg = XNamespace.Get("http://www.w3.org/2000/svg");
            return new XElement(XName.Get("line", svg.NamespaceName),
                new XAttribute("x1", Math.Round(Start.X, 1)),
                new XAttribute("y1", Math.Round(Start.Y)),
                new XAttribute("x2", Math.Round(End.X)),
                new XAttribute("y2", Math.Round(End.Y))
            );
        }

        public static bool operator !=(Line first, Line other)
        {
            return !(first == other);
        }

        public Shape GetShape()
        {
            System.Windows.Shapes.Line shapeLine = new System.Windows.Shapes.Line() { X1 = Start.X, Y1 = Start.Y, X2 = End.X, Y2 = End.Y };
            shapeLine.Stroke = new System.Windows.Media.SolidColorBrush(System.Windows.Media.Colors.Black);
            return shapeLine;
        }
    }
}
