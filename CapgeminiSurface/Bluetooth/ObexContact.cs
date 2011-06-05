using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media.Imaging;

namespace SurfaceBluetooth
{
    /// <summary>
    /// Represents a vCard contact item.
    /// </summary>
    public class ObexContactItem : ObexItem
    {
        public ObexContactItem()
        {
            
        }

        #region First Name

        public static readonly DependencyProperty FirstNameProperty =
            DependencyProperty.Register("FirstName", typeof(string), typeof(ObexContactItem));

        /// <summary>
        /// The contact's first (given) name e.g. John
        /// </summary>
        public string FirstName
        {
            get
            {
                return (string)GetValue(FirstNameProperty);
            }
            set
            {
                SetValue(FirstNameProperty, value);
            }
        }

        #endregion

        #region Last Name

        public static readonly DependencyProperty LastNameProperty =
            DependencyProperty.Register("LastName", typeof(string), typeof(ObexContactItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The contact's last (family) name e.g. Doe
        /// </summary>
        public string LastName
        {
            get
            {
                return (string)GetValue(LastNameProperty);
            }
            set
            {
                SetValue(LastNameProperty, value);
            }
        }
        #endregion

        #region Full Name
        public static readonly DependencyProperty FullNameProperty =
            DependencyProperty.Register("FullName", typeof(string), typeof(ObexContactItem));

        public string FullName
        {
            get
            {
                return FirstName + " " + LastNameProperty;
            }
        }
        #endregion


        #region MobileTelephoneNumber
        public static readonly DependencyProperty MobileTelephoneNumberProperty =
           DependencyProperty.Register("MobileTelephoneNumber", typeof(string), typeof(ObexContactItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// The contact's mobile telephone number
        /// </summary>
        public string MobileTelephoneNumber
        {
            get
            {
                return (string)GetValue(MobileTelephoneNumberProperty);
            }
            set
            {
                SetValue(MobileTelephoneNumberProperty, value);
            }
        }
        #endregion

        #region EmailAddress
        public static readonly DependencyProperty EmailAddressProperty =
            DependencyProperty.Register("EmailAddress", typeof(string), typeof(ObexContactItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or sets the contact's email address.
        /// </summary>
        public string EmailAddress
        {
            get
            {
                return (string)GetValue(EmailAddressProperty);
            }
            set
            {
                SetValue(EmailAddressProperty, value);
            }
        }
        #endregion

        #region Picture

        public static readonly DependencyProperty PictureUriProperty =
            DependencyProperty.Register("PictureUri", typeof(Uri), typeof(ObexContactItem),
            new FrameworkPropertyMetadata(null));

        /// <summary>
        /// Gets or Sets a picture to use for the contact item.
        /// </summary>
        public Uri PictureUri
        {
            get
            {
                return (Uri)GetValue(PictureUriProperty);
            }
            set
            {
                SetValue(PictureUriProperty, value);
            }
        }

        #endregion

        #region Content Type
        public override string ContentType
        {
            get { return InTheHand.Net.Mime.MediaTypeNames.Text.vCard; }
        }
        #endregion

        public override string FileName
        {
            get { return FirstName + "%20" + LastName + ".vcf"; }
        }
        #region Write To Stream
        /// <summary>
        /// Writes a vCard to the specified stream using the stored contact properties.
        /// </summary>
        /// <param name="s">A writable stream.</param>
        public override void WriteToStream(System.IO.Stream s)
        {
            // Write a basic vcard item to the stream (in this sample it will be an ObexWebRequest but it's just bytes so any stream will do)
            // Could write to a filestream for example
            System.IO.StreamWriter sw = new System.IO.StreamWriter(s, System.Text.Encoding.ASCII);
            sw.NewLine = "\r\n";
            // This tag indicates the start of a vCard item - after the content is added an END:VCARD must follow
            sw.WriteLine("BEGIN:VCARD");
            // Version 2.1 of the vCard spec is widely used and our contact types are quite simple
            sw.WriteLine("VERSION:2.1");

            if (!string.IsNullOrEmpty(this.FirstName) || !string.IsNullOrEmpty(this.LastName))
            {
                // Display name "Firstname Lastname"
                sw.WriteLine("FN:" + (string.IsNullOrEmpty(this.FirstName) ? "" : this.FirstName) + " " + (string.IsNullOrEmpty(this.LastName) ? "" : this.LastName));
                // Full name is "Lastname;Firstname"
                sw.WriteLine("N:" + (string.IsNullOrEmpty(this.LastName) ? "" : this.LastName) + ";" + (string.IsNullOrEmpty(this.FirstName) ? "" : this.FirstName));
            }
            if (!string.IsNullOrEmpty(this.MobileTelephoneNumber))
            {
                // Telephone of sub-type cellular, voice
                sw.WriteLine("TEL;CELL;VOICE:" + this.MobileTelephoneNumber);
            }
            if (!string.IsNullOrEmpty(this.EmailAddress))
            {
                // Marked as the preferred email even though we are only storing one
                sw.WriteLine("EMAIL;PREF;INTERNET:" + this.EmailAddress);
            }
            if (this.PictureUri != null)
            {
                // Always generate a jpeg from the bitmap and encode as base64 string
                sw.WriteLine("PHOTO;TYPE=JPEG;ENCODING=BASE64:");

                // Here we process the picture
                // MemoryStream provides a resizable buffer which we can write the image to
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                // Using JPEG encoder
                JpegBitmapEncoder jbe = new JpegBitmapEncoder();

                BitmapFrame bf = BitmapFrame.Create(this.PictureUri);
                // Add single frame to encoder
                jbe.Frames.Add(bf);
                // Save JPEG image to our memory buffer
                jbe.Save(ms);

                // Access the raw bytes of the JPEG
                byte[] rawImage = ms.ToArray();
                // In a Base64 string each 3 bytes is converted to 4 characters so create a new buffer to store the Base64 version
                char[] b64Image = new char[(int)(Math.Ceiling(rawImage.Length / 3d) * 4d)];
                // Convert to the Base64 characters in our new buffer
                Convert.ToBase64CharArray(rawImage, 0, rawImage.Length, b64Image, 0);

                // Loop through the buffer to write each 76 characters to a new line
                for (int i = 0; i < b64Image.Length; i += 76)
                {
                    int charsLeft = b64Image.Length - (i);
                    // For vCard the line must be preceded with a space
                    sw.WriteLine(" " + new string(b64Image, i, charsLeft > 76 ? 76 : charsLeft));
                }
                // Must be followed by a blank line to indicate the end of the image
                sw.WriteLine();
            }

            // Final tag marks the end of the vCard item
            sw.WriteLine("END:VCARD");
            sw.Flush();
        }
        #endregion

        /// <summary>
        /// Returns the display name in the form Firstname Lastname.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return FirstName + " " + LastName;
        }
    }
}
