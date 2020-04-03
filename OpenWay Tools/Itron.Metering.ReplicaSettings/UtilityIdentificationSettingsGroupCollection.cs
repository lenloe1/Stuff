using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Itron.Metering.ReplicaSettings
{
    /// <summary>
    /// A collection of utility identification settings definitions.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/22/13 jrf 2.80.22 TQ8285 Created
    //  06/11/13 jrf 2.80.37 TQ8285 Renamed class from HeadEndSystemSettingsCollection to
    //                              UtilityIdentificationSettingsGroupCollection.
    //
    public class UtilityIdentificationSettingsGroupCollection : CollectionBase
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
        public UtilityIdentificationSettingsGroupCollection()
            : base()
        {
        }

        /// <summary>
        /// Copy Constructor.
        /// </summary>
        /// <param name="SettingsCollection">The utility identification settigns collection to duplicate.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public UtilityIdentificationSettingsGroupCollection(UtilityIdentificationSettingsGroupCollection SettingsCollection)
            : base()
        {
            if (null != SettingsCollection)
            {
                //Populate this collection with each member of passed in collection.
                foreach (UtilityIdentificatonSettingsGroupDefinition Definition in SettingsCollection)
                {
                    Add(new UtilityIdentificatonSettingsGroupDefinition(Definition));
                }
            }
        }
        

        /// <summary>
        /// Method returns the index in the collection of the given utility identification settings definiton.
        /// </summary>
        /// <param name="value">The element to look for.</param>
        /// <returns>The element's index in the collection.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public int IndexOf(UtilityIdentificatonSettingsGroupDefinition value)
        {
            return (List.IndexOf(value));
        }


        /// <summary>
        /// Method adds a given utility identification settings definiton to the collection.
        /// </summary>
        /// <param name="value">The element to add.</param>
        /// <returns>The position into which the new element was inserted.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public int Add(UtilityIdentificatonSettingsGroupDefinition value)
        {
            return (List.Add(value));
        }

        /// <summary>
        /// Method removes a given utility identification settings definiton from the collection.
        /// </summary>
        /// <param name="value">The element to remove.</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public void Remove(UtilityIdentificatonSettingsGroupDefinition value)
        {
            List.Remove(value);
        }

        /// <summary>
        /// Method indicates if a given utility identification settings definiton is contained within the collection.
        /// </summary>
        /// <param name="value">The element to look for.</param>
        /// <returns>Whether or not the element is in the collection.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool Contains(UtilityIdentificatonSettingsGroupDefinition value)
        {
            return (List.Contains(value));
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current 
        /// UtilityIdentificationSettingsCollection.
        /// </summary>
        /// <param name="ObjectToCompare">
        /// The Object to compare with the current 
        /// UtilityIdentificationSettingsCollection.
        /// </param>
        /// <returns>
        /// true if the specified Object is equal to the current 
        /// UtilityIdentificationSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public override bool Equals(Object ObjectToCompare)
        {
            bool blnEquals = false;
            UtilityIdentificationSettingsGroupCollection CollectionToCompare = ObjectToCompare as UtilityIdentificationSettingsGroupCollection;

            // If parameter is null return false.
            if (ObjectToCompare == null)
            {
                blnEquals = false;
            }
            // If parameter cannot be cast to UtilityIdentificationSettingsCollection return false.
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
                    foreach (UtilityIdentificatonSettingsGroupDefinition Definition in this)
                    {
                        //if just one system is not contained in the other, not equal
                        if (false == CollectionToCompare.Contains(Definition))
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
        /// Determines whether the specified UtilityIdentificationSettingsCollection is equal to the current 
        /// UtilityIdentificationSettingsCollection.
        /// </summary>
        /// <param name="CollectionToCompare">
        /// The UtilityIdentificationSettingsCollection to compare with the current 
        /// UtilityIdentificationSettingsCollection.
        /// </param>
        /// <returns>
        /// true if the specified UtilityIdentificationSettingsCollection is equal to the current 
        /// UtilityIdentificationSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public bool Equals(UtilityIdentificationSettingsGroupCollection CollectionToCompare)
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
                    foreach (UtilityIdentificatonSettingsGroupDefinition Definition in this)
                    {
                        //if just one system is not contained in the other, not equal
                        if (false == CollectionToCompare.Contains(Definition))
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
        /// Determines whether the specified UtilityIdentificationSettingsCollection are equal.
        /// </summary>
        /// <param name="Collection1">The first UtilityIdentificationSettingsCollection to compare.</param>
        /// <param name="Collection2">The second UtilityIdentificationSettingsCollection to compare.</param>
        /// <returns>
        /// true if the first UtilityIdentificationSettingsCollection is equal to the second 
        /// UtilityIdentificationSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator ==(UtilityIdentificationSettingsGroupCollection Collection1, UtilityIdentificationSettingsGroupCollection Collection2)
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
                    foreach (UtilityIdentificatonSettingsGroupDefinition Definition in Collection1)
                    {
                        //if just one definition is not contained in the other, not equal
                        if (false == Collection2.Contains(Definition))
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
        /// Determines whether the specified UtilityIdentificationSettingsCollections are not equal.
        /// </summary>
        /// <param name="Collection1">The first UtilityIdentificationSettingsCollection to compare.</param>
        /// <param name="Collection2">The second UtilityIdentificationSettingsCollection to compare.</param>
        /// <returns>
        /// true if the first UtilityIdentificationSettingsCollection is not equal to the second 
        /// UtilityIdentificationSettingsCollection; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator !=(UtilityIdentificationSettingsGroupCollection Collection1, UtilityIdentificationSettingsGroupCollection Collection2)
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

            foreach (UtilityIdentificatonSettingsGroupDefinition item in this)
            {
                iHashCode ^= item.GetHashCode();
            }

            return iHashCode;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Property to get/set element of the utility identification settings collection 
        /// indicated by the specified index.
        /// </summary>
        /// <param name="index">The index of the element to retrieve.</param>
        /// <returns>The element indicated by the index.</returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public UtilityIdentificatonSettingsGroupDefinition this[int index]
        {
            get
            {
                return ((UtilityIdentificatonSettingsGroupDefinition)List[index]);
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
    /// The definition of one set of utility identification settings.
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  04/22/13 jrf 2.80.22 TQ8285 Created
    //  06/11/13 jrf 2.80.37 TQ8285 Renamed class from HeadEndSystemSettings to
    //                              UtilityIdentificatonSettingsGroupDefinition.
    //
    public class UtilityIdentificatonSettingsGroupDefinition
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
        public UtilityIdentificatonSettingsGroupDefinition()
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
        //  06/11/13 jrf 2.80.37 TQ8285 Factoring in ert utility id.
        //
        public UtilityIdentificatonSettingsGroupDefinition(UtilityIdentificatonSettingsGroupDefinition SystemSettings)
        {
            InitializeMembers();

            if (null != SystemSettings)
            {
                m_strName = SystemSettings.Name;
                m_strDescription = SystemSettings.Description;
                m_byRFLANUtilityID = SystemSettings.RFLANUtilityID;
                if (null != SystemSettings.CellularGatewayAddress)
                {
                    m_CellularGatewayAddress = new IPAddress(SystemSettings.CellularGatewayAddress.GetAddressBytes());
                }
                m_usCellularGatwayPort = SystemSettings.CellularGatewayPort;
                m_usERTUtilityID = SystemSettings.ERTUtilityID;
            }
            else
            {
                m_strName = "";
                m_strDescription = "";
                m_byRFLANUtilityID = 0;
                m_CellularGatewayAddress = null;
                m_usCellularGatwayPort = 0;
                m_usERTUtilityID = 0;
            }
        }

        /// <summary>
        /// Determines whether the specified Object is equal to the current 
        /// UtilityIdentificatonSettingsDefinition object.
        /// </summary>
        /// <param name="ObjectToCompare">
        /// The Object to compare with the current 
        /// UtilityIdentificatonSettingsDefinition object.
        /// </param>
        /// <returns>
        /// true if the specified Object is equal to the current 
        /// UtilityIdentificatonSettingsDefinition object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Factoring in ert utility id.
        //
        public override bool Equals(System.Object ObjectToCompare)
        {
            bool blnEquals = false;
            UtilityIdentificatonSettingsGroupDefinition SystemToCompare = ObjectToCompare as UtilityIdentificatonSettingsGroupDefinition;

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
                    && (RFLANUtilityID == SystemToCompare.RFLANUtilityID)
                    && (CellularGatewayAddress == SystemToCompare.CellularGatewayAddress)
                    && (CellularGatewayPort == SystemToCompare.CellularGatewayPort)
                    && (ERTUtilityID == SystemToCompare.ERTUtilityID);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified UtilityIdentificatonSettingsDefinition object is equal to the current 
        /// UtilityIdentificatonSettingsDefinition object.
        /// </summary>
        /// <param name="SystemToCompare">
        /// The UtilityIdentificatonSettingsDefinition object to compare with the current 
        /// UtilityIdentificatonSettingsDefinition object.
        /// </param>
        /// <returns>
        /// true if the specified UtilityIdentificatonSettingsDefinition object is equal to the current 
        /// UtilityIdentificatonSettingsDefinition object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Factoring in ert utility id.
        //
        public bool Equals(UtilityIdentificatonSettingsGroupDefinition SystemToCompare)
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
                    && (RFLANUtilityID == SystemToCompare.RFLANUtilityID)
                    && (CellularGatewayAddress == SystemToCompare.CellularGatewayAddress)
                    && (CellularGatewayPort == SystemToCompare.CellularGatewayPort)
                    && (ERTUtilityID == SystemToCompare.ERTUtilityID);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified UtilityIdentificatonSettingsDefinition objects are equal.
        /// </summary>
        /// <param name="System1">The first UtilityIdentificatonSettingsDefinition object to compare.</param>
        /// <param name="System2">The second UtilityIdentificatonSettingsDefinition object to compare.</param>
        /// <returns>
        /// true if the first UtilityIdentificatonSettingsDefinition object is equal to the second 
        /// UtilityIdentificatonSettingsDefinition object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Factoring in ert utility id.
        //
        public static bool operator ==(UtilityIdentificatonSettingsGroupDefinition System1, UtilityIdentificatonSettingsGroupDefinition System2)
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
                    && (System1.RFLANUtilityID == System2.RFLANUtilityID)
                    && (System1.CellularGatewayAddress == System2.CellularGatewayAddress)
                    && (System1.CellularGatewayPort == System2.CellularGatewayPort)
                    && (System1.ERTUtilityID == System2.ERTUtilityID);
            }

            return blnEquals;
        }

        /// <summary>
        /// Determines whether the specified UtilityIdentificatonSettingsDefinition objects are not equal.
        /// </summary>
        /// <param name="System1">The first UtilityIdentificatonSettingsDefinition object to compare.</param>
        /// <param name="System2">The second UtilityIdentificatonSettingsDefinition object to compare.</param>
        /// <returns>
        /// true if the first UtilityIdentificatonSettingsDefinition object is not equal to the second 
        /// HeadEndSystemSettings object; otherwise, false.
        /// </returns>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //
        public static bool operator !=(UtilityIdentificatonSettingsGroupDefinition System1, UtilityIdentificatonSettingsGroupDefinition System2)
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
        //  06/11/13 jrf 2.80.37 TQ8285 Added ert utility id to hash.
        //
        public override int GetHashCode()
        {
            return (m_strName.GetHashCode() 
                  ^ m_strDescription.GetHashCode() 
                  ^ m_byRFLANUtilityID 
                  ^ m_CellularGatewayAddress.GetHashCode() 
                  ^ m_usCellularGatwayPort
                  ^ m_usERTUtilityID);
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The name given to this grouping of utility identification settings.
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
        /// The description of this grouping of utility identification settings.
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
        /// The RFLAN utility ID of this grouping of utility identification settings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public byte RFLANUtilityID
        {
            get
            {
                return m_byRFLANUtilityID;
            }
            set
            {
                m_byRFLANUtilityID = value;
            }
        }

        /// <summary>
        /// A displayable formatted utility ID string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.Byte.ToString(System.String)")]
        public string DisplayableRFLANUtilityID
        {
            get
            {
                string strUtilityID = "0x" + m_byRFLANUtilityID.ToString("X2");

                if (0 == m_byRFLANUtilityID)
                {
                    strUtilityID = Properties.Resources.NotSet;
                }

                return strUtilityID;
            }
        }

        /// <summary>
        /// The ERT utility ID of this grouping of utility identification settings.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8285 Created.
        //
        public ushort ERTUtilityID
        {
            get
            {
                return m_usERTUtilityID;
            }
            set
            {
                m_usERTUtilityID = value;
            }
        }

        /// <summary>
        /// A displayable formatted ERT utility ID string.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8285 Created.
        //
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString(System.String)")]
        public string DisplayableERTUtilityID
        {
            get
            {
                string strUtilityID = "0x" + m_usERTUtilityID.ToString("X2");

                if (0 == m_usERTUtilityID)
                {
                    strUtilityID = Properties.Resources.NotSet;
                }

                return strUtilityID;
            }
        }

        /// <summary>
        /// The cellular gateway's IP address of this grouping of utility identification settings.
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
        /// The cellular gateway's port of this grouping of utility identification settings.
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
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", MessageId = "System.UInt16.ToString")]
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
        /// Gets bool indicating if the ERT utility has been set.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/11/13 jrf 2.80.37 TQ8285 Created.
        //
        public bool ERTUtilityIDConfigured
        {
            get
            {
                bool blnSet = false;

                if (0 != m_usERTUtilityID)
                {
                    blnSet = true;
                }

                return blnSet;
            }
        }

        /// <summary>
        /// Gets bool indicating if the RFLAN utility ID has been set.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed.
        //
        public bool RFLANUtilityIDConfigured
        {
            get
            {
                bool blnSet = false;

                if (0 != m_byRFLANUtilityID)
                {
                    blnSet = true;
                }

                return blnSet;
            }
        }

        /// <summary>
        /// Gets bool indicating if at least one utility identification setting has been configured.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  04/22/13 jrf 2.80.22 TQ8285 Created
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed and added check if ERT utility ID is configured.
        //
        public bool Configured
        {
            get
            {
                return (true == ERTUtilityIDConfigured || true == CellularGatewayConfigured || true == RFLANUtilityIDConfigured);
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
        //  06/11/13 jrf 2.80.37 TQ8285 Renamed/Added variables.
        //
        private void InitializeMembers()
        {
            m_strName = "";
            m_strDescription = "";
            m_byRFLANUtilityID = 0;
            m_CellularGatewayAddress = null;
            m_usCellularGatwayPort = 0;
            m_usERTUtilityID = 0;
        }

        #endregion

        #region Members

        private string m_strName;
        private string m_strDescription;
        private byte m_byRFLANUtilityID;
        private IPAddress m_CellularGatewayAddress;
        private ushort m_usCellularGatwayPort;
        private ushort m_usERTUtilityID;

        #endregion
    }
}
