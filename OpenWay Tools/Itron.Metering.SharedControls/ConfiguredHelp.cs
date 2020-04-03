using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Configuration;
using Itron.Metering.Utilities;

namespace Itron.Metering.SharedControls
{
    /// <summary>
    /// Static class that can be used to show a help dialog.
    /// </summary>
    public static class ConfiguredHelp
    {
        /// <summary>
        /// Shows the specified help dialog.
        /// </summary>
        /// <param name="parent">The parent control for the help dialog.</param>
        /// <param name="strHelpName">The ID of the help to display.</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/08 RCG 1.50.29 N/A    Created

        public static void ShowHelp(Control parent, string strHelpName)
        {
            HelpConfig HelpConfiguration = 
                (HelpConfig)ConfigurationManager.GetSection("Itron.Metering/HelpConfig");
            HelpProvider Provider = new HelpProvider();

            Provider.HelpNamespace = HelpConfiguration.File;

            if (HelpConfiguration.HelpIDs[strHelpName] != null)
            {
                Help.ShowHelp(parent, Provider.HelpNamespace, HelpNavigator.TopicId, HelpConfiguration.HelpIDs[strHelpName].ID);
            }
            else
            {
                Help.ShowHelp(parent, Provider.HelpNamespace, HelpNavigator.TopicId, strHelpName);
            }            
        }

        /// <summary>
        /// Shows the help index for the configured help file.
        /// </summary>
        /// <param name="parent">The parent control for the help dialog</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/08 RCG 1.50.29 N/A    Created

        public static void ShowHelpIndex(Control parent)
        {
            HelpConfig HelpConfiguration =
                (HelpConfig)ConfigurationManager.GetSection("Itron.Metering/HelpConfig");
            HelpProvider Provider = new HelpProvider();

            Provider.HelpNamespace = HelpConfiguration.File;

            Help.ShowHelpIndex(parent, Provider.HelpNamespace);
        }

        /// <summary>
        /// Shows the search for the configured help file
        /// </summary>
        /// <param name="parent">The parent control for the help dialog</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/08 RCG 1.50.29 N/A    Created

        public static void ShowHelpSearch(Control parent)
        {
            HelpConfig HelpConfiguration =
                (HelpConfig)ConfigurationManager.GetSection("Itron.Metering/HelpConfig");
            HelpProvider Provider = new HelpProvider();

            Provider.HelpNamespace = HelpConfiguration.File;

            Help.ShowHelp(parent, Provider.HelpNamespace, HelpNavigator.Find, "");
        }

        /// <summary>
        /// Shows the table of contents for the configured help file
        /// </summary>
        /// <param name="parent">The parent control for the help dialog</param>
        //  Revision History	
        //  MM/DD/YY who Version Issue# Description
        //  -------- --- ------- ------ -------------------------------------------
        //  06/04/08 RCG 1.50.29 N/A    Created

        public static void ShowHelpTableOfContents(Control parent)
        {
            HelpConfig HelpConfiguration =
                (HelpConfig)ConfigurationManager.GetSection("Itron.Metering/HelpConfig");
            HelpProvider Provider = new HelpProvider();

            Provider.HelpNamespace = HelpConfiguration.File;

            Help.ShowHelp(parent, Provider.HelpNamespace, HelpNavigator.TableOfContents);
        }

    }
}
