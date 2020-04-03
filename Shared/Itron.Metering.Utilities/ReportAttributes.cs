using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Collections;
using System.Globalization;
using System.IO;
using Microsoft.Win32.SafeHandles;
using System.Xml;

namespace Itron.Metering.Utilities
{
	/// <summary>
	///     
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	public class ReportAttributes
	{
		#region Definitions 

		private const string ItronAttributes = "ItronMetering";

		private const string REPORT_ATTRIBUTES = "ReportAttributes";
		private const string ELEMENT_DEVICE_ID = "DeviceID";
		private const string ELEMENT_REPORT_SPEC = "ReportSpecification";
		private const string ELEMENT_SOURCE = "Source";
		private const string ELEMENT_TITLE = "Title";
		private const string ELEMENT_PROFILE_START = "ProfileStart";
		private const string ELEMENT_PROFILE_END = "ProfileEnd";

		#endregion

		#region Public Members


		/// <summary>
		///     
		/// </summary>
		public String DeviceID
		{
			get { return m_strDeviceID; }
			set { m_strDeviceID = value; }
		}

		/// <summary>
		///     
		/// </summary>
		public String ReportTitle
		{
			get { return m_strReportTitle; }
			set { m_strReportTitle = value; }
		}

		/// <summary>
		///     
		/// </summary>
		public String ReportSource
		{
			get { return m_strSource; }
			set { m_strSource = value; }
		}

		/// <summary>
		///     
		/// </summary>
		public DateTime ProfileStart
		{
			get { return m_dtProfileStart; }
			set { m_dtProfileStart = value; }
		}

		/// <summary>
		///     
		/// </summary>
		public DateTime ProfileEnd
		{
			get { return m_dtProfileEnd; }
			set { m_dtProfileEnd = value; }
		}
		
		/// <summary>
		///     
		/// </summary>
		public String ReportSpecification
		{
			get { return m_strReportSpecification; }
			set { m_strReportSpecification = value; }
		}

		#endregion

		#region Public Methods

		/// <summary>
		///     
		/// </summary>
		public ReportAttributes(String strPathName)
		{
			m_strPathName = strPathName;

			m_ADSFile = new ADSFile();
		}

		/// <summary>
		///     
		/// </summary>
		/// <returns>
		///     A bool value...
		/// </returns>
		public Boolean Read()
		{
			Boolean boolReadSuccess = false;

			try
			{
				if ( Exists(m_strPathName ))
				{
					StreamReader attributeReader = m_ADSFile.OpenText(m_strPathName, ItronAttributes);
					XmlTextReader xmlReader = new XmlTextReader(attributeReader);

					if (xmlReader != null)
					{
						xmlReader.MoveToContent();

						while (xmlReader.Read())
						{
							if (xmlReader.NodeType == XmlNodeType.Element)
							{
								String strElementName = xmlReader.Name;

								switch (strElementName)
								{
									case ELEMENT_DEVICE_ID:
										if (xmlReader.Read())
										{
											DeviceID = xmlReader.Value;
										}
										break;
									case ELEMENT_PROFILE_END:
										if (xmlReader.Read())
										{
											String strEndTime = xmlReader.Value;

											if (!String.IsNullOrEmpty(strEndTime))
											{
												ProfileEnd = DateTime.Parse(strEndTime, CultureInfo.InvariantCulture);
											}
										}
										break;
									case ELEMENT_PROFILE_START:
										if (xmlReader.Read())
										{
											String strTime = xmlReader.Value;

											if (!String.IsNullOrEmpty(strTime))
											{
												ProfileStart = DateTime.Parse(strTime, CultureInfo.InvariantCulture);
											}
										}
										break;
									case ELEMENT_REPORT_SPEC:
										if (xmlReader.Read())
										{
											ReportSpecification = xmlReader.Value;
										}
										break;
									case ELEMENT_SOURCE:
										if (xmlReader.Read())
										{
											ReportSource = xmlReader.Value;
										}
										break;
									case ELEMENT_TITLE:
										if (xmlReader.Read())
										{
											ReportTitle = xmlReader.Value;
										}
										break;
								}
							}
						}
					}
					boolReadSuccess = true;

					xmlReader.Close();
					attributeReader.Close();

					m_ADSFile.Close();
				}
			}

			catch
			{
				// We normally don't want to display an error here....
			}

			return boolReadSuccess;
		}

		/// <summary>
		///     
		/// </summary>
		public Boolean Write()
		{
			Boolean boolWriteSuccess = false;

			try
			{
				if ( !Exists(m_strPathName ))
				{
					FileStream attributeStream = m_ADSFile.Create(m_strPathName, ItronAttributes);
					XmlTextWriter xmlWriter = new XmlTextWriter(attributeStream, null);

					if (xmlWriter != null)
					{
						xmlWriter.Formatting = Formatting.Indented;

						xmlWriter.WriteStartElement(REPORT_ATTRIBUTES);

						xmlWriter.WriteElementString(ELEMENT_DEVICE_ID, DeviceID);
						xmlWriter.WriteElementString(ELEMENT_REPORT_SPEC, ReportSpecification);
						xmlWriter.WriteElementString(ELEMENT_TITLE, ReportTitle);
						xmlWriter.WriteElementString(ELEMENT_SOURCE, ReportSource);

						if (ProfileStart != null)
						{
							xmlWriter.WriteElementString(ELEMENT_PROFILE_START, ProfileStart.ToString(CultureInfo.CurrentCulture));
						}

						if (ProfileEnd != null)
						{
							xmlWriter.WriteElementString(ELEMENT_PROFILE_END, ProfileEnd.ToString(CultureInfo.CurrentCulture));
						}

						xmlWriter.WriteEndElement(); // End of root 
						xmlWriter.Flush();
						xmlWriter.Close();

						attributeStream.Flush();
						attributeStream.Close();

						boolWriteSuccess = true;

						m_ADSFile.Close();
					}
				}
			}

			catch
			{
				// We normally don't want to display an error here....
			}

			return boolWriteSuccess;
		}

		/// <summary>
		///     
		/// </summary>
		/// <param name="strPathName" type="string">
		/// </param>
		/// <returns>
		///     A bool value...
		/// </returns>
		static public Boolean Exists(String strPathName)
		{
			return ADSFile.Exists(strPathName, ItronAttributes);
		}

		#endregion

		#region Private Members

		private String m_strPathName;
		private String m_strDeviceID;
		private String m_strReportSpecification;
		private String m_strReportTitle;
		private String m_strSource;
		private DateTime m_dtProfileStart;
		private DateTime m_dtProfileEnd;

		private ADSFile m_ADSFile;
		
		#endregion

		/// <summary>
		/// ADS File mimmicks the <c>File</c> class providing support for writing NTFS Alternate Data streams
		/// <example>StreamWriter sw = ADSFile.AppendText([filename], [stream])</example>
		/// </summary>
		/// <seealso cref="File"/>
		protected class ADSFile
		{
			#region constants
			private const uint GENERIC_READ = 0x80000000;
			private const uint GENERIC_WRITE = 0x40000000;

			private const uint CREATE_NEW = 1;
			private const uint CREATE_ALWAYS = 2;
			private const uint OPEN_EXISTING = 3;
			private const uint OPEN_ALWAYS = 4;
			private const uint TRUNCATE_EXISTING = 5;

			private const uint FILE_BEGIN = 0;
			private const uint FILE_CURRENT = 1;
			private const uint FILE_END = 2;

			private const uint FILE_SHARE_NONE = 0;
			private const uint FILE_SHARE_READ = 1;

			#endregion

			/// <summary>
			/// Win32 API call to create or open a file
			/// </summary>
			/// <param name="filename">Name of file to create including stream</param>
			/// <param name="access">File access permissions</param>
			/// <param name="sharemode">Share mode</param>
			/// <param name="security_attributes">Additional file security attributes</param>
			/// <param name="creation">File creation options</param>
			/// <param name="flags">optional flags</param>
			/// <param name="template">Creation template</param>
			/// <returns><c>IntPtr</c>Pointer to file handle</returns>
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			private static extern IntPtr CreateFile(string filename, uint access, uint sharemode,
				IntPtr security_attributes, uint creation, uint flags, IntPtr template);

			/// <summary>
			/// Win32 API call to close a file handle
			/// </summary>
			/// <param name="handle"><c>InPtr</c> to file handle</param>
			/// <returns>HRESULT</returns>
			[DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
			private static extern bool CloseHandle(IntPtr handle);

			/// <summary>
			///     
			/// </summary>
			internal ADSFile()
			{
				m_SafeHandle = null;
			}

			/// <summary>
			///     
			/// </summary>
			~ADSFile()
			{
				Close();
			}

			internal FileStream Create(string path, string stream)
			{
				FileStream fs = null;

				//create or open file
				IntPtr handle = CreateFile(path + ":" + stream, GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ,
					IntPtr.Zero, OPEN_ALWAYS, 0, IntPtr.Zero);

				// create the file stream with write access with the file handle
				// file stream owns handle
				m_SafeHandle = new SafeFileHandle(handle, true);

				if (!m_SafeHandle.IsInvalid)
				{
					fs = new FileStream(m_SafeHandle, FileAccess.ReadWrite);
				}

				return fs;
			}

			/// <summary>
			/// Opens an existing UTF-8 encoded <c>StreamReader</c> stream within a file for reading.
			/// <seealso cref="File.OpenText"/>
			/// </summary>
			/// <param name="path">The path to the file with stream.</param>
			/// <param name="stream">The stream to read from.</param>
			/// <returns>A <c>StreamReader</c> on the specified path.</returns>
			/// <exception cref="FileNotFoundException">Unable to open specified file or stream.</exception>
			internal StreamReader OpenText(string path, string stream)
			{
				//create or open file
				IntPtr handle = CreateFile(path + ":" + stream, GENERIC_READ, FILE_SHARE_READ,
					IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

				m_SafeHandle = new SafeFileHandle(handle, true);

				if ( m_SafeHandle.IsInvalid )
				{
					throw new FileNotFoundException("Unable to create stream: " + stream + " in file: " + path + ".", path);
				}
				// create the file stream with write access with the file handle
				// file stream owns handle

				FileStream fs = new FileStream(m_SafeHandle, FileAccess.Read);

				// convert stream to StreamReader for appending text
				StreamReader sw = new StreamReader((Stream)fs);
				return sw;
			}

			internal void Close()
			{
				if (m_SafeHandle != null)
				{
					if (!m_SafeHandle.IsClosed)
					{
						m_SafeHandle.Close();
					}
				}
			}

			private SafeFileHandle m_SafeHandle;

			/// <summary>
			/// Delete a stream within a file
			/// </summary>
			/// <param name="path">The path to the file with stream.</param>
			/// <param name="stream">The stream to read/write to.</param>
			/// <returns><c>bool</c> indicating existance of stream</returns>
			internal static bool Exists(string path, string stream)
			{
				IntPtr handle;
				bool bExists = false;

				//get handle with least privledges
				handle = CreateFile(path + ":" + stream, GENERIC_READ, FILE_SHARE_READ,
					IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);

				if ((int)handle != -1)
				{
					bExists = true;

					CloseHandle(handle);
				}

				return bExists;
			}
		}
	}



}
