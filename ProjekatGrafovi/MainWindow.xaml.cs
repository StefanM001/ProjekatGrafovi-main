using QuickGraph;
using QuickGraph.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace ProjekatGrafovi
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}

		public static Dictionary<int, Cvor> allVerticles = new Dictionary<int, Cvor>();
		public static List<Cvor> verticlesList = new List<Cvor>();
		public static List<Grana> edgesList = new List<Grana>();
		public static Random rand = new Random();
        Dictionary<int, object> nodes = new Dictionary<int, object>();

        private string verticlesString;
		public string VerticlesString
		{
			get
			{
				return verticlesString;
			}
			set
			{
				verticlesString = value;
				OnPropertyChanged("VerticlesString");
			}
		}

		private string edgesString;
		public string EdgesString
		{
			get
			{
				return edgesString;
			}
			set
			{
				edgesString = value;
				OnPropertyChanged("EdgesString");
			}
		}


		public MainWindow()
		{
			InitializeComponent();
			DataContext = this;
		}


		public bool ValidationNoEmptySpace()
		{
			bool valid = true;

			if(VerticlesString.Equals("") || VerticlesString == null)
			{
				valid = false;
				verticles.BorderBrush = Brushes.Red;
				verticles.BorderThickness = new Thickness(3);
			}

			if (EdgesString.Equals("") || EdgesString == null)
			{
				valid = false;
				edges.BorderBrush = Brushes.Red;
				edges.BorderThickness = new Thickness(5);
			} 


			return valid;
		}

		private void StartAgain()
		{
			VerticlesString = "";
			EdgesString = "";
			edgesList.Clear();
			allVerticles.Clear();
			verticlesList.Clear();
		}

		private void AddNewVertex(string[] verticlesSplit)
		{
			for (int i = 0; i < verticlesSplit.Length; i++)
			{
				int id;
				if (!Int32.TryParse(verticlesSplit[i], out id))
				{
					MessageBox.Show("Input was not in correct form", "Add new vertex", MessageBoxButton.OK, MessageBoxImage.Error);
					VerticlesString = "";
					EdgesString = "";
					verticles.BorderBrush = Brushes.Red;
					verticles.BorderThickness = new Thickness(3);
					return;
				}
				else
				{
					//int x = rand.Next(50, 700);
					//int y = rand.Next(1, 200);
					int x = 0;
					int y = 0;
					allVerticles.Add(id, new Cvor(id, x, y));
					verticlesList.Add(new Cvor(id, x, y));
				}
			}
		}

		private void AddNewEdge(string[] edgesSplit)
		{
			for (int i = 0; i < edgesSplit.Count(); i++)
			{
				string[] numbers = edgesSplit[i].Split(',');

				int prviID;
				int drugiID;

				if (!Int32.TryParse(numbers[0], out prviID) || !Int32.TryParse(numbers[1], out drugiID))
				{
					MessageBox.Show("Input was not in correct form", "Add new edge", MessageBoxButton.OK, MessageBoxImage.Error);
					VerticlesString = "";
					EdgesString = "";
					edges.BorderBrush = Brushes.Red;
					edges.BorderThickness = new Thickness(5);
					return;
				}
				else
				{
					Cvor prvi = new Cvor();
					Cvor drugi = new Cvor();

					if(prviID == drugiID)
					{
						Grana g = new Grana(allVerticles[prviID], allVerticles[prviID]);
						edgesList.Add(g);
					}
					else
					{
                        if (!allVerticles.ContainsKey(prviID) || !allVerticles.ContainsKey(drugiID))
                        {
                            MessageBox.Show($"Grana sa cvorovima {prviID} i {drugiID} sadrzi neinicijalizovan cvor!");
                            VerticlesString = "";
                            EdgesString = "";
                            return;
                        }
                        else
                        {
                            Grana g = new Grana(allVerticles[prviID], allVerticles[drugiID]);
                            edgesList.Add(g);
                        }
                    }					
                }
            }
        }

		public void CircularLayout(List<Cvor> nodes, List<Grana> edges)
		{
            double centerX = canvas.Width / 2; // X koordinata centra kruga
            double centerY = canvas.Height / 2; // Y koordinata centra kruga
            double radius = Math.Min(canvas.Width, canvas.Height) / 2 - 50; // Rastojanje čvorova od centra kruga

            double angle = 2 * Math.PI / nodes.Count; // Ugao između dva susedna čvora na krugu

			for (int i = 0; i < nodes.Count; i++)
			{
				double theta = i * angle; // Ugao za trenutni čvor

				// Izračunavanje koordinata za čvor na krugu
				double x = centerX + radius * Math.Cos(theta);
				double y = centerY + radius * Math.Sin(theta);

				nodes[i].X = x;
				nodes[i].Y = y;

				if (edges.Find(e => e.prvi.Id == nodes[i].Id) != null)
				{
					edges.Find(e => e.prvi.Id == nodes[i].Id).prvi.X = x;
					edges.Find(e => e.prvi.Id == nodes[i].Id).prvi.Y = y;
				}

				if (edges.Find(e => e.drugi.Id == nodes[i].Id) != null) 
				{
                    edges.Find(e => e.drugi.Id == nodes[i].Id).drugi.X = x;
                    edges.Find(e => e.drugi.Id == nodes[i].Id).drugi.Y = y;
                }
            }

            DrawGraph(nodes, edges);
		}

		private void DrawGraph(List<Cvor> nodes, List<Grana> edges)
		{
            foreach (var edge in edges)
            {
                bool isReverseEdge = edges.Any(e => e.prvi == edge.drugi && e.drugi == edge.prvi);

                if (!isReverseEdge)
                {
                    Line line = new Line();
                    line.X1 = edge.prvi.X;
                    line.Y1 = edge.prvi.Y;
                    line.X2 = edge.drugi.X;
                    line.Y2 = edge.drugi.Y;
                    line.StrokeThickness = 5;
                    line.Stroke = Brushes.Red;
                    canvas.Children.Add(line);
                }
                else
                {
					if(edge.prvi.Id < edge.drugi.Id)
					{
                        Line line = new Line();
                        line.X1 = edge.prvi.X;
                        line.Y1 = edge.prvi.Y;
                        line.X2 = edge.drugi.X;
                        line.Y2 = edge.drugi.Y;
                        line.StrokeThickness = 5;
                        line.Stroke = Brushes.Red;
                        canvas.Children.Add(line);
                    }
					else
					{
                        double centerX = (edge.prvi.X + edge.drugi.X) / 2.0;
                        double centerY = (edge.prvi.Y + edge.drugi.Y) / 2.0;
                        double radius = Math.Abs(edge.drugi.X - edge.prvi.X) / 2.0;

                        System.Windows.Shapes.Path path = new System.Windows.Shapes.Path();
                        path.Stroke = Brushes.Red;
                        path.StrokeThickness = 5;

                        // Definisanje polukružnog luka
                        System.Windows.Media.PathGeometry pathGeometry = new System.Windows.Media.PathGeometry();
                        System.Windows.Media.ArcSegment arcSegment = new System.Windows.Media.ArcSegment(new Point(edge.drugi.X, edge.drugi.Y), new Size(radius, radius), 0, false, System.Windows.Media.SweepDirection.Counterclockwise, true);
                        System.Windows.Media.PathFigure pathFigure = new System.Windows.Media.PathFigure();
                        pathFigure.StartPoint = new Point(edge.prvi.X, edge.prvi.Y);
						pathFigure.IsClosed = false;
                        pathFigure.Segments.Add(arcSegment);
                        pathGeometry.Figures.Add(pathFigure);

                        path.Data = pathGeometry;
                        canvas.Children.Add(path);
                    }
                }
            }

            foreach (var node in nodes)
            {
                Ellipse ellipse1 = new Ellipse();
                ellipse1.Width = 30;
                ellipse1.Height = 30;
                ellipse1.StrokeThickness = 5;
                ellipse1.Stroke = Brushes.Red;
                ellipse1.Fill = Brushes.Red;

                Canvas.SetLeft(ellipse1, node.X - ellipse1.Width / 2);
                Canvas.SetTop(ellipse1, node.Y - ellipse1.Height / 2);

                canvas.Children.Add(ellipse1);

                TextBlock prviText = new TextBlock();
                prviText.Text = node.Id.ToString();
                prviText.Foreground = Brushes.White;
                prviText.FontWeight = FontWeights.Bold;
                prviText.FontSize = 25;
                prviText.TextAlignment = TextAlignment.Center;
                prviText.HorizontalAlignment = HorizontalAlignment.Center;
                prviText.VerticalAlignment = VerticalAlignment.Center;
                prviText.Width = 30;
                prviText.Height = 30;

                double prviX = Canvas.GetLeft(ellipse1) + ellipse1.ActualWidth / 2;
                double prviY = Canvas.GetTop(ellipse1) + ellipse1.ActualHeight / 2;

                Canvas.SetLeft(prviText, prviX - ellipse1.ActualWidth / 2);
                Canvas.SetTop(prviText, prviY - ellipse1.ActualHeight / 2);

                canvas.Children.Add(prviText);
            }

            FileClass fc = new FileClass();

            int idSCV = fc.ReadNumberTxt();
            fc.SaveToSvg(canvas, idSCV);
            idSCV++;
            fc.WriteNumberText(idSCV);
        }        

        private void Generisi_Click(object sender, RoutedEventArgs e)
		{
			if (!ValidationNoEmptySpace())
			{
				MessageBox.Show("You can't leave empty spaces for verticles or edges!", "Error - empty spaces", MessageBoxButton.OK, MessageBoxImage.Warning) ;
			}
			else
			{
				canvas.Children.Clear();

				string[] cvoroviSplit = VerticlesString.Split(',');

				AddNewVertex(cvoroviSplit);

				string[] graneSplit = edgesString.Split(';');

				AddNewEdge(graneSplit);

				//DrawGraph();

				CircularLayout(verticlesList, edgesList);
				//DrawGraph(verticlesList, edgesList);

                verticles.BorderBrush = Brushes.AliceBlue;
				edges.BorderBrush = Brushes.AliceBlue;

				StartAgain();
			}

		}

        /*private void DrawGraph()
		{
			foreach (Grana g in edgesList)
			{
				Line line = new Line();

				line.X1 = g.prvi.X;
				line.Y1 = g.prvi.Y;
				line.X2 = g.drugi.X;
				line.Y2 = g.drugi.Y;

				line.StrokeThickness = 5;

				line.Stroke = Brushes.Red;

				Ellipse ellipse1 = new Ellipse();
				Ellipse ellipse2 = new Ellipse();

				ellipse1.Width = 30;
				ellipse1.Height = 30;
				ellipse1.StrokeThickness = 5;
				ellipse1.Stroke = Brushes.Red;
				ellipse1.Fill = Brushes.Red;

				ellipse2.Width = 30;
				ellipse2.Height = 30;
				ellipse2.StrokeThickness = 5;
				ellipse2.Stroke = Brushes.Red;
				ellipse2.Fill = Brushes.Red;

				Canvas.SetLeft(ellipse1, g.prvi.X * 1.0 - 15);
				Canvas.SetTop(ellipse1, g.prvi.Y * 1.0 - 15);

				Canvas.SetLeft(ellipse2, g.drugi.X * 1.0 - 15);
				Canvas.SetTop(ellipse2, g.drugi.Y * 1.0 - 15);

				TextBlock prviText = new TextBlock();
				prviText.Text = g.prvi.Id.ToString();
				prviText.Foreground = Brushes.White;
				prviText.FontWeight = FontWeights.Bold;
				prviText.FontSize = 25;
				prviText.TextAlignment = TextAlignment.Center;
				prviText.HorizontalAlignment = HorizontalAlignment.Center;
				prviText.VerticalAlignment = VerticalAlignment.Center;
				prviText.Width = 30;
				prviText.Height = 30;

				double prviX = Canvas.GetLeft(ellipse1) + ellipse1.ActualWidth / 2;
				double prviY = Canvas.GetTop(ellipse1) + ellipse1.ActualHeight / 2;

				double drugiX = Canvas.GetLeft(ellipse2) + ellipse2.ActualWidth / 2;
				double drugiY = Canvas.GetTop(ellipse2) + ellipse2.ActualHeight / 2;

				Canvas.SetLeft(prviText, prviX - ellipse1.ActualWidth / 2);
				Canvas.SetTop(prviText, prviY - ellipse1.ActualHeight / 2);

				TextBlock drugiText = new TextBlock();
				drugiText.Text = g.drugi.Id.ToString();
				drugiText.Foreground = Brushes.White;
				drugiText.FontWeight = FontWeights.Bold;
				drugiText.FontSize = 25;
				drugiText.TextAlignment = TextAlignment.Center;
				drugiText.HorizontalAlignment = HorizontalAlignment.Center;
				drugiText.VerticalAlignment = VerticalAlignment.Center;
				drugiText.Width = 30;
				drugiText.Height = 30;

				Canvas.SetLeft(drugiText, drugiX - ellipse2.ActualWidth / 2);
				Canvas.SetTop(drugiText, drugiY - ellipse2.ActualHeight / 2);

				canvas.Children.Add(ellipse1);
				canvas.Children.Add(ellipse2);
				canvas.Children.Add(line);
				canvas.Children.Add(prviText);
				canvas.Children.Add(drugiText);
			}

			FileClass fc = new FileClass();

			int idSCV = fc.ReadNumberTxt();
			fc.SaveToSvg(canvas, idSCV);
			idSCV++;
			fc.WriteNumberText(idSCV);
		}  */
    }
}
