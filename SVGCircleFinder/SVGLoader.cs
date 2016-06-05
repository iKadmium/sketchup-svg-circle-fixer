using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    class SVGLoader
    {
        List<Line> Lines { get; set; }
        List<IClosedShape> Paths { get; set; }

        public SVGLoader(string filename)
        {
            XDocument document = XDocument.Load(filename);
            string xmlNamespace = "http://www.w3.org/2000/svg";
            XElement groupElement = document.Root.Element(XName.Get("g", xmlNamespace));
            Lines = (from element in groupElement.Elements(XName.Get("line", xmlNamespace))
                     select new Line(element)).ToList();

            Paths = new List<IClosedShape>();
            
            var unpathedLines = from line in Lines.AsParallel()
                                where !Paths.Any(x => x.Lines.Contains(line))
                                select line;

            while(unpathedLines.Count() > 0)
            {
                LinePath path = new LinePath(unpathedLines.First(), unpathedLines);
                if(Circle.IsCircle(path.Lines))
                {
                    Paths.Add(new Circle(path.Lines)); 
                }
                else
                {
                    Paths.Add(path);
                }
            }
        }

        public string Info
        {
            get
            {
                StringBuilder builder = new StringBuilder();
                var circles = from path in Paths
                              where path.GetType() == typeof(Circle)
                              select path as Circle;

                builder.AppendLine(circles.Count() + " Circles found");
                var circleRadii = from circle in circles
                                  group circle by Math.Round(circle.Radius,1) into radii
                                  select radii;
                
                foreach(var radius in circleRadii)
                {
                    builder.AppendLine("\t" + radius.Count() + " of diameter " + (radius.Key * 2));
                }

                return builder.ToString();
            }
        }
        
        public XDocument Serialize()
        {
            XNamespace svg = XNamespace.Get("http://www.w3.org/2000/svg");
            XDocument document = new XDocument(
                new XElement(
                    XName.Get("svg", svg.NamespaceName),
                    new XAttribute("version", "1.1"),
                    from path in Paths
                    select path.Serialize()
                )
            );
            
            return document;
        }

        internal IEnumerable<Shape> GetFixedShapes()
        {
            List<Shape> shapes = new List<Shape>();
            foreach (IClosedShape path in Paths)
            {
                Shape shape = path.GetShape();
                shapes.Add(path.GetShape());
            }
            return shapes;
        }

        public IEnumerable<Shape> GetOriginalLines()
        {
            List<Shape> shapes = new List<Shape>();
            foreach(Line line in Lines)
            {
                Shape shape = line.GetShape();
                shapes.Add(shape);
            }
            return shapes;
        }
        
    }
}
