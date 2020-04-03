using System;
using System.Text;
using System.Xml;
using System.IO;
using System.Reflection;
using System.Diagnostics;

namespace Itron.Metering.TOU
{
    /// <summary>
    /// Static class that contains functionality for exporting files for use with
    /// the FCS system.
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ ---------------------------------------------
    //  06/18/07 RCG 8.10.09		Created

    public class FCSExport
    {
        #region Constants

        private const string MANIFEST_FILE_NAME = "CollectorSoftwarePackageManifest.xml";
        private const string COMPONENT_DIRECTORY = "Rate";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor for FCSExport
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/18/07 RCG 8.10.18		Created
        public FCSExport()
        {
            m_strVersion = "1.0.0.0";
        }

        /// <summary>
        /// Exports the TOU file to the specified directory for use with FCS TOU reconfigure
        /// </summary>
        /// <param name="strExportDirectory">The directory to export the TOU file to.</param>
        /// <param name="ExportedTOUSchedule">The TOU schedule to export.</param>
        /// <exception cref="ArgumentException">Thrown when the Export Directory does not exist or the TOU file does not exist.</exception>
        /// <exception cref="ArgumentNullException">Thrown if the ExportedTOUSchedule object is null.</exception>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/18/07 RCG 8.10.09		Created
        //  07/31/07 KRC 8.10.16        Changing FCS Export again
        //  08/22/07 KRC 8.10.21        Adding Version Number to Export Path
        //  01/21/10 AF  2.40.08 148197 Added #if's to compile under CE
        //
        public void ExportTOU(string strExportDirectory, CTOUSchedule ExportedTOUSchedule)
        {
            string strDestinationPath;
            string strFilePath;
            string strFileName = "";
            XmlWriter FCSXmlWriter;
            XmlWriterSettings FCSXmlWriterSettings;
            FileInfo TOUFileInfo;

            // We are going to rename the xml file to be TOU + the ID.
            strFileName = "TOU" + ExportedTOUSchedule.TOUID.ToString() + ".xml";

            // Build up the Version Number: Major SW Version.Minor SW Version.Upper Export Ver.Lower Export Ver
            m_strVersion = Assembly.GetExecutingAssembly().GetName().Version.Major.ToString() + "." +
                         Assembly.GetExecutingAssembly().GetName().Version.Minor.ToString();
            int nDivResult = ExportedTOUSchedule.TOUExportCount / 100;
            int nModResult = ExportedTOUSchedule.TOUExportCount % 100;
            m_strVersion += "." + nDivResult.ToString() + "." + nModResult.ToString();

            // Make sure we were given a valid directory
            if (Directory.Exists(strExportDirectory) == false)
            {
                throw new ArgumentException("Specified path does not exist.", "strExportDirectory");
            }

            if (ExportedTOUSchedule == null)
            {
                throw new ArgumentNullException("ExportedTOUSchedule", "Can not export null TOU Schedule.");
            }

            // Create the directory where we will place the files in
            strDestinationPath = strExportDirectory + "\\" + ExportedTOUSchedule.TOUName + "\\";
            strFilePath = ExportedTOUSchedule.DirectoryName + "\\" + ExportedTOUSchedule.FileName;

            // Make sure that the TOU file exists
            if (File.Exists(strFilePath) == false)
            {
                throw new ArgumentException("TOU Schedule does not exist", "ExportedTOUSchedule");
            }

            Directory.CreateDirectory(strDestinationPath);
            Directory.CreateDirectory(strDestinationPath + COMPONENT_DIRECTORY + "\\");
            Directory.CreateDirectory(strDestinationPath + COMPONENT_DIRECTORY + "\\" + m_strVersion + "\\");

            // Create the FCS .xml file
            FCSXmlWriterSettings = new XmlWriterSettings();
            FCSXmlWriterSettings.Encoding = Encoding.Unicode;
            FCSXmlWriterSettings.Indent = true;

            FCSXmlWriter = XmlWriter.Create(strDestinationPath + MANIFEST_FILE_NAME, FCSXmlWriterSettings);

            // Write the header information
            FCSXmlWriter.WriteStartDocument();
            FCSXmlWriter.WriteComment("Itron Collector Software Package Manifest");

            // Write the root node
            FCSXmlWriter.WriteStartElement("CollectorSoftwarePackageManifest", "http://www.itron.com/nVanta");

            // Write the feature node
            FCSXmlWriter.WriteStartElement("CollectorSoftwareFeature");

            FCSXmlWriter.WriteElementString("FeatureName", ExportedTOUSchedule.TOUName + " TOU Schedule");
            FCSXmlWriter.WriteElementString("Description", "Contains the " + ExportedTOUSchedule.TOUName + " TOU Schedule");
            FCSXmlWriter.WriteElementString("Version", m_strVersion);
            FCSXmlWriter.WriteElementString("InternalCollectorType", "0");
            FCSXmlWriter.WriteElementString("SupportedOSVersions", "115 : 202");

            // Write the Component node for OS 115
            FCSXmlWriter.WriteStartElement("CollectorSoftwareComponent");

            FCSXmlWriter.WriteElementString("ComponentName", ExportedTOUSchedule.FileName);
            FCSXmlWriter.WriteElementString("Description", ExportedTOUSchedule.Description);
            FCSXmlWriter.WriteElementString("Version", "1.0.0.0");
            FCSXmlWriter.WriteElementString("FileName", strFileName);

            TOUFileInfo = new FileInfo(strFilePath);
            FCSXmlWriter.WriteElementString("FileSize", TOUFileInfo.Length.ToString());

            FCSXmlWriter.WriteStartElement("FileDateTime");
#if WindowsCE // pyk - making happy CE builds
            DateTime time = TOUFileInfo.LastWriteTime - (DateTime.Now - DateTime.UtcNow);
#else
            DateTime time = TOUFileInfo.LastWriteTimeUtc;
#endif

            String strTime = time.ToString("yyyy-MM-ddTHH:mm:ss");
            FCSXmlWriter.WriteValue(strTime);
            FCSXmlWriter.WriteEndElement();

            FCSXmlWriter.WriteElementString("FileInstallLocation", COMPONENT_DIRECTORY + "\\" + m_strVersion);
            FCSXmlWriter.WriteElementString("CollectorOSVersion", "115");

            // End of CollectorSoftwareComponent Element
            FCSXmlWriter.WriteEndElement();

            // Write the Component node for OS 202
            FCSXmlWriter.WriteStartElement("CollectorSoftwareComponent");

            FCSXmlWriter.WriteElementString("ComponentName", ExportedTOUSchedule.FileName);
            FCSXmlWriter.WriteElementString("Description", ExportedTOUSchedule.Description);
            FCSXmlWriter.WriteElementString("Version", "1.0.0.0");
            FCSXmlWriter.WriteElementString("FileName", strFileName);

            FCSXmlWriter.WriteElementString("FileSize", TOUFileInfo.Length.ToString());

            FCSXmlWriter.WriteStartElement("FileDateTime");
#if WindowsCE // pyk - making happy CE builds
            time = TOUFileInfo.LastWriteTime - (DateTime.Now - DateTime.UtcNow);
#else
            time = TOUFileInfo.LastWriteTimeUtc;
#endif
            strTime = time.ToString("yyyy-MM-ddTHH:mm:ss");
            FCSXmlWriter.WriteValue(strTime);
            FCSXmlWriter.WriteEndElement();

            FCSXmlWriter.WriteElementString("FileInstallLocation", COMPONENT_DIRECTORY + "\\" + m_strVersion);
            FCSXmlWriter.WriteElementString("CollectorOSVersion", "202");

            // End of CollectorSoftwareComponent Element
            FCSXmlWriter.WriteEndElement();

            // End of CollectorSoftwareFeature Element
            FCSXmlWriter.WriteEndElement();

            // End of CollectorSoftwarePackageManifest Element
            FCSXmlWriter.WriteEndElement();

            // Close the document
            FCSXmlWriter.WriteEndDocument();
            FCSXmlWriter.Close();

            // Now Copy the TOU file to the directory
            File.Copy(strFilePath, strDestinationPath + COMPONENT_DIRECTORY + "\\" + m_strVersion + "\\" + strFileName, true);
        }


        #endregion

        #region Public Properties

        /// <summary>
        /// Provides access to the string created to represent the TOU Export Version.
        /// </summary>
        public string TOUExportVersion
        {
            get
            {
                return m_strVersion;
            }
        }

        #endregion

        #region Members

        private string m_strVersion;

        #endregion
    }
}
