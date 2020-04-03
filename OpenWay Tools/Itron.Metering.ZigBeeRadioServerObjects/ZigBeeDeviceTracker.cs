using System;
using System.Collections.Generic;
using System.Text;
using Itron.Metering.ZigBeeRadioServerObjects;
using Itron.Metering.Zigbee;

namespace Itron.Metering.ZigBeeRadioServerObjects
{
    /// <summary>
    /// Keeps track of visible ZigBee Device using a ZigBeeRadioCallBack object.
    /// </summary>
    public class ZigBeeDeviceTracker
    {
        #region Constants

        /// <summary>
        /// The default amount of time to keep a device in the list once it has been seen.
        /// </summary>
        public static readonly TimeSpan DEFAULT_KEEP_TIME = TimeSpan.MaxValue;

        #endregion

        #region Public Events

        /// <summary>
        /// Event raised when the list of tracked items has been updated.
        /// </summary>
        public event ZigBeeRadioScannedEvent DevicesUpdated;

        /// <summary>
        /// Event raised when the list of tracked electric meters has been updated.
        /// </summary>
        public event ZigBeeRadioScannedEvent ElectricMetersUpdated;

        /// <summary>
        /// Event raised when the list of tracked Cell Relays have has updated.
        /// </summary>
        public event ZigBeeRadioScannedEvent CellRelaysUpdated;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor. Creates a device tracker using the default keep time.
        /// </summary>
        /// <param name="callback">The callback for the radio service that should be tracked.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public ZigBeeDeviceTracker(ZigBeeRadioCallBack callback)
            : this(callback, DEFAULT_KEEP_TIME)
        {
        }

        /// <summary>
        /// Constructor Creates a device tracker using the specified keep time.
        /// </summary>
        /// <param name="callback">The callback for the radio service that should be tracked.</param>
        /// <param name="timeToKeep">The amount of time to keep a device once it has been seen.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public ZigBeeDeviceTracker(ZigBeeRadioCallBack callback, TimeSpan timeToKeep)
        {
            m_Devices = new List<ZigBeeDevice>();
            m_TimeToKeep = timeToKeep;

            m_RadioCallBack = callback;
            m_RadioCallBack.NetworkScanned += new ZigBeeRadioScannedEvent(m_RadioCallBack_NetworkScanned);
        }

        /// <summary>
        /// Clears the current list of devices.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public void Clear()
        {
            m_Devices = new List<ZigBeeDevice>();
            OnDevicesUpdated(new ZigBeeRadioScannedEventArgs(Devices));
            OnElectricMetersUpdated(new ZigBeeRadioScannedEventArgs(ElectricMeters));
            OnCellRelaysUpdated(new ZigBeeRadioScannedEventArgs(CellRelays));
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of devices that are currently being tracked.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        public List<ZigBeeDevice> Devices
        {
            get
            {
                return m_Devices;
            }
        }

        /// <summary>
        /// Gets the list of Electric Meters that are being tracked.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/08 RCG 1.00           Created

        public List<ZigBeeDevice> ElectricMeters
        {
            get
            {
                List<ZigBeeDevice> MeterList = new List<ZigBeeDevice>();

                foreach (ZigBeeDevice Device in m_Devices)
                {
                    if (Device.DeviceType == ZigbeeDeviceType.ELECTRIC_METER)
                    {
                        MeterList.Add(Device);
                    }
                }

                return MeterList;
            }
        }

        /// <summary>
        /// Gets the list of Cell Relays that are being tracked.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/08 RCG 1.00           Created

        public List<ZigBeeDevice> CellRelays
        {
            get
            {
                List<ZigBeeDevice> CellRelayList = new List<ZigBeeDevice>();

                foreach (ZigBeeDevice Device in m_Devices)
                {
                    if (Device.DeviceType == ZigbeeDeviceType.CELL_RELAY)
                    {
                        CellRelayList.Add(Device);
                    }
                }

                return CellRelayList;
            }
        }

        /// <summary>
        /// Gets or sets the amount of time to keep a device in the list once it
        /// has been seen.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/06/08 RCG 1.00           Created

        public TimeSpan TimeToKeep
        {
            get
            {
                return m_TimeToKeep;
            }
            set
            {
                m_TimeToKeep = value;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Handles the networks scanned 
        /// </summary>
        /// <param name="sender">The object that sent the event.</param>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        private void m_RadioCallBack_NetworkScanned(object sender, ZigBeeRadioScannedEventArgs e)
        {
            // Create a new list by copying the latest list of devices.
            List<ZigBeeDevice> UpdatedDeviceList = new List<ZigBeeDevice>();

            foreach (ZigBeeDevice CurrentDevice in e.Devices)
            {
                UpdatedDeviceList.Add(CurrentDevice);
            }

            // Add in the older devices that have not already been added, and remove devices that
            // are too old.
            foreach (ZigBeeDevice CurrentDevice in m_Devices)
            {
                if (UpdatedDeviceList.Contains(CurrentDevice) == false &&
                    (DateTime.Now - CurrentDevice.ScanTime) < m_TimeToKeep)
                {
                    UpdatedDeviceList.Add(CurrentDevice);
                }
            }

            m_Devices = UpdatedDeviceList;
            OnDevicesUpdated(new ZigBeeRadioScannedEventArgs(Devices));
            OnElectricMetersUpdated(new ZigBeeRadioScannedEventArgs(ElectricMeters));
            OnCellRelaysUpdated(new ZigBeeRadioScannedEventArgs(CellRelays));
        }

        /// <summary>
        /// Raises the DevicesUpdated event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/03/08 RCG 1.00           Created

        private void OnDevicesUpdated(ZigBeeRadioScannedEventArgs e)
        {
            if (DevicesUpdated != null)
            {
                DevicesUpdated(this, e);
            }
        }

        /// <summary>
        /// Raises the ElectricMetersUpdated event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/08 RCG 1.00           Created
        
        private void OnElectricMetersUpdated(ZigBeeRadioScannedEventArgs e)
        {
            if (ElectricMetersUpdated != null)
            {
                ElectricMetersUpdated(this, e);
            }
        }

        /// <summary>
        /// Raises the CellRelaysUpdated event.
        /// </summary>
        /// <param name="e">The event arguments.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  03/18/08 RCG 1.00           Created

        private void OnCellRelaysUpdated(ZigBeeRadioScannedEventArgs e)
        {
            if (CellRelaysUpdated != null)
            {
                CellRelaysUpdated(this, e);
            }
        }

        #endregion

        #region Member Variables

        private ZigBeeRadioCallBack m_RadioCallBack;
        private TimeSpan m_TimeToKeep;
        private List<ZigBeeDevice> m_Devices;

        #endregion
    }
}
