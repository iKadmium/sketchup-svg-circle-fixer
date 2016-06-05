using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    class Circle : IClosedShape
    {
        public static double MarginForError = 0.1;

        public Point Centre { get; set; }
        public double Radius { get; set; }
        public List<Line> Lines { get; set; }

        public string DebugText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                builder.AppendLine("Radius: " + Radius);
                builder.AppendLine("Centre: " + Centre.ToString());
                builder.AppendLine();
                foreach (Line line in Lines)
                {
                    builder.AppendLine(line.ToString());
                }
                return builder.ToString();
            }
        }
        
        public Circle(Point centre, double radius)
        {
            Centre = centre;
            Radius = radius;
            Lines = new List<Line>();
        }

        public Circle(List<Line> lines)
        {
            double averageX = lines.Average(x => x.Start.X);
            double averageY = lines.Average(x => x.Start.Y);

            Centre = new Point(averageX, averageY);
            var distances = from line in lines
                            select line.Start.Distance(Centre);
            Radius = distances.Average();

            Lines = lines;
        }

        public static bool IsCircle(IEnumerable<Line> lines)
        {
            double minX = lines.Min(x => x.Start.X);
            double maxX = lines.Max(x => x.Start.X);
            double minY = lines.Min(x => x.Start.Y);
            double maxY = lines.Max(x => x.Start.Y);

            double width = maxX - minX;
            double height = maxY - minY;

            if(width.ApproxEquals(height))
            {
                double averageX = lines.Average(x => x.Start.X);
                double averageY = lines.Average(x => x.Start.Y);

                //theoretical centre = this point
                Point centre = new Point(averageX, averageY);
                var distances = from line in lines
                                select line.Start.Distance(centre);
                var averageDistance = distances.Average();
                var maxDifference = distances.Max() - averageDistance;
                if(maxDifference < 0.0001)
                {
                    return true;
                }
            }

            return false;
        }

        public XElement Serialize()
        {
            XNamespace svg = XNamespace.Get("http://www.w3.org/2000/svg");
            return new XElement(
                XName.Get("circle", svg.NamespaceName),
                new XAttribute("cx", Math.Round(Centre.X, 1)),
                new XAttribute("cy", Math.Round(Centre.Y, 1)),
                new XAttribute("r", Math.Round(Radius, 1)),
                new XAttribute("stroke", "#000000"),
                new XAttribute("fill", "none")
            );
        }

        public Shape GetShape()
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = Radius * 2;
            ellipse.Height = Radius * 2;
            ellipse.Stroke = new SolidColorBrush(Colors.Blue);

            Canvas.SetLeft(ellipse, Centre.X - Radius);
            Canvas.SetTop(ellipse, Centre.Y - Radius);
            return ellipse;
        }
    }
}
