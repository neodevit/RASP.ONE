using RaspaDB;
using RaspaEntity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RaspaCentral
{
	public sealed partial class GPIO : UserControl
	{
		public GPIO()
		{
			this.InitializeComponent();
			
		}

		public void drawGPIO_PIR(int GPIOpin)
		{
			drawGPIOcomp(enumComponente.pir, GPIOpin, 0, 0);
		}
		public void drawGPIO_LIGHT(int GPIOpin)
		{
			drawGPIOcomp(enumComponente.light, GPIOpin, 0, 0);
		}
		public void drawGPIO_TEMP(int GPIOpin)
		{
			drawGPIOcomp(enumComponente.temperature, GPIOpin, 0, 0);
		}
		public void drawGPIO_UMIDITY(int GPIOpin)
		{
			drawGPIOcomp(enumComponente.umidity, GPIOpin, 0, 0);
		}

		public void drawGPIO_PUSH(int GPIOpin)
		{
			drawGPIOcomp(enumComponente.push, GPIOpin, 0, 0);
		}

		private void drawGPIOcomp(enumComponente componente,int GPIO_A,int GPIO_B, int GPIO_C)
		{
			// disegna pin gpio
			drawGPIOPin();

			DBCentral DB = new DBCentral();
			// disegna collegamenti
			switch (componente)
			{
				case enumComponente.nessuno:
					break;
				case enumComponente.nodo:
				case enumComponente.centrale:
					break;
				case enumComponente.light:
					// COMPOONENTE PHOTO
					schemaPhoto("ms-appx:///Assets/component_light.png", 17, 280, 190, 190);

					// POWER 
					schemaConnection(BASE, 2, enumTipoPIN.power, "S1", 270, 145);
					// GROUND
					schemaConnection(BASE, 6, enumTipoPIN.ground, "S2", 285, 165);
					// SIGNAL
					GPIOPin light = DB.GetGPIOPinByGPIOnum(GPIO_A);
					schemaConnection(BASE, light.NUM, enumTipoPIN.gpio, "S3", 300, 185);
					// -------------------------------------------------------------------------------------------
					schemaPhoto("ms-appx:///Assets/component_light2.png", 190, 310, 230, 230);


					break;
				case enumComponente.pir:
					// COMPOONENTE PHOTO
					schemaPhoto("ms-appx:///Assets/component_pir.png", 17, 257, 200, 381);

					// GND
					schemaConnection(BASE, 6, enumTipoPIN.ground, "S1", 358, 280);
					// SIGNAL
					GPIOPin pir = DB.GetGPIOPinByGPIOnum(GPIO_A);
					schemaConnection(BASE, pir.NUM, enumTipoPIN.gpio, "S2", 335, 280);
					// POWER
					schemaConnection(BASE, 2, enumTipoPIN.power, "S3", 310, 280);
					break;

				case enumComponente.temperature:
				case enumComponente.umidity:
					// COMPOONENTE PHOTO
					schemaPhoto("ms-appx:///Assets/component_temp.png", 17, 257, 200, 381);

					// GND
					schemaConnection(BASE, 6, enumTipoPIN.ground, "S1", 340, 250);
					// SIGNAL
					GPIOPin temp = DB.GetGPIOPinByGPIOnum(GPIO_A);
					schemaConnection(BASE, temp.NUM, enumTipoPIN.gpio, "S2", 317, 250);
					// POWER
					schemaConnection(BASE, 2, enumTipoPIN.power, "S3", 292, 250);

					break;
				case enumComponente.push:
					// COMPOONENTE PHOTO
					schemaPhoto("ms-appx:///Assets/component_push.png", 17, 257, 200, 381);

					// GND
					schemaConnection(BASE, 6, enumTipoPIN.ground, "S1", 340, 250);
					// SIGNAL
					GPIOPin push = DB.GetGPIOPinByGPIOnum(GPIO_A);
					schemaConnection(BASE, push.NUM, enumTipoPIN.gpio, "S2", 317, 250);
					// POWER
					schemaConnection(BASE, 2, enumTipoPIN.power, "S3", 292, 250);

					break;
			}
		}
		#region draw GPIO PIN
		private void drawGPIOPin()
		{
			BASE.Children.Clear();

			int statTop = 30;
			int stepTop = 11;
			int X=0;
			int Y=0;
			int Xf = 0;
			int Yf = 0;

			int i= 0;
			for (i=1;i<=40; i++)
			{
				List<string> righe = new List<string>(); ;
				righe.Add(i.ToString());

				// CIRCLE
				X = (isPari(i)) ? 120 : 100;								// se pari metto colonna destra
				Y = (!isPari(i)) ? statTop + (stepTop * i):Y;			// se pari non creo nuova riga
				DrawPIN_CIRCLE(BASE, "pin" + i, X, Y, 16, 1);

				// TESTO CIRCLE
				Xf = (isPari(i)) ? 121 : 101;								// se pari metto colonna destra
				Yf = (!isPari(i)) ? statTop + (stepTop * i)+2 : Yf;     // se pari non creo nuova riga
				TextAlignment just = TextAlignment.Center;
				//TextAlignment just = (!isPari(i)) ? TextAlignment.Right : TextAlignment.Left;
				DrawPIN_TEXT(BASE, "p" + i, Xf, Yf, righe, 8,14, just,null);


			}

			// read pin from DB
			DBCentral DB = new DBCentral();
			GPIOPins pins = DB.GetGPIOPin();
			

			int Xl = 0;
			int Yl = 0;
			int Xq = 0;
			int Yq = 0;
			int TstatTop = 30;
			int TstepTop = 11;
			i = 0;
			// LABEL PINS
			for (int pun = 0; pun <= pins.Count-1; pun++)
			{
				i++;
				int deltaTop = 0;
				List<string> righe = new List<string>(); ;
				SolidColorBrush back = new SolidColorBrush(Colors.Transparent);

				switch(pins[pun].Tipo)
				{
					case enumTipoPIN.gpio:
						righe.Add(pins[pun].NomeGPIO.ToString());
						back = new SolidColorBrush(Colors.PaleGreen);
						deltaTop = 2;
						break;
					case enumTipoPIN.power:
						righe.Add(pins[pun].NomeNUM.ToString());
						back = new SolidColorBrush(Colors.LightSalmon);
						deltaTop = 2;
						break;
					case enumTipoPIN.ground:
						righe.Add(pins[pun].NomeNUM.ToString());
						back = new SolidColorBrush(Colors.Lavender);
						deltaTop = 2;
						break;
					case enumTipoPIN.other:
						righe.Add(pins[pun].NomeNUM.ToString());
						back = new SolidColorBrush(Colors.NavajoWhite);
						deltaTop = 2;
						break;
					case enumTipoPIN.gpioAndOther:
						righe.Add(pins[pun].NomeGPIO.ToString());
						righe.Add(pins[pun].NomeNUM.ToString());
						back = new SolidColorBrush(Colors.AliceBlue);
						deltaTop = -1;
						break;
				}
				// TESTO LABEL
				Xl = (isPari(i)) ? 143 : 0;										// se pari metto colonna destra
				Yl = (!isPari(i)) ? TstatTop + (TstepTop * i)  : Yl;	// se pari non creo nuova riga
				Xq = (isPari(i)) ? 140 : 5;										// se pari metto colonna destra
				Yq = (!isPari(i)) ? TstatTop + (TstepTop * i)  : Yq;			// se pari non creo nuova riga
				TextAlignment justL = (!isPari(i)) ? TextAlignment.Right : TextAlignment.Left;
				// descrizione
				string descrizione = pins[pun].Descrizione;

				DrawPIN_BOX(BASE, Xq, Yq, 90, 16, back);
				DrawPIN_TEXT(BASE, "l" + i, Xl, Yl + deltaTop, righe, 7, 90, justL, descrizione);
			}

		}
		private void DrawPIN_CIRCLE(Canvas root ,string name, double X, double Y,int circle, int Border)
		{
			Ellipse e = new Ellipse();
			e.Name = name;
			e.Fill = new SolidColorBrush(Colors.Transparent);
			e.Stroke = new SolidColorBrush(Colors.SteelBlue);
			e.Opacity = 100;
			e.Height = circle;
			e.Width = circle;
			e.StrokeThickness = Border;
			e.VerticalAlignment = VerticalAlignment.Top;
			e.HorizontalAlignment = HorizontalAlignment.Left;
			Canvas.SetLeft(e, X);
			Canvas.SetTop(e, Y);

			root.Children.Add(e);
		}
		private void DrawPIN_TEXT(Canvas root, string name, double X, double Y, List<string> righe, int Fontsize,int width,  TextAlignment just,string ToolTips)
		{
			TextBlock text = new TextBlock();
			text.Name = name;
			text.FontSize = Fontsize;
			text.TextAlignment = just;
			text.TextLineBounds = TextLineBounds.Full;
			text.Width = width;
			text.Foreground = new SolidColorBrush(Colors.Black);
			text.VerticalAlignment = VerticalAlignment.Center;
			//text.HorizontalAlignment = HorizontalAlignment.Left;
			int i = 0;
			foreach (string riga in righe)
			{
				i++;
				if (i>1)
					text.Inlines.Add(new LineBreak());
				text.Inlines.Add(new Run() { Text = riga, FontWeight = FontWeights.Bold });
			}

			Canvas.SetLeft(text, X);
			Canvas.SetTop(text, Y);

			if (!string.IsNullOrEmpty(ToolTips))
				ToolTipService.SetToolTip(text, ToolTips);

			root.Children.Add(text);
		}
		private void DrawPIN_BOX(Canvas root, double X, double Y, int width,int Height, SolidColorBrush backcolor)
		{
			Rectangle rect = new Rectangle();
			rect.Width = width;
			rect.Height = Height;
			rect.Fill = backcolor;
			Canvas.SetLeft(rect, X);
			Canvas.SetTop(rect, Y);
			root.Children.Add(rect);
		}
		private bool isPari(int Num)
		{
			bool res = false;
			if (Num % 2 == 0)
				res = true;
			return res;
		}
		#endregion
		#region draw GPIO schema
		private void schemaPhoto(string Path,int top,int left,int width,int height)
		{
			Image imgA = new Image();
			imgA.Source = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(Path, UriKind.Absolute));
			imgA.Height = height;
			imgA.Width = width;
			Canvas.SetLeft(imgA, left);
			Canvas.SetTop(imgA, top);
			BASE.Children.Add(imgA);
		}
		private void schemaConnection(Canvas root, int PIN, enumTipoPIN tipo,string name,int left, int top)
		{
			// PIN DISEGNO
			Ellipse e = new Ellipse();
			e.Name = "Elipsc"+name;
			e.Stroke = new SolidColorBrush(Colors.SteelBlue);
			e.Opacity = 100;
			e.Height = 16;
			e.Width = 16;
			e.StrokeThickness = 2;
			e.VerticalAlignment = VerticalAlignment.Top;
			e.HorizontalAlignment = HorizontalAlignment.Left;
			Canvas.SetLeft(e, left);
			Canvas.SetTop(e, top);

			// TESTO
			TextBlock T = new TextBlock();
			T.Name = name;
			T.FontSize = 9;
			T.TextAlignment = TextAlignment.Center;
			T.TextLineBounds = TextLineBounds.Full;
			T.Width = 10;
			T.VerticalAlignment = VerticalAlignment.Center;
			Canvas.SetLeft(T, left+3);
			Canvas.SetTop(T, top+1);

			switch (tipo)
			{
				case enumTipoPIN.gpio:
				case enumTipoPIN.gpioAndOther:
					T.Text = PIN.ToString();
					e.Fill = new SolidColorBrush(Colors.PaleGreen);
					T.Foreground = new SolidColorBrush(Colors.Black);
					break;
				case enumTipoPIN.ground:
					T.Text = "-";
					e.Fill = new SolidColorBrush(Colors.Black);
					T.Foreground = new SolidColorBrush(Colors.White);
					break;
				case enumTipoPIN.power:
					T.Text = "+";
					e.Fill = new SolidColorBrush(Colors.LightSalmon);
					T.Foreground = new SolidColorBrush(Colors.Black);
					break;
				case enumTipoPIN.other:
					T.Text = "!";
					e.Fill = new SolidColorBrush(Colors.NavajoWhite);
					T.Foreground = new SolidColorBrush(Colors.Black);
					break;
			}

			BASE.Children.Add(e);
			BASE.Children.Add(T);

			// DISEGNO GPIO
			Ellipse GPIOCircle = (Ellipse)BASE.FindName("pin" + PIN);
			GPIOCircle.Fill = e.Fill;
			TextBlock GPIOCircleText = (TextBlock)BASE.FindName("p" + PIN);
			GPIOCircleText.Foreground = T.Foreground;


		}
		#endregion
	}
}
