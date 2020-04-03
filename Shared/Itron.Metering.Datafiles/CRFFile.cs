///////////////////////////////////////////////////////////////////////////////
//                         PROPRIETARY RIGHTS NOTICE:
//
//All rights reserved. This material contains the valuable properties and trade
//                                secrets of
//
//                                Itron, Inc.
//                            West Union, SC., USA,
//
//   embodying substantial creative efforts and trade secrets, confidential 
//  information, ideas and expressions. No part of which may be reproduced or 
//transmitted in any form or by any means electronic, mechanical, or otherwise. 
//  Including photocopying and recording or in connection with any information 
//storage or retrieval system without the permission in writing from Itron, Inc.
//
//                           Copyright © 2008 - 2016
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Itron.Metering.DeviceDataTypes;
using Itron.Metering.Progressable;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Itron.Ami.Common.Security.X509XmlSigning;
using System.Security.Cryptography.X509Certificates;


namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// Class representing the Itron CRF File format
    /// </summary>
    //  Revision History	
    //  MM/DD/YY Who Version Issue# Description
    //  -------- --- ------- ------ -------------------------------------------
    //  06/30/09 AF  2.20.10 136279 Changed "CIM" to "CRF"
    //  09/30/09 AF  2.30.05        Added support for event CRF files
    //
    public class CRFFile : IProgressable
    {
        #region Constants

        // Node Names
        private const string ROOT_NODE = "MeterReadingDocument";
        private const string ROOT_NODE_EVENT = "EventDocument";
        private const string XML = "xml";
        private const string IMPORTEXPORTTAG = "ImportExportParameters";
        private const string DATAFORMATTAG = "DataFormat";
        private const string VERSION = "version='1.0' encoding = 'Windows-1252'";
        private const string VERSION_EVT = "version=\"1.0\" encoding=\"utf-8\"";
        
        // Attribute Names
        private const string XSITAG = "xmlns:xsi";
        private const string XSDTAG = "xmlns:xsd";
        private const string XMLNSTAG = "xmlns";

        // Temp directory
        private const string TEMPDIR = @"C:\Temp";
     
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor used for reading the data out of a CRF File
        /// </summary>
        /// <param name="strFile">Full path to the CRF FIle</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 09/23/08 KRC 2.00            Creating CRF File
        // 05/07/15 PGH 4.50.124        Read the CRF File
        //
        public CRFFile(string strFile)
        {
            m_strFile = strFile;

            if (File.Exists(strFile))
            {
                XDocument NewDocument = XDocument.Load(strFile);

                if (NewDocument.Root != null)
                {
                    if (NewDocument.Root.Name.LocalName == ROOT_NODE)
                    {
                        ReadMeterReadingDocument(NewDocument);
                    }
                    else if (NewDocument.Root.Name.LocalName == ROOT_NODE_EVENT)
                    {
                        ReadEventDocument(NewDocument);
                    }
                }
            }
            else
            {
                throw new FileNotFoundException(Properties.Resources.FILE_DOES_NOT_EXIST, strFile);
            }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version  Issue#     Description
        // -------- --- -------  ------     ---------------------------------------
        // 09/19/08 KRC 2.00                Creating CRF File
        // 07/25/16 MP  4.70.9   WR674212   Adding fields for including a meter information header
        public CRFFile(string strFile, List<CRFDataType> CRFData, string ESN, bool bUseUTC,
            bool IncludeMeterHeader = false, MeterInfoHeader MeterHeader = null)
        {
            m_strFile = strFile;
            m_CRFData = CRFData;
            m_ESN = ESN;
            m_bUseUTC = bUseUTC;
            m_AddHeader = IncludeMeterHeader;

            if (m_AddHeader && null != MeterHeader)
            {
                m_MeterHeader = MeterHeader;
            }
        }

        /// <summary>
        /// Create the CRF File
        /// </summary>
        // Revision History	
        // MM/DD/YY who Version Issue#      Description
        // -------- --- ------- ------      ---------------------------------------
        // 09/19/08 KRC 2.00                Creating CRF File
        // 11/06/08 AF  2.00.04             Added progress indication
        // 10/02/09 AF  2.30.05             Added a try/catch block to better track the results
        // 11/25/09 AF  2.30.22             Changed the catch to quiet a compiler warning about
        //                                  preserving the call stack
        // 06/04/12 RCG 2.54.00 TRQ6157     Adding support for signing CRFs
        // 07/25/16 MP  4.70.9  WR674212    Adding fields for including a meter information header
        public CreateCRFResult WriteCRFFile()
        {
            CreateCRFResult Result = CreateCRFResult.FILE_CREATION_ERROR;
            m_xmldoc = new XmlDocument();

            XmlProcessingInstruction xmlInstruction = null;
            XmlNode xmlnodeProcessing = null;
            XmlNode xmlnodeRoot = null;
            XmlAttribute newAttribute = null;

            OnShowProgress(new ShowProgressEventArgs(1, m_CRFData.Count + 1, "Writing CRF File...", "Writing CRF File..."));

            try
            {
                //Add in the processing instructions for the version
                xmlInstruction = m_xmldoc.CreateProcessingInstruction(XML,
                    VERSION);
                xmlnodeProcessing = xmlInstruction;
                m_xmldoc.AppendChild(xmlnodeProcessing);

                //Add the root node MeterReadingDocument
                xmlnodeRoot = m_xmldoc.CreateNode(XmlNodeType.Element,
                    ROOT_NODE, "");
                newAttribute = m_xmldoc.CreateAttribute(XSITAG);
                newAttribute.Value = "http://www.w3.org/2001/XMLSchema-Instance";
                xmlnodeRoot.Attributes.Append(newAttribute);

                newAttribute = m_xmldoc.CreateAttribute(XSDTAG);
                newAttribute.Value = "http://www.w3.org/2001/XMLSchema";
                xmlnodeRoot.Attributes.Append(newAttribute);
                m_xmldoc.AppendChild(xmlnodeRoot);

                //Add the ImportExportParmeters node under the the root node
                AddParametersNode(xmlnodeRoot);

                if (m_AddHeader)
                {
                    // Add Meter Information node
                    XmlAttribute infoattr = null;
                    XmlNode MeterInfoNode = m_xmldoc.CreateElement("METERINFO");

                    infoattr = m_xmldoc.CreateAttribute("ESN");
                    infoattr.Value = m_ESN;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("DeviceClass");
                    infoattr.Value = m_MeterHeader.DeviceClass;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("RegisterFirmwareVersion");
                    infoattr.Value = m_MeterHeader.FWVersion;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("MeterForm");
                    infoattr.Value = m_MeterHeader.Form;
                    MeterInfoNode.Attributes.Append(infoattr);

                    xmlnodeRoot.AppendChild(MeterInfoNode);
                }

                OnStepProgress(new ProgressEventArgs());

                foreach (CRFDataType CRFData in m_CRFData)
                {
                    Result = CRFData.Write(m_xmldoc, m_ESN);
                    OnStepProgress(new ProgressEventArgs());
                }
            }
            catch (Exception)
            {
                Result = CreateCRFResult.FILE_CREATION_ERROR;
                throw;
            }

            try
            {
                if (m_SigningCertificate != null)
                {
                    // We need to sign the file
                    X509DetachedXmlSigner Signer = new X509DetachedXmlSigner();
                    Signer.SigningCertificate = m_SigningCertificate;

                    m_xmldoc = Signer.Sign(m_xmldoc);
                }

                m_xmldoc.Save(m_strFile);
            }
            catch (Exception)
            {
                Result = CreateCRFResult.FILE_CREATION_ERROR;
            }

            OnHideProgress(new EventArgs());

            return Result;
        }

        /// <summary>
        /// Create the event CRF File
        /// </summary>
        /// <returns>The result code from writing the file</returns>
        // Revision History	
        // MM/DD/YY Who Version Issue#     Description
        // -------- --- ------- -------    -------------------------------------------
        // 09/30/09 AF  2.30.05            Created
        // 10/02/09 AF  2.30.05            Added a try/catch block to better track the results
        // 10/29/09 AF  2.30.15 144280     The xmlns:xsd attribute was missing
        // 11/25/09 AF  2.30.22            Changed the catch to quiet a compiler warning about
        //                                 preserving the call stack
        // 06/04/12 RCG 2.54.00 TRQ6157    Adding support for signing CRFs
        // 07/25/16 MP  4.70.9  WR674212   Adding fields for including a meter information header
        public CreateCRFResult WriteCRFEventFile()
        {
            CreateCRFResult Result = CreateCRFResult.FILE_CREATION_ERROR;
            m_xmldoc = new XmlDocument();

            XmlProcessingInstruction xmlInstruction = null;
            XmlNode xmlnodeProcessing = null;
            XmlNode xmlnodeRoot = null;
            XmlAttribute newAttribute = null;

            OnShowProgress(new ShowProgressEventArgs(1, m_CRFData.Count + 1, "Writing CRF Event File...", "Writing CRF Event File..."));

            try
            {
                //Add in the processing instructions for the version
                xmlInstruction = m_xmldoc.CreateProcessingInstruction(XML,
                    VERSION_EVT);
                xmlnodeProcessing = xmlInstruction;
                m_xmldoc.AppendChild(xmlnodeProcessing);

                //Add the root node, EventDocument
                xmlnodeRoot = m_xmldoc.CreateNode(XmlNodeType.Element,
                    ROOT_NODE_EVENT, "");

                newAttribute = m_xmldoc.CreateAttribute(XSDTAG);
                newAttribute.Value = "http://www.w3.org/2001/XMLSchema";
                xmlnodeRoot.Attributes.Append(newAttribute);
                m_xmldoc.AppendChild(xmlnodeRoot);

                newAttribute = m_xmldoc.CreateAttribute(XSITAG);
                newAttribute.Value = "http://www.w3.org/2001/XMLSchema-Instance";
                xmlnodeRoot.Attributes.Append(newAttribute);

                newAttribute = m_xmldoc.CreateAttribute(XMLNSTAG);
                newAttribute.Value = "http://www.itron.com/ItronInternalXsd/1.0/";
                xmlnodeRoot.Attributes.Append(newAttribute);
                m_xmldoc.AppendChild(xmlnodeRoot);

                if (m_AddHeader)
                {
                    // Add Meter Information node
                    XmlAttribute infoattr = null;
                    XmlNode MeterInfoNode = m_xmldoc.CreateElement("METERINFO");

                    infoattr = m_xmldoc.CreateAttribute("ESN");
                    infoattr.Value = m_ESN;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("DeviceClass");
                    infoattr.Value = m_MeterHeader.DeviceClass;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("RegisterFirmwareVersion");
                    infoattr.Value = m_MeterHeader.FWVersion;
                    MeterInfoNode.Attributes.Append(infoattr);

                    infoattr = m_xmldoc.CreateAttribute("MeterForm");
                    infoattr.Value = m_MeterHeader.Form;
                    MeterInfoNode.Attributes.Append(infoattr);

                    xmlnodeRoot.AppendChild(MeterInfoNode);
                }

                OnStepProgress(new ProgressEventArgs());

                foreach (CRFDataType CRFData in m_CRFData)
                {
                    Result = CRFData.Write(m_xmldoc, m_ESN);
                    OnStepProgress(new ProgressEventArgs());
                }
            }
            catch (Exception)
            {
                Result = CreateCRFResult.FILE_CREATION_ERROR;
                throw;
            }

            try
            {
                if (m_SigningCertificate != null)
                {
                    // We need to sign the file
                    X509DetachedXmlSigner Signer = new X509DetachedXmlSigner();
                    Signer.SigningCertificate = m_SigningCertificate;

                    m_xmldoc = Signer.Sign(m_xmldoc);
                }

                m_xmldoc.Save(m_strFile);
            }
            catch (Exception)
            {
                Result = CreateCRFResult.FILE_CREATION_ERROR;
            }

            OnHideProgress(new EventArgs());

            return Result;
        }

        /// <summary>
        /// Create an HTML file from a CRF Meter Reading Document
        /// </summary>
        // MM/DD/YY who Version  Issue#    Description
        // -------- --- -------  ------    ---------------------------------------
        // 05/11/15 PGH 4.50.124 RTT556298 Created
        // 10/02/15 PGH 4.50.206 RTT556298 Updated
        // 12/18/15 PGH 4.50.222 RTT556298 Updated to use CRFile class properties
        // 04/15/16 PGH 4.50.247 676157    Ignore case on .XML extension
        // 07/25/16 MP  4.70.9   WR674212  Adding fields for including a meter information header
        public void CreateHTMLFromMeterReadingDocument()
        {
            DateTime Now = DateTime.Now;
            string formattedTime = Now.ToString("yyMMddhhmmss", CultureInfo.CurrentCulture);
            string HTMLFilePath = Regex.Replace(FileName, ".XML", "_" + formattedTime + ".HTML", RegexOptions.IgnoreCase);
            string[] Items = HTMLFilePath.Split('\\');
            string HTMLFilename = Items[Items.Length - 1];

            try
            {
                // Use temp directory
                if (!Directory.Exists(TEMPDIR))
                {
                    Directory.CreateDirectory(TEMPDIR);
                }
                HTMLFilePath = TEMPDIR + "\\" + HTMLFilename;
            }
            catch (Exception)
            {
                // Use same directory as CRF file
            }

            FileStream FileStream = new FileStream(HTMLFilePath, FileMode.Create);
            StreamWriter StreamWriter = new StreamWriter(FileStream, Encoding.UTF8);

            #region CSS Setup

            Items = FileName.Split('\\');
            string Title = Items[Items.Length - 1];

            StreamWriter.WriteLine("<!DOCTYPE html>");
            StreamWriter.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            StreamWriter.WriteLine("<head runat=\"server\">");
            StreamWriter.WriteLine("    <title>" + Title.ToString(CultureInfo.CurrentCulture) + "</title>");
            StreamWriter.WriteLine("    <style type=\"text/css\">");
            StreamWriter.WriteLine("        body { font-family: Calibri; padding: 0 1em 2em 1em; margin: auto; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        ul { list-style: none; *list-style: none; padding: 0; font-weight: bold; }");
            StreamWriter.WriteLine("            ul ul { margin: 0.25em 0 0.25em 2em; font-weight: normal; }");
            StreamWriter.WriteLine("                ul ul li { margin: 0.25em 0 0.25em 0em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        .navigation a");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            position: relative;");
            StreamWriter.WriteLine("            padding: .25em .25em .25em .25em;");
            StreamWriter.WriteLine("            *padding: .1em;");
            StreamWriter.WriteLine("            margin: .25em 0 .25em 2.5em;");
            StreamWriter.WriteLine("            color: #444;");
            StreamWriter.WriteLine("            text-decoration: none;");
            StreamWriter.WriteLine("            transition: all .3s ease-out;");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:before");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                position: absolute;");
            StreamWriter.WriteLine("                left: -2.5em;");
            StreamWriter.WriteLine("                top: 50%;");
            StreamWriter.WriteLine("                margin-top: -1em;");
            StreamWriter.WriteLine("                background: #fa8072;");
            StreamWriter.WriteLine("                height: 2em;");
            StreamWriter.WriteLine("                width: 2em;");
            StreamWriter.WriteLine("                line-height: 2em;");
            StreamWriter.WriteLine("                text-align: center;");
            StreamWriter.WriteLine("                font-weight: bold;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:after");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                position: absolute;");
            StreamWriter.WriteLine("                content: '';");
            StreamWriter.WriteLine("                border: .5em solid transparent;");
            StreamWriter.WriteLine("                left: -1em;");
            StreamWriter.WriteLine("                top: 50%;");
            StreamWriter.WriteLine("                margin-top: -.5em;");
            StreamWriter.WriteLine("                transition: all .3s ease-out;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:hover:after { left: -1em; border-left-color: #606060; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h1, h2");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            color: white;");
            StreamWriter.WriteLine("            padding: 0.25em 0.5em 0.50em 0.5em;");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            -moz-border-radius: 10px;");
            StreamWriter.WriteLine("            -webkit-border-radius: 10px;");
            StreamWriter.WriteLine("            border-radius: 10px;");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            background-color: #808080;");
            StreamWriter.WriteLine("            background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#808080));");
            StreamWriter.WriteLine("            background: -moz-linear-gradient(top, #606060, #808080);");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            -moz-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            -webkit-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h2 { margin-top: 3em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h3 { float: right; margin-top: -3.5em; margin-right: 1em; font-size: 1em; }");
            StreamWriter.WriteLine("        h3 a:link, h3 a:visited { color: white; text-decoration: none; }");
            StreamWriter.WriteLine("        h3 a:hover { color: lightblue; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        table");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            border-collapse: collapse;");
            StreamWriter.WriteLine("            -moz-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            -webkit-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            text-align: left;");
            StreamWriter.WriteLine("            color: #606060;");
            StreamWriter.WriteLine("            margin: 0em 1em 0em 1em;");
            StreamWriter.WriteLine("            min-width: 540px;");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table thead tr td");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                background-color: White;");
            StreamWriter.WriteLine("                vertical-align: middle;");
            StreamWriter.WriteLine("                padding: 0.6em;");
            StreamWriter.WriteLine("                font-size: 0.8em;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table thead tr th");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                padding: 0.5em;");
            StreamWriter.WriteLine("                background-color: #808080;");
            StreamWriter.WriteLine("                background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#909090));");
            StreamWriter.WriteLine("                background: -moz-linear-gradient(top, #606060, #909090);");
            StreamWriter.WriteLine("                color: white;");
            StreamWriter.WriteLine("                text-align: left;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table tbody tr:nth-child(odd) { background-color: #fafafa; }");
            StreamWriter.WriteLine("            table tbody tr:nth-child(even) { background-color: #efefef; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table tbody tr.separator");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                background-color: #808080;");
            StreamWriter.WriteLine("                background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#909090));");
            StreamWriter.WriteLine("                background: -moz-linear-gradient(top, #606060, #909090);");
            StreamWriter.WriteLine("                color: #dadada;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table td { vertical-align: middle; padding: 0.5em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        th.datetime { width: 200px; }");
            StreamWriter.WriteLine("        th.item { width: 220px; }");
            StreamWriter.WriteLine("        th.value { width: 100px; }");
            StreamWriter.WriteLine("        th.date { width: 100px; }");
            StreamWriter.WriteLine("        th.datum { width: 150px; }");
            StreamWriter.WriteLine("        tr.expandable { cursor: pointer; }");
            StreamWriter.WriteLine("        tr.expandable:hover { color: lightblue; }");
            StreamWriter.WriteLine("        tr.clickable-row { cursor: pointer; }");
            StreamWriter.WriteLine("        tr.clickable-row:hover { color: lightblue; }");
            StreamWriter.WriteLine("    </style>");

            StreamWriter.WriteLine("<script src=\"http://code.jquery.com/jquery-1.5.1.min.js\" type=\"text/javascript\"></script>");
            StreamWriter.WriteLine("<script type=\"text/javascript\">");
            StreamWriter.WriteLine("$(document).ready(function() {");
            StreamWriter.WriteLine("$('.expandable').click(function () {");
            StreamWriter.WriteLine("$(this).nextAll('tr').each( function() {");
            StreamWriter.WriteLine("if($(this).is('.expandable')) {");
            StreamWriter.WriteLine("return false;");
            StreamWriter.WriteLine("}");
            StreamWriter.WriteLine("$(this).toggle();");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("$('.expandable').nextAll('tr').each( function() {");
            StreamWriter.WriteLine("if(!($(this).is('.expandable')))");
            StreamWriter.WriteLine("$(this).hide();");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("$('.clickable-row').click(function() {");
            StreamWriter.WriteLine("window.document.location = $(this).data(\"href\");");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("</script>");

            StreamWriter.WriteLine("</head>");

            #endregion CSS Setup

            List<NoncontiguousMeterReading> RegisterData = new List<NoncontiguousMeterReading>();
            List<NoncontiguousMeterReading> AncillaryData = new List<NoncontiguousMeterReading>();
            List<ContiguousMeterReading> IntervalData = new List<ContiguousMeterReading>();

            string ESN = "";
            bool First = true;
            bool UseSourceValidation = false;
            bool UseStatusCodes = false;
            bool SourceValidationFound = false;
            bool StatusCodesFound = false;

            if (MeterReadingDocument.Channels != null)
            {
                foreach (ChannelElement ChannelElement in MeterReadingDocument.Channels.ListOfChannels)
                {
                    // Register Data
                    if (ChannelElement.IsRegister != null && ChannelElement.IsRegister == true)
                    {
                        NoncontiguousMeterReading NoncontiguousReading = new NoncontiguousMeterReading();

                        if (ChannelElement.Readings != null && ChannelElement.Readings.ListOfReadings.Count == 1)
                        {
                            MeterReading MReading = new MeterReading();
                            if (ChannelElement.Readings.ListOfReadings[0].ReadingTime != null)
                            {
                                MReading.ReadingTime = (DateTime)ChannelElement.Readings.ListOfReadings[0].ReadingTime;
                            }
                            if (ChannelElement.Readings.ListOfReadings[0].Value != null)
                            {
                                MReading.Value = (double)ChannelElement.Readings.ListOfReadings[0].Value;
                            }
                            if (ChannelElement.Readings.ListOfReadings[0].ReadingStatus != null
                                && ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus != null
                                && ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.SourceValidaion != null)
                            {
                                MReading.SourceValidation = ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.SourceValidaion;
                                if (ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.StatusCodes != null)
                                {
                                    List<CodeElement> ListOfStatusCodes = ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.StatusCodes.ListOfCodes;
                                    foreach (CodeElement CodeElement in ListOfStatusCodes)
                                    {
                                        MReading.StatusCodes.Add(CodeElement.Code);
                                    }
                                }
                                if (string.IsNullOrEmpty(MReading.SourceValidation) == false && SourceValidationFound == false)
                                {
                                    UseSourceValidation = true;
                                    SourceValidationFound = true;
                                }
                                if (MReading.StatusCodes.Count > 0 && StatusCodesFound == false)
                                {
                                    UseStatusCodes = true;
                                    StatusCodesFound = true;
                                }
                            }
                            NoncontiguousReading.Reading = MReading;
                        }
                        if (ChannelElement.ChannelID != null && !String.IsNullOrEmpty(ChannelElement.ChannelID.EndPointUOMID))
                        {
                            string[] UOMID = ChannelElement.ChannelID.EndPointUOMID.Split(':');
                            if (UOMID.Length == 2)
                            {
                                if (First == true)
                                {
                                    ESN = UOMID[0];
                                    First = false;
                                }
                                NoncontiguousReading.UOMID = UOMID[1];
                            }
                        }

                        RegisterData.Add(NoncontiguousReading);
                    }

                    // Ancillary Data
                    if (ChannelElement.IsRegister != null && ChannelElement.IsRegister == false)
                    {
                        NoncontiguousMeterReading NoncontiguousReading = new NoncontiguousMeterReading();

                        if (ChannelElement.Readings != null && ChannelElement.Readings.ListOfReadings.Count == 1)
                        {
                            MeterReading MReading = new MeterReading();
                            if (ChannelElement.Readings.ListOfReadings[0].ReadingTime != null)
                            {
                                MReading.ReadingTime = (DateTime)ChannelElement.Readings.ListOfReadings[0].ReadingTime;
                            }
                            if (ChannelElement.Readings.ListOfReadings[0].Value != null)
                            {
                                MReading.Value = (double)ChannelElement.Readings.ListOfReadings[0].Value;
                            }
                            if (ChannelElement.Readings.ListOfReadings[0].ReadingStatus != null
                                && ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus != null
                                && ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.SourceValidaion != null)
                            {
                                MReading.SourceValidation = ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.SourceValidaion;
                                if (ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.StatusCodes != null)
                                {
                                    List<CodeElement> ListOfStatusCodes = ChannelElement.Readings.ListOfReadings[0].ReadingStatus.UnencodedStatus.StatusCodes.ListOfCodes;
                                    foreach (CodeElement CodeElement in ListOfStatusCodes)
                                    {
                                        MReading.StatusCodes.Add(CodeElement.Code);
                                    }
                                }
                                if (string.IsNullOrEmpty(MReading.SourceValidation) == false && SourceValidationFound == false)
                                {
                                    UseSourceValidation = true;
                                    SourceValidationFound = true;
                                }
                                if (MReading.StatusCodes.Count > 0 && StatusCodesFound == false)
                                {
                                    UseStatusCodes = true;
                                    StatusCodesFound = true;
                                }
                            }
                            NoncontiguousReading.Reading = MReading;
                        }
                        if (ChannelElement.ChannelID != null && !String.IsNullOrEmpty(ChannelElement.ChannelID.EndPointUOMID))
                        {
                            string[] UOMID = ChannelElement.ChannelID.EndPointUOMID.Split(':');
                            if (UOMID.Length == 2)
                            {
                                if (First == true)
                                {
                                    ESN = UOMID[0];
                                    First = false;
                                }
                                NoncontiguousReading.UOMID = UOMID[1];
                            }
                        }

                        AncillaryData.Add(NoncontiguousReading);
                    }

                    // Interval Data
                    if (ChannelElement.ContiguousIntervalSets != null && ChannelElement.ContiguousIntervalSets.ContiguousIntervalSet != null
                        && ChannelElement.ContiguousIntervalSets.ContiguousIntervalSet.TimePeriod != null
                        && ChannelElement.ContiguousIntervalSets.ContiguousIntervalSet.Readings != null)
                    {
                        ContiguousMeterReading ContiguousReading = new ContiguousMeterReading();

                        TimePeriodElement TimePeriod = ChannelElement.ContiguousIntervalSets.ContiguousIntervalSet.TimePeriod;
                        if (TimePeriod.StartTime != null)
                        {
                            ContiguousReading.StartTime = (DateTime)TimePeriod.StartTime;
                        }
                        if (TimePeriod.EndTime != null)
                        {
                            ContiguousReading.EndTime = (DateTime)TimePeriod.EndTime;
                        }

                        if (ChannelElement.ChannelID != null && !String.IsNullOrEmpty(ChannelElement.ChannelID.EndPointUOMID))
                        {
                            string[] UOMID = ChannelElement.ChannelID.EndPointUOMID.Split(':');
                            if (UOMID.Length == 2)
                            {
                                if (First == true)
                                {
                                    ESN = UOMID[0];
                                    First = false;
                                }
                                ContiguousReading.UOMID = UOMID[1];
                            }
                        }

                        if (ChannelElement.IntervalLength != null)
                        {
                            ContiguousReading.IntervalLength = (int)ChannelElement.IntervalLength;
                        }

                        ReadingsElement MeterReadingsElement = ChannelElement.ContiguousIntervalSets.ContiguousIntervalSet.Readings;
                        List<MeterReading> Readings = new List<MeterReading>();
                        int Count = 0;
                        foreach (ReadingElement ReadingElement in MeterReadingsElement.ListOfReadings)
                        {
                            MeterReading MReading = new MeterReading();

                            if (ReadingElement.Value != null)
                            {
                                MReading.Value = (double)ReadingElement.Value;
                            }

                            if (TimePeriod.StartTime != null)
                            {
                                MReading.ReadingTime = (DateTime)TimePeriod.StartTime + new TimeSpan(0, (Count * ContiguousReading.IntervalLength), 0);
                            }
                            Count++;

                            if (ReadingElement.ReadingStatus != null
                                && ReadingElement.ReadingStatus.UnencodedStatus != null
                                && ReadingElement.ReadingStatus.UnencodedStatus.SourceValidaion != null)
                            {
                                MReading.SourceValidation = ReadingElement.ReadingStatus.UnencodedStatus.SourceValidaion;
                                if (ReadingElement.ReadingStatus.UnencodedStatus.StatusCodes != null)
                                {
                                    List<CodeElement> ListOfStatusCodes = ReadingElement.ReadingStatus.UnencodedStatus.StatusCodes.ListOfCodes;
                                    foreach (CodeElement CodeElement in ListOfStatusCodes)
                                    {
                                        MReading.StatusCodes.Add(CodeElement.Code);
                                    }
                                }
                            }
                            Readings.Add(MReading);
                        }
                        ContiguousReading.Readings = Readings;
                        IntervalData.Add(ContiguousReading);
                    }
                }
            }
            
            StreamWriter.WriteLine("<body>");

            #region Meter Info

            if (MeterReadingDocument.MeterInformation != null)
            {
                // Indexes for Meter information:
                // 0 = ESN
                // 1 = Device Class
                // 2 = Register Firmware Version
                // 3 = Meter form
                // Also: There HAS to be a better way to line up the spacing, I just haven't found it yet.
                StreamWriter.WriteLine("<div id=\"MeterInfo\">");
                StreamWriter.WriteLine("\t<h1> Meter Information </h1>");

                StreamWriter.WriteLine("\t<p> <b>ESN:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" + MeterReadingDocument.MeterInformation.MeterInfo[0] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Device Class:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" + MeterReadingDocument.MeterInformation.MeterInfo[1] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Register Firmware Version:</b>&nbsp&nbsp&nbsp&nbsp&nbsp" + MeterReadingDocument.MeterInformation.MeterInfo[2] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Meter Form:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&#x2006" + MeterReadingDocument.MeterInformation.MeterInfo[3] + "</p>");

                StreamWriter.WriteLine("</div>");
            }

            #endregion

            #region Table of Contents

            //Write the Table of Contents
            StreamWriter.WriteLine("<nav role=\"navigation\" class=\"table-of-contents\" id=\"top\">");
            StreamWriter.WriteLine("    <h1>" + Properties.Resources.TABLE_OF_CONTENTS + "</h1>");
            StreamWriter.WriteLine("    <ul class=\"navigation\">");
            if (string.IsNullOrEmpty(ESN))
            {
                StreamWriter.WriteLine("        <li><a href=\"#div1\">" + Title.ToString(CultureInfo.CurrentCulture) + "</a>");
            }
            else
            {
                string EndPoint = "ESN " + ESN;
                StreamWriter.WriteLine("        <li><a href=\"#div1\">" + EndPoint.ToString(CultureInfo.CurrentCulture) + "</a>");
            }
            StreamWriter.WriteLine("        <ul>");

            if (RegisterData.Count > 0)
            {
                StreamWriter.WriteLine("    <li><a href=\"#div2\">" + Properties.Resources.REGISTER_DATA + "</a>");
            }
            if (AncillaryData.Count > 0)
            {
                StreamWriter.WriteLine("    <li><a href=\"#div3\">" + Properties.Resources.ANCILLARY_DATA + "</a>");
            }
            if (IntervalData.Count > 0)
            {
                StreamWriter.WriteLine("    <li><a href=\"#div4\">" + Properties.Resources.INTERVAL_DATA + "</a>");
            }
            StreamWriter.WriteLine("    </ul>");
            StreamWriter.WriteLine("</nav>");

            #endregion Table of Contents

            #region Register Data

            if (RegisterData.Count > 0)
            {
                StreamWriter.WriteLine("<div id=\"div2\">");
                StreamWriter.WriteLine("<h2 id=\"register\">" + Properties.Resources.REGISTER_DATA + "</h2>");
                StreamWriter.WriteLine("<h3><a href=\"#top\">" + Properties.Resources.TOP_OF_PAGE + "</a></h3>");
                StreamWriter.WriteLine("<table border=\"1\">");
                StreamWriter.WriteLine("<thead>");
                StreamWriter.WriteLine("<tr>");
                StreamWriter.WriteLine("<th class=\"datetime\">" + Properties.Resources.DATE + "</th>");
                StreamWriter.WriteLine("<th class=\"item\">" + Properties.Resources.ITEM + "</th>");
                StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.VALUE + "</th>");
                if (UseSourceValidation)
                {
                    StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.SOURCE_VALIDATION + "</th>");
                }
                if (UseStatusCodes)
                {
                    StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.STATUS_CODES + "</th>");
                }
                StreamWriter.WriteLine("</tr>");
                StreamWriter.WriteLine("</thead>");
                StreamWriter.WriteLine("<tbody>");

                foreach (NoncontiguousMeterReading MReading in RegisterData)
                {
                    StreamWriter.WriteLine("<tr>");
                    StreamWriter.WriteLine("<td>" + MReading.Reading.ReadingTime.ToString(CultureInfo.CurrentCulture) + "</td>");
                    StreamWriter.WriteLine("<td>" + MReading.UOMID.ToString(CultureInfo.CurrentCulture) + "</td>");
                    StreamWriter.WriteLine("<td>" + MReading.Reading.Value.ToString("F2", CultureInfo.CurrentCulture) + "</td>");
                    if (UseSourceValidation)
                    {
                        StreamWriter.WriteLine("<td>" + MReading.Reading.SourceValidation.ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    if (UseStatusCodes)
                    {
                        string Codes = "";
                        foreach (string Code in MReading.Reading.StatusCodes)
                        {
                            Codes += Code + " ";
                        }
                        StreamWriter.WriteLine("<td>" + Codes.ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    StreamWriter.WriteLine("</tr>");
                }
                StreamWriter.WriteLine("</tbody>");
                StreamWriter.WriteLine("</table>");
            }

            #endregion Register Data

            #region Ancillary Data

            if (AncillaryData.Count > 0)
            {
                StreamWriter.WriteLine("<div id=\"div3\">");
                StreamWriter.WriteLine("<h2 id=\"counter\">" + Properties.Resources.ANCILLARY_DATA + "</h2>");
                StreamWriter.WriteLine("<h3><a href=\"#top\">" + Properties.Resources.TOP_OF_PAGE + "</a></h3>");
                StreamWriter.WriteLine("<table border=\"1\">");
                StreamWriter.WriteLine("<thead>");
                StreamWriter.WriteLine("<tr>");
                StreamWriter.WriteLine("<th class=\"datetime\">" + Properties.Resources.DATE + "</th>");
                StreamWriter.WriteLine("<th class=\"item\">" + Properties.Resources.ITEM + "</th>");
                StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.VALUE + "</th>");
                if (UseSourceValidation)
                {
                    StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.SOURCE_VALIDATION + "</th>");
                }
                if (UseStatusCodes)
                {
                    StreamWriter.WriteLine("<th class=\"value\">" + Properties.Resources.STATUS_CODES + "</th>");
                }
                StreamWriter.WriteLine("</tr>");
                StreamWriter.WriteLine("</thead>");
                StreamWriter.WriteLine("<tbody>");

                foreach (NoncontiguousMeterReading MReading in AncillaryData)
                {
                    StreamWriter.WriteLine("<tr>");
                    StreamWriter.WriteLine("<td>" + MReading.Reading.ReadingTime.ToString(CultureInfo.CurrentCulture) + "</td>");
                    StreamWriter.WriteLine("<td>" + MReading.UOMID.ToString(CultureInfo.CurrentCulture) + "</td>");
                    StreamWriter.WriteLine("<td>" + MReading.Reading.Value.ToString(CultureInfo.CurrentCulture) + "</td>");
                    if (UseSourceValidation)
                    {
                        StreamWriter.WriteLine("<td>" + MReading.Reading.SourceValidation.ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    if (UseStatusCodes)
                    {
                        string Codes = "";
                        foreach (string Code in MReading.Reading.StatusCodes)
                        {
                            Codes += Code + " ";
                        }
                        StreamWriter.WriteLine("<td>" + Codes.ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    StreamWriter.WriteLine("</tr>");
                }
                StreamWriter.WriteLine("</tbody>");
                StreamWriter.WriteLine("</table>");
            }

            #endregion

            #region Interval Data

            List<DateTime> ReadingTimes = new List<DateTime>();
            List<string> Measurements = new List<string>();
            List<string> StatusCodes = new List<string>();
            List<string> SourceValidations = new List<string>();
            string Header = "";
            First = true;

            foreach (ContiguousMeterReading ContiguousIntervalData in IntervalData)
            {
                if (First == true)
                {
                    UseSourceValidation = false;
                    UseStatusCodes = false;
                    Header += ContiguousIntervalData.UOMID;
                    foreach (MeterReading Reading in ContiguousIntervalData.Readings)
                    {
                        string Codes = "";
                        foreach (string Code in Reading.StatusCodes)
                        {
                            Codes += Code + " ";
                        }
                        if (!string.IsNullOrEmpty(Codes) && !UseStatusCodes)
                        {
                            UseStatusCodes = true;
                        }
                        ReadingTimes.Add(Reading.ReadingTime);
                        string MReading = Reading.Value.ToString("F2", CultureInfo.CurrentCulture);
                        Measurements.Add(MReading);
                        string StatusCode = Codes.ToString(CultureInfo.CurrentCulture);
                        StatusCodes.Add(StatusCode);
                        string SourceValidation = Reading.SourceValidation.ToString(CultureInfo.CurrentCulture);
                        if (!string.IsNullOrEmpty(SourceValidation) && !UseSourceValidation)
                        {
                            UseSourceValidation = true;
                        }
                        SourceValidations.Add(SourceValidation);
                    }
                    First = false;
                }
                else
                {
                    Header += ":" + ContiguousIntervalData.UOMID;
                    int Count = 0;
                    foreach (MeterReading Reading in ContiguousIntervalData.Readings)
                    {
                        string MReading = Reading.Value.ToString("F2", CultureInfo.CurrentCulture);
                        Measurements[Count] += ":" + MReading;
                        Count++;
                    }
                }
            }

            if (UseSourceValidation)
            {
                Header += ":" + Properties.Resources.SOURCE_VALIDATION;
            }
            if (UseStatusCodes)
            {
                Header += ":" + Properties.Resources.STATUS_CODES;
            }

            if (Measurements.Count > 0)
            {
                StreamWriter.WriteLine("<div id=\"div4\">");

                StreamWriter.WriteLine("<h2 id=\"interval_data\">" + Properties.Resources.INTERVAL_DATA + "</h2>");
                StreamWriter.WriteLine("<h3><a href=\"#top\">" + Properties.Resources.TOP_OF_PAGE + "</a></h3>");

                StreamWriter.WriteLine("<table border=\"1\">");
                StreamWriter.WriteLine("<thead>");
                StreamWriter.WriteLine("<tr>");
                StreamWriter.WriteLine("<th class=\"date\"></th>");

                string[] HItems = Header.Split(':');

                foreach (string Item in HItems)
                {
                    StreamWriter.WriteLine("<th class=\"datum\">" + Item.ToString(CultureInfo.CurrentCulture) + "</th>");
                }

                StreamWriter.WriteLine("</tr>");
                StreamWriter.WriteLine("</thead>");
                StreamWriter.WriteLine("<tbody>");

                int ColSpan = HItems.Length + 1;
                DateTime CurrentDay = DateTime.MinValue;
                int CurrentHour = 0;
                int NumDays = 0;
                string Day = "";

                for (int iIndex = 0; iIndex < ReadingTimes.Count; iIndex++)
                {
                    DateTime CurrentInterval = ReadingTimes[iIndex];

                    // make the day expandable
                    if (iIndex == 0 || CurrentDay != CurrentInterval.Date)
                    {
                        NumDays++;
                        Day = "Day" + NumDays.ToString(CultureInfo.InvariantCulture);
                        CurrentDay = CurrentInterval.Date;
                        StreamWriter.WriteLine("<tr class=\"expandable\" id=\"" + Day + "\">");
                        StreamWriter.WriteLine("<td colspan=\"" + ColSpan.ToString(CultureInfo.InvariantCulture) + "\"><b>" + CurrentDay.ToShortDateString().ToString(CultureInfo.InvariantCulture) + "</b></td>");
                        StreamWriter.WriteLine("</tr>");
                    }

                    // make the hour a clickable row
                    if (iIndex == 0 || CurrentHour != CurrentInterval.Hour)
                    {
                        DateTime CurrentTime;
                        CurrentHour = CurrentInterval.Hour;
                        CurrentTime = CurrentDay.AddHours((double)CurrentHour);
                        StreamWriter.WriteLine("<tr class=\"clickable-row\" data-href=\"#" + Day + "\">");
                        StreamWriter.WriteLine("<td colspan=\"" + ColSpan.ToString(CultureInfo.InvariantCulture) + "\"><b>" + CurrentTime.ToShortTimeString().ToString(CultureInfo.InvariantCulture) + "</b></td>");
                        StreamWriter.WriteLine("</tr>");
                    }

                    StreamWriter.WriteLine("<tr>");
                    StreamWriter.WriteLine("<td>" + CurrentInterval.ToShortTimeString().ToString(CultureInfo.CurrentCulture) + "</td>");
                    string[] MItems = Measurements[iIndex].Split(':');
                    foreach (string Item in MItems)
                    {
                        StreamWriter.WriteLine("<td>" + Item.ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    if (UseSourceValidation)
                    {
                        StreamWriter.WriteLine("<td>" + SourceValidations[iIndex].ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    if (UseStatusCodes)
                    {
                        StreamWriter.WriteLine("<td>" + StatusCodes[iIndex].ToString(CultureInfo.CurrentCulture) + "</td>");
                    }
                    StreamWriter.WriteLine("</tr>");
                }


                StreamWriter.WriteLine("</tbody>");
                StreamWriter.WriteLine("</table>");
            }

            #endregion Contiguous Meter Readings

            StreamWriter.WriteLine("</body>");
            StreamWriter.WriteLine("</html>");

            //Closing StreamWriter also closes FileStream
            StreamWriter.Close();

            if (File.Exists(HTMLFilePath))
            {
                System.Diagnostics.Process.Start(HTMLFilePath);
            }
        }

        /// <summary>
        /// Create an HTML file from a CRF Event Document
        /// </summary>
        // MM/DD/YY who Version  Issue#    Description
        // -------- --- -------  ------    ---------------------------------------
        // 05/11/15 PGH 4.50.124 RTT556298 Created
        // 10/08/15 PGH 4.50.207 RTT556298 Updated
        // 12/18/15 PGH 4.50.222 RTT556298 Updated to use CRFile class properties
        // 04/15/16 PGH 4.50.247 676157    Ignore case on .XML extension
        // 07/25/16 MP  4.70.9   WR674212   Adding fields for including a meter information header
        public void CreateHTMLFromEventDocument()
        {
            DateTime Now = DateTime.Now;
            string formattedTime = Now.ToString("yyMMddhhmmss", CultureInfo.CurrentCulture);
            string HTMLFilePath = Regex.Replace(FileName, ".XML", "_" + formattedTime + ".HTML", RegexOptions.IgnoreCase);
            string[] Items = HTMLFilePath.Split('\\');
            string HTMLFilename = Items[Items.Length - 1];

            try
            {
                // Use temp directory
                if (!Directory.Exists(TEMPDIR))
                {
                    Directory.CreateDirectory(TEMPDIR);
                }
                HTMLFilePath = TEMPDIR + "\\" + HTMLFilename;
            }
            catch (Exception)
            {
                // Use same directory as CRF file
            }

            FileStream FileStream = new FileStream(HTMLFilePath, FileMode.Create);
            StreamWriter StreamWriter = new StreamWriter(FileStream, Encoding.UTF8);

            #region CSS Setup

            Items = FileName.Split('\\');
            string Title = Items[Items.Length - 1];

            StreamWriter.WriteLine("<!DOCTYPE html>");
            StreamWriter.WriteLine("<html xmlns=\"http://www.w3.org/1999/xhtml\">");
            StreamWriter.WriteLine("<head runat=\"server\">");
            StreamWriter.WriteLine("    <title>" + Title.ToString(CultureInfo.CurrentCulture) + "</title>");
            StreamWriter.WriteLine("    <style type=\"text/css\">");
            StreamWriter.WriteLine("        body { font-family: Calibri; padding: 0 1em 2em 1em; margin: auto; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        ul { list-style: none; *list-style: none; padding: 0; font-weight: bold; }");
            StreamWriter.WriteLine("            ul ul { margin: 0.25em 0 0.25em 2em; font-weight: normal; }");
            StreamWriter.WriteLine("                ul ul li { margin: 0.25em 0 0.25em 0em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        .navigation a");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            position: relative;");
            StreamWriter.WriteLine("            padding: .25em .25em .25em .25em;");
            StreamWriter.WriteLine("            *padding: .1em;");
            StreamWriter.WriteLine("            margin: .25em 0 .25em 2.5em;");
            StreamWriter.WriteLine("            color: #444;");
            StreamWriter.WriteLine("            text-decoration: none;");
            StreamWriter.WriteLine("            transition: all .3s ease-out;");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:before");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                position: absolute;");
            StreamWriter.WriteLine("                left: -2.5em;");
            StreamWriter.WriteLine("                top: 50%;");
            StreamWriter.WriteLine("                margin-top: -1em;");
            StreamWriter.WriteLine("                background: #fa8072;");
            StreamWriter.WriteLine("                height: 2em;");
            StreamWriter.WriteLine("                width: 2em;");
            StreamWriter.WriteLine("                line-height: 2em;");
            StreamWriter.WriteLine("                text-align: center;");
            StreamWriter.WriteLine("                font-weight: bold;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:after");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                position: absolute;");
            StreamWriter.WriteLine("                content: '';");
            StreamWriter.WriteLine("                border: .5em solid transparent;");
            StreamWriter.WriteLine("                left: -1em;");
            StreamWriter.WriteLine("                top: 50%;");
            StreamWriter.WriteLine("                margin-top: -.5em;");
            StreamWriter.WriteLine("                transition: all .3s ease-out;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            .navigation a:hover:after { left: -1em; border-left-color: #606060; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h1, h2");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            color: white;");
            StreamWriter.WriteLine("            padding: 0.25em 0.5em 0.50em 0.5em;");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            -moz-border-radius: 10px;");
            StreamWriter.WriteLine("            -webkit-border-radius: 10px;");
            StreamWriter.WriteLine("            border-radius: 10px;");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            background-color: #808080;");
            StreamWriter.WriteLine("            background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#808080));");
            StreamWriter.WriteLine("            background: -moz-linear-gradient(top, #606060, #808080);");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            -moz-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            -webkit-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h2 { margin-top: 3em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        h3 { float: right; margin-top: -3.5em; margin-right: 1em; font-size: 1em; }");
            StreamWriter.WriteLine("        h3 a:link, h3 a:visited { color: white; text-decoration: none; }");
            StreamWriter.WriteLine("        h3 a:hover { color: lightblue; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        table");
            StreamWriter.WriteLine("        {");
            StreamWriter.WriteLine("            border-collapse: collapse;");
            StreamWriter.WriteLine("            -moz-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            -webkit-box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            box-shadow: 5px 5px 10px rgba(0,0,0,0.3);");
            StreamWriter.WriteLine("            text-align: left;");
            StreamWriter.WriteLine("            color: #606060;");
            StreamWriter.WriteLine("            margin: 0em 1em 0em 1em;");
            StreamWriter.WriteLine("            min-width: 540px;");
            StreamWriter.WriteLine("        }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table thead tr td");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                background-color: White;");
            StreamWriter.WriteLine("                vertical-align: middle;");
            StreamWriter.WriteLine("                padding: 0.6em;");
            StreamWriter.WriteLine("                font-size: 0.8em;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table thead tr th");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                padding: 0.5em;");
            StreamWriter.WriteLine("                background-color: #808080;");
            StreamWriter.WriteLine("                background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#909090));");
            StreamWriter.WriteLine("                background: -moz-linear-gradient(top, #606060, #909090);");
            StreamWriter.WriteLine("                color: white;");
            StreamWriter.WriteLine("                text-align: left;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table tbody tr:nth-child(odd) { background-color: #fafafa; }");
            StreamWriter.WriteLine("            table tbody tr:nth-child(even) { background-color: #efefef; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table tbody tr.separator");
            StreamWriter.WriteLine("            {");
            StreamWriter.WriteLine("                background-color: #808080;");
            StreamWriter.WriteLine("                background: -webkit-gradient(linear, left top, left bottom, from(#606060), to(#909090));");
            StreamWriter.WriteLine("                background: -moz-linear-gradient(top, #606060, #909090);");
            StreamWriter.WriteLine("                color: #dadada;");
            StreamWriter.WriteLine("            }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("            table td { vertical-align: middle; padding: 0.5em; }");
            StreamWriter.WriteLine("");
            StreamWriter.WriteLine("        th.datetime { width: 200px; }");
            StreamWriter.WriteLine("        th.item { width: 100px; }");
            StreamWriter.WriteLine("        th.value { width: 100px; }");
            StreamWriter.WriteLine("        th.date { width: 100px; }");
            StreamWriter.WriteLine("        th.datum { width: 150px; }");
            StreamWriter.WriteLine("        th.desc { width: 350px; }");
            StreamWriter.WriteLine("        tr.expandable { cursor: pointer; }");
            StreamWriter.WriteLine("        tr.expandable:hover { color: lightblue; }");
            StreamWriter.WriteLine("        tr.clickable-row { cursor: pointer; }");
            StreamWriter.WriteLine("        tr.clickable-row:hover { color: lightblue; }");
            StreamWriter.WriteLine("    </style>");

            StreamWriter.WriteLine("<script src=\"http://code.jquery.com/jquery-1.5.1.min.js\" type=\"text/javascript\"></script>");
            StreamWriter.WriteLine("<script type=\"text/javascript\">");
            StreamWriter.WriteLine("$(document).ready(function() {");
            StreamWriter.WriteLine("$('.expandable').click(function () {");
            StreamWriter.WriteLine("$(this).nextAll('tr').each( function() {");
            StreamWriter.WriteLine("if($(this).is('.expandable')) {");
            StreamWriter.WriteLine("return false;");
            StreamWriter.WriteLine("}");
            StreamWriter.WriteLine("$(this).toggle();");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("$('.expandable').nextAll('tr').each( function() {");
            StreamWriter.WriteLine("if(!($(this).is('.expandable')))");
            StreamWriter.WriteLine("$(this).hide();");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("$('.clickable-row').click(function() {");
            StreamWriter.WriteLine("window.document.location = $(this).data(\"href\");");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("});");
            StreamWriter.WriteLine("</script>");

            StreamWriter.WriteLine("</head>");

            #endregion CSS Setup

            string ESN = "";
            bool First = true;

            List<EventDocumentEvent> EventData = new List<EventDocumentEvent>();

            if (EventDocument.Events != null)
            {
                foreach (EventElement EventElement in EventDocument.Events.ListOfEvents)
                {
                    EventDocumentEvent Event = new EventDocumentEvent();
                    if (EventElement.CollectionSystemIDElement != null && EventElement.CollectionSystemIDElement.CollectionSystemID != null)
                    {
                        Event.CollectionSystemID = EventElement.CollectionSystemIDElement.CollectionSystemID;
                    }
                    if (EventElement.ObjectIDElement != null && EventElement.ObjectIDElement.ObjectID != null)
                    {
                        Event.ObjectID = EventElement.ObjectIDElement.ObjectID;
                        if (First)
                        {
                            First = false;
                            ESN = Event.ObjectID;
                        }
                    }
                    if (EventElement.ObjectTypeElement != null && EventElement.ObjectTypeElement.ObjectType != null)
                    {
                        Event.ObjectType = EventElement.ObjectTypeElement.ObjectType;
                    }
                    if (EventElement.EventTypeElement != null && EventElement.EventTypeElement.EventType != null)
                    {
                        Event.EventType = EventElement.EventTypeElement.EventType;
                    }
                    if (EventElement.IsHistoricalElement != null && EventElement.IsHistoricalElement.IsHistorical != null)
                    {
                        Event.IsHistorical = (bool)EventElement.IsHistoricalElement.IsHistorical;
                    }
                    if (EventElement.EventDateTimeElement != null && EventElement.EventDateTimeElement.EventDateTime != null)
                    {
                        Event.EventDateTime = (DateTime)EventElement.EventDateTimeElement.EventDateTime;
                    }
                    if (EventElement.CaptureDateTimeElement != null && EventElement.CaptureDateTimeElement.CaptureDateTime != null)
                    {
                        Event.CaptureDateTime = (DateTime)EventElement.CaptureDateTimeElement.CaptureDateTime;
                    }
                    EventData.Add(Event);
                }
            }
            StreamWriter.WriteLine("<body>");

            #region Meter Info

            if (EventDocument.MeterInformation != null)
            {
                // Indexes for Meter information:
                // 0 = ESN
                // 1 = Device Class
                // 2 = Register Firmware Version
                // 3 = Meter Form
                // Also: There HAS to be a better way to line up the spacing, I just haven't found it yet.
                StreamWriter.WriteLine("<div id=\"MeterInfo\">");
                StreamWriter.WriteLine("\t<h1> Meter Information </h1>");

                StreamWriter.WriteLine("\t<p> <b>ESN:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" + EventDocument.MeterInformation.MeterInfo[0] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Device Class:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" + EventDocument.MeterInformation.MeterInfo[1] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Register Firmware Version:</b>&nbsp&nbsp&nbsp&nbsp&nbsp" + EventDocument.MeterInformation.MeterInfo[2] + "</p>");

                StreamWriter.WriteLine("\t<p> <b>Meter Form:</b>&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp" +
                    "&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&nbsp&#x2006" + EventDocument.MeterInformation.MeterInfo[3] + "</p>");

                StreamWriter.WriteLine("</div>");
            }

            #endregion

            #region Table of Contents
          
            //Write the Table of Contents
            StreamWriter.WriteLine("<nav role=\"navigation\" class=\"table-of-contents\" id=\"top\">");
            StreamWriter.WriteLine("    <h1>" + Properties.Resources.TABLE_OF_CONTENTS + "</h1>");
            StreamWriter.WriteLine("    <ul class=\"navigation\">");
            if (string.IsNullOrEmpty(ESN))
            {
                StreamWriter.WriteLine("        <li><a href=\"#div1\">" + Title.ToString(CultureInfo.CurrentCulture) + "</a>");
            }
            else
            {
                string EndPoint = "ESN " + ESN;
                StreamWriter.WriteLine("        <li><a href=\"#div1\">" + EndPoint.ToString(CultureInfo.CurrentCulture) + "</a>");
            }
            StreamWriter.WriteLine("        <ul>");

            if (EventData.Count > 0)
            {
                StreamWriter.WriteLine("    <li><a href=\"#div2\">" + Properties.Resources.EVENT_DATA + "</a>");
            }

            StreamWriter.WriteLine("    </ul>");
            StreamWriter.WriteLine("</nav>");

            #endregion Table of Contents

            #region Event Data

            StreamWriter.WriteLine("<div id=\"div2\">");

            StreamWriter.WriteLine("<h2 id=\"interval_data\">" + Properties.Resources.EVENT_DATA + "</h2>");
            StreamWriter.WriteLine("<h3><a href=\"#top\">" + Properties.Resources.TOP_OF_PAGE + "</a></h3>");

            StreamWriter.WriteLine("<table border=\"1\">");
            StreamWriter.WriteLine("<thead>");
            StreamWriter.WriteLine("<tr>");
            StreamWriter.WriteLine("<th class=\"date\"></th>");
            StreamWriter.WriteLine("<th class=\"desc\">" + Properties.Resources.EVENT + "</th>");
            StreamWriter.WriteLine("</tr>");
            StreamWriter.WriteLine("</thead>");
            StreamWriter.WriteLine("<tbody>");

            int ColSpan = 3;
            DateTime CurrentDay = DateTime.MinValue;
            int CurrentHour = 0;
            int NumDays = 0;
            string Day = "";

            List<EventDocumentEvent> SortedEventData = EventData.OrderBy(o => o.EventDateTime).ToList();
            for (int iIndex = 0; iIndex < EventData.Count; iIndex++)
            {
                DateTime CurrentInterval = SortedEventData[iIndex].EventDateTime;

                // make the day expandable
                if (iIndex == 0 || CurrentDay != CurrentInterval.Date)
                {
                    NumDays++;
                    Day = "Day" + NumDays.ToString(CultureInfo.InvariantCulture);
                    CurrentDay = CurrentInterval.Date;
                    StreamWriter.WriteLine("<tr class=\"expandable\" id=\"" + Day + "\">");
                    StreamWriter.WriteLine("<td colspan=\"" + ColSpan.ToString(CultureInfo.InvariantCulture) + "\"><b>" + CurrentDay.ToShortDateString().ToString(CultureInfo.InvariantCulture) + "</b></td>");
                    StreamWriter.WriteLine("</tr>");
                }

                // make the hour a clickable row
                if (iIndex == 0 || CurrentHour != CurrentInterval.Hour)
                {
                    DateTime CurrentTime;
                    CurrentHour = CurrentInterval.Hour;
                    CurrentTime = CurrentDay.AddHours((double)CurrentHour);
                    StreamWriter.WriteLine("<tr class=\"clickable-row\" data-href=\"#" + Day + "\">");
                    StreamWriter.WriteLine("<td colspan=\"" + ColSpan.ToString(CultureInfo.InvariantCulture) + "\"><b>" + CurrentTime.ToShortTimeString().ToString(CultureInfo.InvariantCulture) + "</b></td>");
                    StreamWriter.WriteLine("</tr>");
                }

                StreamWriter.WriteLine("<tr>");
                StreamWriter.WriteLine("<td>" + CurrentInterval.ToShortTimeString().ToString(CultureInfo.CurrentCulture) + "</td>");
                StreamWriter.WriteLine("<td>" + SortedEventData[iIndex].EventType.ToString(CultureInfo.CurrentCulture) + "</td>");
                StreamWriter.WriteLine("</tr>");
            }


            StreamWriter.WriteLine("</tbody>");
            StreamWriter.WriteLine("</table>");

            #endregion Event Data

            StreamWriter.WriteLine("</body>");
            StreamWriter.WriteLine("</html>");

            //Closing StreamWriter also closes FileStream
            StreamWriter.Close();

            if (File.Exists(HTMLFilePath))
            {
                System.Diagnostics.Process.Start(HTMLFilePath);
            }
        }

        #endregion

        #region Public Properties
        /// <summary>
        /// Public Property that will return the LoadProfileData object
        /// that is built from the defined CRFFile.
        /// </summary>
        public LoadProfileData LPData
        {
            get
            {
                LoadProfileData ProfileData = null;
                foreach (CRFDataType CRFData in m_CRFData)
                {
                    if (CRFData.GetType() == typeof(CRFLPDataType))
                    {
                        CRFLPDataType CRFLPData = CRFData as CRFLPDataType;
                        ProfileData = CRFLPData.LPData;
                    }
                }

                return ProfileData;
            }
        }

        /// <summary>
        /// Property to provide the file name of the current CRF file
        /// </summary>
        public string FileName
        {
            get
            {
                return m_strFile;
            }
        }

        /// <summary>
        /// Meter Reading Document
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public MeterReadingDocument MeterReadingDocument { get; private set; }

        /// <summary>
        /// Event Document
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public EventDocument EventDocument { get; private set; }

        /// <summary>
        /// Gets or sets the SigningCertificate used when creating the CRF
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version Issue#  Description
        //  -------- --- ------- ------- ---------------------------------------------
        //  06/04/12 RCG 2.54.00 TRQ6157 Created
        
        public X509Certificate2 SigningCertificate
        {
            get
            {
                return m_SigningCertificate;
            }
            set
            {
                m_SigningCertificate = value;
            }
        }

        #endregion

        #region Public Events

        /// <summary>
        /// Event used to display a progress bar
        /// </summary>
        public virtual event ShowProgressEventHandler ShowProgressEvent;

        /// <summary>
        /// Event used to cause a progress bar to perform a step
        /// </summary>
        public virtual event StepProgressEventHandler StepProgressEvent;

        /// <summary>
        /// Event used to hide a progress bar
        /// </summary>
        public virtual event HideProgressEventHandler HideProgressEvent;

        #endregion

        #region Protected Methods

        /// <summary>
        /// Raises the event to show the progress bar.
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/06/08 AF                 Adding progressable support	

        protected virtual void OnShowProgress(ShowProgressEventArgs e)
        {
            if (ShowProgressEvent != null)
            {
                ShowProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that causes the progress bar to perform a step
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/06/08 AF                 Adding progressable support	

        protected virtual void OnStepProgress(ProgressEventArgs e)
        {
            if (StepProgressEvent != null)
            {
                StepProgressEvent(this, e);
            }
        }

        /// <summary>
        /// Raises the event that hides or closes the progress bar
        /// </summary>
        /// <param name="e">The event arguments to use.</param>
        // Revision History	
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 11/06/08 AF                 Adding progressable support	

        protected virtual void OnHideProgress(EventArgs e)
        {
            if (HideProgressEvent != null)
            {
                HideProgressEvent(this, e);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Adds the Description node to the specified root node.
        /// </summary>
        /// <param name="xmlnodeRoot">The root node to add the description to.</param>
        /// Revision History
        /// MM/DD/YY who Version Issue# Description
        /// -------- --- ------- ------ ---------------------------------------
        /// 09/23/08 KRC 2.00.00 N/A	Creation of class  

        private void AddParametersNode(XmlNode xmlnodeRoot)
        {
            XmlAttribute newAttribute;
            XmlNode xmlnodeParameters = m_xmldoc.CreateElement(IMPORTEXPORTTAG);
            newAttribute = m_xmldoc.CreateAttribute("CreateResubmitFile");
            newAttribute.Value = "false";
            xmlnodeParameters.Attributes.Append(newAttribute);
            xmlnodeRoot.AppendChild(xmlnodeParameters);

            XmlNode xmlNodeDataFormat = m_xmldoc.CreateElement(DATAFORMATTAG);
            newAttribute = m_xmldoc.CreateAttribute("DSTTransitionType");
            newAttribute.Value = "NIST_Compliant";
            xmlNodeDataFormat.Attributes.Append(newAttribute);
            newAttribute = m_xmldoc.CreateAttribute("ReadingTimestampType");

            if (m_bUseUTC)
            {
                newAttribute.Value = "Utc";
            }
            else
            {
                newAttribute.Value = "MeterDefault";
            }

            xmlNodeDataFormat.Attributes.Append(newAttribute);
            xmlnodeParameters.AppendChild(xmlNodeDataFormat);
        }

        /// <summary>
        /// Read the CRF Meter Reading Document from the XDocument
        /// </summary>
        /// <param name="NewDocument"></param>
        private void ReadMeterReadingDocument(XDocument NewDocument)
        {
            XElement Root = NewDocument.Root;
            MeterReadingDocument = new MeterReadingDocument(Root);
        }

        /// <summary>
        /// Read the CRF Event Document from the XDocument
        /// </summary>
        /// <param name="NewDocument"></param>
        private void ReadEventDocument(XDocument NewDocument)
        {
            XElement Root = NewDocument.Root;
            EventDocument = new EventDocument(Root);
        }


        #endregion
        
        #region Members

        private string m_strFile = "";
        private List<CRFDataType> m_CRFData = null;
        private XmlDocument m_xmldoc = null;
        private string m_ESN = "";
        private bool m_bUseUTC = false;
        private X509Certificate2 m_SigningCertificate = null;
        private bool m_AddHeader = true;
        private MeterInfoHeader m_MeterHeader = null;

        #endregion
    }

    /// <summary>
    /// Enumerates the possible return codes from a request to create an CRF file from
    /// a meter.  Note that CRF files are only supported on OpenWay meters and are
    /// roughly equivalent to HHF files.
    /// </summary>
    public enum CreateCRFResult : byte
    {
        /// <summary>
        /// SUCCES = 0, Operation succeeded
        /// </summary>
        SUCCESS = 0,
        /// <summary>
        /// INVALID_PATH = 1, The file path does not exist
        /// </summary>
        INVALID_PATH = 1,
        /// <summary>
        /// File Creation Error = 2, File could not be created.
        /// </summary>
        FILE_CREATION_ERROR = 2,
        /// <summary>
        /// PROTOCOL_ERROR = 3, An error occurred while reading the data
        /// </summary>
        PROTOCOL_ERROR = 3,
        /// <summary>
        /// SECURITY_ERROR = 4, There is insufficient security to perform the operation
        /// </summary>
        SECURITY_ERROR = 4,
    }

    /// <summary>
    /// Class for CRF file attribute
    /// </summary>
    public class CRFFileAttribute
    {

        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public CRFFileAttribute()
        {
            Name = "";
            Value = "";
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Attribute Name
        /// </summary>
        public string Name
        {

            get
            {

                return m_Name;
            }
            set
            {
                m_Name = value;
            }
        }

        /// <summary>
        /// Gets the Attribute Value
        /// </summary>
        public string Value
        {

            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }

        }

        #endregion

        #region Members

        private string m_Name;
        private string m_Value;

        #endregion
    }

    #region MeterReadingDocument

    /// <summary>
    /// Class for storing a CRF MeterReadingDocument XML file
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version  Issue#    Description
    //  -------- --- -------  -------   ---------------------------------------------
    //  05/12/15 PGH 4.50.124 RTT556298 Created
    // 07/25/16 MP  4.70.9   WR674212   Adding fields for including a meter information header
    public class MeterReadingDocument
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "MeterReadingDocument";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public MeterReadingDocument()
        {
             Attributes = new List<CRFFileAttribute>();
             ImportExportParameters = null;
             Channels = null;
             m_MeterInfo = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the MeterReadingDocument</param>
        public MeterReadingDocument(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                if (element.HasAttributes)
                {
                    IEnumerable<XAttribute> ElementAttributes = element.Attributes().Where(e => e.Name.LocalName.Equals(ELEMENT_NAME));
                    foreach(XAttribute Attribute in ElementAttributes)
                    {
                      CRFFileAttribute CRFAttribute = new CRFFileAttribute();
                      CRFAttribute.Name = Attribute.Name.LocalName;
                      CRFAttribute.Value = Attribute.Value;
                      Attributes.Add(CRFAttribute);
                    }
                }

                // Descendants
                 IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ImportExportParametersElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ImportExportParameters = new ImportExportParametersElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(MeterInfoElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    m_MeterInfo = new MeterInfoElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ChannelsElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    Channels = new ChannelsElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid MeterReadingDocument element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the XML attributes for the MeterReadingDocument element
        /// </summary>
        public List<CRFFileAttribute> Attributes { get; private set; }

        /// <summary>
        /// Gets the ImportExportParameters element
        /// </summary>
        public ImportExportParametersElement ImportExportParameters { get; private set; }

        /// <summary>
        /// Gets the Channels element
        /// </summary>
        public ChannelsElement Channels { get; private set; }

        /// <summary>
        /// Gets list of meter information
        /// </summary>
        public MeterInfoElement MeterInformation { get { return m_MeterInfo; } }

        #endregion

        #region Members

        private MeterInfoElement m_MeterInfo = null;

        #endregion

    }

    /// <summary>
    /// Class for ImportExportParameters element
    /// </summary>
    public class ImportExportParametersElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_CREATE_RESUBMIT_FILE = "CreateResubmitFile";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ImportExportParameters";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ImportExportParametersElement()
        {
            Attributes = new List<CRFFileAttribute>();
            DataFormat = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ImportExportParameters</param>
        public ImportExportParametersElement(XElement element)
            : this()
        {

            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                if (element.Attribute(ATTRIBUTE_CREATE_RESUBMIT_FILE) != null)
                {
                    CRFFileAttribute Attribute = new CRFFileAttribute();
                    Attribute.Name = ATTRIBUTE_CREATE_RESUBMIT_FILE;
                    Attribute.Value = element.Attribute(ATTRIBUTE_CREATE_RESUBMIT_FILE).Value;
                    Attributes.Add(Attribute);
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(DataFormatElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    DataFormat = new DataFormatElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid ImportExportParameters element", "element");
            }

        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the XML attributes for the ImportExportParameters element
        /// </summary>
        public List<CRFFileAttribute> Attributes { get; private set; }

        /// <summary>
        /// Gets the DataFormat element
        /// </summary>
        public DataFormatElement DataFormat { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for MeterInfo element
    /// </summary>
    public class MeterInfoElement
    {
        #region Constants

        /// <summary>
        /// Element name for meter info
        /// </summary>
        public const string ELEMENT_NAME = "METERINFO";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="Element"></param>
        public MeterInfoElement(XElement Element)
        {
            // Collect all of our attributes!
            string AttributeValue;
            m_MeterInfo = new List<string>();
            if (Element != null && Element.Name.LocalName == ELEMENT_NAME)
            {
                if (Element.Attribute("ESN") != null)
                {
                    AttributeValue = Element.Attribute("ESN").Value;
                    m_MeterInfo.Add(AttributeValue);
                }
                if (Element.Attribute("DeviceClass") != null)
                {
                    AttributeValue = Element.Attribute("DeviceClass").Value;
                    m_MeterInfo.Add(AttributeValue);
                }
                if (Element.Attribute("RegisterFirmwareVersion") != null)
                {
                    AttributeValue = Element.Attribute("RegisterFirmwareVersion").Value;
                    m_MeterInfo.Add(AttributeValue);
                }
                if (Element.Attribute("ConfigurationVersion") != null)
                {
                    AttributeValue = Element.Attribute("ConfigurationVersion").Value;
                    m_MeterInfo.Add(AttributeValue);
                }
                if (Element.Attribute("MeterForm") != null)
                {
                    AttributeValue = Element.Attribute("MeterForm").Value;
                    m_MeterInfo.Add(AttributeValue);
                }
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Public accessor to meter info list
        /// </summary>
        public List<string> MeterInfo { get { return m_MeterInfo; } }

        #endregion

        #region Members

        private List<string> m_MeterInfo;

        #endregion
    }

    /// <summary>
    /// Class for Channels element
    /// </summary>
    public class ChannelsElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Channels";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ChannelsElement()
        {
            ListOfChannels = new List<ChannelElement>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Channels</param>
        public ChannelsElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ChannelElement.ELEMENT_NAME));

                foreach (XElement Descendant in Descendants)
                {
                    ChannelElement Channel = new ChannelElement(Descendant);
                    ListOfChannels.Add(Channel);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Channels element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// List of Channels
        /// </summary>
        public List<ChannelElement> ListOfChannels { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Channel element
    /// </summary>
    public class ChannelElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_READINGS_IN_PULSE = "ReadingsInPulse";
        private const string ATTRIBUTE_IS_REGISTER = "IsRegister";
        private const string ATTRIBUTE_INTERVAL_LENGTH = "IntervalLength";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Channel";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ChannelElement()
        {
            ReadingsInPulse = null;
            IsRegister = null;
            IntervalLength = null;
            Readings = null;
            ChannelID = null;
            ContiguousIntervalSets = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Channel</param>
        public ChannelElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIBUTE_READINGS_IN_PULSE) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_READINGS_IN_PULSE).Value;
                    ReadingsInPulse = bool.Parse(Value);
                }

                if (element.Attribute(ATTRIBUTE_IS_REGISTER) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_IS_REGISTER).Value;
                    IsRegister = bool.Parse(Value);
                }

                if (element.Attribute(ATTRIBUTE_INTERVAL_LENGTH) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_INTERVAL_LENGTH).Value;
                    IntervalLength = int.Parse(Value, CultureInfo.InvariantCulture);
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ChannelIDElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ChannelID = new ChannelIDElement(Descendants.First());
                }

                // Descendants
                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ContiguousIntervalSetsElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ContiguousIntervalSets = new ContiguousIntervalSetsElement(Descendants.First());
                }

                // Descendants
                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ReadingsElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    Readings = new ReadingsElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Channel element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// ReadingsInPulse attribute value
        /// </summary>
        public bool? ReadingsInPulse { get; private set; }

        /// <summary>
        /// IsRegister attribute value
        /// </summary>
        public bool? IsRegister { get; private set; }

        /// <summary>
        /// IntervalLength attribute value
        /// </summary>
        public int? IntervalLength { get; private set; }

        /// <summary>
        /// Gets the ChannelID element
        /// </summary>
        public ChannelIDElement ChannelID { get; private set; }

        /// <summary>
        /// Gets the ContiguousIntervalSets element
        /// </summary>
        public ContiguousIntervalSetsElement ContiguousIntervalSets { get; private set; }

        /// <summary>
        /// Gets the Readings element
        /// </summary>
        public ReadingsElement Readings { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for ChannelID element
    /// </summary>
    public class ChannelIDElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_ENDPOINT_UOM_ID = "EndPointUOMID";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ChannelID";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ChannelIDElement()
        {
            EndPointUOMID = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ChannelID</param>
        public ChannelIDElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIBUTE_ENDPOINT_UOM_ID) != null)
                {
                    EndPointUOMID = element.Attribute(ATTRIBUTE_ENDPOINT_UOM_ID).Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid ChannelID element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// End Point Unit of Measure ID
        /// </summary>
        public string EndPointUOMID { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for ContiguousIntervalSets element
    /// </summary>
    public class ContiguousIntervalSetsElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ContiguousIntervalSets";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ContiguousIntervalSetsElement()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ContiguousIntervalSets</param>
        public ContiguousIntervalSetsElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ContiguousIntervalSetElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                     ContiguousIntervalSet = new ContiguousIntervalSetElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid ContiguousIntervalSets element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ContiguousIntervalSet element
        /// </summary>
        public ContiguousIntervalSetElement ContiguousIntervalSet { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for ContiguousIntervalSet element
    /// </summary>
    public class ContiguousIntervalSetElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_NUMBER_OF_READINGS = "NumberOfReadings";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ContiguousIntervalSet";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ContiguousIntervalSetElement()
        {
            NumberOfReadings = null;
            TimePeriod = null;
            Readings = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ContiguousIntervalSet</param>
        public ContiguousIntervalSetElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.Attribute(ATTRIBUTE_NUMBER_OF_READINGS) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_NUMBER_OF_READINGS).Value;
                    NumberOfReadings = int.Parse(Value, CultureInfo.InvariantCulture);
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(TimePeriodElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    TimePeriod = new TimePeriodElement(Descendants.First());
                }

                // Descendants
                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ReadingsElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    Readings = new ReadingsElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid ContiguousIntervalSet element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Number of readings attribute value
        /// </summary>
        public int? NumberOfReadings { get; private set; }

        /// <summary>
        /// Gets the TimePeriod element
        /// </summary>
        public TimePeriodElement TimePeriod { get; private set; }

        /// <summary>
        /// Gets the Readings element
        /// </summary>
        public ReadingsElement Readings { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for TimePeriod element
    /// </summary>
    public class TimePeriodElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_END_TIME = "EndTime";
        private const string ATTRIBUTE_START_TIME = "StartTime";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "TimePeriod";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public TimePeriodElement()
        {
            EndTime = null;
            StartTime = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the TimePeriod</param>
        public TimePeriodElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                if (element.Attribute(ATTRIBUTE_END_TIME) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_END_TIME).Value;
                    EndTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                }

                if (element.Attribute(ATTRIBUTE_START_TIME) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_START_TIME).Value;
                    StartTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid TimePeriod element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// End time attribute value
        /// </summary>
        public DateTime? EndTime { get; private set; }

        /// <summary>
        /// Start time attribute value
        /// </summary>
        public DateTime? StartTime { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Readings element
    /// </summary>
    public class ReadingsElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Readings";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ReadingsElement()
        {
            ListOfReadings = new List<ReadingElement>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Readings</param>
        public ReadingsElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ReadingElement.ELEMENT_NAME));

                foreach (XElement Descendant in Descendants)
                {
                    ReadingElement Reading = new ReadingElement(Descendant);
                    ListOfReadings.Add(Reading);
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Readings element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the list of Reading elements
        /// </summary>
        public List<ReadingElement> ListOfReadings { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Reading element
    /// </summary>
    public class ReadingElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_VALUE = "Value";
        private const string ATTRIBUTE_READING_TIME = "ReadingTime";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Reading";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ReadingElement()
        {
            ReadingTime = null;
            Value = null;
            ReadingStatus = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Reading</param>
        public ReadingElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                if (element.Attribute(ATTRIBUTE_VALUE) != null)
                {
                    string TheValue = element.Attribute(ATTRIBUTE_VALUE).Value;
                    Value = double.Parse(TheValue, CultureInfo.InvariantCulture);
                }

                if (element.Attribute(ATTRIBUTE_READING_TIME) != null)
                {
                    string Value = element.Attribute(ATTRIBUTE_READING_TIME).Value;
                    ReadingTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ReadingStatusElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ReadingStatus = new ReadingStatusElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Reading Status element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Value attribute value
        /// </summary>
        public double? Value { get; private set; }

        /// <summary>
        /// ReadingTime attribute value
        /// </summary>
        public DateTime? ReadingTime { get; private set; }

        /// <summary>
        /// Gets the ReadingStatus element
        /// </summary>
        public ReadingStatusElement ReadingStatus { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Reading Status element
    /// </summary>
    public class ReadingStatusElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ReadingStatus";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ReadingStatusElement()
        {
            UnencodedStatus = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Reading Status</param>
        public ReadingStatusElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(UnencodedStatusElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    UnencodedStatus = new UnencodedStatusElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Reading Status element", "element");
            }
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets the UnencodedStatus element
        /// </summary>
        public UnencodedStatusElement UnencodedStatus { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Unencoded Status element
    /// </summary>
    public class UnencodedStatusElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_SOURCE_VALIDATION = "SourceValidation";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "UnencodedStatus";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public UnencodedStatusElement()
        {
            SourceValidaion = null;
            StatusCodes = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Unencoded Status</param>
        public UnencodedStatusElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                if (element.Attribute(ATTRIBUTE_SOURCE_VALIDATION) != null)
                {
                    SourceValidaion = element.Attribute(ATTRIBUTE_SOURCE_VALIDATION).Value;
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(StatusCodesElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    StatusCodes = new StatusCodesElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Unencoded Status element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Source Validation attribute value
        /// </summary>
        public string SourceValidaion { get; private set; }

        /// <summary>
        /// Gets the Status Codes element
        /// </summary>
        public StatusCodesElement StatusCodes { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for Status Codes element
    /// </summary>
    public class StatusCodesElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "StatusCodes";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public StatusCodesElement()
        {
            ListOfCodes = new List<CodeElement>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Status Codes</param>
        public StatusCodesElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(CodeElement.ELEMENT_NAME));

                foreach (XElement Descendant in Descendants)
                {
                    CodeElement Code = new CodeElement(Descendant);
                    ListOfCodes.Add(Code);
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Status Codes element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the List of Codes
        /// </summary>
        public List<CodeElement> ListOfCodes { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for the Code element
    /// </summary>
    public class CodeElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Code";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public CodeElement()
        {
            Code = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Code</param>
        public CodeElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    Code = element.Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid Code element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Code
        /// </summary>
        public string Code { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for DataFormat element
    /// </summary>
    public class DataFormatElement
    {

        #region Constants

        // Attribute Names
        private const string ATTRIBUTE_DST_TRANSITION_TYPE = "DSTTransitionType";
        private const string ATTRIBUTE_READING_TIMESTAMP_TYPE = "ReadingTimestampType";

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "DataFormat";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public DataFormatElement()
        {
            Attributes = new List<CRFFileAttribute>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the DataFormat</param>
        public DataFormatElement(XElement element)
            : this()
        {

            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {


                if (element.Attribute(ATTRIBUTE_DST_TRANSITION_TYPE) != null)
                {

                    CRFFileAttribute Attribute = new CRFFileAttribute();
                    Attribute.Name = ATTRIBUTE_DST_TRANSITION_TYPE;
                    Attribute.Value = element.Attribute(ATTRIBUTE_DST_TRANSITION_TYPE).Value;
                    Attributes.Add(Attribute);

                }

                if (element.Attribute(ATTRIBUTE_READING_TIMESTAMP_TYPE) != null)
                {

                    CRFFileAttribute Attribute = new CRFFileAttribute();
                    Attribute.Name = ATTRIBUTE_READING_TIMESTAMP_TYPE;
                    Attribute.Value = element.Attribute(ATTRIBUTE_READING_TIMESTAMP_TYPE).Value;
                    Attributes.Add(Attribute);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid DataFormat element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the XML attributes for the MeterReadingDocument element
        /// </summary>
        public List<CRFFileAttribute> Attributes { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    #endregion

    #region EventDocument

    /// <summary>
    /// Class for storing a CRF EventDocument XML file
    /// </summary>
    //  Revision History
    //  MM/DD/YY Who Version  Issue#    Description
    //  -------- --- -------  -------   ---------------------------------------------
    //  05/12/15 PGH 4.50.124 RTT556298 Created
    public class EventDocument
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "EventDocument";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <summary>
        /// Constructor
        /// </summary>
        public EventDocument()
        {
            Attributes = new List<CRFFileAttribute>();
            Events = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Event Document</param>
        public EventDocument(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (element.HasAttributes)
                {
                    IEnumerable<XAttribute> ElementAttributes = element.Attributes().Where(e => e.Name.LocalName.Equals(ELEMENT_NAME));
                    foreach (XAttribute Attribute in ElementAttributes)
                    {
                        CRFFileAttribute CRFAttribute = new CRFFileAttribute();
                        CRFAttribute.Name = Attribute.Name.LocalName;
                        CRFAttribute.Value = Attribute.Value;
                        Attributes.Add(CRFAttribute);
                    }
                }

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(MeterInfoElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    m_MeterInfo = new MeterInfoElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(EventsElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    Events = new EventsElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid EventDocument element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the XML attributes for the MeterReadingDocument element
        /// </summary>
        public List<CRFFileAttribute> Attributes { get; private set; }

        /// <summary>
        /// Gets the Events element
        /// </summary>
        public EventsElement Events { get; private set; }

        /// <summary>
        /// Publicly accessible Meter info element
        /// </summary>
        public MeterInfoElement MeterInformation { get { return m_MeterInfo; } }

        #endregion

        #region Members
        private MeterInfoElement m_MeterInfo;
        #endregion
    }

    /// <summary>
    /// Class for an Events element
    /// </summary>
    public class EventsElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Events";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <summary>
        /// Constructor
        /// </summary>
        public EventsElement()
        {
            ListOfEvents = new List<EventElement>();
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Events</param>
        public EventsElement(XElement element)
            : this()
        {

            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(EventElement.ELEMENT_NAME));

                foreach (XElement Descendent in Descendants)
                {
                    EventElement Event = new EventElement(Descendent);
                    ListOfEvents.Add(Event);
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Events element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the list of events
        /// </summary>
        public List<EventElement> ListOfEvents { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an Event element
    /// </summary>
    public class EventElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "Event";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <summary>
        /// Constructor
        /// </summary>
        public EventElement()
        {
            CollectionSystemIDElement = null;
            ObjectIDElement = null;
            ObjectTypeElement = null;
            EventTypeElement = null;
            IsHistoricalElement = null;
            EventDateTimeElement = null;
            CaptureDateTimeElement = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the Event</param>
        public EventElement(XElement element)
            : this()
        {

            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {

                // Descendants
                IEnumerable<XElement> Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(CollectionSystemIDElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    CollectionSystemIDElement = new CollectionSystemIDElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ObjectIDElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ObjectIDElement = new ObjectIDElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(ObjectTypeElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    ObjectTypeElement = new ObjectTypeElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(EventTypeElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    EventTypeElement = new EventTypeElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(IsHistoricalElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    IsHistoricalElement = new IsHistoricalElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(EventDateTimeElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    EventDateTimeElement = new EventDateTimeElement(Descendants.First());
                }

                Descendants = element.Elements().Where(e => e.Name.LocalName.Equals(CaptureDateTimeElement.ELEMENT_NAME));

                if (Descendants.Count() > 0)
                {
                    CaptureDateTimeElement = new CaptureDateTimeElement(Descendants.First());
                }

            }
            else
            {
                throw new ArgumentException("Not a valid Event element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the CollectionSystemID element
        /// </summary>
        public CollectionSystemIDElement CollectionSystemIDElement { get; private set; }

        /// <summary>
        /// Gets the ObjectID element
        /// </summary>
        public ObjectIDElement ObjectIDElement { get; private set; }

        /// <summary>
        /// Gets the ObjectType element
        /// </summary>
        public ObjectTypeElement ObjectTypeElement { get; private set; }

        /// <summary>
        /// Gets the EventType element
        /// </summary>
        public EventTypeElement EventTypeElement { get; private set; }

        /// <summary>
        /// Gets the IsHistorical element
        /// </summary>
        public IsHistoricalElement IsHistoricalElement { get; private set; }

        /// <summary>
        /// Gets the EventDateTime element
        /// </summary>
        public EventDateTimeElement EventDateTimeElement { get; private set; }

        /// <summary>
        /// Gets the CaptureDateTime element
        /// </summary>
        public CaptureDateTimeElement CaptureDateTimeElement { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for a CollectionSystemID element
    /// </summary>
    public class CollectionSystemIDElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "CollectionSystemID";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public CollectionSystemIDElement()
        {
            CollectionSystemID = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the CollectionSystemID</param>
        public CollectionSystemIDElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    CollectionSystemID = element.Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid CollectionSystemID element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the CollectionSystemID
        /// </summary>
        public string CollectionSystemID { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an ObjectID element
    /// </summary>
    public class ObjectIDElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ObjectID";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectIDElement()
        {
            ObjectID = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ObjectID</param>
        public ObjectIDElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    ObjectID = element.Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid ObjectID element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ObjectID
        /// </summary>
        public string ObjectID { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an ObjectType element
    /// </summary>
    public class ObjectTypeElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "ObjectType";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public ObjectTypeElement()
        {
            ObjectType = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the ObjectType</param>
        public ObjectTypeElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    ObjectType = element.Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid ObjectType element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the ObjectType
        /// </summary>
        public string ObjectType { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an EventType element
    /// </summary>
    public class EventTypeElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "EventType";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public EventTypeElement()
        {
            EventType = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the EventType</param>
        public EventTypeElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    EventType = element.Value;
                }
            }
            else
            {
                throw new ArgumentException("Not a valid EventType element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the EventType
        /// </summary>
        public string EventType { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an IsHistorical element
    /// </summary>
    public class IsHistoricalElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "IsHistorical";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public IsHistoricalElement()
        {
            IsHistorical = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the IsHistorical value</param>
        public IsHistoricalElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    string Value = element.Value;
                    int iValue = int.Parse(Value, CultureInfo.InvariantCulture);
                    if (iValue == 1)
                    {
                        IsHistorical = true;
                    }
                    else if (iValue == 0)
                    {
                        IsHistorical = false;
                    }
                }
            }
            else
            {
                throw new ArgumentException("Not a valid IsHistorical element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the IsHistorical value
        /// </summary>
        public bool? IsHistorical { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an EventDateTime element
    /// </summary>
    public class EventDateTimeElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "EventDateTime";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public EventDateTimeElement()
        {
            EventDateTime = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the EventDateTime</param>
        public EventDateTimeElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    string Value = element.Value;
                    EventDateTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid EventDateTime element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the EventDateTime
        /// </summary>
        public DateTime? EventDateTime { get; private set; }

        #endregion

        #region Members

        #endregion
    }

    /// <summary>
    /// Class for an CapturetDateTime element
    /// </summary>
    public class CaptureDateTimeElement
    {

        #region Constants

        /// <summary>
        /// The element name 
        /// </summary>
        internal const string ELEMENT_NAME = "CaptureDateTime";

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        public CaptureDateTimeElement()
        {
            CaptureDateTime = null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="element">The element containing the CapturetDateTime</param>
        public CaptureDateTimeElement(XElement element)
            : this()
        {
            if (element != null && element.Name.LocalName == ELEMENT_NAME)
            {
                if (!string.IsNullOrEmpty(element.Value))
                {
                    string Value = element.Value;
                    CaptureDateTime = DateTime.Parse(Value, CultureInfo.InvariantCulture);
                }
            }
            else
            {
                throw new ArgumentException("Not a valid CaptureDateTime element", "element");
            }
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the CapturetDateTime
        /// </summary>
        public DateTime? CaptureDateTime { get; private set; }

        #endregion

        #region Members

        #endregion
    }
    
    #endregion

    /// <summary>
    /// Class for ContiguousMeterReading
    /// </summary>
    public class ContiguousMeterReading
    {

        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public ContiguousMeterReading()
        {
            UOMID = "";
            StartTime = DateTime.MinValue;
            EndTime = DateTime.MinValue;
            IntervalLength = 0;
            Readings = new List<MeterReading>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Unit of Measure ID
        /// </summary>
        public string UOMID
        {
            get
            {
                return m_UOMID;
            }
            set
            {
                m_UOMID = value;
            }
        }

        /// <summary>
        /// Gets the Start time
        /// </summary>
        public DateTime StartTime
        {
            get
            {
                return m_StartTime;
            }
            set
            {
                m_StartTime = value;
            }
        }

        /// <summary>
        /// Gets the End time
        /// </summary>
        public DateTime EndTime
        {
            get
            {
                return m_EndTime;
            }
            set
            {
                m_EndTime = value;
            }
        }

        /// <summary>
        /// Gets the IntervalLength
        /// </summary>
        public int IntervalLength
        {
            get
            {
                return m_IntervalLength;
            }
            set
            {
                m_IntervalLength = value;
            }
        }

        /// <summary>
        /// Gets the Readings
        /// </summary>
        public List<MeterReading> Readings
        {
            get
            {
                return m_Readings;
            }
            set
            {
                m_Readings = value;
            }
        }

        #endregion

        #region Members

        private string m_UOMID;
        private DateTime m_StartTime;
        private DateTime m_EndTime;
        private int m_IntervalLength;
        private List<MeterReading> m_Readings;

        #endregion
    }

    /// <summary>
    /// Class for MeterReading
    /// </summary>
    public class MeterReading
    {

        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public MeterReading()
        {
            ReadingTime = DateTime.MinValue;
            Value = 0.0;
            SourceValidation = "";
            StatusCodes = new List<string>();
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Reading time
        /// </summary>
        public DateTime ReadingTime
        {
            get
            {
                return m_ReadingTime;
            }
            set
            {
                m_ReadingTime = value;
            }
        }

        /// <summary>
        /// Gets the Value
        /// </summary>
        public double Value
        {
            get
            {
                return m_Value;
            }
            set
            {
                m_Value = value;
            }
        }

        /// <summary>
        /// Gets the Source Validation value
        /// </summary>
        public string SourceValidation
        {
            get
            {
                return m_SourceValidation;
            }
            set
            {
                m_SourceValidation = value;
            }
        }

        /// <summary>
        /// Gets the List of Status Codes
        /// </summary>
        public List<string> StatusCodes
        {
            get
            {
                return m_StatusCodes;
            }
            set
            {
                m_StatusCodes = value;
            }
        }

        #endregion

        #region Members

        private DateTime m_ReadingTime;
        private double m_Value;
        private string m_SourceValidation;
        private List<string> m_StatusCodes;

        #endregion
    }

    /// <summary>
    /// Class for NoncontiguousMeterReading
    /// </summary>
    public class NoncontiguousMeterReading
    {

        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public NoncontiguousMeterReading()
        {
            UOMID = "";
            Reading = null;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Unit of Measure ID
        /// </summary>
        public string UOMID
        {
            get
            {
                return m_UOMID;
            }
            set
            {
                m_UOMID = value;
            }
        }

        /// <summary>
        /// Gets the Meter Reading
        /// </summary>
        public MeterReading Reading
        {
            get
            {
                return m_Reading;
            }
            set
            {
                m_Reading = value;
            }
        }

        #endregion

        #region Members

        private string m_UOMID;
        private MeterReading m_Reading;

        #endregion
    }

    /// <summary>
    /// Class for an EventDocument Event
    /// </summary>
    public class EventDocumentEvent
    {

        #region Constants
        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        //  Revision History
        //  MM/DD/YY Who Version  Issue#    Description
        //  -------- --- -------  -------   ---------------------------------------------
        //  05/12/15 PGH 4.50.124 RTT556298 Created
        public EventDocumentEvent()
        {
            CollectionSystemID = "";
            ObjectID = "";
            ObjectType = "";
            EventType = "";
            IsHistorical = false;
            EventDateTime = DateTime.MinValue;
            CaptureDateTime = DateTime.MinValue;
        }

        #endregion


        #region Public Properties

        /// <summary>
        /// Gets the Collection System ID
        /// </summary>
        public string CollectionSystemID
        {
            get
            {
                return m_CollectionSystemID;
            }
            set
            {
                m_CollectionSystemID = value;
            }
        }

        /// <summary>
        /// Gets the Object ID
        /// </summary>
        public string ObjectID
        {
            get
            {
                return m_ObjectID;
            }
            set
            {
                m_ObjectID = value;
            }
        }

        /// <summary>
        /// Gets the Object Type
        /// </summary>
        public string ObjectType
        {
            get
            {
                return m_ObjectType;
            }
            set
            {
                m_ObjectType = value;
            }
        }

        /// <summary>
        /// Gets the Event Type
        /// </summary>
        public string EventType
        {
            get
            {
                return m_EventType;
            }
            set
            {
                m_EventType = value;
            }
        }

        /// <summary>
        /// Gets the IsHistorical value
        /// </summary>
        public bool IsHistorical
        {
            get
            {
                return m_IsHistorical;
            }
            set
            {
                m_IsHistorical = value;
            }
        }

        /// <summary>
        /// Gets the Event DateTime
        /// </summary>
        public DateTime EventDateTime
        {
            get
            {
                return m_EventDateTime;
            }
            set
            {
                m_EventDateTime = value;
            }
        }

        /// <summary>
        /// Gets the Capture DateTime
        /// </summary>
        public DateTime CaptureDateTime
        {
            get
            {
                return m_CaptureDateTime;
            }
            set
            {
                m_CaptureDateTime = value;
            }
        }

        #endregion

        #region Members

        private string m_CollectionSystemID;
        private string m_ObjectID;
        private string m_ObjectType;
        private string m_EventType;
        private bool m_IsHistorical;
        private DateTime m_EventDateTime;
        private DateTime m_CaptureDateTime;

        #endregion
    }

    /// <summary>
    /// Contains information found in header
    /// </summary>
    public class MeterInfoHeader
    {
        #region Members

        private string m_FWversion;
        private string m_ESN;
        private string m_DeviceClass;
        private string m_Form;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="ESN"></param>
        /// <param name="DeviceClass"></param>
        /// <param name="FWVersion"></param>
        /// <param name="Form"></param>
        public MeterInfoHeader(string ESN, string DeviceClass, string FWVersion, string Form)
        {
            m_DeviceClass = DeviceClass;
            m_FWversion = FWVersion;
            m_ESN = ESN;
            m_Form = Form;  
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Publicly accesible ESN
        /// </summary>
        public string ESN { get { return m_ESN; } }
        
        /// <summary>
        /// Publicly accessible FWVersion
        /// </summary>
        public string FWVersion { get { return m_FWversion; } }
        
        /// <summary>
        /// Publicly accessible Device Class
        /// </summary>
        public string DeviceClass { get { return m_DeviceClass; } }

        /// <summary>
        /// Publicly accessible Form
        /// </summary>
        public string Form { get { return m_Form; } }

        #endregion
    }

}
