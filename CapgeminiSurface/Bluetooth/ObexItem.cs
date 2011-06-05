using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace SurfaceBluetooth
{
    public abstract class ObexItem : DependencyObject
    {
        /// <summary>
        /// A filename for the item to send (displayed on receiving device)
        /// </summary>
        public abstract string FileName
        {
            get;
        }

        /// <summary>
        /// The MIME content type of the item
        /// </summary>
        public abstract string ContentType 
        {
            get;
        }

        /// <summary>
        /// Writes the formatted contents to a stream for sending
        /// </summary>
        /// <param name="s"></param>
        public abstract void WriteToStream(System.IO.Stream s);

        /// <summary>
        /// Holds a reference to the dragged SVI item for Drag-drop support
        /// </summary>
        public FrameworkElement DraggedElement
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the original position so that we can restore after a successful drag-drop operation
        /// </summary>
        public Point OriginalCenter
        {
            get;
            set;
        }

        /// <summary>
        /// Stores the original orientation so that we can restore after a successful drag-drop operation
        /// </summary>
        public double OriginalOrientation
        {
            get;
            set;
        }
    }
}
