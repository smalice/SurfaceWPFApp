using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.ComponentModel;

using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;

namespace SurfaceBluetooth
{
    /// <summary>
    /// Wraps BluetoothDeviceInfo and provides change notification
    /// </summary>
    public class BluetoothDevice : INotifyPropertyChanged
    {
        private BluetoothDeviceInfo bdi;

        internal BluetoothDevice(BluetoothAddress ba)
        {
            bdi = new BluetoothDeviceInfo(ba);
        }

        internal BluetoothDevice(BluetoothDeviceInfo bdi)
        {
            this.bdi = bdi;
        }

        //forces a refresh of device properties and notifies subscribers of changes
        internal void Refresh(BDIF flags)
        {
            bdi.Refresh();
            if ((flags & BDIF.BDIF_NAME) == BDIF.BDIF_NAME)
            {
                //raise event
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("DeviceName"));
                }
            }
            if ((flags & BDIF.BDIF_PAIRED) == BDIF.BDIF_PAIRED)
            {
                //raise event
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Authenticated"));
                }
            }
            if ((flags & BDIF.BDIF_CONNECTED) == BDIF.BDIF_CONNECTED)
            {
                //raise event
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("Connected"));
                }
            }
        }

        /// <summary>
        /// The display name of the remote device.
        /// </summary>
        public string DeviceName
        {
            get
            {
                return bdi.DeviceName;
            }
        }

        /// <summary>
        /// Returns true if the device is currently connected.
        /// </summary>
        public bool Connected
        {
            get
            {
                return bdi.Connected;
            }
        }

        /// <summary>
        /// Returns the unique address of the remote device.
        /// </summary>
        public BluetoothAddress DeviceAddress
        {
            get
            {
                return bdi.DeviceAddress;
            }
        }

        /// <summary>
        /// Returns true if remote device is paired.
        /// </summary>
        public bool Authenticated
        {
            get
            {
                return bdi.Authenticated;
            }
        }

        /// <summary>
        /// Returns an image to represent the device based on it's class of device bits.
        /// </summary>
        public System.Windows.Media.Imaging.BitmapSource Image
        {
            get
            {
                switch (bdi.ClassOfDevice.Device)
                {
                    case DeviceClass.CellPhone:
                    case DeviceClass.SmartPhone:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/cellphone.png"));
                    case DeviceClass.PdaComputer:
                    case DeviceClass.HandheldComputer:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/handheld.png"));
                    case DeviceClass.LaptopComputer:
                        return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/laptop.png"));
                }
                return new System.Windows.Media.Imaging.BitmapImage(new Uri("pack://application:,,,/Resources/objectTransfer.gif"));

            }
        }

        /// <summary>
        /// Compares two BluetoothDevice objects.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is BluetoothDevice)
            {
                BluetoothDevice other = obj as BluetoothDevice;
                if (this.DeviceAddress == other.DeviceAddress)
                {
                    return true;
                }
            }
            return false;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion
    }
}
