using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Globalization;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Media.SpeechSynthesis;

namespace RaspaTools
{
	public class SpeechService
	{
		private readonly SpeechSynthesizer speechSynthesizer;
		private readonly MediaPlayer speechPlayer;
		public SpeechService()
		{
			try
			{
				speechSynthesizer = CreateSpeechSynthesizer();
				speechPlayer = new MediaPlayer();
			}catch{ }
		}

		private static SpeechSynthesizer CreateSpeechSynthesizer()
		{
			var synthesizer = new SpeechSynthesizer();
			try
			{
				synthesizer.Voice = getVoce("it-IT"); 
			}catch{ }
				return synthesizer;
		}

		private static VoiceInformation getVoce(string Lingua)
		{
			return SpeechSynthesizer.AllVoices.SingleOrDefault(i => i.Gender == VoiceGender.Female && i.Language==Lingua) ?? SpeechSynthesizer.DefaultVoice;
		}

		private async Task SayAsync(string text)
		{
			try
			{
				using (var stream = await speechSynthesizer.SynthesizeTextToStreamAsync(text))
				{
					speechPlayer.Source = MediaSource.CreateFromStream(stream, stream.ContentType);
				}
				speechPlayer.Play();
			}
			catch { }
		}

		public async void parla(string testo)
		{
			try
			{
				await SayAsync(testo);
			}
			catch { }
		}

	}
}
