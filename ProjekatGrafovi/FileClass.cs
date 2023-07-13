using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Xml;

namespace ProjekatGrafovi
{
    public class FileClass
    {
		public int ReadNumberTxt()
		{
			if (!File.Exists("IDCsv.txt"))
			{
				File.Create("IDCsv.txt").Dispose();

				using (TextWriter tw = new StreamWriter("IDCsv.txt"))
				{
					tw.WriteLine("0");
				}
			}
			else if (File.Exists("IDCsv.txt"))
			{
				string content = File.ReadAllText("IDCsv.txt", Encoding.UTF8);

				return Int32.Parse(content);
			}
			return 0;
		}

		public void WriteNumberText(int id)
		{
			if (!File.Exists("IDCsv.txt"))
			{
				File.Create("IDCsv.txt").Dispose();

				using (TextWriter tw = new StreamWriter("IDCsv.txt"))
				{
					tw.WriteLine("0");
				}

			}
			else if (File.Exists("IDCsv.txt"))
			{
				using (TextWriter tw = new StreamWriter("IDCsv.txt"))
				{
					tw.WriteLine(id);
				}
			}
		}

		public void SaveToSvg(Canvas canvas, int idSCV)
		{
			XmlDocument doc = new XmlDocument();
			XmlElement root = doc.CreateElement("svg");
			root.SetAttribute("version", "1.1");
			root.SetAttribute("width", "750");
			root.SetAttribute("height", "250");
			root.SetAttribute("xmlns", "http://www.w3.org/2000/svg");

			foreach(UIElement childCanvas in canvas.Children)
			{
				if(childCanvas.GetType() == typeof(Line))
				{
					Line line = (Line)childCanvas;
					XmlElement l = doc.CreateElement("line");
					string x1 = line.X1.ToString();
					l.SetAttribute("x1", x1);
					l.SetAttribute("y1", line.Y1.ToString());
					l.SetAttribute("x2", line.X2.ToString());
					l.SetAttribute("y2", line.Y2.ToString());
					l.SetAttribute("stroke", "red");
					root.AppendChild(l);
				}
			}

            foreach (UIElement childCanvas in canvas.Children)
            {
                if (childCanvas is System.Windows.Shapes.Path)
                {
                    System.Windows.Shapes.Path path = (System.Windows.Shapes.Path)childCanvas;
                    System.Windows.Media.PathGeometry pathGeometry = path.Data as System.Windows.Media.PathGeometry;
                    if (pathGeometry != null && pathGeometry.Figures.Count > 0)
                    {
                        System.Windows.Media.PathFigure pathFigure = pathGeometry.Figures[0];

                        System.Windows.Point startPoint = pathFigure.StartPoint;
                        string dAttribute = $"M {startPoint.X},{startPoint.Y}";

                        foreach (System.Windows.Media.PathSegment segment in pathFigure.Segments)
                        {
                            if (segment is System.Windows.Media.ArcSegment arcSegment)
                            {
                                double radiusX = arcSegment.Size.Width;
                                double radiusY = arcSegment.Size.Height;
                                double xAxisRotation = arcSegment.RotationAngle;
                                bool isLargeArc = arcSegment.IsLargeArc;
                                bool isClockwise = arcSegment.SweepDirection == System.Windows.Media.SweepDirection.Clockwise ? true : false;
                                double endX = arcSegment.Point.X;
                                double endY = arcSegment.Point.Y;

                                string arcFlag = isLargeArc ? "1" : "0";
                                string sweepFlag = isClockwise ? "1" : "0";

                                dAttribute += $" A {radiusX},{radiusY} {xAxisRotation} {arcFlag},{sweepFlag} {endX},{endY}";
                            }
                        }

                        XmlElement pathElement = doc.CreateElement("path");
                        pathElement.SetAttribute("d", dAttribute);
                        pathElement.SetAttribute("stroke", "red");
						pathElement.SetAttribute("fill", "white");
                        root.AppendChild(pathElement);
                    }
                }
            }

            foreach (UIElement childCanvas in canvas.Children)
            {
                if (childCanvas is Ellipse)
                {
                    Ellipse ellipse = (Ellipse)childCanvas;
                    double centerX = Canvas.GetLeft(ellipse) + ellipse.Width / 2.0;
                    double centerY = Canvas.GetTop(ellipse) + ellipse.Height / 2.0;

                    XmlElement ellipseElement = doc.CreateElement("ellipse");
                    ellipseElement.SetAttribute("cx", centerX.ToString());
                    ellipseElement.SetAttribute("cy", centerY.ToString());
                    ellipseElement.SetAttribute("rx", (ellipse.Width / 2.0).ToString());
                    ellipseElement.SetAttribute("ry", (ellipse.Height / 2.0).ToString());
                    ellipseElement.SetAttribute("stroke", "red");
                    ellipseElement.SetAttribute("fill", "white"); // Postavljanje boje na belu
                    root.AppendChild(ellipseElement);
                }
            }

            foreach (UIElement childCanvas in canvas.Children)
            {
                if (childCanvas is TextBlock)
                {
                    TextBlock textBlock = (TextBlock)childCanvas;
                    double left = Canvas.GetLeft(textBlock);
                    double top = Canvas.GetTop(textBlock);
                    string text = textBlock.Text;

                    XmlElement textElement = doc.CreateElement("text");
                    textElement.SetAttribute("x", (left+11).ToString());
                    textElement.SetAttribute("y", (top + 17).ToString());
                    textElement.SetAttribute("fill", "black");
                    textElement.SetAttribute("font-size", "19");
                    textElement.InnerText = text;
                    root.AppendChild(textElement);
                }
            }

            doc.AppendChild(root);

			string filePath = $"ProjekatGraf{idSCV}.svg";
			doc.Save(filePath);

		}
    }
}
