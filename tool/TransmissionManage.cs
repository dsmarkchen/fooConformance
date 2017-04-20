// ***********************************************************************
// Assembly         : ToolConnector
// Author           : SXu
// Created          : 04-09-2013
//
// Last Modified By : SXu
// Last Modified On : 04-10-2013
// ***********************************************************************
// <copyright file="TransmissionManage.cs" company="Evolution Engineering">
//     Copyright (c) Evolution Engineering.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Peak.Can.Basic;
using System.Configuration;
using CommonConfiguration;
using CommonConfiguration.CommonConfiguration;
using CommonConfiguration.CommonModule;
using CommonConfiguration.ConfigTools;
using System.Threading;

namespace ToolConnector
{
    /// <summary>
    /// Class TransmissionManager
    /// </summary>
    public class TransmissionManager
    {
        #region variables.
        /// <summary>
        /// The manager
        /// </summary>
        BaseConnectorManager manager;
        /// <summary>
        /// Enable software Power contorl 
        /// </summary>
        private bool _isSoftwarePowerRunning = true;

        /// <summary>
        /// Is software power control enable.
        /// </summary>
        public bool IsSoftwarePowerRunning
        {
            get
            {
                return this._isSoftwarePowerRunning;
            }
            set
            {
                this._isSoftwarePowerRunning = value;
            }
        }
        /// <summary>
        /// is to receive software power control response.
        /// </summary>
        private bool _isSoftwarePowerControlResponseEnable = false;
        /// <summary>
        /// Is softeware power control response enable
        /// </summary>
        public bool IsSoftwarePowerControlResponseEnable
        {
            get
            {
                return this._isSoftwarePowerControlResponseEnable;
            }
            set
            {
                this._isSoftwarePowerControlResponseEnable = value;
            }
        }


        /// <summary>
        /// Power ON all tool for bench test thread.
        /// </summary>
        private Thread PowerONTimeOutThread;

        /// <summary>
        ///  Is to disposed.
        /// </summary>
        private bool isDisposed = false;


        #endregion

        #region constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="TransmissionManager"/> class.
        /// </summary>
        public TransmissionManager()
            : this(false)
        {
        }

        /// <summary>
        ///  consider baurdrate in transmission manager
        /// </summary>
        /// <param name="baurdrate"></param>
        public TransmissionManager(int baurdrate)
            : this(false, baurdrate)
        {
        }
        /// <summary>
        /// create one instance with blue tooth connection.
        /// </summary>
        /// <param name="COMINDEX"></param>
        /// <returns></returns>
        public static TransmissionManager CreateBlueToothTransmissionManager(int comIndex)
        {
            TransmissionManager transmissionManager = new TransmissionManager(true);
           /* int comIndex = 4;
            try
            {
                comIndex = int.Parse(ConfigurationManager.AppSettings["COMINDEX"]);
            }
            catch { }*/
            if (comIndex <= 0)
            {
                comIndex = 4;
            }
            transmissionManager.manager = new BlueToothManager(comIndex);
            return transmissionManager;
       }

        /// <summary>
        /// create one instance with blue tooth connection.
        /// </summary>
        /// <param name="COMINDEX"></param>
        /// <returns></returns>
        public static TransmissionManager CreateCanbusTransmissionManager()
        {
            TransmissionManager transmissionManager = new TransmissionManager(false);
            return transmissionManager;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransmissionManager"/> class.
        /// </summary>
        private TransmissionManager(bool isBlueTooth)
        {
            if (isBlueTooth)
            {
               
            }
            else
            {
                manager = new CanbusManager();
            }
            this.CANBusPowerOnMSG = CanbusManager.createMsg( (int)NodeIdentityEnum.node_battery, ConstInfo.Command_battery_power, ConstInfo.SurfaceCanBusAddr, null, 0 );
            this.CANBusPowerOnMSG.LEN = 3;
            if ( ConstInfo.IsSoftwarePowerON )
            {
                this.CANBusPowerOnMSG.DATA[ 2 ] = 2; // set power on time out time to 10 minutes.
            }
            else
            {
                this.CANBusPowerOnMSG.DATA[ 2 ] = 3; // read power status.
            }

            this.SoftwarePowerControlStart();

           /* string powerFlag = ConfigurationManager.AppSettings["controlPoweron"];
            if (powerFlag == "true")
            {
                this.isControlPowerON = true;
            }
            else
            {
                this.isControlPowerON = false;
            }*/
        }



        private TransmissionManager(bool isBlueTooth, int baudrate)
        {
            if (isBlueTooth)
            {

            }
            else
            {
                manager = new CanbusManager(baudrate);
            }
        }

        public void reconnect(int rate)
        {
            if ( !this.isDisposed )
            {
                manager.reconnect( rate );
            }
        }

        public void Dispose()
        {
            this.isDisposed = true;
        }

        /// <summary>
        /// software power control 
        /// </summary>
        private void SoftwarePowerControlStart()
        {
            if ( this.PowerONTimeOutThread == null )
            {
                this.PowerONTimeOutThread = new Thread( PowerOnTiemout );
                this.PowerONTimeOutThread.IsBackground = true;
                this.PowerONTimeOutThread.Start();
            }
        }
        #endregion
       
        #region canbus connection
        /// <summary>
        /// Disconnect canbus.
        /// </summary>
        public void disconnectCanBus()
        {   
            manager.disconnect();
        }

        /// <summary>
        /// Gets a value indicating whether this instance is connect to can bus.
        /// </summary>
        /// <value><c>true</c> if this instance is connect to can bus; otherwise, <c>false</c>.</value>
        public bool IsConnectToCanBus
        {
            get
            {
                return this.manager.IsConnected();
            }
        }
        #endregion

        #region can bus message send
        /// <summary>
        /// Sends the PCAN data.
        /// </summary>
        /// <param name="msg">The MSG.</param>
        /// <returns>TPCANStatus.</returns>
        public TPCANStatus sendPCANData(ref TPCANMsg msg)
        {
            if ( this.isDisposed )
            {
                return TPCANStatus.PCAN_ERROR_BUSOFF;
            }
            else
            {
                return this.manager.sendPCANData( ref msg );
            }
        }
        #endregion

        #region can bus response
        /// <summary>
        /// Determines whether [is can busreponse continue] [the specified dest ID].
        /// </summary>
        /// <param name="destID">The dest ID.</param>
        /// <param name="source">The source.</param>
        /// <param name="commandID">The command ID.</param>
        /// <returns><c>true</c> if [is canbus response continue] [the specified dest ID]; otherwise, <c>false</c>.</returns>
        public bool isCanBusreponseContinue(int destID, int source, int commandID)
        {
            return isCanbusResponseContinue(destID, source, commandID, 1.5);
        }
        /// <summary>
        /// Determines whether [is canbus response continue] [the specified dest ID].
        /// </summary>
        /// <param name="destID">The dest ID.</param>
        /// <param name="source">The source.</param>
        /// <param name="responseID">The response ID.</param>
        /// <param name="waitTime">The wait time.</param>
        /// <returns><c>true</c> if [is canbus response continue] [the specified dest ID]; otherwise, <c>false</c>.</returns>
        public bool isCanbusResponseContinue(int destID, int source, int responseID, double waitTime)
        {
            if ( this.isDisposed )
            {
                return false;
            }
            else
            {
                return this.manager.isCanBusResponseContinue( destID, source, responseID, waitTime );
            }
        }
            
        /// <summary>
        /// get message response
        /// </summary>
        /// <param name="destID"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="waitTime"></param>
        /// <param name="receiveDataLen"></param>
        /// <returns></returns>
        public byte[] getCanBusResponData(int destID, int source, int command, double waitTime, ref int receiveDataLen)
        {
            if ( this.isDisposed )
            {
                return null;
            }
            else
            {
                return this.manager.getCanBusResponseData( destID, source, command, waitTime, ref receiveDataLen );
            }
        }


        /// <summary>
        /// get full message from can bus
        /// </summary>
        /// <param name="destID"></param>
        /// <param name="source"></param>
        /// <param name="command"></param>
        /// <param name="waitTime"></param>
        /// <param name="receiveDataLen"></param>
        /// <returns></returns>
        public CanbusMessage getCanBusResponDataEx(int destID, int source, int command, double waitTime, ref int receiveDataLen)
        {
            if ( this.isDisposed )
            {
                return null;
            }
            else
                return this.manager.getCanBusResponseData( destID, source, command, waitTime );
            {
            }
        }

        #endregion

        #region board information
        /// <summary>
        /// Receives the board information.
        /// </summary>
        /// <param name="destId">The dest id.</param>
        /// <param name="sourceId">The source id.</param>
        /// <param name="responseCommand">The response command.</param>
        /// <returns>List{System.Byte}.</returns>
        public BoardInformation receiveBoardInformation(int destId, int sourceId, int responseCommand)
        {
            List<byte> infos =  this.manager.receiveBoardInformation(destId, sourceId, responseCommand);
            if (infos == null)
            {
                return null;
            }
            else
            {
                return new BoardInformation(infos);
            }
        }
        #endregion

        #region get D&I serial number and firmware version
        /// <summary>
        /// get D&I serial numbe and firmware version. 
        /// </summary>
        /// <returns></returns>
        public DirectionModuleInfo GetDirectionModualInformation()
        {
            //*
            TPCANStatus stsResult;
            TPCANMsg requstDI = CanbusManager.createMsg( (int)NodeIdentityEnum.node_interface, ConstInfo.Command_interface_DIInformation, ConstInfo.SurfaceCanBusAddr, null, 0 );
            requstDI.LEN = 3;
            stsResult = this.sendPCANData( ref requstDI );

            List<byte> result = this.manager.receiveDirctionModuleInformation( ConstInfo.SurfaceCanBusAddr, (int)NodeIdentityEnum.node_interface, ConstInfo.Command_interface_DIInformation );
            
           //*/

            /*
            List<byte> result = new List<byte>();
            byte[] resultByte = new byte[]
            {
                0x41, 0x50, 0x53, 0x20,
                0x20, 0x45, 0x44, 0x30,
                0x31, 0x30, 0x35, 0x48,
                0x4E, 0x20, 0x56, 0x65,
                0x72, 0x3A, 0x34, 0x2E,
                0x31, 0x4E, 0x65, 0x74,
                0x75, 0x43, 0x45, 0x78,
                0x0D, 0x0D, 0x0A, 0x04
            };
            result.AddRange( resultByte );
            */
            if ( result != null && result.Count > 0 )
            {
                return new DirectionModuleInfo( result );
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region battery power control

      
        //when is power off , will not wait for response
        /// <summary>
        /// 
        /// </summary>
        /// <param name="isRealControlPowerOn"></param>
        /// <param name="isOn"></param>
        /// <returns></returns>
        public bool BatteryPowerControl(bool isControlPowerOnOff,bool isOn)
        {
            if ( !isControlPowerOnOff ) return true;
            if ( !isOn ) return true;//not really power off for now.
            byte[] res = this.BatteryPoweroNControl();
            if ( res == null )
            {
                return false;
            }
            else
            {
                return ( res.Length > 2 ) && ( res[ 2 ] == 1 );
            }
            
        }

        public byte[] BatteryPoweroNControl( )
        {           

            TPCANStatus stsResult;
            TPCANMsg pownOn = CanbusManager.createMsg( (int)NodeIdentityEnum.node_battery, ConstInfo.Command_battery_power, ConstInfo.SurfaceCanBusAddr, null, 0 );
            pownOn.LEN = 3;
            pownOn.DATA[ 2 ] = 1;
            //pownOn.DATA[ 2 ] = 0;//OFF            
            ConstInfo.SaveLogToFile( "POWER COMMAND TO BATTERY-- on" );
            stsResult = sendPCANData( ref pownOn );
            int dl = 0;
            byte[] res = this.getCanBusResponData( ConstInfo.SurfaceCanBusAddr, (int)NodeIdentityEnum.node_battery, ConstInfo.Command_battery_power, this.PowerProcessTime, ref dl );
            if ( dl <= 0 ) return null;
            byte[] result = new byte[ dl ];
            for ( int i = 0; i < res.Length && i < dl; i++ )
            {
                result[ i ] = res[ i ];
            }
            return result;            
        }

        /// <summary>
        /// time of batter to power on process
        /// </summary>
        private double _powerProcessTime = 3;
        /// <summary>
        /// time of batter to power on process
        /// </summary>
        private double PowerProcessTime
        {
            get
            {
                /*
                if (this._powerProcessTime < 0)
                {
                    string powerTime = ConfigurationManager.AppSettings["poweronProcessTime"];
                    if (powerTime != null)
                    {
                        try
                        {
                            this._powerProcessTime = double.Parse(powerTime);
                        }
                        catch { this._powerProcessTime = 1; }
                    }
                    else
                    {
                        this._powerProcessTime = 1;
                    }
                }
                return this._powerProcessTime;*/
                return this._powerProcessTime;
            }
            set
            {
                if (value <= 0)
                {
                    this._powerProcessTime = 3;
                }
                else
                {
                    this._powerProcessTime = value;
                }
            }
        }
        #endregion

        #region battery time setting
        /// <summary>
        /// operation fo battery time synchronization
        /// </summary>
        /// <param name="isSetTime"></param>
        /// <returns></returns>
        public byte[] BatteryTimeOperation(bool isSetTime)
        {
            //send date time
            TimeSpan span = DateTime.Now - ConstInfo.GetBaseTime();
            uint seconds = Convert.ToUInt32(span.TotalSeconds);
            byte[] timeByte = BitConverter.GetBytes(seconds);

            TPCANMsg time = CanbusManager.createMsg(NodeIdentityEnum.node_battery, ConstInfo.Command_batery_dateTime, ConstInfo.SurfaceCanBusAddr, timeByte, 0);
            if (isSetTime)
            {
                time.DATA[2] = 0; //set time
            }
            else
            {
                time.DATA[2] = 1;// get battery time
            }
            sendPCANData(ref time);
            int dataLen = 0;
            byte[] datas = this.getCanBusResponData(ConstInfo.SurfaceCanBusAddr, (int)NodeIdentityEnum.node_battery, ConstInfo.Command_batery_dateTime, 2, ref dataLen);
            if (dataLen != 8 || datas[2] != time.DATA[2]) return null;
            if (isSetTime)
            {
                uint ses = BitConverter.ToUInt32(datas, 4);
                if (ses != seconds) return null;
                return datas;
            }
            else
            {
                return datas;
            }

        }
        #endregion

        #region Batter Software Power ON Control

        /// <summary>
        /// CAN bus software power on message 
        /// </summary>
        private TPCANMsg CANBusPowerOnMSG;


        /// <summary>
        /// Power on time out thread.
        /// </summary>
        private void PowerOnTiemout()
        {
            if ( !ConstInfo.IsSoftwarePowerON )
            {
                return;
            }


            Dictionary<int, string> statusMap = new Dictionary<int, string>();
            string batteryTelemetryKey = "95-51";
            Dictionary<string, MSGModel> dic = ConstInfo.GetMessagesMap();
            if ( dic.ContainsKey( batteryTelemetryKey ) )
            {
                Dictionary<int, string> sm = dic[ batteryTelemetryKey ].FormatMap;
                if ( sm != null && sm.Count > 0 )
                {
                    foreach ( int key in sm.Keys )
                    {
                        statusMap.Add( key, sm[ key ] );
                    }
                }
            }

            if ( statusMap.Count <= 0 )
            {
                statusMap.Add( 1, "OFF-Start Up" );
                statusMap.Add( 2, "ON-Pressure" );
                statusMap.Add( 3, "ON-Temperature" );
                statusMap.Add( 4, "ON-Software Control" );
                statusMap.Add( 5, "ON-Safety_OFF" );
                statusMap.Add( 6, "OFF-Safety_ON" );
                statusMap.Add( 7, "OFF-Low Voltage" );
            }

            while (!this.isDisposed)
            {
                if (this._isSoftwarePowerRunning && this.manager != null )
                {
                    int resLen = 0;
                    byte[] data = null;
                    this.sendPCANData( ref this.CANBusPowerOnMSG );
                    if ( this._isSoftwarePowerControlResponseEnable && this.SoftwarePowerControlEvent != null )
                    {
                        data = this.getCanBusResponData( ConstInfo.SurfaceCanBusAddr, (int)NodeIdentityEnum.node_battery, ConstInfo.Command_battery_power, 0.25, ref resLen );                   
                        int telemetryStatus = -1;
                        if ( resLen >= 5 )
                        {
                            telemetryStatus = data[ 4 ];
                        }

                        int ONFFStatus = -1;
                        if ( resLen >= 4 )
                        {
                            ONFFStatus = data[ 3 ];
                        }


                        string telemetryStatusStr = "";
                        if ( statusMap.ContainsKey( telemetryStatus ) )
                        {
                            telemetryStatusStr = statusMap[ telemetryStatus ];
                        }
                        else
                        {
                            string ONFFStatusStr = "";
                            switch ( ONFFStatus )
                            {
                                case -1:
                                    ONFFStatusStr = "";
                                    break;
                                case 1:
                                    ONFFStatusStr = "Going to OFF";
                                    break;
                                case 2:
                                    ONFFStatusStr = "Power ON";
                                    break;
                                case 3:
                                    ONFFStatusStr = "Power OFF";
                                    break;
                                default:
                                    ONFFStatusStr = "";
                                    break;
                            }

                            telemetryStatusStr = ONFFStatusStr;
                        }

                        this.SoftwarePowerControlEvent( telemetryStatusStr );
                    }
                }
                for ( int i = 0; i < 10; i++ )
                {
                    if ( this.isDisposed ) return;
                    Thread.Sleep( 1000 );
                }

            }
        }

        /// <summary>
        /// send out softwaer power on command.
        /// </summary>
        public void SendSoftwarePowerONCommandWioutRes()
        {            
            if (ConstInfo.IsSoftwarePowerON && this.manager != null )
            {             
                this.sendPCANData( ref this.CANBusPowerOnMSG );
            }
        }

        /// <summary>
        /// Software power control delegate.
        /// </summary>
        /// <param name="powerStatus"></param>
        public delegate void SoftwarePowerContrlDelegate(String powerStatus);
        /// <summary>
        /// Software power control response event.
        /// </summary>
        public event SoftwarePowerContrlDelegate SoftwarePowerControlEvent;
        #endregion

    }
}
