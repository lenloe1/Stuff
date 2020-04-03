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
//                              Copyright © 2008 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Itron.Metering.CustomSchedule;
using Itron.Metering.Utilities;

namespace Itron.Metering.Datafiles
{
    /// <summary>
    /// 
    /// </summary>
    public class CENTRON_IMAGEMIF: ANSIMIF
    {
        #region Constants

        private const string MCS_PEDATA
            = "<?xml version=\"1.0\"?>\n<?xml-stylesheet type='text/xsl' href='MCS.xsl'?>\n<PC-PROP98><MCS/></PC-PROP98>";
        private const string PEDATA = "PEData";
        private const string YEAR = "Year";
        private const string START_TIME = "StartTime";
        private const int MFG_MULT_CUSTOM_SCHED_ITEMS = 27399;
        private const int MULT_CUSTOM_SCHED_DATA = 10;
        private const int MAX_MULTIPLE_CUSTOM_SCHEDULES = 25;

        #endregion

        #region Public Methods

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="strFileName">The full path for the MIF.</param>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  10/30/08 jrf 9.50           Created.
        //
        public CENTRON_IMAGEMIF(string strFileName)
            : base(strFileName)
        {
            m_lstMultipleCustomSchedules = new List<CCustomSchedule>();
            m_blnMultipleCustomSchedulesUsed = new CachedBool();
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Returns an bool that indicates if multiple custom schedule are used.
        /// </summary>
        // MM/DD/YY who Version Issue# Description
        // -------- --- ------- ------ ---------------------------------------
        // 08/13/08	jrf	9.50.05        Created
        // 01/07/09 jrf 9.50.29 125402 The MCS_STATUS server item was returning an
        //                             indication of whether multiple custom schedules 
        //                             were configurable and not whether they were 
        //                             actually configured.  Reworked method to check
        //                             if any multiple custom schedules were configured
        //                             by checking if a current multiple custom schedule 
        //                             was setup.
        //
        public bool MultipleCustomSchedulesUsed
        {
            get
            {
                object objValue = null;
                DateTime[] adtMCSDates = null;

                // Check to see if we have already retrieved the value.
                if (m_blnMultipleCustomSchedulesUsed.Cached == false)
                {
                    if (RetrieveItem(MFG_MULT_CUSTOM_SCHED_ITEMS, ref objValue))
                    {
                        try
                        {
                            adtMCSDates = (DateTime[])objValue;
                        }
                        catch
                        {
                            m_blnMultipleCustomSchedulesUsed.Value = false;
                        }

                        if (null != adtMCSDates && 0 != adtMCSDates.Length)
                        {
                            m_blnMultipleCustomSchedulesUsed.Value = true;
                        }
                        else
                        {
                            m_blnMultipleCustomSchedulesUsed.Value = false;
                        }
                    }
                    else
                    {
                        // An error occurred
                        m_blnMultipleCustomSchedulesUsed.Value = false;
                    }
                }

                return m_blnMultipleCustomSchedulesUsed.Value;
            }
        }

        /// <summary>
        /// This property gets the multiple custom schedules from the MIF.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/30/08 jrf 9.50           Created
        //
        public List<CCustomSchedule> MultipleCustomSchedules
        {
            get
            {
                ReverseEngineerMultipleCustomSchedules();

                return m_lstMultipleCustomSchedules;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// This method reverse engineers multiple custom schedules from a given device server.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  10/06/08 jrf 9.50           Created
        //  12/12/08 jrf 9.50.26 124330 Set a max number of multiple custom schedules 
        //                              to look for to avoid entering an infinite loop.
        //                       124361 Added code to handle slots with no schedules configured.
        //  01/07/09 jrf 9.50.29 125402 Switched comparison statements in the last while loop.
        //                              The comparison checking if the index is valid needed to 
        //                              come before the comparison the comparison accessing the 
        //                              list.
        //
        private void ReverseEngineerMultipleCustomSchedules()
        {
            string strFilePath = CRegistryHelper.GetFilePath(PEDATA) + PEDATA + XML_EXT;
            int iMCSIndex = 0;
            int iMCSNumber = 0;
            object objType = MULT_CUSTOM_SCHED_DATA;
            object objSubType = iMCSIndex;
            object objFileName = strFilePath;
            object objStartDate = null;
            object objStopDate = null;
            CCustomSchedule BillingSchedule;
            DateCollection BillingDates = new DateCollection();
            DateTime dtStartDate;
            string strMIFName = Path.GetFileNameWithoutExtension(FileName);
            bool blnCustomScheduleFound = true;
            FileStream Stream;
            StreamWriter Writer;
            XmlReader Reader;
            short sDisplayProgress = 0;

            while (iMCSIndex < MAX_MULTIPLE_CUSTOM_SCHEDULES)
            {
                //Set the PEData file to a default empty format
                Stream = new FileStream(strFilePath, FileMode.Create);
                Writer = new StreamWriter(Stream);

                Writer.Write(MCS_PEDATA);

                Writer.Close();
                Stream.Close();

                //Get the multiple custom schedule data out of the device server
                VirtualDevice.GetXMLData(objType, objSubType, objFileName, sDisplayProgress, ref objStartDate, ref objStopDate);

                Reader = XmlReader.Create(strFilePath);

                //if we don't have a year node then no custom schedule
                //was retreived
                blnCustomScheduleFound = Reader.ReadToFollowing(YEAR);

                if (true == blnCustomScheduleFound)
                {
                    //Clear out dates for the previous schedule
                    BillingDates.Clear();

                    //Read each starting time and add them to the billing schedule
                    while (true == Reader.ReadToFollowing(START_TIME))
                    {
                        string strDate = Reader.ReadElementContentAsString();
                        dtStartDate = (DateTime)Convert.ChangeType(strDate, typeof(DateTime), CultureInfo.InvariantCulture);
                        BillingDates.Add(dtStartDate);
                    }

                    //Convert to a 1 based index for the name
                    iMCSNumber = iMCSIndex + 1;

                    BillingSchedule = new CCustomSchedule();
                    BillingSchedule.AddDates(BillingDates);
                    BillingSchedule.Name = strMIFName + "_" + iMCSNumber.ToString(CultureInfo.InvariantCulture);
                    BillingSchedule.Description = m_rmStrings.GetString("REVERSE_ENGINEERED_FROM") + Path.GetFileName(FileName);

                    m_lstMultipleCustomSchedules.Add(BillingSchedule);
                }
                else
                {
                    //Keep the place for an empty slot.  We may have other schedules
                    //configured after this.
                    m_lstMultipleCustomSchedules.Add(null);
                }

                Reader.Close();

                iMCSIndex++;
                objSubType = iMCSIndex;
            }

            //Remove null schedules from the end
            iMCSIndex--;

            while (iMCSIndex >= 0 && null == m_lstMultipleCustomSchedules[iMCSIndex])
            {
                m_lstMultipleCustomSchedules.RemoveAt(iMCSIndex);
                iMCSIndex--;
            }
        }

        #endregion

        #region Members

        private List<CCustomSchedule> m_lstMultipleCustomSchedules;
        private CachedBool m_blnMultipleCustomSchedulesUsed;

        #endregion
    }
}
