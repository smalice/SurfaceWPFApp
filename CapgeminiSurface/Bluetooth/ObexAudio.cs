using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace SurfaceBluetooth
{
    public class ObexAudio : ObexItem
    {
        public class Manager
        {
            public static readonly Manager Instance = new Manager();
            private readonly List<ObexAudio> playingAudio = new List<ObexAudio>();

            public void AddPlayingAudio(ObexAudio playingAudio)
            {
                this.playingAudio.Add(playingAudio);
            }

            public void RemovePlayingAudio(ObexAudio playingAudio)
            {
                this.playingAudio.Remove(playingAudio);
            }

            public void StopAll()
            {
                List<ObexAudio> playing = new List<ObexAudio>(playingAudio);

                foreach (ObexAudio audio in playing)
                {
                    audio.Stop();
                }
            }

            public int PlayingAudioCount { get { return playingAudio.Count; } }
        }

        public override string ContentType
        {
            get { return "audio/mp3"; }
        }

        public override string FileName
        {
            get { return "Audio.mp3"; }
        }

        public override void WriteToStream(Stream s)
        {
            string path = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AudioFile.LocalPath.Substring(1));

            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            byte[] buffer = new byte[4092];
            int bytesRead = fs.Read(buffer, 0, buffer.Length);
            while (bytesRead > 0)
            {
                s.Write(buffer, 0, bytesRead);
                bytesRead = fs.Read(buffer, 0, buffer.Length);
            }

        }

        #region private data

        private MediaPlayer Player { get; set; }
        public event EventHandler MediaEnded
        {
            add
            {
                Player.MediaEnded += value;
            }
            remove
            {
                Player.MediaEnded -= value;
            }
        }

        #endregion

        #region Dependency Properties

        public static readonly DependencyProperty AlbumArtProperty =
            DependencyProperty.Register("AlbumArt", typeof(Uri), typeof(ObexAudio));

        public Uri AlbumArt
        {
            get { return (Uri)GetValue(AlbumArtProperty); }
            set { SetValue(AlbumArtProperty, value); }
        }

        public static readonly DependencyProperty AudioFileProperty =
            DependencyProperty.Register("AudioFile", typeof(Uri), typeof(ObexAudio),
            new UIPropertyMetadata(null, OnAudioFilePropertyChanged));
        /// <summary>
        /// Filename of where we can load the ringtone
        /// </summary>
        public Uri AudioFile
        {
            get { return (Uri)GetValue(AudioFileProperty); }
            set { SetValue(AudioFileProperty, value); }
        }
        private static void OnAudioFilePropertyChanged(object sender, DependencyPropertyChangedEventArgs args)
        {
            ObexAudio ringtone = sender as ObexAudio;
            if (ringtone != null)
            {
                ringtone.UpdateMediaPlayer((Uri)args.NewValue);
            }
        }

        public static readonly DependencyProperty IsPlayingProperty =
            DependencyProperty.Register("IsPlaying", typeof(bool), typeof(ObexAudio), new UIPropertyMetadata(false, OnIsPlayingChanged));
        /// <summary>
        /// Set to true when the ringtone is playing, otherwise false.
        /// </summary>
        public bool IsPlaying
        {
            get { return (bool)GetValue(IsPlayingProperty); }
            set { SetValue(IsPlayingProperty, value); }
        }

        private static void OnIsPlayingChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            ObexAudio ringtone = (ObexAudio)sender;

            if ((bool)args.NewValue)
            {
                ringtone.Play();
                Manager.Instance.AddPlayingAudio(ringtone);
            }
            else
            {
                ringtone.Pause();
                Manager.Instance.RemovePlayingAudio(ringtone);
            }
        }

        #endregion

        #region Media Player support

        private void UpdateMediaPlayer(Uri uri)
        {
            if (Player == null)
            {
                Player = new MediaPlayer { Volume = 0.6 };
                Player.MediaEnded += PlayerMediaEnded;
                Player.MediaFailed += Player_MediaFailed;
            }

            Player.Open(uri);
            IsPlaying = false;
        }

        void Player_MediaFailed(object sender, ExceptionEventArgs e)
        {
            Stop();
        }

        /// <summary>
        /// Start playing the ringtone.
        /// </summary>
        public void Play()
        {
            if (Player != null)
            {
                if (Player.Position >= Player.NaturalDuration)
                {
                    Player.Position = TimeSpan.Zero;
                }
                Player.Play();
                IsPlaying = true;
            }
        }

        /// <summary>
        /// Pause the ringtone
        /// </summary>
        public void Pause()
        {
            if (Player != null)
            {
                Player.Pause();
                IsPlaying = false;
            }
        }

        /// <summary>
        /// Stop playing the ringtone and restart.
        /// </summary>
        public void Stop()
        {
            if (Player != null)
            {
                Player.Stop();
                Manager.Instance.RemovePlayingAudio(this);
                IsPlaying = false;
                Player.Position = TimeSpan.FromSeconds(0.0);
            }
        }

        /// <summary>
        /// Handler for event sent by the WPF MediaPlayer when it
        /// reaches the end of the media.
        /// </summary>
        private void PlayerMediaEnded(object sender, EventArgs e)
        {
            Stop();
        }
        #endregion

    }
}
