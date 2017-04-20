// ***********************************************************************
// Assembly         : ToolConnector
// Author           : SXu
// Created          : 03-22-2013
//
// Last Modified By : SXu
// Last Modified On : 04-10-2013
// ***********************************************************************
// <copyright file="CanbusManager.cs" company="Evolution Engineering">
//     Copyright (c) Evolution Engineering.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Peak.Can.Basic;
using System.Threading;
using CommonConfiguration;
using CommonConfiguration.CommonConfiguration;

namespace ToolConnector
{
    /// <summary>
    /// Class CanbusManager
    /// </summary>
    public class CanbusManager : BaseConnectorManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CanbusManager"/> class.
        /// </summary>
        public CanbusManager()
        {
            this.init0();
        }

        /// <summary>
        /// constructor of canbus manager 
        ///   -- init canbus with given baudrate
        /// </summary>
        /// <param name="rate"></param>
        public CanbusManager(int rate)
        {
            m_Baudrate = (TPCANBaudrate) rate;
            this.init0();
        }

        /// <summary>
        /// property for baudrate
        /// </summary>
        public TPCANBaudrate Baudrate {
            get { return m_Baudrate; }
            set { m_Baudrate = value;}
        }

        /// <summary>
        /// The baud rate
        /// </summary>
        private TPCANBaudrate m_Baudrate = TPCANBaudrate.PCAN_BAUD_125K;
        /// <summary>
        /// The m_ handles array
        /// </summary>
        byte[] m_HandlesArray;
        /// <summary>
        /// The is running
        /// </summary>
        private bool isRunning = true;
        /// <summary>
        /// The _can channel
        /// </summary>
        private byte _canChannel;
        /// <summary>
        /// The is connect to canbus
        /// </summary>
        private bool isConnectToCanbus = false;

        /// <summary>
        /// The sleep time
        /// </summary>
        private int sleepTime = 0;// with no dealy

        /// <summary>
        /// Gets a value indicating whether this instance is connect to can bus.
        /// </summary>
        /// <value><c>true</c> if this instance is connect to can bus; otherwise, <c>false</c>.</value>
        public bool IsConnected()
        {
            return this.isConnectToCanbus;
        }

        /// <summary>
        /// Init0s this instance.
        /// </summary>
        private void init0()
        {
            m_HandlesArray = new byte[] 
            {
                PCANBasic.PCAN_USBBUS1,
                PCANBasic.PCAN_USBBUS2,
                PCANBasic.PCAN_USBBUS3,
                PCANBasic.PCAN_USBBUS4,
                PCANBasic.PCAN_USBBUS5,
                PCANBasic.PCAN_USBBUS6,
                PCANBasic.PCAN_USBBUS7,
                PCANBasic.PCAN_USBBUS8
            };

            this.initCanBusChannel();

            //Thread receiveThread = new Thread(receiveCanbusDataToCache);
            //receiveThread.Start();
        }

        #region canBus setting
        /// <summary>
        /// Inits the can bus channel.
        /// </summary>
        private void initCanBusChannel()
        {
            UInt32 iBuffer;
            TPCANStatus stsResult;

            // Clears the Channel combioBox and fill it again with 
            // the PCAN-Basic handles for no-Plug&Play hardware and
            // the detected Plug&Play hardware
            try
            {
                for (int i = 0; i < m_HandlesArray.Length; i++)
                {
                    // Checks for a Plug&Play Handle and, according with the return value, includes it
                    // into the list of available hardware channels.
                    stsResult = PCANBasic.GetValue(m_HandlesArray[i], TPCANParameter.PCAN_CHANNEL_CONDITION, out iBuffer, sizeof(UInt32));
                    if ((stsResult == TPCANStatus.PCAN_ERROR_OK) && (iBuffer == PCANBasic.PCAN_CHANNEL_AVAILABLE))
                    {                       
                        this._canChannel = m_HandlesArray[i];
                        break;
                    }
                }                
            }
            catch (DllNotFoundException)
            {
                //MessageBox.Show("Unable to find the library: PCANBasic.dll !", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Environment.Exit(-1);
            }
            catch (Exception ee)
            {
                Console.WriteLine(ee.Message);
            }

            if (this._canChannel > 0)
            {
                // Connects a selected PCAN-Basic channel
                stsResult = PCANBasic.Initialize(this._canChannel, m_Baudrate, TPCANType.PCAN_TYPE_ISA, 0, 0); //no use this line parameters
                if (stsResult != TPCANStatus.PCAN_ERROR_OK)
                {
                    isConnectToCanbus = false;
                }
                else
                {
                    isConnectToCanbus = true;
                }
            }
        }
        #endregion

        /// <summary>
        /// Receives the canbus data to cache.
        /// </summary>
        private void receiveCanbusDataToCache()
        {
            /*
            while (this.isRunning)
            {

            }
            /*

            TPCANMsg CANMsg;
            // While this mode is selected
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;
            DateTime time = DateTime.Now;
            DateTime now;
            TimeSpan span;

            int countFlag = 0;
            while (this.isRunning)
            {
                now = DateTime.Now;
                span = now - time;
                if (span.TotalSeconds > waitTime)// if waiting time is over 1.5s, we think it has no response
                {
                    return false;
                }
                // Waiting for Receive-Event
                //if (m_ReceiveEvent.WaitOne(50))
                {
                    do
                    {
                        stsResult = PCANBasic.Read(this._canChannel, out CANMsg, out CANTimeStamp);
                        if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                        {
                            if (CANMsg.ID != destID) continue;// it was not from specify node
                            if (CANMsg.LEN < 3) continue;
                            if (CANMsg.DATA[1] != source) continue;
                            if (CANMsg.DATA[0] == responseID)
                            {
                                return CANMsg.DATA[2] == 1;
                            }
                        }
                        countFlag++;
                        if (countFlag >= 1000)
                        {
                            span = now - time;
                            if (span.TotalSeconds > waitTime)// if waiting time is over 1.5s, we think it has no response
                            {
                                return false;
                            }
                            countFlag = 0;
                        }

                    } while (this.isRunning && (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
                }
            }
            return false;*/
        }


        /// <summary>
        /// Disconnect to can bus.
        /// </summary>
        public void disconnect()//object sender, EventArgs e)
        {
            if (this._canChannel > 0)
            {
                PCANBasic.Uninitialize(this._canChannel);
            }
            this.isRunning = false;
        }

        /// <summary>
        /// reconnect with giving speed
        /// </summary>
        /// <param name="x"></param>
        public void reconnect(int x = (int) TPCANBaudrate.PCAN_BAUD_125K)
        {
            disconnect();
            m_Baudrate = (TPCANBaudrate)x;
            reConnect();
        }
        /// <summary>
        /// re-connect to can bus.
        /// </summary>
        public void reConnect()
        {
            this.init0();
            this.isRunning = true;  
        } 

        /// <summary>
        /// Sends the PCAN data.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>TPCANStatus.</returns>
        public TPCANStatus sendPCANData(ref TPCANMsg msg)
        {
            TPCANStatus stsResult = TPCANStatus.PCAN_ERROR_BUSOFF;
            if (this.isRunning)
            {
                stsResult = PCANBasic.Write(this._canChannel, ref msg);
                if (sleepTime > 0)
                {
                    Thread.Sleep(sleepTime);
                }
            }
            return stsResult;
        }

        /// <summary>
        /// true: continue to send
        /// false: resend data
        /// </summary>
        /// <param name="destID">The dest ID.</param>
        /// <param name="source">The source.</param>
        /// <param name="commandID">The command ID.</param>
        /// <returns><c>true</c> if [is canbus response continue] [the specified dest ID]; otherwise, <c>false</c>.</returns>
        public bool isCanBusResponseContinue(int destID, int source,int commandID)
        {
            return isCanBusResponseContinue(destID,source,commandID, 1.5);
        }
        /// <summary>
        /// Determines whether [is canbus response continue] [the specified dest ID].
        /// </summary>
        /// <param name="destID">The dest ID.</param>
        /// <param name="source">The source.</param>
        /// <param name="responseID">The response ID.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <returns><c>true</c> if [is canbus response continue] [the specified dest ID]; otherwise, <c>false</c>.</returns>
        public bool isCanBusResponseContinue(int destID, int source,int responseID, double waitTime)
        {
            int dataLen = 0;
            byte[] data = this.getCanBusResponseData(destID,source,responseID,waitTime,ref dataLen);
            if(dataLen < 3) return false;
            return data[2] == 1;
        }
       /// <summary>
       /// get can bus response data
       /// </summary>
       /// <param name="destID"></param>
       /// <param name="source"></param>
       /// <param name="command"></param>
       /// <param name="waitTime"></param>
       /// <param name="receiveDataLen"></param>
       /// <returns></returns>
        public byte[] getCanBusResponseData( int destID, int source, int command, double waitTime, ref int receiveDataLen )
        {
            CanbusMessage message = getCanBusResponseData( destID, source, command, waitTime );
            if ( message != null )
            {
                receiveDataLen = message.DataLen;
                return message.ByteData;
            }
            else
            {
                receiveDataLen = 0;
                return null;
            }
            
        }
        /// <summary>
        /// lock object.
        /// </summary>
        private object lockObjForPowerOnThreadAndMessageReq = new object();

           /// <summary>
       /// get can bus response data
       /// </summary>
       /// <param name="destID"></param>
       /// <param name="source"></param>
       /// <param name="command"></param>
       /// <param name="waitTime"></param>
       /// <param name="receiveDataLen"></param>
       /// <returns></returns>
        public CanbusMessage getCanBusResponseData(int destID, int source, int command, double waitTime)
        {
            TPCANMsg CANMsg;
            // While this mode is selected
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;
            DateTime now = DateTime.Now;
            DateTime end = now.AddSeconds(waitTime);
            int flag_num = 0;
            lock ( this.lockObjForPowerOnThreadAndMessageReq )
            {
                while ( this.isRunning )
                {
                    now = DateTime.Now;
                    if ( now > end )// if waiting time is over 1.5s, we think it has no response
                    {
                        return null; //
                    }
                    // Waiting for Receive-Event
                    //if (m_ReceiveEvent.WaitOne(50))
                    {
                        do
                        {
                            stsResult = PCANBasic.Read( this._canChannel, out CANMsg, out CANTimeStamp );
                            if ( stsResult == TPCANStatus.PCAN_ERROR_OK )
                            {
                                if ( destID != -1 && CANMsg.ID != destID ) continue;// it was not from specify node
                                if ( CANMsg.LEN < 1 ) continue;

                                if ( source != -1 && CANMsg.DATA[ 1 ] != source ) continue;
                                if ( command != -1 && CANMsg.DATA[ 0 ] != command ) continue;

                                //receiveDataLen = CANMsg.LEN;
                                CanbusMessage message = new CanbusMessage( DateTime.Now, (int)CANMsg.ID, CANMsg.LEN, CANMsg.DATA );
                                return message;
                            }
                            flag_num++;
                            if ( flag_num >= 1000 )
                            {
                                flag_num = 0;
                                now = DateTime.Now;
                                if ( now > end )// if waiting over time, we think it has no response
                                {
                                    return null; //
                                }
                            }

                        } while ( this.isRunning && ( !Convert.ToBoolean( stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY ) ) );
                    }
                }
            }
            return null;
        }
             

        #region receive boardInfo from node
        /// <summary>
        /// Receives the board information.
        /// </summary>
        /// <param name="destId">The dest id.</param>
        /// <param name="sourceId">The source id.</param>
        /// <param name="responseCommand">The response command.</param>
        /// <returns>board info bytes</returns>
        public List<byte> receiveBoardInformation(int destId, int sourceId, int responseCommand)
        {
            /* test code
            List<byte> bytes = new List<byte>();
            for ( int i = 0; i < 24; i++ )
            {
                bytes.Add( (byte)i );
            }
            return bytes;
            //*/
            double waitTime = 0.5;
            TPCANMsg CANMsg;
            // While this mode is selected
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;
            DateTime now = DateTime.Now;
            DateTime end = now.AddSeconds(waitTime);

            List<byte> boardInfoBytes = new List<byte>();
            int receiveIndex = 1;
            bool isBreak = false;
            int cycle = 0;
            while (this.isRunning)
            {
                if (isBreak)
                {
                    break;
                }
                now = DateTime.Now;
                if (now > end)// if waiting time is over setting, we think it has no response
                {
                    break;
                }
                // Waiting for Receive-Event
                //if (m_ReceiveEvent.WaitOne(50))
                {
                    do
                    {
                        cycle++;
                        stsResult = PCANBasic.Read(this._canChannel, out CANMsg, out CANTimeStamp);
                        if (stsResult == TPCANStatus.PCAN_ERROR_OK)
                        {
                            if (CANMsg.LEN < 8) continue;
                            if (CANMsg.ID != destId) continue;
                            if (CANMsg.DATA[0] != responseCommand) continue;
                            if (CANMsg.DATA[1] != sourceId) continue;
                            if (CANMsg.DATA[3] == receiveIndex)
                            {
                                for (int j = 4; j <= 7; j++)
                                {
                                    boardInfoBytes.Add(CANMsg.DATA[j]);
                                }
                                receiveIndex++;
                                if (receiveIndex == 7)// receive 6 fill packages,.
                                {
                                    isBreak = true;
                                    break;
                                }
                            }
                            else
                            {
                                if (receiveIndex == 1)
                                {
                                    continue;
                                }
                                else
                                {
                                    isBreak = true;
                                    break;
                                }
                            }
                        }
                        if (cycle >= 1000)
                        {
                            now = DateTime.Now;
                            if (now > end)// if waiting time is over setting, we think it has no response
                            {
                                isBreak = true;
                                break;
                            }
                            cycle = 0;
                        }
                    } while (this.isRunning && (!Convert.ToBoolean(stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY)));
                }
            }
            return boardInfoBytes;
        }
        #endregion

        #region request D&I serial number and firmware version

        /// <summary>
        /// Receives the board information.
        /// </summary>
        /// <param name="destId">The dest id.</param>
        /// <param name="sourceId">The source id.</param>
        /// <param name="responseCommand">The response command.</param>
        /// <returns>board info bytes</returns>
        public List<byte> receiveDirctionModuleInformation( int canbusID, int sourceId, int responseCommand )
        {
            double waitTime = 0.6;
            TPCANMsg CANMsg;
            // While this mode is selected
            TPCANTimestamp CANTimeStamp;
            TPCANStatus stsResult;
            DateTime now = DateTime.Now;
            DateTime end = now.AddSeconds( waitTime );
            List<byte[]> receivedData = new List<byte[]>();
            bool isBreak = false;
            int cycle = 0;
            while ( this.isRunning )
            {
                if ( isBreak )
                {
                    break;
                }
                now = DateTime.Now;
                if ( now > end )// if waiting time is over setting, we think it has no response
                {
                    break;
                }
                // Waiting for Receive-Event
                //if (m_ReceiveEvent.WaitOne(50))
                {
                    do
                    {
                        cycle++;
                        stsResult = PCANBasic.Read( this._canChannel, out CANMsg, out CANTimeStamp );
                        if ( stsResult == TPCANStatus.PCAN_ERROR_OK )
                        {
                            if ( CANMsg.LEN <= 4 ) continue;
                            if ( CANMsg.ID != canbusID ) continue;
                            if ( CANMsg.DATA[ 0 ] != responseCommand ) continue;
                            if ( CANMsg.DATA[ 1 ] != sourceId ) continue;

                            receivedData.Add( (byte[])CANMsg.DATA.Clone() );

                        }
                        if ( cycle >= 1000 )
                        {
                            now = DateTime.Now;
                            if ( now > end )// if waiting time is over setting, we think it has no response
                            {
                                isBreak = true;
                                break;
                            }
                            cycle = 0;
                        }
                    } while ( this.isRunning && ( !Convert.ToBoolean( stsResult & TPCANStatus.PCAN_ERROR_QRCVEMPTY ) ) );
                }
            }
            /*
            receivedData.Clear();

            receivedData.Add( new byte[] { 0xC6, 0x95, 0x00, 0x00, 0x20, 0x00, 0x00, 0x00 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x01, 0x00, 0x41, 0x50, 0x53, 0x20 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x02, 0x00, 0x20, 0x45, 0x44, 0x30 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x03, 0x00, 0x31, 0x30, 0x35, 0x48 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x04, 0x00, 0x4E, 0x20, 0x56, 0x65 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x05, 0x00, 0x72, 0x3A, 0x34, 0x2E } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x06, 0x00, 0x31, 0x4E, 0x65, 0x74 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x07, 0x00, 0x75, 0x43, 0x45, 0x78 } );
            receivedData.Add( new byte[] { 0xC6, 0x95, 0x08, 0x00, 0x0D, 0x0D, 0x0A, 0x04 } ); 
            */

            List<byte> informationBytes = new List<byte>();
            if ( receivedData.Count > 0 )
            {
                int dataLen = 0;
                bool isFinish = false;
                for ( int i = 0; i < receivedData.Count; i++ )
                {
                    byte[] nextData = null;
                    for ( int j = 0; j < receivedData.Count; j++ )
                    {
                        byte[] onePackage = receivedData[ j ];
                        if ( ( onePackage[ 2 ] + (onePackage[ 3 ] << 8) ) == i )
                        {
                            nextData = onePackage;
                            //receivedData.RemoveAt( j );
                            break;
                        }
                    }
                    if ( nextData == null )
                    {
                        isFinish = true;
                    }
                    else
                    {
                        if ( i == 0 )
                        {
                            if ( nextData.Length < 6 )
                            {
                                isFinish = true;
                            }
                            else
                            {
                                dataLen = nextData[ 4 ] + (nextData[ 5 ] << 8);
                                if(dataLen <= 0) isFinish = true;
                            }
                        }
                        else
                        {
                            if ( nextData.Length < 4 )
                            {
                                isFinish = true;
                            }
                            else
                            {
                                int pDataLen = nextData.Length - 4;
                                int needNumber = dataLen - informationBytes.Count;
                                if ( pDataLen < needNumber )
                                {
                                    needNumber = pDataLen;
                                }
                                else
                                {
                                    isFinish = true;
                                }
                                for ( int di = 4; di < 4 + needNumber; di++ )
                                {
                                    informationBytes.Add(nextData[di]);
                                }
                                
                            }
                        }
                    }
                    if ( isFinish ) break;
                }
            }
            return informationBytes;
        }

        #endregion

        #region inner create canbus messge
        /// <summary>
        /// Creates the MSG.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="command">The command.</param>
        /// <param name="source">The source.</param>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <returns>TPCANMsg.</returns>
        public static TPCANMsg createMsg(NodeIdentityEnum dest, int command, int source, byte[] data, int start)
        {
            return createMsg((int)dest, command, source, data, start);
        }
        /// <summary>
        /// Creates the MSG.
        /// </summary>
        /// <param name="dest">The dest.</param>
        /// <param name="command">The command.</param>
        /// <param name="source">The source.</param>
        /// <param name="data">The data.</param>
        /// <param name="start">The start.</param>
        /// <returns>TPCANMsg.</returns>
        public static TPCANMsg createMsg(int dest, int command, int source, byte[] data, int start)
        {
            int len = 0;
            if (data != null)
            {
                len = data.Length - start;
            }
            if (len > 4) len = 4;

            TPCANMsg msg = new TPCANMsg();
            //msg.
            msg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;
            msg.ID = (uint)dest;
            msg.DATA = new byte[8];
            //msg.LEN = (byte)(len+2);
            msg.LEN = 8;

            msg.DATA[0] = (byte)command; //
            msg.DATA[1] = (byte)source;

            for (int i = 2; i < 8; i++)
            {
                msg.DATA[i] = 0XFF;
            }
            for (int i = 0; i < len; i++)
            {
                msg.DATA[4 + i] = data[start + i];
            }

            return msg;
        }
        #endregion end inner create canbus messge



        /// <summary>
        /// Create one tool rest delay  can msg.
        /// </summary>
        public static TPCANMsg CreateToolResetDelayCanMsg
        {
            get
            {
                TPCANMsg toolResetDelayCanMsg = CanbusManager.createMsg( NodeIdentityEnum.node_battery, ConstInfo.command_batteryResetDelay, ConstInfo.SurfaceCanBusAddr, null, 0 );
                toolResetDelayCanMsg.LEN = 8;
                toolResetDelayCanMsg.DATA[ 0 ] = ConstInfo.command_batteryResetDelay;
                toolResetDelayCanMsg.DATA[ 1 ] = ConstInfo.SurfaceCanBusAddr;
                toolResetDelayCanMsg.DATA[ 2 ] = 1;//enable tool reset delay.

                toolResetDelayCanMsg.DATA[ 4 ] = 20;// delay low 1 //default delay 20s
                toolResetDelayCanMsg.DATA[ 5 ] = 0;//delay low 2
                toolResetDelayCanMsg.DATA[ 6 ] = 0; // delay high 1
                toolResetDelayCanMsg.DATA[ 7 ] = 0;//delay high 2

                return toolResetDelayCanMsg;
            }
        }
    }
}
