using System.IO;
using System.Media;
using System.Threading;

namespace CapgeminiSurface.Util
{
	public class ThreadedSoundPlayer : SoundPlayer
	{
		public bool SoundIsPlaying { get; private set; }

		public ThreadedSoundPlayer(Stream fileStream) : base(fileStream)
		{
			SoundIsPlaying = false;
		}

		public void PlaySound()
		{
			if (SoundIsPlaying)
			{
				return;
			}
			var threadSound = new Thread(PlaySoundThread);
			threadSound.Start();
		}
		
		protected virtual void PlaySoundThread()
		{
			SoundIsPlaying = true;
			PlaySync();
			SoundIsPlaying = false;
		}
	}
}
