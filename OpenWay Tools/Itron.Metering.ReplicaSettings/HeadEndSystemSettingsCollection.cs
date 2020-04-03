using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// A collection of head-end system settings
    /// </summary>
    public class HeadEndSystemSettingsCollection : CollectionBase
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public HeadEndSystemSettingsCollection()
            : base()
        {
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="HeadEndSystems">The Head-end system settings to duplicate.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public HeadEndSystemSettingsCollection(HeadEndSystemSettingsCollection HeadEndSystems)
            : base()
        {
            if (null != HeadEndSystems)
            {
                //Populate this collection with each member of passed in collection.
                foreach (HeadEndSystemSettings System in HeadEndSystems)
                {
                    Add(new HeadEndSystemSettings(System));
                }
            }
        }
        

        /// <summary>
        /// Method returns the index in the collection of the given head-end system definiton.
        /// </summary>
        /// <param name="value">The element to look for.</param>
        /// <returns>The element's index in the collection.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public int IndexOf(HeadEndSystemSettings value)
        {
            return (List.IndexOf(value));
        }


        /// <summary>
        /// Method adds a given head-end system definiton to the collection.
        /// </summary>
        /// <param name="value">The element to add.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public int Add(HeadEndSystemSettings value)
        {
            return (List.Add(value));
        }

        /// <summary>
        /// Method removes a given head-end system definiton from the collection.
        /// </summary>
        /// <param name="value">The element to remove.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public void Remove(HeadEndSystemSettings value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Method indicates if a given head-end system definiton is contained within the collection.
        /// </summary>
        /// <param name="value">The element to look for.</param>
        /// <returns>Whether or not the element is in the collection.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool Contains(HeadEndSystemSettings value)
        {
            return (List.Contains(value));
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current 
        /// HeadEndSystemSettingsCollection.
        /// </summary>
        /// <param name="ObjectToCompare">
        /// The Object to compare with the current 
        /// HeadEndSystemSettingsCollection.
        /// </param>
        /// <returns>
        /// true if the specified Object is equal to the current 
        /// HeadEndSystemSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public override bool Equals(Object ObjectToCompare)
        {
            bool blnEquals = false;
            HeadEndSystemSettingsCollection CollectionToCompare = ObjectToCompare as HeadEndSystemSettingsCollection;

            // If parameter is null return false.
            if (ObjectToCompare == null)
            {
                blnEquals = false;
            }
            // If parameter cannot be cast to HeadEndSystemSettingsCollection return false.
            else if ((System.Object)CollectionToCompare == null)
            {
                blnEquals = false;
            }
            else
            {
                // Return true if the collections have the same number of elements and every element from one is contained in the other.
                blnEquals = true;

                if (Count == CollectionToCompare.Count)
                {
                    foreach (HeadEndSystemSettings System in this)
                    {
                        //if just one system is not contained in the other, not equal
                        if (false == CollectionToCompare.Contains(System))
                        {
                            blnEquals = false;
                            break; //no need to check any more.
                        }
                    }
                }
                else //different number of elememnts, not equal
                {
                    blnEquals = false;
                }
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettingsCollection is equal to the current 
        /// HeadEndSystemSettingsCollection.
        /// </summary>
        /// <param name="CollectionToCompare">
        /// The HeadEndSystemSettingsCollection to compare with the current 
        /// HeadEndSystemSettingsCollection.
        /// </param>
        /// <returns>
        /// true if the specified HeadEndSystemSettingsCollection is equal to the current 
        /// HeadEndSystemSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool Equals(HeadEndSystemSettingsCollection CollectionToCompare)
        {
            bool blnEquals = false;

            // If parameter is null return false:
            if ((object)CollectionToCompare == null)
            {
                return false;
            }
            else
            {
                // Return true if the collections have the same number of elements and every element from one is contained in the other.
                blnEquals = true;

                if (Count == CollectionToCompare.Count)
                {
                    foreach (HeadEndSystemSettings System in this)
                    {
                        //if just one system is not contained in the other, not equal
                        if (false == CollectionToCompare.Contains(System))
                        {
                            blnEquals = false;
                            break; //no need to check any more.
                        }
                    }
                }
                else //different number of elememnts, not equal
                {
                    blnEquals = false;
                }
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettingsCollections are equal.
        /// </summary>
        /// <param name="Collection1">The first HeadEndSystemSettingsCollection to compare.</param>
        /// <param name="Collection2">The second HeadEndSystemSettingsCollection to compare.</param>
        /// <returns>
        /// true if the first HeadEndSystemSettingsCollection is equal to the second 
        /// HeadEndSystemSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator ==(HeadEndSystemSettingsCollection Collection1, HeadEndSystemSettingsCollection Collection2)
        {
            bool blnEquals = false;

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(Collection1, Collection2))
            {
                blnEquals = true;
            }
            // If one is null, but not both, return false.
            else if (((object)Collection1 == null) || ((object)Collection2 == null))
            {
                blnEquals = false;
            }
            else
            {
                // Return true if the collections have the same number of elements and every element from one is contained in the other.
                blnEquals = true;

                if (Collection1.Count == Collection2.Count)
                {
                    foreach (HeadEndSystemSettings HeadEndSystem in Collection1)
                    {
                        //if just one system is not contained in the other, not equal
                        if (false == Collection2.Contains(HeadEndSystem))
                        {
                            blnEquals = false;
                            break; //no need to check any more.
                        }
                    }
                }
                else //different number of elememnts, not equal
                {
                    blnEquals = false;
                }
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettingsCollections are not equal.
        /// </summary>
        /// <param name="Collection1">The first HeadEndSystemSettingsCollection to compare.</param>
        /// <param name="Collection2">The second HeadEndSystemSettingsCollection to compare.</param>
        /// <returns>
        /// true if the first HeadEndSystemSettingsCollection is not equal to the second 
        /// HeadEndSystemSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator !=(HeadEndSystemSettingsCollection Collection1, HeadEndSystemSettingsCollection Collection2)
        {
            return !(Collection1 == Collection2);
        }

        /// <summary>
        /// Serves as a hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public override int GetHashCode()
        {
            int iHashCode = 0;

            foreach (HeadEndSystemSettings item in this)
            {
                iHashCode ^= item.GetHashCode();
            }

            return iHashCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to get/set element of the head-end systems settings collection 
        /// indicated by the specified index.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <returns>The element indicated by the index.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public HeadEndSystemSettings this[int index]
        {
            get
            {
                return ((HeadEndSystemSettings)List[index]);
            }
            set
            {
                List[index] = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// The systems settings for one head-end system.
    /// </summary>
    public class HeadEndSystemSettings
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public HeadEndSystemSettings()
        {
            InitializeMembers();
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public HeadEndSystemSettings(HeadEndSystemSettings SystemSettings)
        {
            InitializeMembers();

            if (null != SystemSettings)
            {
                m_strName = SystemSettings.Name;
                m_strDescription = SystemSettings.Description;
                m_byUtilityID = SystemSettings.UtilityID;
                if (null != SystemSettings.CellularGatewayAddress)
                {
                    m_CellularGatewayAddress = new IPAddress(SystemSettings.CellularGatewayAddress.GetAddressBytes());
                }
                m_usCellularGatwayPort = SystemSettings.CellularGatewayPort;
            }
            else
            {
                m_strName = "";
                m_strDescription = "";
                m_byUtilityID = 0;
                m_CellularGatewayAddress = null;
                m_usCellularGatwayPort = 0;
            }
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current 
        /// HeadEndSystemSettings object.
        /// </summary>
        /// <param name="ObjectToCompare">
        /// The Object to compare with the current 
        /// HeadEndSystemSettings object.
        /// </param>
        /// <returns>
        /// true if the specified Object is equal to the current 
        /// HeadEndSystemSettings object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public override bool Equals(System.Object ObjectToCompare)
        {
            bool blnEquals = false;
            HeadEndSystemSettings SystemToCompare = ObjectToCompare as HeadEndSystemSettings;

            // If parameter is null return false.
            if (ObjectToCompare == null)
            {
                blnEquals = false;
            }
            // If parameter cannot be cast to HeadEndSystemSettings return false.
            else if ((System.Object)SystemToCompare == null)
            {
                blnEquals = false;
            }
            else
            {
                // Return true if all the fields match:
                blnEquals = (Name == SystemToCompare.Name)
                    && (Description == SystemToCompare.Description)
                    && (UtilityID == SystemToCompare.UtilityID)
                    && (CellularGatewayAddress == SystemToCompare.CellularGatewayAddress)
                    && (CellularGatewayPort == SystemToCompare.CellularGatewayPort);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettings object is equal to the current 
        /// HeadEndSystemSettings object.
        /// </summary>
        /// <param name="SystemToCompare">
        /// The HeadEndSystemSettings object to compare with the current 
        /// HeadEndSystemSettings object.
        /// </param>
        /// <returns>
        /// true if the specified HeadEndSystemSettings object is equal to the current 
        /// HeadEndSystemSettings object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool Equals(HeadEndSystemSettings SystemToCompare)
        {
            bool blnEquals = false;

            // If parameter is null return false:
            if ((object)SystemToCompare == null)
            {
                blnEquals = false;
            }
            else
            {
                // Return true if all the fields match:
                blnEquals = (Name == SystemToCompare.Name)
                    && (Description == SystemToCompare.Description)
                    && (UtilityID == SystemToCompare.UtilityID)
                    && (CellularGatewayAddress == SystemToCompare.CellularGatewayAddress)
                    && (CellularGatewayPort == SystemToCompare.CellularGatewayPort);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettings objects are equal.
        /// </summary>
        /// <param name="System1">The first HeadEndSystemSettings object to compare.</param>
        /// <param name="System2">The second HeadEndSystemSettings object to compare.</param>
        /// <returns>
        /// true if the first HeadEndSystemSettings object is equal to the second 
        /// HeadEndSystemSettings object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator ==(HeadEndSystemSettings System1, HeadEndSystemSettings System2)
        {
            bool blnEquals = false;

            // If both are null, or both are same instance, return true.
            if (System.Object.ReferenceEquals(System1, System2))
            {
                blnEquals = true;
            }

            // If one is null, but not both, return false.
            else if (((object)System1 == null) || ((object)System2 == null))
            {
                blnEquals = false;
            }
            else
            {
                // Return true if all the fields match:
                blnEquals = (System1.Name == System2.Name)
                    && (System1.Description == System2.Description)
                    && (System1.UtilityID == System2.UtilityID)
                    && (System1.CellularGatewayAddress == System2.CellularGatewayAddress)
                    && (System1.CellularGatewayPort == System2.CellularGatewayPort);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified HeadEndSystemSettings objects are not equal.
        /// </summary>
        /// <param name="System1">The first HeadEndSystemSettings object to compare.</param>
        /// <param name="System2">The second HeadEndSystemSettings object to compare.</param>
        /// <returns>
        /// true if the first HeadEndSystemSettings object is not equal to the second 
        /// HeadEndSystemSettings object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator !=(HeadEndSystemSettings System1, HeadEndSystemSettings System2)
        {
            return !(System1 == System2);
        }

        /// <summary>
        /// Serves as a hash function.
        /// </summary>
        /// <returns>A hash code for the current object.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public override int GetHashCode()
        {
            return (m_strName.GetHashCode() 
                  ^ m_strDescription.GetHashCode() 
                  ^ m_byUtilityID 
                  ^ m_CellularGatewayAddress.GetHashCode() 
                  ^ m_usCellularGatwayPort);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The name of the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public string Name
        {
            get
            {
                return m_strName;
            }
            set
            {
                m_strName = value;
            }
        }

        /// <summary>
        /// The description of the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public string Description
        {
            get
            {
                return m_strDescription;
            }
            set
            {
                m_strDescription = value;
            }
        }

        /// <summary>
        /// The utility ID of the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public byte UtilityID
        {
            get
            {
                return m_byUtilityID;
            }
            set
            {
                m_byUtilityID = value;
            }
        }

        /// <summary>
        /// A displayable formatted utility ID string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public string DisplayableUtilityID
        {
            get
            {
                string strUtilityID = "0x" + m_byUtilityID.ToString("X2");

                if (0 == m_byUtilityID)
                {
                    strUtilityID = Properties.Resources.NotSet;
                }

                return strUtilityID;
            }
        }

        /// <summary>
        /// The cellular gateway's IP address of the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public IPAddress CellularGatewayAddress
        {
            get
            {
                return m_CellularGatewayAddress;
            }
            set
            {
                m_CellularGatewayAddress = value;
            }
        }

        /// <summary>
        /// The cellular gateway's port of the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public ushort CellularGatewayPort
        {
            get
            {
                return m_usCellularGatwayPort;
            }
            set
            {
                m_usCellularGatwayPort = value;
            }
        }

        /// <summary>
        /// A displayable formatted cellular gateway string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public string DisplayableCellularGateway
        {
            get
            {
                string strCellularGateway = "";


                if (null == m_CellularGatewayAddress || 0 == m_CellularGatewayAddress.GetAddressBytes().Length)
                {
                    strCellularGateway = Properties.Resources.NotSet;
                }
                else
                {
                    strCellularGateway = m_CellularGatewayAddress.ToString() + ":" + m_usCellularGatwayPort.ToString();
                }

                return strCellularGateway;
            }
        }

        /// <summary>
        /// Gets bool indicating if the cellular gateway has been set.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool CellularGatewayConfigured
        {
            get
            {
                bool blnSet = false;

                if (null != m_CellularGatewayAddress)
                {
                    blnSet = true;
                }

                return blnSet;
            }
        }

        /// <summary>
        /// Gets bool indicating if the utility has been set.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool UtilityIDConfigured
        {
            get
            {
                bool blnSet = false;

                if (0 != m_byUtilityID)
                {
                    blnSet = true;
                }

                return blnSet;
            }
        }

        /// <summary>
        /// Gets bool indicating if settings have been configured to provide a connection
        /// to the head-end system.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool ConnectionConfigured
        {
            get
            {
                return (true == CellularGatewayConfigured || true == UtilityIDConfigured);
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Initializes all member variables to default values.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        private void InitializeMembers()
        {
            m_strName = "";
            m_strDescription = "";
            m_byUtilityID = 0;
            m_CellularGatewayAddress = null;
            m_usCellularGatwayPort = 0;
        }

        #endregion

        #region Members

        private string m_strName;
        private string m_strDescription;
        private byte m_byUtilityID;
        private IPAddress m_CellularGatewayAddress;
        private ushort m_usCellularGatwayPort;

        #endregion
    }
}
