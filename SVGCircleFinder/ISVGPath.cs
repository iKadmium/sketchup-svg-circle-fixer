using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    interface IClosedShape
    {
        System.Windows.Shapes.Shape GetShape();
        XElement Serialize();
        List<Line> Lines { get; }
        string DebugText { get; }
    }
}
