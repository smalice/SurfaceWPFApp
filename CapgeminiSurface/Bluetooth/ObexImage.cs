using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SurfaceBluetooth
{
    public class ObexImage : ObexItem
    {
        public override string ContentType
        {
            get { return InTheHand.Net.Mime.MediaTypeNames.Image.Jpg; }
        }

        public override string FileName
        {
            get { return "Image.jpg"; }
        }

        public override void WriteToStream(System.IO.Stream s)
        {
            // Here we process the picture
            // Using JPEG encoder
            JpegBitmapEncoder jbe = new JpegBitmapEncoder();

            BitmapFrame bf = BitmapFrame.Create(this.ImageUri);
            // Add single frame to encoder
            jbe.Frames.Add(bf);
            // Save JPEG image to our memory buffer
            jbe.Save(s);
        }

        #region Picture

        public static readonly DependencyProperty ImageUriProperty =
            DependencyProperty.Register("ImageUri", typeof(Uri), typeof(ObexImage),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or Sets the image.
        /// </summary>
        public Uri ImageUri
        {
            get
            {
                return (Uri)GetValue(ImageUriProperty);
            }
            set
            {
                SetValue(ImageUriProperty, value);
            }
        }

        #endregion
    }
}
