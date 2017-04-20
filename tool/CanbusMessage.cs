// ***********************************************************************
// Assembly         : ToolConnector
// Author           : SXu
// Created          : 04-09-2013
//
// Last Modified By : SXu
// Last Modified On : 04-09-2013
// ***********************************************************************
// <copyright file="CanbusMessage.cs" company="Evolution Engineering">
//     Copyright (c) Evolution Engineering.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ToolConnector
{
    /// <summary>
    /// Class CanbusMessage
    /// </summary>
    public class CanbusMessage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanbusMessage"/> class.
        /// </summary>
        /// <param name="date">The date.</param>
        /// <param name="destID">The dest ID.</param>
        /// <param name="dataLen">The data len.</param>
        /// <param name="byteData">The byte data.</param>
        public CanbusMessage(DateTime date, int destID, int dataLen, byte[] byteData)
        {
            this.date = date;
            this.destID = destID;
            if (dataLen < 0) dataLen = 0;
            this.dataLength = dataLen;
            if (byteData == null)
            {
                this.byteData = new byte[dataLen];
            }
            else if (dataLen == byteData.Length)
            {
                this.byteData = byteData;
            }
            else if (dataLen < byteData.Length)
            {
                this.byteData = new byte[dataLen];
                Array.Copy(byteData, 0, this.byteData, 0, dataLen);
            }
            else
            {
                this.byteData = new byte[dataLen];
                Array.Copy(byteData, 0, this.byteData, 0, byteData.Length);
            }
        }

        /// <summary>
        /// data Length
        /// </summary>
        private int dataLength = 0;
        /// <summary>
        /// The cyc time
        /// </summary>
        private int cycTime = 0;
        /// <summary>
        /// The comment
        /// </summary>
        private string comment = null;
        /// <summary>
        /// The cycle count
        /// </summary>
        private int cycleCount = 0;
        /// <summary>
        /// The date
        /// </summary>
        private DateTime date;

        /// <summary>
        /// The byte data
        /// </summary>
        private byte[] byteData;
        /// <summary>
        /// The dest ID
        /// </summary>
        private int destID= 0;

        /// <summary>
        /// data length
        /// </summary>
        public int DataLen
        {
            get
            {
                return this.dataLength;
            }
        }
        
        /// <summary>
        /// Gets the date.
        /// </summary>
        /// <value>The date.</value>
        public DateTime Date
        {
            get
            {
                return this.date;
            }
        }
        /// <summary>
        /// Gets the date STR.
        /// </summary>
        /// <value>The date STR.</value>
        public string DateStr
        {
            get
            {
                if (this.date == null)
                {
                    return "";
                }
                else
                {
                    return this.date.ToString("hh:mm:ss");
                }
            }
        }


        /// <summary>
        /// Gets the dest ID.
        /// </summary>
        /// <value>The dest ID.</value>
        public int DestID
        {
            get
            {
                return this.destID;
            }
        }

        /// <summary>
        /// Gets the DLC.
        /// </summary>
        /// <value>The DLC.</value>
        public int DLC
        {
            get
            {
                return this.byteData.Length;
            }
        }
        /// <summary>
        /// Gets the byte data.
        /// </summary>
        /// <value>The byte data.</value>
        public byte[] ByteData
        {
            get
            {
                return this.byteData;
            }
        }

        /// <summary>
        /// Gets the double data.
        /// </summary>
        /// <value>The double data.</value>
        public string DoubleData
        {
            get
            {
                if (this.byteData.Length ==8)
                {
                    byte[] bytes =  new byte[4];
                    try
                    {
                        return BitConverter.ToSingle(this.byteData, 4).ToString("0.####");
                    }
                    catch
                    {
                    }

                }
                return "";
            }
        }

        /// <summary>
        /// Gets or sets the cycle time.
        /// </summary>
        /// <value>The cycle time.</value>
        public int CycleTime
        {
            get
            {
                return this.cycTime;
            }
            set
            {
                this.cycTime = value;
            }
        }

        /// <summary>
        /// Gets or sets the comment.
        /// </summary>
        /// <value>The comment.</value>
        public string Comment
        {
            get
            {
                if (this.comment != null)
                {
                    return this.comment;
                }
                else
                {
                    return "";
                }
            }
            set
            {
                this.comment = value;
            }
        }

        /// <summary>
        /// Gets or sets the cycle count.
        /// </summary>
        /// <value>The cycle count.</value>
        public int CycleCount
        {
            get
            {
                return this.cycleCount;
            }
            set
            {
                this.cycleCount = value;
            }
        }

        /// <summary>
        /// Ups the cycle count.
        /// </summary>
        public void upCycleCount()
        {
            this.cycleCount ++;
        }
    }
}
