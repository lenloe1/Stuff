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
//                              Copyright © 2008-2014
//                                Itron, Inc.
/////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
	/// <summary>
	/// This class provides helper functions for use in verifying firmware versions.
	/// Version numbers are typically provided as floating point values and simple
	/// comparisions of floating point values can be problematic.  These methods
	/// take advantage of the fixed format of version numbers and provide safer
	/// methods for comparison
	/// </summary>
	public static class VersionChecker
	{
		/// <summary>
		/// This comparision method allows the caller to specify the tolerance
		/// used when comparing two floating point values.  If the difference
		/// between the two values is within the given tolerance than the values
		/// are assumed to be equal although the actual floating point values may
		/// not be
		/// </summary>
		/// <param name="fltCurrentVersion" type="float">
		/// The version number as returned by the file or device that is to be 
		/// validated
		/// </param>
		/// <param name="fltTargetVersion" type="float">
		/// The fixed or target version number that the file or device is being
		/// compared to.
		/// </param>
		/// <param name="fltTolerance" type="float">
		/// The maximum allowable difference between the two version numbers that
		/// can exist while still considering the two numbers to be equal
		/// </param>
		/// <returns>
		/// Returns -1 if the current version is less than the target
		/// Returns 0 if the current version is equal to the target
		/// Returns 1 if the current version is greater than the target
		/// </returns>
		/// <remarks>
		///  Revision History	
		///  MM/DD/YY Who Version Issue# Description
		///  -------- --- ------- ------ -------------------------------------------
		///  06/26/08 mah                Created
		/// </remarks>
		public static int CompareTo(float fltCurrentVersion, float fltTargetVersion, float fltTolerance)
		{
			float fltMinTarget = fltTargetVersion - fltTolerance;
			float fltMaxTarget = fltTargetVersion + fltTolerance;

			if ( fltCurrentVersion < fltMinTarget)
			{
				return -1;
			}
			else if (fltCurrentVersion > fltMaxTarget)
			{
				return 1;
			}
			else
			{
				return 0;
			}
		}

        /// <summary>
        /// Compares the two double values to see if they are equal within the given tolerance
        /// </summary>
        /// <param name="firstValue">The first value to compare</param>
        /// <param name="secondValue">The second value to compare</param>
        /// <param name="tolerance">The tolerance allowed for the values to be equal</param>
        /// <returns>-1 if the first value is less than second. 1 if the first value is greater than the second. 0 if the first and second are within the specified tolerance</returns>
        //  Revision History
        //  MM/DD/YY Who Version Issue# Description
        //  -------- --- ------- ------ ---------------------------------------------
        //  06/08/12 RCG 2.60.30 N/A    Created
        
        public static int CompareTo(double firstValue, double secondValue, double tolerance)
        {
            double dMinTarget = secondValue - tolerance;
            double dMaxTarget = secondValue + tolerance;
            int CompareValue = 0;

            if (firstValue < dMinTarget)
            {
                CompareValue = -1;
            }
            else if (firstValue > dMaxTarget)
            {
                CompareValue = 1;
            }
            // else They are equal (0)

            return CompareValue;
        }

        /// <summary>
        /// Returns -1 if the current version is less than the target
        /// Returns 0 if the current version is equal to the target
        /// Returns 1 if the current version is greater than the target
        /// This comparison method should be used to compare version numbers with the
        /// format of #.###   If more or less precision is required, a different 
        /// tolerance should be used.
        /// </summary>
        /// <param name="fltCurrentVersion" type="float">
        /// The version number as returned by the file or device that is to be 
        /// validated
        /// </param>
        /// <param name="fltTargetVersion" type="float">
        /// The fixed or target version number that the file or device is being
        /// compared to.
        /// </param>
        /// <returns>
        /// Returns -1 if the current version is less than the target
        /// Returns 0 if the current version is equal to the target
        /// Returns 1 if the current version is greater than the target
        /// </returns>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  06/26/08 mah                Created
        /// </remarks>

        public static int CompareTo(float fltCurrentVersion, float fltTargetVersion)
		{
			return CompareTo(fltCurrentVersion, fltTargetVersion, 0.0005F);
		}

        /// <summary>
        /// This comparison method should be used to compare version numbers with the
        /// format of #.###   If more or less precision is required, a different 
        /// tolerance should be used.
        /// </summary>
        /// <param name="currentVersion" type="float">
        /// The version number as returned by the file or device that is to be 
        /// validated
        /// </param>
        /// <param name="targetVersion" type="float">
        /// The fixed or target version number that the file or device is being
        /// compared to.
        /// </param>
        /// <returns>
        /// Returns -1 if the current version is less than the target
        /// Returns 0 if the current version is equal to the target
        /// Returns 1 if the current version is greater than the target
        /// </returns>
        /// <remarks>
        ///  Revision History	
        ///  MM/DD/YY Who Version Issue# Description
        ///  -------- --- ------- ------ -------------------------------------------
        ///  09/18/14 jrf                Created
        /// </remarks>

        public static int CompareTo(double currentVersion, double targetVersion)
        {
            return CompareTo(currentVersion, targetVersion, 0.0005F);
        }

        /// <summary>
        /// This comparison method can be used to compare version, revision and build values.
        /// </summary>
        /// <param name="currentVersion" type="float">
        /// The version number as returned by the file or device that is to be 
        /// validated.
        /// </param>
        /// <param name="currentBuild" type="byte">
        /// The build number as returned by the file or device that is to be 
        /// validated.
        /// </param>
        /// <param name="targetVersion" type="float">
        /// The fixed or target version number that the file or device is being
        /// compared to.
        /// </param>
        /// <param name="targetBuild" type="byte">
        /// The fixed or target build number that the file or device is being
        /// compared to.
        /// </param>
        /// <returns>-1 if the current is less than target. 
        ///           1 if the current is greater than the target. 
        ///           0 if the current and target are equal</returns>
        //  Revision History
        //  MM/DD/YY Who Version ID Number Description
        //  -------- --- ------- -- ------ ---------------------------------------------
        //  03/27/14 jrf 3.50.56 WR 465611 Created.        
        public static int CompareTo(float currentVersion, byte currentBuild, float targetVersion, byte targetBuild)
        {
            int VersionCompareResult = CompareTo(currentVersion, targetVersion);
            int VersionAndBuildCompareResult = VersionCompareResult; //until we know better.

            if (0 == VersionCompareResult)
            {
                //Now we need to check the build
                if (currentBuild < targetBuild)
                {
                    VersionAndBuildCompareResult = -1;
                }
                else if (currentBuild > targetBuild)
                {
                    VersionAndBuildCompareResult = 1;
                }
                // else They are equal (0), leave it as is
            }

            return VersionAndBuildCompareResult;
        }

	}
}
