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
using System.Text;

namespace Itron.Metering.Device
{
    /// <summary>
    /// This class provides static conversion functions used to convert SCS 
    /// basepage addresses to byte arrays and vice-versa
    /// </summary>
    internal class SCSConversion
    {
        static public int BytetoInt(ref byte[] byteBuffer, int length)
        {
            int result = 0;

            if (length == 1)
                result = byteBuffer[0];

            if (length == 2)
                result = (byteBuffer[0] << 8) + byteBuffer[1];

            if (length == 3)
                result = (byteBuffer[0] << 16) + (byteBuffer[1] << 8) + byteBuffer[2];

            if (length == 4)
                result = (byteBuffer[0] << 24) + (byteBuffer[1] << 16) + (byteBuffer[2] << 8) + byteBuffer[3];
            
            return result;
        }

    }
}
