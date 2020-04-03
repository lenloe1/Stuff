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
//                            Copyright © 2007 - 2008 
//                                Itron, Inc.
//////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Windows.Forms;
using C1.Win.C1FlexGrid;
//using Itron.Metering.FieldPro;
using Itron.Metering.Progressable;
using Itron.Metering.Utilities;

namespace Itron.Metering.SharedControls
{
	/// <summary>
	/// Base class for all of the views possible in data manager.  This class provides common functions
	/// for all derived views such as managing screen refreshing, handling progress indicators, error handling
	/// and data item selection.
	/// </summary>
	public partial class FlexGridView : UserControl, IProgressable, IExportable, IPrintable, IDisplayHelp
	{
		#region Public Events

		/// <summary>
		/// This event is fired by a data view whenever the user checks or removes
		/// a selection from the data view
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		public event FlexGridSelectionChangedHandler FlexGridSelectionChangedEvent;

		/// <summary>
		/// This event is fired whenever a progress bar should be displayed
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  
        //
        public virtual event ShowProgressEventHandler ShowProgressEvent;

		/// <summary>
		/// This event is fired to indicate that the progress bar should be removed
		/// and normal processing can resume
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        public virtual event HideProgressEventHandler HideProgressEvent;

		/// <summary>
		/// This event is fired to more the progress indictor a single step forward
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        public virtual event StepProgressEventHandler StepProgressEvent;

		#endregion

		#region Public Methods

		/// <summary>
		/// Default Constructor - to be used by the forms designer only
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected FlexGridView()
            : this("")
        {
        }

		/// <summary>
		/// Primary constructor.  This constructor MUST be called by all
		/// derived classes
		/// </summary>
        /// <param name="strTitle">The title of the view.  The title will be displayed
		/// on all printed output
		/// </param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Added member variables.
        //
		public FlexGridView(string strTitle )
		{
			InitializeComponent();

			m_strTitle = strTitle;
			m_strExportSheetName = Title;
            m_strPrintHeader = Application.ProductName + "\n" + Title;
            m_strPrintDocName = this.Text;
		}

		/// <summary>
		/// Displays the print preview dialog
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Used a member variable to set the print document's 
        //                              name and the printed header.
        //
        virtual public void PrintPreview()
		{
			if (null != DataGrid)
			{
				try
				{
                    DataGrid.PrintGrid(m_strPrintDocName, PrintGridFlags.ShowPreviewDialog | PrintGridFlags.FitToPageWidth | PrintGridFlags.ExtendLastCol,
										m_strPrintHeader, DateTime.Now.ToString());
				}

				catch (Exception err)
				{
					DisplayError( "Resources.ERROR_PRINT_PREVIEW", err);
				}
			}
		}

		/// <summary>
		/// Displays the print dialog
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Used a member variable to set the print document's 
        //                              name and the printed header.
        //
        virtual public void Print()
		{
			if (null != DataGrid)
			{
				try
				{
                    DataGrid.PrintGrid(m_strPrintDocName, PrintGridFlags.ShowPrintDialog | PrintGridFlags.FitToPageWidth | PrintGridFlags.ExtendLastCol,
                                        m_strPrintHeader, DateTime.Now.ToString());
				}

				catch (Exception err)
				{
					DisplayError( "Resources.ERROR_PRINT_DISPLAY", err);
				}
			}
		}

		/// <summary>
		/// Displays the page setup dialog - NOTE this method is not implemented but must be included
		/// to complete the IPrintable interface specification
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//
		//
		public void PageSetup()
		{
			DisplayError("Resources.ERROR", new NotImplementedException("Page setup method not implemented"));
		}

		/// <summary>
		/// Handles saving the view to Excel
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Used a member variable to set the exported file's 
        //                              sheet name.
        virtual public void ExportToXLS()
		{
			try
			{
				if (null != DataGrid)
				{
					saveFileDialog.FileName = Title;

					DialogResult dlgResult = saveFileDialog.ShowDialog();

					if (DialogResult.OK == dlgResult)
					{
                        DataGrid.SaveExcel(saveFileDialog.FileName, m_strExportSheetName, FileFlags.IncludeFixedCells | FileFlags.AsDisplayed | FileFlags.SaveMergedRanges);
					}
				}
			}

			catch (Exception e)
			{
				DisplayError( "Resources.ERROR_EXPORT", e );
			}
		}

		/// <summary>
		/// Displays the appropriate help topic
		/// </summary>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//
		//
		public void DisplayHelpTopic()
		{
            ConfiguredHelp.ShowHelp(this, HelpTopic);
		}

		#endregion

		#region Public Properties

		/// <summary>
        /// Gets or sets the title of this view.  The title will be used on all printed and exported
		/// versions of the view
		/// </summary>
		public string Title
		{
			get { return m_strTitle; }
			set { m_strTitle = value; }
		}

		/// <summary>
		/// This property identifies the help topic that will be displayed when the help button on the application window 
		/// is clicked.
		/// </summary>
		public string HelpTopic
		{
			get { return m_strHelpTopic; }
			set { m_strHelpTopic = value; }
		}

		/// <summary>
		/// This property identifies the help file that will be used
		/// </summary>
		public string HelpFilePath
		{
			get { return m_strHelpFilePath; }
			set { m_strHelpFilePath = value; }
		}

        #endregion

        #region Protected Methods

		/// <summary>
		///     
		/// </summary>
		/// <param name="strFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <remarks>
		///     
		/// </remarks>
		protected delegate void RefreshViewDelegate( String strFileName );

		/// <summary>
		///     
		/// </summary>
		/// <param name="strFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <remarks>
		///     
		/// </remarks>
		protected delegate void DeleteViewDelegate(String strFileName);

		/// <summary>
		///     
		/// </summary>
		/// <param name="strFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <remarks>
		///     
		/// </remarks>
		protected delegate void CreateViewDelegate(String strFileName);

		/// <summary>
		///     
		/// </summary>
		/// <param name="strOldFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <param name="strNewFileName" type="string">
		///     <para>
		///         
		///     </para>
		/// </param>
		/// <remarks>
		///     
		/// </remarks>
		protected delegate void RenameViewDelegate(String strOldFileName, String strNewFileName);

		/// <summary>
		/// 
		/// </summary>
		/// <param name="e">The event arguments</param>
		//  Revision History	
		//  MM/DD/YY Who Version Issue# Description
		//  -------- --- ------- ------ -------------------------------------------
		//
		//
		protected virtual void OnFlexGridSelectionChanged( FlexGridSelectionChangedEventArgs e)
		{
			if (FlexGridSelectionChangedEvent != null)
			{
				FlexGridSelectionChangedEvent(this, e);
			}
		}//OnFlexGridSelectionChanged

		/// <summary>
		/// Raises the ShowProgressEvent for this sub control
		/// </summary>
		/// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected virtual void OnShowProgress(ShowProgressEventArgs e)
		{
			if (ShowProgressEvent != null)
			{
				ShowProgressEvent(this, e);
			}
		}//OnShowProgress

		/// <summary>
		/// Raises a StepProgressEvent for this sub control
		/// </summary>
		/// <param name="e">The event arguments</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected virtual void OnStepProgress(ProgressEventArgs e)
		{
			if (StepProgressEvent != null)
			{
				StepProgressEvent(this, e);
			}
		}//OnStepProgress

		/// <summary>
		/// Rasies a HideProgressEvent for this sub control
		/// </summary>
		/// <param name="e">The event argument</param>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        protected virtual void OnHideProgress(EventArgs e)
		{
			if (HideProgressEvent != null)
			{
				HideProgressEvent(this, e);
			}
		}//OnHideProgress

		/// <summary>
        /// Selects (sets the check box) all data items currently shown in the data
		/// grid
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        virtual protected void SelectAll()
		{
			if (null != DataGrid)
			{
				foreach (Row row in DataGrid.Rows)
				{
					if (row.Index > 0 && row.Visible == true)
					{
						row[0] = true;
					}
				}
			}
		}

		/// <summary>
        /// Clears all data item selections from the data grid.
		/// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //
        //
        virtual protected void ClearAll()
		{
			if (null != DataGrid)
			{
				foreach (Row row in DataGrid.Rows)
				{
					if (row.Index > 0 && row.Visible == true)
					{
						row[0] = false;
					}
				}
			}
		}

		/// <summary>
		/// This method is by derived classes to indicate which directory or file the current
		/// view is representing and to begin watching that file or directory for changes so that
		/// the view can be refreshed when needed.
		/// 
		/// Note that this method can be overrided as needed to set different filters on the file or directory
		/// to be watched.  By default the watcher will only fire events when the last write time of the 
		/// file or directory changes.
		/// </summary>
		/// <param name="strDirectoryName">
		/// This is the path of the directory watch for changes</param>
		/// <param name="strFileName">
		/// This is the name of the specific file or file mask to watch for changes
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void InitializeAutoRefresh(String strDirectoryName, String strFileName )
		{
			try
			{
				m_DirectoryWatcher = new FileSystemWatcher();
				m_DirectoryWatcher.Path = strDirectoryName;
				m_DirectoryWatcher.Filter = strFileName;
				m_DirectoryWatcher.SynchronizingObject = this;

				// Only fire an event when the directory or file name is written to.
				m_DirectoryWatcher.NotifyFilter = (NotifyFilters.LastWrite | NotifyFilters.DirectoryName | NotifyFilters.FileName);
				m_DirectoryWatcher.IncludeSubdirectories = false;

				// Add event handlers.
				m_DirectoryWatcher.Changed += new FileSystemEventHandler(OnViewDataChanged);
				m_DirectoryWatcher.Deleted += new FileSystemEventHandler(OnViewDataDeleted);
				m_DirectoryWatcher.Created += new FileSystemEventHandler(OnViewDataCreated);
				m_DirectoryWatcher.Renamed += new RenamedEventHandler(OnViewDataRenamed);

				// Note that the watcher will NOT fire any events until we enable it -  please don't remove
				// the following line!
				EnableAutoRefresh();
			}
			catch (Exception err)
			{
				DisplayError( "Resources.ERROR_AUTO_REFRESH_ERROR", err);
			}
		}

		/// <summary>
		/// This method is by derived classes to change which directory or file the current
		/// view is watching changes so that the view can be refreshed when needed.
		/// </summary>
		/// <param name="strDirectoryName">
		/// This is the path of the directory watch for changes</param>
		/// <param name="strFileName">
		/// This is the name of the specific file or file mask to watch for changes
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/26/07 MAH                 Created
		///
		/// </remarks>
		protected void ResetAutoRefresh(String strDirectoryName, String strFileName)
		{
			try
			{
				DisableAutoRefresh();

				m_DirectoryWatcher.Path = strDirectoryName;
				m_DirectoryWatcher.Filter = strFileName;

				EnableAutoRefresh();
			}
			catch( Exception err )
			{
				DisplayError( "Could not reset directory watcher", err);
			}
		}

		/// <summary>
		/// This method enables the update notifications and automatic refreshing of 
		/// the data view when the root directory or file changes.   Note that this method
		/// is called as a part of the InitializeAutoRefresh method so it is only needed to 
		/// re-enable update notifications after they have been disabled.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		protected void EnableAutoRefresh()
		{
            if (m_DirectoryWatcher != null) 
                m_DirectoryWatcher.EnableRaisingEvents = true;
		}

		/// <summary>
		/// This method disables the update notifications and automatic refreshing of 
		/// the data view when the root directory or file changes.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		protected void DisableAutoRefresh()
		{
            if (m_DirectoryWatcher != null )
			    m_DirectoryWatcher.EnableRaisingEvents = false;
		}

		/// <summary>
		/// This method is called whenever a renamed event is raised.  By default, the
		/// data view is refreshed but this method can be overriden to perform custom actions
		/// as needed by derived classes
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void OnViewDataRenamed(object source, RenamedEventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(e.OldFullPath))
				{
					// Before we can refresh the data view, we need to know where this request is coming 
					// from.  We can only interact with the control if the request originated on the thread that
					// owns the flex grid.  If the request originated from any other thread, then the refresh must
					// redirected to the proper thread.
					if (m_ctlDataGrid.InvokeRequired == false)
					{
						RenameDataViewItem(e.OldFullPath, e.FullPath);
					}
					else
					{
						// Refresh the view asynchronously

						RenameViewDelegate RenameViewAsync = new RenameViewDelegate(RenameDataViewItem);

						object[] parameterArray = new object[2];
						parameterArray[0] = e.OldFullPath;
						parameterArray[1] = e.FullPath;

						this.Invoke(RenameViewAsync, parameterArray);
					}
				}
			}

			catch
			{
				// It is possible that a refresh message is sent to a data view that has been switched out.  In this case the data view
				// and all child controls no longer exist.  Rather than display an error, just don't attempt to refresh something that 
				// isn't visible anyway
			}
		}

		/// <summary>
		/// This method is called whenever a data changed event is raised.  By default, the
		/// data view is refreshed but this method can be overriden to perform custom actions
		/// as needed by derived classes
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void OnViewDataChanged(object source, FileSystemEventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(e.FullPath))
				{
					// Before we can refresh the data view, we need to know where this request is coming 
					// from.  We can only interact with the control if the request originated on the thread that
					// owns the flex grid.  If the request originated from any other thread, then the refresh must
					// redirected to the proper thread.
					if (m_ctlDataGrid.InvokeRequired == false)
					{
						RefreshDataViewItem(e.FullPath);
					}
					else
					{
						// Refresh the view asynchronously

						RefreshViewDelegate RefreshViewAsync = new RefreshViewDelegate(RefreshDataViewItem);

						object[] parameterArray = new object[1];
						parameterArray[0] = e.FullPath;

						this.Invoke(RefreshViewAsync, parameterArray);
					}
				}
			}

			catch
			{
				// It is possible that a refresh message is sent to a data view that has been switched out.  In this case the data view
				// and all child controls no longer exist.  Rather than display an error, just don't attempt to refresh something that 
				// isn't visible anyway
			}
		}

		/// <summary>
		/// This method is called whenever a data changed event is raised.  By default, the
		/// data view is refreshed but this method can be overriden to perform custom actions
		/// as needed by derived classes
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void OnViewDataDeleted(object source, FileSystemEventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(e.FullPath))
				{
					// Before we can refresh the data view, we need to know where this request is coming 
					// from.  We can only interact with the control if the request originated on the thread that
					// owns the flex grid.  If the request originated from any other thread, then the refresh must
					// redirected to the proper thread.
					if (m_ctlDataGrid.InvokeRequired == false)
					{
						DeleteDataViewItem(e.FullPath);
					}
					else
					{
						// Refresh the view asynchronously

						DeleteViewDelegate DeleteViewAsync = new DeleteViewDelegate(DeleteDataViewItem);

						object[] parameterArray = new object[1];
						parameterArray[0] = e.FullPath;

						this.Invoke(DeleteViewAsync, parameterArray);
					}
				}
			}

			catch
			{
				// It is possible that a refresh message is sent to a data view that has been switched out.  In this case the data view
				// and all child controls no longer exist.  Rather than display an error, just don't attempt to refresh something that 
				// isn't visible anyway
			}
		}

		/// <summary>
		/// This method is called whenever a data changed event is raised.  By default, the
		/// data view is refreshed but this method can be overriden to perform custom actions
		/// as needed by derived classes
		/// </summary>
		/// <param name="source"></param>
		/// <param name="e"></param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void OnViewDataCreated(object source, FileSystemEventArgs e)
		{
			try
			{
				if (!String.IsNullOrEmpty(e.FullPath))
				{
					// Before we can refresh the data view, we need to know where this request is coming 
					// from.  We can only interact with the control if the request originated on the thread that
					// owns the flex grid.  If the request originated from any other thread, then the refresh must
					// redirected to the proper thread.
					if (m_ctlDataGrid.InvokeRequired == false)
					{
						CreateDataViewItem(e.FullPath);
					}
					else
					{
						// Refresh the view asynchronously

						CreateViewDelegate CreateViewAsync = new CreateViewDelegate(CreateDataViewItem);

						object[] parameterArray = new object[1];
						parameterArray[0] = e.FullPath;

						this.Invoke(CreateViewAsync, parameterArray);
					}
				}
			}

			catch
			{
				// It is possible that a refresh message is sent to a data view that has been switched out.  In this case the data view
				// and all child controls no longer exist.  Rather than display an error, just don't attempt to refresh something that 
				// isn't visible anyway
			}
		}

		/// <summary>
		/// Refreshes the current data view asynchronously.  This method can be overrided by derived
		/// classes to perform custom actions on refresh operations but care must be taken to ensure 
		/// thread safety since this method is typically invoked by a thread that does not own the 
		/// UI controls.  Note that the default implementation completely reinitializes the data view.  Therefore
		/// user selections are cleared.
		/// </summary>
		/// <param name="strFileName">
		/// The full path name of the item that was updated.  This information is provided so that derived
		/// classes can decide if the entire view must be refreshed or only a single item
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void RefreshDataViewItem( String strFileName )
		{
			PopulateDataView();
		}//end RefreshDataView

		/// <summary>
		/// Refreshes the current data view asynchronously to reflect the fact that an item was renamed.  
		/// This method should be overriden by derived classes to perform custom actions on refresh 
		/// operations but care must be taken to ensure thread safety since this method is typically 
		/// invoked by a thread that does not own the UI controls.  Note that there is no default 
		/// implementation.  Therefore any view that needs to react to the renaming of a data file must 
		/// implement this method
		/// </summary>
		/// <param name="strOldFileName">
		/// The original full path name of the file that was renamed.  This information is provided so that derived
		/// classes can decide if the entire view must be refreshed or only a single item
		/// </param>
		/// <param name="strNewFileName">
		/// The new full path name of the file that was renamed.  </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  11/20/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void RenameDataViewItem(String strOldFileName, String strNewFileName)
		{
		}//end RenameDataViewItem

		/// <summary>
		/// Refreshes the current data view asynchronously to reflect the fact that an item was deleted.  
		/// This method should be overriden by derived classes to perform custom actions on refresh 
		/// operations but care must be taken to ensure thread safety since this method is typically 
		/// invoked by a thread that does not own the UI controls.  Note that there is no default 
		/// implementation.  Therefore any view that needs to react to the deletion of a data file must 
		/// implement this method
		/// </summary>
		/// <param name="strFileName">
		/// The full path name of the file that was deleted.  This information is provided so that derived
		/// classes can decide if the entire view must be refreshed or only a single item
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/26/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void DeleteDataViewItem(String strFileName)
		{
		}//end DeleteDataViewItem

		/// <summary>
		/// Refreshes the current data view asynchronously to reflect the addition of a new data item.  
		/// This method can be overrided by derived classes to perform custom actions on refresh 
		/// operations but care must be taken to ensure thread safety since this method is typically 
		/// invoked by a thread that does not own the UI controls.  Note that there is no default 
		/// implementation.
		/// </summary>
		/// <param name="strFileName">
		/// The full path name of the item that was added.  This information is provided so that derived
		/// classes can decide if the entire view must be refreshed or only a single item
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void CreateDataViewItem(String strFileName)
		{
		}//end CreateDataViewItem

		/// <summary>
		/// This method must be overriden by derived classes to populate the data view.
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void PopulateDataView()
		{
		}

		/// <summary>
		/// Use this method to display non-fatal error messages.  This method must be used 
		/// to insure consistency between all data views and to provide as much visibility as 
		/// possible to all error information. 
		/// </summary>
		/// <param name="strErrorMessage">
		/// This is the primary message that will be displayed to the user</param>
		/// <param name="e">
		/// This is the exception object that originally caused the error.  It is important to
		/// provide this information to the user when available to assist in debugging and error 
		/// resolution
		/// </param>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  07/19/07 MAH                 Created
		///
		/// </remarks>
		virtual protected void DisplayError(String strErrorMessage, Exception e)
		{
			ErrorForm.DisplayError(strErrorMessage, e);
		}

        /// <summary>
        /// Adds a heading row to the flex grid.
        /// </summary>
        /// <param name="strTitle">The title of the heading row.</param>
        /// <param name="iLevel">The level of the heading row.</param>
        // Revision History	
        // MM/DD/YY who Version Issue#    Description
        // -------- --- ------- --------- ---------------------------------------
        //  05/18/12 jrf 2.60.23 TREQ6032 Created to share common code between controls.
        //
        protected void AddHeadingRow(string strTitle, int iLevel)
        {
            Row rowComponentSummary = m_ctlDataGrid.Rows.Add();
            rowComponentSummary.StyleNew.BackColor = Color.FromArgb(220, 220, 220);
            rowComponentSummary.IsNode = true;
            rowComponentSummary.Node.Level = iLevel;
            rowComponentSummary[0] = strTitle;
        }

        /// <summary>
        /// Adds a new row with the specified data to the flex grid.
        /// </summary>
        /// <param name="strItemName">The name of the item being added.</param>
        /// <param name="strValue">The values of the item.</param>
        /// <param name="iLevel">The level of the row to add.</param>
        // Revision History	
        // MM/DD/YY who Version Issue#    Description
        // -------- --- ------- --------- ---------------------------------------
        //  05/18/12 jrf 2.60.23 TREQ6032 Created to share common code between controls.
        //
        protected void AddDataRow(string strItemName, string strValue, int iLevel)
        {
            Row rowNewRow = m_ctlDataGrid.Rows.Add();
            rowNewRow.IsNode = true;
            rowNewRow.Node.Level = iLevel;

            rowNewRow[0] = strItemName;
            rowNewRow[1] = strValue;
        }

		#endregion

		#region Protected Properties

		/// <summary>
		/// Gets or sets the Data Grid.  This property MUST be set by all derived classes for this
		/// class to operate properly
		/// </summary>
		protected C1FlexGrid DataGrid
		{
			get { return m_ctlDataGrid; }
			set { m_ctlDataGrid = value; }
		}

		/// <summary>
        /// Returns the number of items currently selected on the data grid.  This is 
		/// a read-only property
		/// </summary>
		protected int SelectionCount
		{
			get
			{			
				// Determine how many columns have been selected
				int nSelectionCount = 0;

				foreach (Row row in DataGrid.Rows)
				{
					if (row.Index > 0)
					{
                        if (null != row[0] && (Boolean)row[0])
						{
							nSelectionCount++;
						}
					}
				}

				return nSelectionCount;
			}
		}

        /// <summary>
        /// Gets or sets the sheet name to use when exporting this view.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Created.
        // 
        protected string ExportSheetName
        {
            get { return m_strExportSheetName; }
            set { m_strExportSheetName = value; }
        }

        /// <summary>
        /// Gets or sets the header to use when printing or print previewing this view.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Created.
        // 
        protected string PrintHeader
        {
            get { return m_strPrintHeader; }
            set { m_strPrintHeader = value; }
        }

        /// <summary>
        /// Gets or sets the document name to use when printing or print previewing this view.
        /// </summary>
        //  Revision History	
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  02/26/08 jrf 10.00.00       Created.
        // 
        protected string PrintDocumentName
        {
            get { return m_strPrintDocName; }
            set { m_strPrintDocName = value; }
        }

		#endregion

		#region Private Members

		private C1FlexGrid m_ctlDataGrid;
		private string m_strTitle;
		private FileSystemWatcher m_DirectoryWatcher;
		private string m_strHelpTopic;
        private string m_strHelpFilePath;
        private string m_strExportSheetName;
        private string m_strPrintHeader;
        private string m_strPrintDocName;

		#endregion

    }//end DataManagerView


	/// <summary>
	///     
	/// </summary>
	/// <remarks>
	///     
	/// </remarks>
	public class FlexGridSelectionChangedEventArgs : EventArgs
	{
		private int m_nRow;

		/// <summary>
		///     
		/// </summary>
		/// <param name="nRow" >
		/// </param>
		public FlexGridSelectionChangedEventArgs(int nRow)
		{
			m_nRow = nRow;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///
		/// </remarks>
		public int Row
		{
			get
			{
				return m_nRow;
			}
		}
	}//end ProcessStartedEventArgs

	/// <summary>
	///     
	/// </summary>
	/// <param name="sender" type="object">
	/// </param>
	/// <param name="e" type="Itron.Metering.SharedControls.ProcessStartedEventArgs">
	/// </param>
	/// <remarks>
	///     
	/// </remarks>
	public delegate void FlexGridSelectionChangedHandler(object sender, FlexGridSelectionChangedEventArgs e);



}
