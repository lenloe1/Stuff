using System;
using System.Collections.Generic;
using System.Text;

namespace Itron.Metering.Utilities
{
    /// <summary>
    /// Indicates the type of algorithm that will be used to calculate CRC values.
    /// </summary>
    public enum CrcAlgorithmType
    {
        /// <summary>
        /// Indicates that the CRC 32 algorithm will be used.
        /// </summary>
        Crc32 = 0,

        /// <summary>
        /// Indicates that the CRC 16-CCITT algorithm will be used.
        /// </summary>
        Crc16Ccitt
    }

    /// <summary>
    /// A helper class for calculating CRC values.
    /// </summary>
    public class CrcHelper
    {
        private CrcAlgorithmType _type;

        #region CRC Control Parameters

        private int _order;             // 'order' [1..32] is the CRC polynom order, counted without the leading '1' bit
        private ulong _polynom;         // 'polynom' is the CRC polynom without leading '1' bit
        //private bool direct;          // 'direct' [false, true] specifies the kind of algorithm: true=direct, no augmented zero bits
        private ulong _crcInitial;      // 'crcinit' is the initial CRC value belonging to that algorithm
        private ulong _crcXor;          // 'crcxor' is the final XOR value
        private bool _reflectIn;        // 'refin' [false, true] specifies if a data byte is reflected before processing (UART) or not
        private bool _reflectOut;       // 'refout' [false, true] specifies if the CRC will be reflected before XOR

        #endregion CRC Control Parameters

        private ulong _crcHighBit;
        private ulong _crcMask;
        private ulong _crcInitialDirect;

        private ulong[] _table;

        /// <summary>
        /// Initalizes a new instance of the class using the provided algorithm type.
        /// </summary>
        /// <param name="type">Indicates what type of CRC algorithm this instance should use.</param>
        public CrcHelper(CrcAlgorithmType type)
        {
            this.Initialize(type);

            this._type = type;
            this._table = this.CreateTable();
        }

        /// <summary>
        /// Gets the type of CRC algorithm that this instance is using.
        /// </summary>
        public CrcAlgorithmType CrcAlgorithmType
        {
            get { return _type; }
        }

        /// <summary>
        /// Uses a fast lookup table algorithm without augmented zero bytes to produce a CRC for the provided input.
        /// </summary>
        /// <remarks>
        /// This algorithm is only usable for polynomial orders of 8, 16, 24 or 32.
        /// </remarks>
        /// <param name="input">The target of the CRC.</param>
        /// <returns>A <see cref="UInt64"/> containing the CRC value that was calculated for the provided input.</returns>
        public ulong CalculateCrc(byte[] input)
        {
            ulong crc = this._crcInitialDirect;

            if (_reflectIn)
            {
                crc = CrcHelper.Reflect(crc, this._order);
            }

            int len = input.Length;
            int index = 0;

            if (!this._reflectIn)
            {
                while (len-- > 0)
                {
                    crc = (crc << 8) ^ this._table[((crc >> (this._order - 8)) & 0xff) ^ input[index++]];
                }
            }
            else
            {
                while (len-- > 0)
                {
                    crc = (crc >> 8) ^ this._table[(crc & 0xff) ^ input[index++]];
                }
            }

            if (this._reflectOut ^ this._reflectIn)
            {
                crc = CrcHelper.Reflect(crc, this._order);
            }

            crc ^= this._crcXor;
            crc &= this._crcMask;

            return crc;
        }

        private void Initialize(CrcAlgorithmType type)
        {
            switch (type)
            {
                case CrcAlgorithmType.Crc16Ccitt:
                    this._order = 16;
                    this._polynom = 0x1021;
                    //this.direct = true;
                    this._crcInitial = 0xffff;
                    this._crcXor = 0xffff;
                    this._reflectIn = true;
                    this._reflectOut = true;

                    break;
                case CrcAlgorithmType.Crc32:
                default:
                    this._order = 32;
                    this._polynom = 0x4c11db7;
                    //this.direct = true;
                    this._crcInitial = 0xffffffff;
                    this._crcXor = 0xffffffff;
                    this._reflectIn = true;
                    this._reflectOut = true;

                    break;
            }

            this._crcHighBit = (ulong)1 << (this._order - 1);
            this._crcMask = ((((ulong)1 << (this._order - 1)) - 1) << 1) | 1;
            this._crcInitialDirect = _crcInitial;
        }

        private static ulong Reflect(ulong crc, int bitnum)
        {
            // reflects the lower 'bitnum' bits of 'crc'

            ulong crcout = 0;

            for (ulong i = (ulong)1 << (bitnum - 1), j = 1; i > 0; i >>= 1)
            {
                if ((crc & i) > 0)
                {
                    crcout |= j;
                }

                j <<= 1;
            }

            return crcout;
        }

        private ulong[] CreateTable()
        {
            ulong bit, crc;
            ulong[] crctab = new ulong[256];

            for (ulong i = 0; i < (ulong)crctab.Length; i++)
            {
                crc = i;

                if (this._reflectIn)
                {
                    crc = CrcHelper.Reflect(crc, 8);
                }

                crc <<= this._order - 8;

                for (int j = 0; j < 8; j++)
                {
                    bit = crc & this._crcHighBit;
                    crc <<= 1;

                    if (bit > 0)
                    {
                        crc ^= this._polynom;
                    }
                }

                if (this._reflectIn)
                {
                    crc = CrcHelper.Reflect(crc, this._order);
                }

                crc &= this._crcMask;
                crctab[i] = crc;
            }

            return crctab;
        }
    }
}
