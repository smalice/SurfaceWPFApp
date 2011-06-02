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
    /// Event arguments when a Bluetooth device arrives or leaves the vicinity.
    /// </summary>
    public class BluetoothDeviceEventArgs : EventArgs
    {
        public BluetoothDeviceEventArgs(BluetoothDeviceInfo info)
        {
            this.DeviceInfo = info;
        }

        /// <summary>
        /// Details of the affected device.
        /// </summary>
        public BluetoothDeviceInfo DeviceInfo
        {
            get;
            private set;
        }
    }

    /// <summary>
    /// Provides continual Bluetooth discovery and events
    /// </summary>
    internal class BluetoothMonitor : IDisposable
    {
        private InTheHand.Net.Bluetooth.RadioMode previousMode;

        private BluetoothClient client;
        private BluetoothWin32Authentication authentication;

        private bool discovering = false;

        #region Devices
        private ObservableCollection<BluetoothDevice> devices = new ObservableCollection<BluetoothDevice>();

        /// <summary>
        /// Exposes a collection of currently discovered devices.
        /// </summary>
        public ObservableCollection<BluetoothDevice> Devices
        {
            get
            {
                return devices;
            }
        }
        #endregion

        internal BluetoothMonitor()
        {
            if (!InTheHand.Net.Bluetooth.BluetoothRadio.IsSupported)
            {
                throw new PlatformNotSupportedException("Bluetooth Radio not detected");
            }
            //make device discoverable, store old mode to restore afterwards
            previousMode = InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio.Mode;
            InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio.Mode = RadioMode.Discoverable;
            
            //client provides discovery services
            client = new BluetoothClient();
            authentication = new BluetoothWin32Authentication(new EventHandler<BluetoothWin32AuthenticationEventArgs>(AuthenticationHandler));

        }

        private void AuthenticationHandler(object o, BluetoothWin32AuthenticationEventArgs e)
        {
            //all attempts to pair with this machine should raise this event
            System.Diagnostics.Debug.WriteLine(e.Device);
            System.Diagnostics.Debug.WriteLine(e.Pin);
            e.Pin = "1234";
        }
        //store native handles used for notification events
        private List<IntPtr> eventHandles = new List<IntPtr>();

        private TimeSpan discoveryDuration = new TimeSpan(0,0,5);
        /// <summary>
        /// Duration of each Discovery phase. Longer duration will discover more devices over a wider range
        /// </summary>
        /// <value>TimeSpan describing how long to run each Discovery. Default is 5 seconds. Maximum value is 57.6 seconds.</value>
        public TimeSpan DiscoveryDuration
        {
            get
            {
                return discoveryDuration;
            }
            set
            {
                if (value.TotalSeconds > 57.6 || value.TotalSeconds < 0.0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                discoveryDuration = value;
            }
        }

        private TimeSpan idleDuration = new TimeSpan(0,0,5);
        /// <summary>
        /// Duration of each Idle phase before a new Discovery is issued.
        /// </summary>
        /// <value>TimeSpan describing how long to wait between each Discovery.
        /// Default is 5 seconds.</value>
        public TimeSpan IdleDuration
        {
            get
            {
                return idleDuration;
            }
            set
            {
                if (value.TotalSeconds < 0.0)
                {
                    throw new ArgumentOutOfRangeException();
                }
                idleDuration = value;
            }
        }

        private int maximumDevices = 255;
        /// <summary>
        /// Defines the maximum number of devices to retrieve when doing a discovery.
        /// This value is before results are filtered for Object Exchange compatible devices.
        /// </summary>
        /// <value>Value must be between 0 and 255 (default).</value>
        public int MaximumDevices
        {
            get
            {
                return maximumDevices;
            }
            set
            {
                if (value < 0 || value > 255)
                {
                    throw new ArgumentOutOfRangeException();
                }
                maximumDevices = value;
            }
        }

        private void InitializeEvents()
        {
            if (eventHandles.Count == 0)
            {
                IntPtr hwnd = IntPtr.Zero;
                // Attempt to get the main window handle (HWND) of the application.
                while (hwnd == IntPtr.Zero)
                {
                    hwnd = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
                    System.Threading.Thread.Sleep(500);
                }

                // Register for the Bluetooth events (these are sent back to the main window handle.
                DEV_BROADCAST_HANDLE dbh = new DEV_BROADCAST_HANDLE();
                dbh.dbch_size = System.Runtime.InteropServices.Marshal.SizeOf(dbh);
                dbh.dbch_devicetype = DBT_DEVTYP_HANDLE;
                dbh.dbch_handle = InTheHand.Net.Bluetooth.BluetoothRadio.AllRadios[0].Handle;
                dbh.dbch_eventguid = GUID_BLUETOOTH_RADIO_IN_RANGE;
                this.eventHandles.Add(RegisterDeviceNotification(hwnd, ref dbh, 0));
                dbh.dbch_eventguid = GUID_BLUETOOTH_RADIO_OUT_OF_RANGE;
                this.eventHandles.Add(RegisterDeviceNotification(hwnd, ref dbh, 0));

                // WPF Interop
                // Adds a hook method to the main application (native) message pump
                System.Windows.Interop.HwndSource hwndSource = System.Windows.Interop.HwndSource.FromHwnd(hwnd);
                hwndSource.AddHook(new System.Windows.Interop.HwndSourceHook(HwndBluetoothHook));

            }
        }

        /// <summary>
        /// Event raised when Discovery phase begins.
        /// </summary>
        public event EventHandler DiscoveryStarted;
        /// <summary>
        /// Event raised when Discovery complete and Idle phase begins.
        /// </summary>
        public event EventHandler DiscoveryCompleted;

        // Raises the DeviceArrived event.
        private void OnDeviceArrived(object o)
        {
            if (deviceArrived != null)
            {
                deviceArrived(this, new BluetoothDeviceEventArgs((InTheHand.Net.Sockets.BluetoothDeviceInfo)o));
            }
        }
        private event EventHandler<BluetoothDeviceEventArgs> deviceArrived;
        /// <summary>
        /// Event raised when a new device is discovered or an existing device is updated (for example adding a Device Name).
        /// </summary>
        public event EventHandler<BluetoothDeviceEventArgs> DeviceArrived
        {
            add
            {
                InitializeEvents();
                deviceArrived += value;
            }
            remove
            {
                deviceArrived -= value;
            }
        }

        // Raises the DeviceLeft event
        private void OnDeviceLeft(object o)
        {
            if (deviceLeft != null)
            {
                deviceLeft(this, new BluetoothDeviceEventArgs((InTheHand.Net.Sockets.BluetoothDeviceInfo)o));
            }
        }
        private event EventHandler<BluetoothDeviceEventArgs> deviceLeft;
        /// <summary>
        /// Event raised when a previously discovered device is not found (indicating it has been turned off or left the vicinity)
        /// </summary>
        public event EventHandler<BluetoothDeviceEventArgs> DeviceLeft
        {
            add
            {
                InitializeEvents();
                deviceLeft += value;
            }
            remove
            {
                deviceLeft -= value;
            }
        }
    
        /// <summary>
        /// Get or set the friendly name for the Bluetooth Radio.
        /// </summary>
        /// <remarks>By default this is set to the machine name (which may be cryptic and not user friendly)</remarks>
        public string RadioFriendlyName
        {
            get
            {
                return InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio.Name;
            }
            set
            {
                InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio.Name = value;
            }
        }
    
        /// <summary>
        /// Starts the background discovery thread. Set the DiscoveryTime and IdleTime first to define the continuous discovery pattern.
        /// </summary>
        public void StartDiscovery()
        {
            if (!discovering)
            {
                InitializeEvents();

                discovering = true;
                System.Threading.Thread t = new System.Threading.Thread(new System.Threading.ThreadStart(DiscoverThread));
                t.IsBackground = true;
                t.Start();
            }
        }

        private void DiscoverThread()
        {
            
            while (discovering)
            {
                if (DiscoveryStarted != null)
                {
                    DiscoveryStarted(this, EventArgs.Empty);
                }
                client.InquiryLength = DiscoveryDuration;

                //perform a discovery
                BluetoothDeviceInfo[] devices = client.DiscoverDevices(MaximumDevices, false, true, true);

                if (DiscoveryCompleted != null)
                {
                    DiscoveryCompleted(this, EventArgs.Empty);
                }

                // Wait for idle duration
                System.Threading.Thread.Sleep(Convert.ToInt32(IdleDuration.TotalMilliseconds));
            }
        }

        /// <summary>
        /// Sets a flag to abort discovery on next cycle.
        /// </summary>
        public void StopDiscovery()
        {
            discovering = false;
        }

        #region IDisposable Members

        public void Dispose()
        {
            StopDiscovery();
            //return to previous mode
            InTheHand.Net.Bluetooth.BluetoothRadio.PrimaryRadio.Mode = previousMode;

            foreach (IntPtr handle in eventHandles)
            {
                bool success = UnregisterDeviceNotification(handle);
            }
            eventHandles.Clear();
        }

        #endregion

        // Imports from BluetoothAPIs.h (and related)

        [StructLayout(LayoutKind.Sequential)]
        internal  struct SYSTEMTIME
        {
            [MarshalAs(UnmanagedType.U2)]
            public short Year;
            [MarshalAs(UnmanagedType.U2)]
            public short Month;
            [MarshalAs(UnmanagedType.U2)]
            public short DayOfWeek;
            [MarshalAs(UnmanagedType.U2)]
            public short Day;
            [MarshalAs(UnmanagedType.U2)]
            public short Hour;
            [MarshalAs(UnmanagedType.U2)]
            public short Minute;
            [MarshalAs(UnmanagedType.U2)]
            public short Second;
            [MarshalAs(UnmanagedType.U2)]
            public short Milliseconds;

            public static SYSTEMTIME FromDateTime(DateTime dt)
            {
                SYSTEMTIME st = new SYSTEMTIME();

                dt = dt.ToUniversalTime();  // SetSystemTime expects the SYSTEMTIME in UTC
                st.Year = (short)dt.Year;
                st.Month = (short)dt.Month;
                st.DayOfWeek = (short)dt.DayOfWeek;
                st.Day = (short)dt.Day;
                st.Hour = (short)dt.Hour;
                st.Minute = (short)dt.Minute;
                st.Second = (short)dt.Second;
                st.Milliseconds = (short)dt.Millisecond;

                return st;
            }

            public DateTime ToDateTime()
            {
                return new DateTime(this.Year, this.Month, this.Day, this.Hour, this.Minute, this.Second, this.Milliseconds, DateTimeKind.Utc);
            }
        }
        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Unicode)]
        internal struct BLUETOOTH_DEVICE_INFO
        {
            internal int dwSize; 
            internal long Address; 
            internal uint ulClassofDevice;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool fConnected;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool fRemembered;
            [MarshalAs(UnmanagedType.Bool)]
            internal bool fAuthenticated;
            internal SYSTEMTIME stLastSeen; 
            internal SYSTEMTIME stLastUsed; 
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst=248)]
            internal string szName;
        }

        // Guids of Bluetooth device events
        internal static readonly Guid GUID_BLUETOOTH_RADIO_IN_RANGE = new Guid("{EA3B5B82-26EE-450E-B0D8-D26FE30A3869}");
        internal static readonly Guid GUID_BLUETOOTH_RADIO_OUT_OF_RANGE = new Guid("{E28867C9-C2AA-4CED-B969-4570866037C4}");
        private const int DBT_DEVTYP_HANDLE = 0x00000006;  // file system handle

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        private static extern IntPtr RegisterDeviceNotification(IntPtr hRecipient, ref DEV_BROADCAST_HANDLE NotificationFilter, int Flags);

        [System.Runtime.InteropServices.DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnregisterDeviceNotification(IntPtr Handle);

        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Sequential)]
        internal struct DEV_BROADCAST_HANDLE
        {
            internal int dbch_size;
            internal int dbch_devicetype;
            internal int dbch_reserved;
            internal IntPtr dbch_handle;
            internal IntPtr dbch_hdevnotify;
            internal Guid dbch_eventguid;
            internal int dbch_nameoffset;
            internal byte dbch_data;
        }

        #region IMessageFilter Members
        private const int WM_DEVICECHANGE = 0x0219;

        //
        // Buffer associated with GUID_BLUETOOTH_RADIO_IN_RANGE
        //
        [StructLayout(LayoutKind.Sequential)]
        internal struct BTH_RADIO_IN_RANGE
        {
            //
            // Information about the remote radio
            //
            internal BTH_DEVICE_INFO deviceInfo;
            //
            // The previous flags value for the BTH_DEVICE_INFO.  The receiver of this
            // notification can compare the deviceInfo.flags and previousDeviceFlags
            // to determine what has changed about this remote radio.
            //
            // For instance, if BDIF_NAME is set in deviceInfo.flags and not in
            // previousDeviceFlags, the remote radio's has just been retrieved.
            //
            internal BDIF previousDeviceFlags;
        }

        [StructLayout(LayoutKind.Sequential, CharSet=CharSet.Ansi)]
        internal struct BTH_DEVICE_INFO
        {
            //
            // Combination BDIF_Xxx flags
            //
            internal BDIF flags;
            //
            // Address of remote device.
            //
            internal long address;
            //
            // Class Of Device.
            //
            internal uint classOfDevice;
            //
            // name of the device
            //
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 248)]
            internal byte[] name;
        }

        

        /// <summary>
        /// Hook into the native message loop for the application to process WM_DEVICECHANGE messages.
        /// </summary>
        /// <param name="hwnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        IntPtr HwndBluetoothHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            // Handle async notifications from the Bluetooth radio here
            if (msg == WM_DEVICECHANGE)
            {
                BluetoothMonitor.DEV_BROADCAST_HANDLE dbh = (BluetoothMonitor.DEV_BROADCAST_HANDLE)Marshal.PtrToStructure(lParam, typeof(BluetoothMonitor.DEV_BROADCAST_HANDLE));
                IntPtr offset = new IntPtr(lParam.ToInt32() + 40);

                if (dbh.dbch_eventguid == BluetoothMonitor.GUID_BLUETOOTH_RADIO_IN_RANGE)
                {

                    BTH_RADIO_IN_RANGE rir = (BTH_RADIO_IN_RANGE)Marshal.PtrToStructure(offset, typeof(BTH_RADIO_IN_RANGE));


                    BluetoothDevice bd = new BluetoothDevice(new InTheHand.Net.BluetoothAddress(rir.deviceInfo.address));


                    // Item is newly discovered - should not exist but check just in case
                    if (!devices.Contains(bd))
                    {
                        DeviceClass dc = (DeviceClass)(rir.deviceInfo.classOfDevice & 0x001ffc);
                        ServiceClass sc = (ServiceClass)(rir.deviceInfo.classOfDevice >> 13);
                        // Either the device advertises support for object transfer or is a smart phone
                        // The latter may introduce false positives for devices which don't support OBEX but is necessary because android supports OBEX but doesn't include the relevant ServiceClass flag.
                        if (((sc & ServiceClass.ObjectTransfer) == ServiceClass.ObjectTransfer) || ((dc & DeviceClass.SmartPhone) == DeviceClass.SmartPhone))
                        {
                            System.Diagnostics.Debug.Write("Radio In Range");
                            System.Diagnostics.Debug.WriteLine(" " + rir.deviceInfo.address.ToString("X"));
                            System.Diagnostics.Debug.WriteLine("Flags " + rir.deviceInfo.flags.ToString());

                            devices.Add(bd);

                            if (deviceArrived != null)
                            {
                                System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(OnDeviceArrived), new InTheHand.Net.Sockets.BluetoothDeviceInfo(new InTheHand.Net.BluetoothAddress(rir.deviceInfo.address)));
                            }
                        }
                    }
                    else
                    {
                        // Only act if this event indicates a change
                        // New devices will have previousDeviceFlags = 0
                        // Other cases can include adding the BDIF_NAME flag when a remote name is queried
                        if (rir.previousDeviceFlags != rir.deviceInfo.flags)
                        {
                            System.Diagnostics.Debug.Write("Radio Updated");
                            System.Diagnostics.Debug.WriteLine(" " + rir.deviceInfo.address.ToString("X"));
                            System.Diagnostics.Debug.WriteLine("New Flags " + (rir.deviceInfo.flags ^ rir.previousDeviceFlags).ToString());

                            // Item is updated - refresh the information
                            devices[devices.IndexOf(bd)].Refresh(rir.deviceInfo.flags ^ rir.previousDeviceFlags);
                        }
                    }
                }
                else if (dbh.dbch_eventguid == BluetoothMonitor.GUID_BLUETOOTH_RADIO_OUT_OF_RANGE)
                {
                    // Read the numeric Bluetooth address.
                    long addr = System.Runtime.InteropServices.Marshal.ReadInt64(offset);
                    InTheHand.Net.BluetoothAddress ba = new InTheHand.Net.BluetoothAddress(addr);
                    // Get device information
                    BluetoothDevice bd = new BluetoothDevice(ba);

                    System.Diagnostics.Debug.Write("Radio Out Of Range");
                    System.Diagnostics.Debug.WriteLine(" " + addr.ToString("X"));

                    if (devices.Contains(bd))
                    {
                        // Handles device going out of range
                        // Remove device pairing if present
                        if (bd.Authenticated)
                        {
                            BluetoothSecurity.RemoveDevice(bd.DeviceAddress);
                        }
                        devices.Remove(bd);
                    }

                    if (deviceLeft != null)
                    {
                        System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(OnDeviceLeft), new InTheHand.Net.Sockets.BluetoothDeviceInfo(ba));
                    }
                }


            }

            handled = false;
            return IntPtr.Zero;
        }
        #endregion
    }

    // Flags describing what device information is available
    [Flags()]
    internal enum BDIF
    {
        BDIF_ADDRESS = (0x00000001),
        BDIF_COD = (0x00000002),
        BDIF_NAME = (0x00000004),
        BDIF_PAIRED = (0x00000008),
        BDIF_PERSONAL = (0x00000010),
        BDIF_CONNECTED = (0x00000020),
    }

}
