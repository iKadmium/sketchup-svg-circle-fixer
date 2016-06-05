using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace SVGCircleFinder
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SVGLoader loader;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Load(string filename)
        {
            loader = new SVGLoader(filename);
            XDocument doc = loader.Serialize();
            string directory = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(filename), "fixed");
            if(!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }
            string destination = System.IO.Path.Combine(directory, System.IO.Path.GetFileName(filename));
            doc.Save(destination);

            originalCanvas.Children.Clear();
            fixedCanvas.Children.Clear();

            foreach (Shape shape in loader.GetOriginalLines())
            {
                originalCanvas.Children.Add(shape);
            }

            foreach (Shape shape in loader.GetFixedShapes())
            {
                fixedCanvas.Children.Add(shape);
            }
            infoLabel.Content = loader.Info;
        }

        private void Canvas_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            Canvas canvas = sender as Canvas;
            double scaleRate = 1.1;
            if (e.Delta < 0)
            {
                scaleRate = 1 / scaleRate;
            }
            
            foreach (ScaleTransform transform in new[] { originalCanvasScale, fixedCanvasScale })
            {
                transform.ScaleX *= scaleRate;
                transform.ScaleY *= scaleRate;
            }
        }

        private void Grid_Drop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            Load(files.First());
        }

        private void Grid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void Grid_DragOver(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.Copy;
        }

        private void tabControl_MouseMove(object sender, MouseEventArgs e)
        {
            IInputElement relative;
            if(tabOriginal.IsSelected)
            {
                relative = originalCanvas;
            }
            else
            {
                relative = fixedCanvas;
            }
            System.Windows.Point position = e.GetPosition(relative);
            statusLabel.Content = position.ToString();
        }
    }
}
