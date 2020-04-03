using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// String Dictionary that can be serialized to XML
    /// </summary>
    [SettingsSerializeAs(SettingsSerializeAs.Xml)]
    public class XmlSerializableStringDictionary : StringDictionary, IXmlSerializable
    {
        #region Constants

        private const string KEYPAIR_NAME = "keyPair";
        private const string KEY_NAME = "key";
        private const string VALUE_NAME = "value";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public XmlSerializableStringDictionary()
            : base()
        {
        }

        /// <summary>
        /// Gets the XML Schema
        /// </summary>
        /// <returns>The XML Schema</returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Reads the object from XML
        /// </summary>
        /// <param name="reader">The XML Reader</param>
        public void ReadXml(XmlReader reader)
        {
            if (reader != null)
            {
                this.Clear();

                while(reader.Read() && reader.NodeType != XmlNodeType.EndElement)
                {
                    if(reader.IsStartElement() && reader.LocalName.Equals(KEYPAIR_NAME))
                    {
                        string key = reader[KEY_NAME];
                        string value = reader[VALUE_NAME];

                        Add(key, value);
                    }
                }
            }
        }

        /// <summary>
        /// Writes the object to XML
        /// </summary>
        /// <param name="writer">The XML Writer</param>
        public void WriteXml(XmlWriter writer)
        {
            foreach(DictionaryEntry curentEntry in this)
            {
                writer.WriteStartElement(KEYPAIR_NAME);
                writer.WriteAttributeString(KEY_NAME, (string)curentEntry.Key);
                writer.WriteAttributeString(VALUE_NAME, (string)curentEntry.Value);
                writer.WriteEndElement();
            }
        }

        #endregion
    }
}
