using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Class used for retrieving the help configuration for the current applicaiton.
    /// </summary>
    public class HelpConfig : ConfigurationSection
    {
        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public HelpConfig()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the Help file the application should use
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        [ConfigurationProperty("File", DefaultValue="", IsRequired=true)]
        public string File
        {
            get
            {
                return (string)this["File"];
            }
            set
            {
                this["File"] = value;
            }
        }

        /// <summary>
        /// Gets the collection of Help IDs for the help file.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        [ConfigurationProperty("HelpIDs", IsDefaultCollection=false)]
        public HelpIDCollection HelpIDs
        {
            get
            {
                return (HelpIDCollection)this["HelpIDs"];
            }
        }

        #endregion
    }

    /// <summary>
    /// Help ID object for the Help configuration.
    /// </summary>
    public class HelpID : ConfigurationElement
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public HelpID()
        {
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the name of the current Help ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        [ConfigurationProperty("Name", DefaultValue = "", IsRequired = true)]
        public string Name
        {
            get
            {
                return (string)this["Name"];
            }
            set
            {
                this["Name"] = value;
            }
        }

        /// <summary>
        /// Gets or sets the ID for the current Help ID
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        [ConfigurationProperty("ID", DefaultValue = "", IsRequired = true)]
        public string ID
        {
            get
            {
                return (string)this["ID"];
            }
            set
            {
                this["ID"] = value;
            }
        }

        #endregion
    }

    /// <summary>
    /// Collection of Help ID objects from the Help Configuration
    /// </summary>
    [ConfigurationCollection(typeof(HelpID), CollectionType = ConfigurationElementCollectionType.BasicMap, AddItemName = "HelpID")]
    public class HelpIDCollection : ConfigurationElementCollection
    {
        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public HelpIDCollection()
        {
        }

        /// <summary>
        /// Gets the type of the collection
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public override ConfigurationElementCollectionType CollectionType
        {
            get
            {
                return ConfigurationElementCollectionType.BasicMap;
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the Help ID for the specified index.
        /// </summary>
        /// <param name="iIndex">The index to get or set.</param>
        /// <returns>The specified Help ID</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public HelpID this[int iIndex]
        {
            get
            {
                return (HelpID)BaseGet(iIndex);
            }
            set
            {
                if (BaseGet(iIndex) != null)
                {
                    BaseRemoveAt(iIndex);
                }

                BaseAdd(iIndex, value);
            }
        }

        /// <summary>
        /// Gets the Help ID with the specified name
        /// </summary>
        /// <param name="strName">The name of the help ID to get</param>
        /// <returns>The Help ID</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        public new HelpID this[string strName]
        {
            get
            {
                return (HelpID)BaseGet(strName);
            }
        }

        #endregion

        #region Protected Methods

        /// <summary>
        /// Creates a new element object
        /// </summary>
        /// <returns>The new element</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        protected override ConfigurationElement CreateNewElement()
        {
            return new HelpID();
        }

        /// <summary>
        /// Gets the key value for the specified element
        /// </summary>
        /// <param name="element">The element to get the key for.</param>
        /// <returns>The element's key</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as HelpID).Name;
        }

        #endregion

        #region Protected Properties


        /// <summary>
        /// Gets the name used to identify this collection of elements in the configuration file
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/03/08 RCG 1.50.29 N/A    Created

        protected override string ElementName
        {
            get
            {
                return "HelpID";
            }
        }

        #endregion

    }
}
