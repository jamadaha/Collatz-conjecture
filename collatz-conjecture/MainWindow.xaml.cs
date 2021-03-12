using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
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

namespace collatz_conjecture
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		int posRot = 10;
		int negRot = 5;

		void GenerateNewTree()
		{
			DeleteTree();
			GenerateTree();
		}

		void GenerateTree()
		{
			int branchCount = (int) BranchCountSlider.Value;

			List<List<JansLinje>> jansLinjer = new List<List<JansLinje>>();
			
			for (int i = 1; i <= branchCount; i++)
				jansLinjer.Add(CreateBranch(i));

			foreach(var item in jansLinjer)
			{
				DrawLines(item);
			}


		}

		void DeleteTree()
		{
			root.Children.Clear();
		}

		int CalculateNextNumber(int number)
		{
			if(number % 2 == 0)
				return number / 2;
			else
				return number * 3 + 1;
		}

		Point CalculateNewPoint(Point startPoint, double newAngle, double lineLength)
		{
			double x3 = startPoint.X + lineLength * Math.Cos(newAngle*(Math.PI/180));
			double y3 = startPoint.Y + lineLength * Math.Sin(newAngle*(Math.PI/180));

			return new Point(x3, y3);
		}

		Random random = new Random();

		List<JansLinje> CreateBranch(int startNum)
		{
			int lastNum;
			int accNum = startNum;
			double angle = 0;
			double lineLength = 40;
			Point lastPoint = new Point(0, 0);
			double h = 0;
			double s = 0.3;
			double v = 1;
			List<JansLinje> jansLinjer = new List<JansLinje>();

			while(accNum != 1)
			{
				lastNum = accNum;
				accNum = CalculateNextNumber(accNum);

				if(accNum > lastNum)
					angle += random.NextDouble() * posRot;
				else
					angle -= random.NextDouble() * negRot;

				Point newPoint = CalculateNewPoint(lastPoint, angle, lineLength);
				h += 3;

				jansLinjer.Add(new JansLinje(lastPoint, newPoint, new SolidColorBrush(HsvToRgb(h, s, v))));


				lastPoint = newPoint;
			}
			return jansLinjer;
		}

		private void DrawLines(List<JansLinje> lines)
		{
			foreach(JansLinje item in lines)
			{

				root.Children.Add(CreateLine(item));
			}
		}

		Line CreateLine(JansLinje jansLinje)
		{
			Line line = new Line();
			line.Stroke = jansLinje.color;
			line.StrokeThickness = 5;
			line.X1 = jansLinje.startPos.X + Width / 2;
			line.Y1 = jansLinje.startPos.Y + Height / 2;
			line.X2 = jansLinje.endPos.X + Width / 2;
			line.Y2 = jansLinje.endPos.Y + Height / 2;
			//line.HorizontalAlignment = HorizontalAlignment.Center;
			//line.VerticalAlignment = VerticalAlignment.Center;
			//root.Children.Add(line);
			return line;
		}

		Color HsvToRgb(double h, double S, double V)
		{
			double H = h;
			while(H < 0) { H += 360; };
			while(H >= 360) { H -= 360; };
			double R, G, B;
			if(V <= 0)
			{ R = G = B = 0; } else if(S <= 0)
			{
				R = G = B = V;
			} else
			{
				double hf = H / 60.0;
				int i = (int)Math.Floor(hf);
				double f = hf - i;
				double pv = V * (1 - S);
				double qv = V * (1 - S * f);
				double tv = V * (1 - S * (1 - f));
				switch(i)
				{

					// Red is the dominant color

					case 0:
						R = V;
						G = tv;
						B = pv;
						break;

					// Green is the dominant color

					case 1:
						R = qv;
						G = V;
						B = pv;
						break;
					case 2:
						R = pv;
						G = V;
						B = tv;
						break;

					// Blue is the dominant color

					case 3:
						R = pv;
						G = qv;
						B = V;
						break;
					case 4:
						R = tv;
						G = pv;
						B = V;
						break;

					// Red is the dominant color

					case 5:
						R = V;
						G = pv;
						B = qv;
						break;

					// Just in case we overshoot on our math by a little, we put these here. Since its a switch it won't slow us down at all to put these here.

					case 6:
						R = V;
						G = tv;
						B = pv;
						break;
					case -1:
						R = V;
						G = pv;
						B = qv;
						break;

					// The color is not defined, we should throw an error.

					default:
						//LFATAL("i Value error in Pixel conversion, Value is %d", i);
						R = G = B = V; // Just pretend its black/white
						break;
				}
			}
			byte r;
			byte g;
			byte b;


			r = (byte)Clamp((int)(R * 255.0));
			g = (byte)Clamp((int)(G * 255.0));
			b = (byte)Clamp((int)(B * 255.0));

			return Color.FromRgb(r, g, b);

		}

		/// <summary>
		/// Clamp a value to 0-255
		/// </summary>
		int Clamp(int i)
		{
			if(i < 0) return 0;
			if(i > 255) return 255;
			return i;
		}

		private void OnBranchCountChange(object sender, RoutedPropertyChangedEventArgs<double> e)
		{
			GenerateNewTree();
		}

		private static bool IsTextAllowed(string text)
		{
			int i;
			return int.TryParse(text, out i) && i >= 0 && i <= 359;
		}

		private void ValdlidateRotationChange(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !IsTextAllowed(((TextBox)sender).Text + e.Text);
		}

		private void OnPosRotationChange(object sender, TextChangedEventArgs e)
		{
			string text = ((TextBox)sender).Text;
			if(IsTextAllowed(text))
			{
				posRot = int.Parse(text);
				GenerateNewTree();
			}
		}

		private void OnNegRotationChange(object sender, TextChangedEventArgs e)
		{
			string text = ((TextBox)sender).Text;
			if (IsTextAllowed(text))
			{
				negRot = int.Parse(text);
				GenerateNewTree();
			}
		}
	}
}
