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
//                              Copyright © 2006 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Globalization;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.OleDb;
using System.Windows.Forms;

namespace Itron.Metering.Utilities
{
	/// <summary>
	/// This is a singleton class that represents a persistent log of meter related
	/// events.  The actual log file encapsilated by this class is an MS Access
	/// file internally accessed via OLE DB.
	/// </summary>
	public sealed class MeterActivityLog 
	{
		#region Public Methods

		/// <summary>
		/// The meter activity log class is a singleton class.  This means that only
		/// one instance of the class can be created and used per process.  Note 
		/// that this class is NOT threadsafe so extreme care must be taken if used
		/// in a multi-threaded application.
		/// 
		/// All access to the activity log class must be made through this property.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public static MeterActivityLog Instance
		{
			get
			{
				if (m_SoleInstance == null)
				{
					lock (syncRoot)
					{
						if (m_SoleInstance == null)
							m_SoleInstance = new MeterActivityLog();
					}
				}

				return m_SoleInstance;
			}
		}

		/// <summary>
		/// This method is responsible for opening a connection to the activity
		/// log database.  Note that this method is a fairly expensive operation in
		/// terms of time and, while DB connections are also rare commodities, applications
		/// must be concious of the trade off between performance and resource management
		/// when maintaining database connections.
		/// </summary>
		/// <exception >
		/// OLEDBExceptions and IO exceptions can be thrown by this method
		/// </exception>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public void Open()
		{
			if (m_OleDbConnection.State == ConnectionState.Closed)
			{
				m_OleDbConnection.Open();
			}
			else
			{
				m_OleDbConnection.ResetState();

				this.Close();

				m_OleDbConnection.Open();
			}
		}
		
		/// <summary>
		/// This method adds a single activity log entry to the database. Note that 
		/// this method will not throw exceptions but will simply return a value of
		/// 'false' if the given log entry cannot be added to the event log.  However,
		/// since the user will be notified via a Message Box, it is imperative that this
		/// method not be used in a background process.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1300:SpecifyMessageBoxOptions"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "System.Windows.Forms.MessageBox.Show(System.String)")]
        public Boolean Add(MeterActivityLogEntry newLogEntry)
		{
			Boolean boolSuccess = false;

			try
			{
				// Make sure that all of the environment and application related 
				// properties for this log entry have been set correctly.
				SetEnvironmentProperties(newLogEntry);

				// Before we try to add anything to the database, do a sanity check
				// first and make sure that the connection to the DB is open and 
				// usable 
				if (m_OleDbConnection.State == ConnectionState.Open)
				{
					OleDbCommand dbCommand = m_OleDbConnection.CreateCommand();

					dbCommand.CommandText = "INSERT INTO ACTIVITYLOG " +
						"(EVENTTIME,UNITID,SERIALNUMBER,DEVICETYPE,ACTIVITYID,ACTIVITYSTATUS,SOURCE,SOURCEVERSION,RESULTFILE,APPLICATION,APPLICATIONVERSION,MACHINENAME,USERID)" +
						"VALUES " +
						"(?,?,?,?,?,?,?,?,?,?,?,?,?)";

					dbCommand.Parameters.Add(new OleDbParameter("EVENTTIME", newLogEntry.EventTime.ToString( CultureInfo.InvariantCulture )));
					dbCommand.Parameters.Add(new OleDbParameter("UNITID", newLogEntry.UnitID));
					dbCommand.Parameters.Add(new OleDbParameter("SERIALNUMBER", newLogEntry.SerialNumber));
					dbCommand.Parameters.Add(new OleDbParameter("DEVICETYPE", newLogEntry.MeterType));
					dbCommand.Parameters.Add(new OleDbParameter("ACTIVITYID", (int)newLogEntry.ActivityType));
					dbCommand.Parameters.Add(new OleDbParameter("ACTIVITYSTATUS", (int)newLogEntry.ActivityStatus));
					dbCommand.Parameters.Add(new OleDbParameter("SOURCE", newLogEntry.Source));
					dbCommand.Parameters.Add(new OleDbParameter("SOURCEVERSION", newLogEntry.SourceVersion));
					dbCommand.Parameters.Add(new OleDbParameter("RESULTFILE", newLogEntry.ResultFile));
					dbCommand.Parameters.Add(new OleDbParameter("APPLICATION", newLogEntry.ApplicationName));
					dbCommand.Parameters.Add(new OleDbParameter("APPLICATIONVERSION", newLogEntry.ApplicationVersion));
					dbCommand.Parameters.Add(new OleDbParameter("MACHINENAME", newLogEntry.MachineName));
					dbCommand.Parameters.Add(new OleDbParameter("USERID", newLogEntry.UserName));

					int nRowsInserted = dbCommand.ExecuteNonQuery();

					boolSuccess = (nRowsInserted == 1);
				}
				else
				{
					boolSuccess = false;
				}
			}

			catch (Exception err )
			{
				MessageBox.Show("Unable to add activity log entry: " + err.Message); // TODO: handle this...
			}
			
			return boolSuccess;
		}

		/// <summary>
		/// This method is responsible for reading the entire activity log contents
		/// and returning them as a list of log entries
		/// </summary>
		/// <exception >
		/// OLEDBExceptions and IO exceptions can be thrown by this method
		/// </exception>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public List<MeterActivityLogEntry> Read()
		{
			List<MeterActivityLogEntry> lstEntries = Read( m_OleDbConnection );

			return lstEntries;
		}

		/// <summary>
		/// This method is responible for adding all of the event log records found
		/// in the given database file to the current meter activity log.  No changes 
		/// will be made to the incoming database file.
		/// </summary>
		/// <exception >
		/// OLEDBExceptions and IO exceptions can be thrown by this method
		/// </exception>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public void Merge(string strPathName)
		{
			// Before we try to merge any records, do a sanity check
			// first and make sure that the connection to the DB is open and 
			// usable 
			if (m_OleDbConnection.State == ConnectionState.Open)
			{
				OleDbConnection dbSource = new OleDbConnection();

				// Build the data source description for the file to be merged
				String strDataSource = "Data Source=" + strPathName + ";";

                dbSource.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; " +
					strDataSource;

				// Next open the source file
				dbSource.Open();

				List<MeterActivityLogEntry> lstEntries = ReadAll(dbSource);

				// Now put all the data records into the destination database
				foreach (MeterActivityLogEntry logEntry in lstEntries)
				{
					Add(logEntry);
				}

				// OK, we're done! Close the connection to the source file and 
				// clean up
				dbSource.Close();  
			}
		}

		/// <summary>
		/// This method permanently deletes any event records that are older than
		/// the given date/time
		/// </summary>
		/// <exception >
		/// OLEDBExceptions and IO exceptions can be thrown by this method
		/// </exception>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public int RollOffEvents(DateTime dtLogLimit)
		{
			int nRowsDeleted = 0;

			// Before we try to roll off records, do a sanity check
			// first and make sure that the connection to the DB is open and 
			// usable 
			if (m_OleDbConnection.State == ConnectionState.Open)
			{
				OleDbCommand dbCommand = m_OleDbConnection.CreateCommand();

				dbCommand.CommandText = "DELETE FROM ACTIVITYLOG WHERE ((EVENTTIME)<#" + dtLogLimit.ToShortDateString() + "#)";

				nRowsDeleted = dbCommand.ExecuteNonQuery();
			}

			return nRowsDeleted;
		}

		/// <summary>
		/// Closes the OLE DB connection to the activity log database    
		/// </summary>
		/// <exception >
		/// OLEDBExceptions and IO exceptions can be thrown by this method
		/// </exception>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public void Close()
		{
			if (m_OleDbConnection.State != ConnectionState.Closed)
			{
                try
                {
                    m_OleDbConnection.Close();
                }
                catch (Exception)
                {
                    // Don't worry about an exception when closing.
                }
			}
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// This read-only property returns the full path name of the 
		/// activity log file.  It is useful when client applications want
		/// to make copies of the log file for exports or back-ups.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public static String ActivityLogFile
		{
			get
			{
				return CRegistryHelper.GetFilePath("MeterActivity.mdb");
			}
		}

		/// <summary>
		/// This boolean property is used to enable/disable filtering of the event 
		/// log by start and stop dates.  If this property is set to true then the
		/// 'Read' method will only return event log entries that are between the 
		/// values of the start and stop date properties.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public Boolean FilterByDate
		{
			get
			{
				return m_boolFilterByDate;
			}
			set
			{
				m_boolFilterByDate = value;
			}
		}

		/// <summary>
		/// This property is used in conjunction with the FilterByDate property.  
		/// If the FilterByDate property is set to true, then this property establishes
		/// the date of the earliest event log record that can be returned by a call
		/// to the 'Read' method.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public DateTime FilterStartDate
		{
			get
			{
				return m_dtFilterStartDate;
			}
			set
			{
				m_dtFilterStartDate = value;
			}
		}

		/// <summary>
		/// This property is used in conjunction with the FilterByDate property.  
		/// If the FilterByDate property is set to true, then this property establishes
		/// the date of the latest event log record that can be returned by a call
		/// to the 'Read' method.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public DateTime FilterEndDate
		{
			get
			{
				return m_dtFilterEndDate;
			}
			set
			{
				m_dtFilterEndDate = value;
			}
		}

		/// <summary>
		/// This boolean property is used to enable/disable filtering of the event 
		/// log by the name of the calling application.  If this property is set 
		/// to true then the 'Read' method will only return event log entries that 
		/// were created by the calling application.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public String ApplicationFilter
		{
			get
			{
				return m_strApplicationFilter;
			}
			set
			{
				m_strApplicationFilter = value;
			}
		}

		/// <summary>
		/// This boolean property is used to enable/disable filtering of the event 
		/// log by the result of the event  If this property is set to true, then the
		/// 'Read' method will only return event log entries that were unsuccessful.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		public Boolean FilterByResult
		{
			get
			{
				return m_boolFilterByResult;
			}
			set
			{
				m_boolFilterByResult = value;
			}
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// This method reads and retrieves activity log records from the persistent
		/// database by building a query based on the classes public properties.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private List<MeterActivityLogEntry> Read(OleDbConnection dbActivityLog)
		{
			List<MeterActivityLogEntry> lstEntries = new List<MeterActivityLogEntry>();

			if (dbActivityLog.State == ConnectionState.Open)
			{
				OleDbCommand dbCommand = dbActivityLog.CreateCommand();

				String strCommandText = BuildQueryCommand();

				dbCommand.CommandText = strCommandText;

				OleDbDataReader reader = dbCommand.ExecuteReader();

				while (reader.Read())
				{
					MeterActivityLogEntry logEntry = new MeterActivityLogEntry();

					ReadActivityRecord(reader, logEntry);

					lstEntries.Add(logEntry);
				}
			}

			return lstEntries;
		}

		/// <summary>
		/// This method returns the entire list of activity log entries from the 
		/// persistent database.  Note that no filters are applied and every 
		/// entry is returned.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private List<MeterActivityLogEntry> ReadAll(OleDbConnection dbActivityLog)
		{
			List<MeterActivityLogEntry> lstEntries = new List<MeterActivityLogEntry>();

			if (dbActivityLog.State == ConnectionState.Open)
			{
				OleDbCommand dbCommand = dbActivityLog.CreateCommand();

				String strCommandText = "SELECT * FROM ACTIVITYLOG"; 

				dbCommand.CommandText = strCommandText;

				OleDbDataReader reader = dbCommand.ExecuteReader();

				while (reader.Read())
				{
					MeterActivityLogEntry logEntry = new MeterActivityLogEntry();

					ReadActivityRecord(reader, logEntry);

					lstEntries.Add(logEntry);
				}
			}

			return lstEntries;
		}


		/// <summary>
		/// This private method is used to build the appropriate query to retrieve
		/// event log records according to the values of the class's filtering 
		/// properties
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private String BuildQueryCommand()
		{
			String strConditional = "";
			
			String strCommandText = "SELECT * FROM ACTIVITYLOG";

			if (FilterByResult)
			{
				strConditional = BuildResultQuery();
			}

			if (FilterByDate)
			{
				if (strConditional.Length > 0)
				{
					strConditional += " AND";
				}

				strConditional += BuildDateQuery();
			}

			if (ApplicationFilter.Length > 0)
			{
				if (strConditional.Length > 0)
				{
					strConditional += " AND";
				}

				strConditional += BuildApplicationQuery();
			}

			if (strConditional.Length > 0)
			{
				strCommandText += " WHERE (" + strConditional + ")";
			}

			return strCommandText;
		}

		/// <summary>
		/// This method builds a SQL condition statement based on the calling
		/// application name
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private String BuildApplicationQuery()
		{
			String strApplicationQuery;

			strApplicationQuery = " ((APPLICATION)=" + ApplicationFilter + ")";

			return strApplicationQuery;
		}

		/// <summary>
		/// This method builds a SQL conditional for filtering event log entries 
		/// based on the start and end filter properties
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private String BuildDateQuery()
		{
			String strDateQuery;

			// We want the query to be effective from midnight of the start date to 23:29 of the 
			// end date.  Make sure the starting time stamp is at midnight of the selected
			// day and that the end time is at midnight following the selected end date
			DateTime dtStart = new DateTime(FilterStartDate.Year, FilterStartDate.Month, FilterStartDate.Day);
			DateTime dtEnd = new DateTime(FilterEndDate.Year, FilterEndDate.Month, FilterEndDate.Day);
			dtEnd = dtEnd.AddDays(1);

			// Now that we know the correct start and stop times, build the query
			// but becare not to incude any events that occur exactly on the given
			// stop date
			strDateQuery = " ((EVENTTIME)>=#" + dtStart.ToString("d", CultureInfo.InvariantCulture) + "#)";
			strDateQuery += " AND";
			strDateQuery += " ((EVENTTIME)<#" + dtEnd.ToString("d", CultureInfo.InvariantCulture) + "#)";
			return strDateQuery;
		}

		/// <summary>
		/// This method builds a SQL conditional for returning only unsuccessful
		/// event log entries 
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private static String BuildResultQuery()
		{
			String strResultQuery;
			
			int nSuccessCode = (int)MeterActivityLogEntry.ActivityStatusEnum.Success;

			strResultQuery = " ((ACTIVITYSTATUS)<>" + nSuccessCode.ToString(CultureInfo.InvariantCulture) + ")";

			return strResultQuery;
		}

		/// <summary>
		/// The primary constructor for the activity log is a private member.
		/// This design prevents client applications from creating copies of 
		/// the activity log and allows for a single instance of this object
		/// to be used throughout an application.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private MeterActivityLog()
		{
			m_OleDbConnection = new OleDbConnection();

			String strDataSource = "Data Source=" + ActivityLogFile + ";";

            m_OleDbConnection.ConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0; " +
				strDataSource;

			// initialize the filters
			FilterByDate = false;
			FilterStartDate = DateTime.Now;
			FilterEndDate = DateTime.Now;
			ApplicationFilter = ""; // an empty string indicates no app filters
			FilterByResult = false;
		}

		/// <summary>
		/// This method is responsible for extracting a meter activity log entry
		/// object from an OLE DB query result set
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private static void ReadActivityRecord(OleDbDataReader reader, MeterActivityLogEntry logEntry)
		{
			for (int nColumn = 0; nColumn < reader.FieldCount; nColumn++)
			{
				String strColumnName = reader.GetName(nColumn).ToUpper(CultureInfo.InvariantCulture);

				switch (strColumnName)
				{
					case "EVENTTIME":
						logEntry.EventTime = reader.GetDateTime(nColumn);
						break;
					case "UNITID":
						logEntry.UnitID = reader[nColumn].ToString();
						break;
					case "SERIALNUMBER":
						logEntry.SerialNumber = reader[nColumn].ToString();
						break;
					case "DEVICETYPE":
						logEntry.MeterType = reader[nColumn].ToString();
						break;
					case "ACTIVITYID":
						logEntry.ActivityType = (MeterActivityLogEntry.ActivityTypeEnum)reader[nColumn];
						break;
					case "ACTIVITYSTATUS":
						logEntry.ActivityStatus = (MeterActivityLogEntry.ActivityStatusEnum)reader[nColumn];
						break;
					case "SOURCE":
						logEntry.Source = reader[nColumn].ToString();
						break;
					case "SOURCEVERSION":
						logEntry.SourceVersion = reader[nColumn].ToString();
						break;
					case "RESULTFILE":
						logEntry.ResultFile = reader[nColumn].ToString();
						break;
					case "APPLICATION":
						logEntry.ApplicationName = reader[nColumn].ToString();
						break;
					case "APPLICATIONVERSION":
						logEntry.ApplicationVersion = reader[nColumn].ToString();
						break;
					case "MACHINENAME":
						logEntry.MachineName = reader[nColumn].ToString();
						break;
					case "USERID":
						logEntry.UserName = reader[nColumn].ToString();
						break;
					default:
						break;
				}
			}
		}

		/// <summary>
		/// This method is responsible for insuring that the default values of a meter
		/// activity log record are set appropriately prior to posting the record to 
		/// the persistent database
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  04/04/08 MAH		Created
		///
		/// </remarks>
		private static void SetEnvironmentProperties(MeterActivityLogEntry newLogEntry)
		{
			if (String.IsNullOrEmpty(newLogEntry.ApplicationName))
			{
				newLogEntry.ApplicationName = Application.ProductName;
			}

			if (String.IsNullOrEmpty(newLogEntry.ApplicationVersion))
			{
				newLogEntry.ApplicationVersion = Application.ProductVersion;
			}

			if (String.IsNullOrEmpty(newLogEntry.MachineName))
			{
				newLogEntry.MachineName = Environment.MachineName;
			}

			if (String.IsNullOrEmpty(newLogEntry.UserName))
			{
				newLogEntry.UserName = Environment.UserName;
			}
		}

		#endregion

		#region Members

		// This is a singleton class and therefore the following member item is declared static.  
		// This is by design even though it violates a naming standard.
		[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1504:ReviewMisleadingFieldNames")]
		private static volatile MeterActivityLog m_SoleInstance;
		private static object syncRoot = new Object();
		private OleDbConnection m_OleDbConnection;
		private Boolean m_boolFilterByDate;
		private String m_strApplicationFilter;
		private DateTime m_dtFilterStartDate;
		private DateTime m_dtFilterEndDate;
		private Boolean m_boolFilterByResult;

		#endregion //Members
	}

}
