using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    class LinePath : IClosedShape
    {
        public List<Line> Lines { get; set; }
        public List<Line> StraightLines { get; set; }
        public List<Arc> Arcs { get; set; }

        public string DebugText
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                foreach(Line line in Lines)
                {
                    builder.AppendLine(line.ToString());
                }
                return builder.ToString();
            }
        }
        
        public LinePath(Line firstLine, IEnumerable<Line> allLines)
        {
            Lines = new List<Line>();

            Line currentLine = firstLine;
            do
            {
                Lines.Add(currentLine);
                currentLine = allLines.FirstOrDefault(
                    x =>    ((x.Start == currentLine.End) || (x.End == currentLine.Start))
                            && !Lines.Contains(x));
            } while (currentLine != null);
            FindArcs();
        }

        public void FindArcs()
        {
            var lengths = Lines.Select(x => x.GetLength()).OrderBy(x => x);
            double minLength = Lines.Min(x => x.GetLength());
            var shortLines = Lines.Where(x => x.GetLength().ApproxEquals(minLength, 0.01));
            StraightLines = Lines.Except(shortLines).ToList();

            Arcs = new List<Arc>();

            var unpathedLines = from line in shortLines.AsParallel()
                                where !Arcs.Any(x => x.Lines.Contains(line))
                                select line;

            while (unpathedLines.Count() > 0)
            {
                Arc arc = new Arc(unpathedLines.First(), unpathedLines);
                Arcs.Add(arc);
            }
        }

        public XElement Serialize()
        {
            XNamespace svg = XNamespace.Get("http://www.w3.org/2000/svg");
            StringBuilder pathDataBuilder = new StringBuilder();
            pathDataBuilder.Append("M " + Lines.First().Start.X + " " + Lines.First().Start.Y + " ");
            foreach(Line line in Lines)
            {
                pathDataBuilder.Append("L " + line.End.X + " " + line.End.Y + " ");
            }
            return new XElement(XName.Get("path", svg.NamespaceName),
                new XAttribute("fill", "none"),
                new XAttribute("stroke", "#000000"),
                new XAttribute("stroke-width", "1"),
                new XAttribute("d", pathDataBuilder.ToString())
            );
        }

        public Shape GetShape()
        {
            Path path = new Path();
            PathGeometry geometry = new PathGeometry();
            PathFigure figure = new PathFigure();
            figure.StartPoint = Lines.First().Start.GetShape();
            foreach (Line line in Lines)
            {
                LineSegment segment = new LineSegment(line.End.GetShape(), true);
                figure.Segments.Add(segment);
            }
            geometry.Figures.Add(figure);
            path.Data = geometry;
            path.Stroke = new SolidColorBrush(Colors.Black);
            return path;
        }
    }
}
