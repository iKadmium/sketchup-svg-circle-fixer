using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SVGCircleFinder
{
    class Arc
    {
        public List<Line> Lines { get; set; }
        public Point Start { get; set; }
        public Point End { get; set; }
        public double Radius { get; set; }

        public Arc(Line firstLine, IEnumerable<Line> allLines)
        {
            Lines = new List<Line>();

            Line currentLine = firstLine;
            do
            {
                Lines.Add(currentLine);
                currentLine = allLines.FirstOrDefault(
                    x => ((x.Start == currentLine.End) || (x.End == currentLine.Start))
                            && !Lines.Contains(x));
            } while (currentLine != null);

            Start = Lines.First().Start;
            End = Lines.Last().End;

            var circumference = 4 * Lines.Sum(x => x.GetLength());
            Radius = circumference / (2 * Math.PI);

        }
    }
}
