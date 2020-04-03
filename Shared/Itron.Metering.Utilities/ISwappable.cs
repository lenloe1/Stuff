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
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Interface for the main control events.
    /// </summary>
    public interface ISwappableMainControl
    {
        /// <summary>
        /// Event for changing the main control
        /// </summary>
        event ChangeControlEventHandler ChangeMainControlEvent;

        /// <summary>
        /// Event for changing the sub control
        /// </summary>
        event ChangeControlEventHandler ChangeSubControlEvent;

        /// <summary>
        /// Event for changing the main control's title
        /// </summary>
        event ChangeControlEventHandler TitleChangedEvent;
    }

    /// <summary>
    /// Interface for the sub control events.
    /// </summary>
    public interface ISwappableSubControl
    {
        /// <summary>
        /// Event for changing the sub control
        /// </summary>
        event ChangeControlEventHandler ChangeSubControlEvent;
    }
}
