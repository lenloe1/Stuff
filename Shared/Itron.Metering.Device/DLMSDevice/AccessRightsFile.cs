using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Globalization;
using Itron.Metering.Utilities;

namespace Itron.Metering.Device.DLMSDevice
{
    /// <summary>
    /// Class used for reading and parsing an Access Rights file. 
    /// </summary>
    public class AccessRightsFile
    {
        #region Constants

        private const string ROOT_ELEMENT = "dlmsSetup";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public AccessRightsFile()
        {
            m_FilePath = null;
        }

        /// <summary>
        /// Loads the Access Rights from the specified file
        /// </summary>
        /// <param name="filePath">The path of the file to load</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public void Load(string filePath)
        {
            m_FilePath = filePath;
            XDocument NewDocument = XDocument.Load(m_FilePath);

            if (NewDocument.Root != null && NewDocument.Root.Name.LocalName == ROOT_ELEMENT)
            {
                XNamespace NameSpace = NewDocument.Root.GetDefaultNamespace();
                m_AccessPoints = new List<ClientAPNode>();

                foreach (XElement AccessPointElement in NewDocument.Descendants(NameSpace.GetName(ClientAPNode.ELEMENT_NAME)))
                {
                    m_AccessPoints.Add(new ClientAPNode(AccessPointElement));
                }

                m_OBISCodes = new List<OBISCodeNode>();

                foreach (XElement OBISCodeElement in NewDocument.Descendants(NameSpace.GetName(OBISCodeNode.ELEMENT_NAME)))
                {
                    m_OBISCodes.Add(new OBISCodeNode(OBISCodeElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Access Rights file", "filePath");
            }
        }

        /// <summary>
        /// Gets the name of the client from the SAP
        /// </summary>
        /// <param name="clientSAP">The SAP to get the client name for</param>
        /// <returns>The name of the SAP</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public string GetClientNameFromSAP(ushort clientSAP)
        {
            string Name = null;

            foreach (ClientAPNode CurrentAP in m_AccessPoints)
            {
                if (CurrentAP.ClientSAP == clientSAP)
                {
                    Name = CurrentAP.Name;
                    break;
                }
            }

            return Name;
        }

        /// <summary>
        /// Gets the Calling AP Title prefix for the specified SAP
        /// </summary>
        /// <param name="clientSAP">The SAP to get the prefix for</param>
        /// <returns>The AP Title Prefix</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/18/13 RCG 3.50.05 N/A    Created
        
        public string GetApTitlePrefixForSAP(ushort clientSAP)
        {
            string APTitle = null;

            foreach (ClientAPNode CurrentAP in m_AccessPoints)
            {
                if (CurrentAP.ClientSAP == clientSAP)
                {
                    APTitle = CurrentAP.ApTitlePrefix;
                }
            }

            return APTitle;
        }

        /// <summary>
        /// Gets the object list Access Point Name
        /// </summary>
        /// <param name="name">The name of the Access Point to get</param>
        /// <returns>The object list</returns>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public List<COSEMLongNameObjectListElement> GetObjectList(string name)
        {
            List<COSEMLongNameObjectListElement> ObjectList = new List<COSEMLongNameObjectListElement>();

            foreach (OBISCodeNode CurrentOBISCode in m_OBISCodes)
            {
                COSEMLongNameObjectListElement CurrentElement = new COSEMLongNameObjectListElement();

                CurrentElement.LogicalName = CurrentOBISCode.OBISCode;
                CurrentElement.ClassID = CurrentOBISCode.ClassID;

                CurrentElement.AccessRight = new COSEMAccessRight();

                foreach (PropertyNode CurrentProperty in CurrentOBISCode.Properties)
                {
                    if (CurrentProperty.PropertyType == PropertyNodeType.Attribute
                        && CurrentProperty.Clients.Where(c => c.Name.Equals(name)).Count() > 0)
                    {
                        COSEMAttributeAccessItem AttributeAccess = new COSEMAttributeAccessItem();
                        ClientNode Client = CurrentProperty.Clients.Where(c => c.Name.Equals(name)).First();

                        AttributeAccess.AttributeID = CurrentProperty.Index;
                        AttributeAccess.AccessMode = COSEMAttributeAccessMode.NoAccess;

                        if (Client.Access != null)
                        {
                            if (Client.Access.Get && Client.Access.Set)
                            {
                                AttributeAccess.AccessMode = COSEMAttributeAccessMode.ReadAndWrite;
                            }
                            else if (Client.Access.Get)
                            {
                                AttributeAccess.AccessMode = COSEMAttributeAccessMode.ReadOnly;
                            }
                            else if (Client.Access.Set)
                            {
                                AttributeAccess.AccessMode = COSEMAttributeAccessMode.WriteOnly;
                            }
                        }

                        CurrentElement.AccessRight.AttributeAccess.Add(AttributeAccess);
                    }
                    else if (CurrentProperty.PropertyType == PropertyNodeType.Method
                        && CurrentProperty.Clients.Where(c => c.Name.Equals(name)).Count() > 0)
                    {
                        COSEMMethodAccessItem MethodAccess = new COSEMMethodAccessItem();
                        ClientNode Client = CurrentProperty.Clients.Where(c => c.Name.Equals(name)).First();

                        MethodAccess.MethodID = CurrentProperty.Index;
                        MethodAccess.AccessMode = COSEMMethodAccessMode.NoAccess;

                        if (Client.Access != null && Client.Access.Action)
                        {
                            MethodAccess.AccessMode = COSEMMethodAccessMode.Access;
                        }

                        CurrentElement.AccessRight.MethodAccess.Add(MethodAccess);
                    }
                }

                if (CurrentElement.AccessRight.AttributeAccess.Count > 0 
                    || CurrentElement.AccessRight.MethodAccess.Count > 0)
                {
                    ObjectList.Add(CurrentElement);
                }
            }

            return ObjectList;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the list of Access Points
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public List<ClientAPNode> AccessPoints
        {
            get
            {
                return m_AccessPoints;
            }
        }

        /// <summary>
        /// Gets the list of OBIS Codes
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public List<OBISCodeNode> OBISCodes
        {
            get
            {
                return m_OBISCodes;
            }
        }

        #endregion

        #region Member Variables

        private string m_FilePath;
        private List<ClientAPNode> m_AccessPoints;
        private List<OBISCodeNode> m_OBISCodes;

        #endregion
    }

    /// <summary>
    /// Object for a Client Access Point Node
    /// </summary>
    public class ClientAPNode
    {
        #region Constants

        /// <summary>
        /// The xml element name
        /// </summary>
        internal const string ELEMENT_NAME = "clientAP";

        private const string ATTRIB_NAME = "name";
        private const string ATTRIB_AP = "accessPointId";
        private const string ATTRIB_SECURITY = "minimumSecurity";
        private const string ATTRIB_BROADCAST = "broadcast";
        private const string ATTRIB_APTITLE = "apTitlePrefix";
        private const string ATTRIB_PRIORITY = "priority";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created

        public ClientAPNode()
        {
            m_Name = "";
            m_ClientSAP = 0;
            m_MinimumSecurity = 0;
            m_Broadcast = false;
            m_Priority = 0;
            m_ApTitlePrefix = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The XML element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public ClientAPNode(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIB_NAME) != null)
                {
                    m_Name = element.Attribute(ATTRIB_NAME).Value;
                }

                if (element.Attribute(ATTRIB_AP) != null)
                {
                    m_ClientSAP = ushort.Parse(element.Attribute(ATTRIB_AP).Value);
                }

                if (element.Attribute(ATTRIB_SECURITY) != null)
                {
                    m_MinimumSecurity = int.Parse(element.Attribute(ATTRIB_SECURITY).Value);
                }

                if (element.Attribute(ATTRIB_BROADCAST) != null)
                {
                    m_Broadcast = bool.Parse(element.Attribute(ATTRIB_BROADCAST).Value);
                }

                if (element.Attribute(ATTRIB_APTITLE) != null)
                {
                    m_ApTitlePrefix = element.Attribute(ATTRIB_APTITLE).Value;
                }

                if (element.Attribute(ATTRIB_PRIORITY) != null)
                {
                    m_Priority = int.Parse(element.Attribute(ATTRIB_PRIORITY).Value);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid clientAP element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the Access Point Name
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the Client SAP for the Access Point
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public ushort ClientSAP
        {
            get
            {
                return m_ClientSAP;
            }
        }

        /// <summary>
        /// Gets the Minimum Security level for the Access Point
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public int MinimumSecurity
        {
            get
            {
                return m_MinimumSecurity;
            }
        }

        /// <summary>
        /// Gets whether or not broadcasts are allowed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public bool BroadCast
        {
            get
            {
                return m_Broadcast;
            }
        }

        /// <summary>
        /// Gets the priority of the AP
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/18/13 RCG 3.50.05 N/A    Created
        
        public int Priority
        {
            get
            {
                return m_Priority;
            }
        }

        /// <summary>
        /// Gets the Calling Ap-Title Prefix
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  11/18/13 RCG 3.50.05 N/A    Created
        
        public string ApTitlePrefix
        {
            get
            {
                return m_ApTitlePrefix;
            }
        }

        #endregion

        #region Member Variables

        private string m_Name;
        private ushort m_ClientSAP;
        private int m_MinimumSecurity;
        private bool m_Broadcast;
        private int m_Priority;
        private string m_ApTitlePrefix;

        #endregion
    }

    /// <summary>
    /// Object for an OBIS Code Node
    /// </summary>
    public class OBISCodeNode
    {
        #region Constants

        /// <summary>
        /// The xml element name
        /// </summary>
        internal const string ELEMENT_NAME = "obisCode";

        private const string ATTRIB_CODE = "code";
        private const string ATTRIB_NAME = "name";
        private const string ATTRIB_CLASS = "class";
        private const string ATTRIB_INTERROGATIONTYPE = "interrogationType";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public OBISCodeNode()
        {
            m_OBISCode = null;
            m_Name = "";
            m_ClassID = 0;
            m_InterrogationType = "";
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The XML element</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public OBISCodeNode(XElement element)
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                XNamespace NameSpace = element.GetDefaultNamespace();

                if (element.Attribute(ATTRIB_CODE) != null)
                {
                    string CodeString = element.Attribute(ATTRIB_CODE).Value;
                    string[] StringValues = CodeString.Split(new char[] { ':', '-', '.', '*' });

                    m_OBISCode = new byte[StringValues.Length];

                    for (int iIndex = 0; iIndex < StringValues.Length; iIndex++)
                    {
                        m_OBISCode[iIndex] = byte.Parse(StringValues[iIndex]);
                    }
                }

                if (element.Attribute(ATTRIB_NAME) != null)
                {
                    m_Name = element.Attribute(ATTRIB_NAME).Value;
                }

                if (element.Attribute(ATTRIB_CLASS) != null)
                {
                    m_ClassID = ushort.Parse(element.Attribute(ATTRIB_CLASS).Value);
                }

                if (element.Attribute(ATTRIB_INTERROGATIONTYPE) != null)
                {
                    m_InterrogationType = element.Attribute(ATTRIB_INTERROGATIONTYPE).Value;
                }

                m_Properties = new List<PropertyNode>();

                foreach (XElement PropertyElement in element.Descendants(NameSpace.GetName(PropertyNode.ELEMENT_NAME)))
                {
                    m_Properties.Add(new PropertyNode(PropertyElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid obiscode element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the OBIS Code for the item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public byte[] OBISCode
        {
            get
            {
                return m_OBISCode;
            }
        }

        /// <summary>
        /// Gets the name of the item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the Class ID of the item
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public ushort ClassID
        {
            get
            {
                return m_ClassID;
            }
        }

        /// <summary>
        /// Gets the interrogation type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public string InterrogationType
        {
            get
            {
                return m_InterrogationType;
            }
        }

        /// <summary>
        /// Gets the list of properties
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created

        public List<PropertyNode> Properties
        {
            get
            {
                return m_Properties;
            }
        }

        #endregion

        #region Member Variables

        private byte[] m_OBISCode;
        private string m_Name;
        private ushort m_ClassID;
        private string m_InterrogationType;
        List<PropertyNode> m_Properties;

        #endregion
    }

    /// <summary>
    /// Property Node types
    /// </summary>
    public enum PropertyNodeType
    {
        /// <summary>
        /// Attribute
        /// </summary>
        [EnumDescription("attribute")]
        Attribute,
        /// <summary>
        /// Method
        /// </summary>
        [EnumDescription("method")]
        Method,
    }

    /// <summary>
    /// Object for a Property Node
    /// </summary>
    public class PropertyNode
    {
        #region Constants

        /// <summary>
        /// The name of xml element
        /// </summary>
        internal const string ELEMENT_NAME = "property";

        private const string ATTRIB_TYPE = "type";
        private const string ATTRIB_INDEX = "index";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public PropertyNode()
        {
            m_PropertyType = PropertyNodeType.Attribute;
            m_Index = 0;
            m_Clients = new List<ClientNode>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the node</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public PropertyNode(XElement element)
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                XNamespace NameSpace = element.GetDefaultNamespace();

                if (element.Attribute(ATTRIB_TYPE) != null)
                {
                    m_PropertyType = EnumDescriptionRetriever.ParseToEnum<PropertyNodeType>(element.Attribute(ATTRIB_TYPE).Value);                    
                }

                if (element.Attribute(ATTRIB_INDEX) != null)
                {
                    m_Index = sbyte.Parse(element.Attribute(ATTRIB_INDEX).Value);
                }

                m_Clients = new List<ClientNode>();

                foreach (XElement ClientElement in element.Descendants(NameSpace.GetName(ClientNode.ELEMENT_NAME)))
                {
                    m_Clients.Add(new ClientNode(ClientElement));
                }
            }
            else
            {
                throw new ArgumentException("Not a valid property element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the property type
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public PropertyNodeType PropertyType
        {
            get
            {
                return m_PropertyType;
            }
        }

        /// <summary>
        /// Gets the index
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public sbyte Index
        {
            get
            {
                return m_Index;
            }
        }

        /// <summary>
        /// Gets the list of clients
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public List<ClientNode> Clients
        {
            get
            {
                return m_Clients;
            }
        }

        #endregion

        #region Member Variables

        private PropertyNodeType m_PropertyType;
        private sbyte m_Index;
        private List<ClientNode> m_Clients;

        #endregion
    }

    /// <summary>
    /// Object for a Client Node
    /// </summary>
    public class ClientNode
    {
        #region Constants

        /// <summary>
        /// The name of the xml element
        /// </summary>
        internal const string ELEMENT_NAME = "client";

        private const string ATTRIB_NAME = "name";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public ClientNode()
        {
            m_Name = "";
            m_Access = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the node</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public ClientNode(XElement element)
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                XNamespace NameSpace = element.GetDefaultNamespace();

                if (element.Attribute(ATTRIB_NAME) != null)
                {
                    m_Name = element.Attribute(ATTRIB_NAME).Value;
                }

                if (element.Descendants(NameSpace.GetName(AccessNode.ELEMENT_NAME)).Count() > 0)
                {
                    m_Access = new AccessNode(element.Descendants(NameSpace.GetName(AccessNode.ELEMENT_NAME)).First());
                }
            }
            else
            {
                throw new ArgumentException("Not a valid client element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the name of the client
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public string Name
        {
            get
            {
                return m_Name;
            }
        }

        /// <summary>
        /// Gets the Access Node for the client
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public AccessNode Access
        {
            get
            {
                return m_Access;
            }
        }

        #endregion

        #region Member Variables

        private string m_Name;
        private AccessNode m_Access;

        #endregion
    }

    /// <summary>
    /// Object for an Access Node
    /// </summary>
    public class AccessNode
    {
        #region Constants

        /// <summary>
        /// The name of the xml element
        /// </summary>
        internal const string ELEMENT_NAME = "access";

        private const string ATTRIB_GET = "get";
        private const string ATTRIB_SET = "set";
        private const string ATTRIB_ACTION = "action";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public AccessNode()
        {
            m_Get = false;
            m_Set = false;
            m_Action = false;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the node</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public AccessNode(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIB_GET) != null)
                {
                    m_Get = bool.Parse(element.Attribute(ATTRIB_GET).Value);
                }

                if (element.Attribute(ATTRIB_SET) != null)
                {
                    m_Set = bool.Parse(element.Attribute(ATTRIB_SET).Value);
                }

                if (element.Attribute(ATTRIB_ACTION) != null)
                {
                    m_Action = bool.Parse(element.Attribute(ATTRIB_ACTION).Value);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid access element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets whether or not gets are allowed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public bool Get
        {
            get
            {
                return m_Get;
            }
        }

        /// <summary>
        /// Gets whether or not sets are allowed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public bool Set
        {
            get
            {
                return m_Set;
            }
        }

        /// <summary>
        /// Gets whether or not actions are allowed
        /// </summary>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------
        //  05/09/13 RCG 2.80.28 N/A    Created
        
        public bool Action
        {
            get
            {
                return m_Action;
            }
        }

        #endregion

        #region Member Variables

        private bool m_Get;
        private bool m_Set;
        private bool m_Action;

        #endregion
    }
}
