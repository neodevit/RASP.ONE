using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at https://go.microsoft.com/fwlink/?LinkId=234236

namespace RaspaCentral
{
    public sealed partial class IPCam : UserControl
    {
		private DispatcherTimer dispatcherTimer;
		private bool play = true;
		private string urlImageCam = "";

		public IPCam()
        {
            this.InitializeComponent();
        }
		public async void playIPCam(string nome,string UrlCamImage)
		{
			IPcamNome.Text = nome;
			urlImageCam = UrlCamImage;
			dispatcherTimer = new DispatcherTimer();
			dispatcherTimer.Tick += dispatcherTimer_Tick;
			dispatcherTimer.Interval = System.TimeSpan.FromSeconds(1);
			VideoPlay(true);
		}
		private async void dispatcherTimer_Tick(object sender, object e)
		{
			try
			{
				if (!string.IsNullOrEmpty(urlImageCam))
				{
					var httpClient = new HttpClient();
					Stream st = await httpClient.GetStreamAsync(urlImageCam);
					var memoryStream = new MemoryStream();
					await st.CopyToAsync(memoryStream);
					memoryStream.Position = 0;
					BitmapImage bitmap = new BitmapImage();
					bitmap.SetSource(memoryStream.AsRandomAccessStream());
					IPCamImmagine.Source = bitmap;
				}
			}
			catch { }
		}
		private void playPause_Tapped(object sender, TappedRoutedEventArgs e)
		{
			VideoPlay(!play);
		}
		private void VideoPlay(bool goPlay)
		{
			if (goPlay)
			{
				play = true;
				playPause.Source = new BitmapImage(new Uri("ms-appx:///Assets/ico_pause.png"));
				if (dispatcherTimer != null)
					dispatcherTimer.Start();
			}
			else
			{
				play = false;
				playPause.Source = new BitmapImage(new Uri("ms-appx:///Assets/ico_play.png"));
				if (dispatcherTimer != null)
					dispatcherTimer.Stop();
			}
		}
		private void esci_Tapped(object sender, TappedRoutedEventArgs e)
		{
			this.Visibility = Visibility.Collapsed;
			IPCamImmagine.Source = null;
			VideoPlay(false);
		}
	}
}
