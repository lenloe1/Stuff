using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Itron.Metering.DeviceDataTypes
{

    /// <summary>
    /// Class that represents a single AMI HAN message record.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/09/11 MSC 2.51.08        Created.
    //
    public class AMIHANMsgRcd
    {
        #region Constants

        private const byte TRANSMISSION_MASK = 0x03;
        private const byte PRIORITY_MASK = 0x0C;
        private const byte CONFIRMATION_MASK = 0x80;

        #endregion

        #region Definitions

        /// <summary>
        /// Message Transmission values as defined in the ZigBee Smart Energy 
        /// profile spec. Bits 0 and 1 of MSG_CTRL
        /// </summary>
        public enum MessageTransmission : byte
        {
            /// <summary>
            /// Send message through normal command function to client.
            /// </summary>
            Normal = 0x00,

            /// <summary>
            /// Send message through normal command function to client 
            /// and pass message onto the Anonymous Inter-PAN 
            /// transmission mechanism.
            /// </summary>
            Normal_And_Anonymous = 0x01,

            /// <summary>
            /// Send message through the Anonymous Inter-PAN 
            /// transmission mechanism.
            /// </summary>
            Anonymous = 0x02,

            /// <summary>
            /// Reserved value for future use.
            /// </summary>
            Reserved = 0x03,
        }

        /// <summary>
        /// Message Priority values as defined in the ZigBee Smart Energy 
        /// profile spec. Bits 2 and 3 of MSG_CTRL
        /// </summary>
        public enum MessagePriority : byte
        {
            /// <summary>
            /// Message to be transferred with a low 
            /// level of importance.
            /// </summary>
            Low = 0x00,

            /// <summary>
            /// Message to be transferred with a medium 
            /// level of importance.
            /// </summary>
            Medium = 0x04,

            /// <summary>
            /// Message to be transferred with a high 
            /// level of importance.
            /// </summary>
            High = 0x08,

            /// <summary>
            /// Message to be transferred with a critical
            /// level of importance.
            /// </summary>
            Critical = 0x0C,
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10        Created.
        //
        public AMIHANMsgRcd()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Message ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public UInt32 MessageId
        {
            get
            {
                return m_Msg_Id;
            }
            set
            {
                m_Msg_Id = value;
            }
        }

        /// <summary>
        /// Start Time (UTC)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public DateTime MessageStart
        {
            get
            {
                return m_Msg_Start_Time;
            }
            set
            {
                m_Msg_Start_Time = value;
            }
        }

        /// <summary>
        /// Duration (Minutes)
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public TimeSpan Duration
        {
            get
            {
                return m_Duration;
            }
            set
            {
                m_Duration = value;
            }
        }

        /// <summary>
        /// Message Length
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public UInt16 MessageLength
        {
            get
            {
                return m_Msg_Len;
            }
            set
            {
                m_Msg_Len = value;
            }
        }

        /// <summary>
        /// Display Message
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public string DisplayMessage
        {
            get
            {
                return m_Display_Msg;
            }
            set
            {
                m_Display_Msg = value;
            }
        }

        /// <summary>
        /// Transmission's Type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MSC 2.51.10          Created.
        public MessageTransmission TransmissionType
        {
            get
            {
                return (MessageTransmission)(m_Msg_Ctrl & TRANSMISSION_MASK);
            }
            set
            {
                m_Msg_Ctrl = (byte)((m_Msg_Ctrl & ~TRANSMISSION_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Message's Priority
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MSC 2.51.10          Created.
        public MessagePriority PriorityLevel
        {
            get
            {
                return (MessagePriority)(m_Msg_Ctrl & PRIORITY_MASK);
            }
            set
            {
                m_Msg_Ctrl = (byte)((m_Msg_Ctrl & ~PRIORITY_MASK) | (byte)value);
            }
        }

        /// <summary>
        /// Confirmation Required
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/14/11 MSC 2.51.10          Created.
        public bool IsConfirmationRequired
        {
            get
            {
                return (m_Msg_Ctrl & CONFIRMATION_MASK) == CONFIRMATION_MASK;
            }
            set
            {
                if (value)
                    m_Msg_Ctrl = (byte)(m_Msg_Ctrl | CONFIRMATION_MASK);
                else
                    m_Msg_Ctrl = (byte)(m_Msg_Ctrl & ~CONFIRMATION_MASK);

            }
        }

        /// <summary>
        /// Message Control
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/13/11 MSC 2.51.10          Created.
        public byte MessageControl
        {
            get
            {
                return m_Msg_Ctrl;
            }
            set
            {
                m_Msg_Ctrl = value;
            }
        }

        #endregion

        #region Members

        private UInt32 m_Msg_Id;
        private byte m_Msg_Ctrl;
        private DateTime m_Msg_Start_Time;
        private TimeSpan m_Duration;
        private UInt16 m_Msg_Len;
        private string m_Display_Msg;

        #endregion
    }
}
