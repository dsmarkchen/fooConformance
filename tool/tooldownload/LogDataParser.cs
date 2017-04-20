// ***********************************************************************
// Assembly         : ToolsLogDataDownload
// Author           : SXu
// Created          : 02-07-2013
//
// Last Modified By : SXu
// Last Modified On : 02-28-2013
// ***********************************************************************
// <copyright file="LogDataParser.cs" company="Evolution Engineering">
//     Copyright (c) Evolution Engineering.  All rights reserved.
// </copyright>
// <summary></summary>
// ***********************************************************************
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using CommonConfiguration.ConfigTools;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Configuration;
using CommonConfiguration;
using EVO;
using CommonConfiguration.CommonConfiguration;

namespace CommanSenderSpace
{
    /// <summary>
    /// Class LogDataParser
    /// </summary>
    public static class LogDataParser
    {

        /// <summary>
        /// Timestamp of flow on(start form version 0)
        /// </summary>
        private static byte timestamp_flowON = 5;
        /// <summary>
        /// Timestamp of flow off(start form version 3)
        /// </summary>
        private static byte timestamp_flowOFF = 6;
        /// <summary>
        /// Timestamp of baseline time (start form version 3)
        /// </summary>
        private static byte timestamp_baseTime = 7;
        /// <summary>
        /// Timestap of Rx Low raw data (start form version 3)
        /// </summary>
        private static byte timestamp_RxLowRaw = 8;
        /// <summary>
        /// Timestap of Rx high raw data (start form version 3)
        /// </summary>
        private static byte timestamp_RxHighRaw = 9;        
        /// <summary>
        /// The flag_config (start form version 0)
        /// </summary>
        private static byte flag_config = 10;

        /// <summary>
        /// Spare
        /// </summary>
        private static byte timestamp_neverUsed = 11;

        /// <summary>
        /// Timestamp of flow on(start form version 0)
        /// </summary>
        private static byte timestamp_PowerON = 12;
        /// <summary>
        /// Timestamp of flow off(start form version 3)
        /// </summary>
        private static byte timestamp_PowerOFF = 13;

        /// <summary>
        /// Timestamp of flow on(start form version 0)
        /// </summary>
        private static byte timestamp_RotatingON = 14;
        /// <summary>
        /// Timestamp of flow off(start form version 3)
        /// </summary>
        private static byte timestamp_RotatingOFF = 15;

        /// <summary>
        /// Flag of synchonization counter(start form version 3)
        /// </summary>
        private static byte flag_synCounter = 16;
      
        /// <summary>
        /// The spare bytes
        /// </summary>
        private static int spareBytes = 6;

        /// <summary>
        /// The double format
        /// </summary>
        private static string doubleFormat = "0.0000";
        /// <summary>
        /// The cell style_center
        /// </summary>
        private static ICellStyle cellStyle_center = null;
        /// <summary>
        /// The cell style_right
        /// </summary>
        private static ICellStyle cellStyle_right = null;


        private static bool isShowParserTestMsg = false;

        private static bool isValidToCorrectTime = true;

        private static bool isValidTimeToUPLine = true;

        private static MSGModel fadeLogInnerFlowSwith = null;
        private static MSGModel fadeLogInnerRotatingSwith = null;
        private static MSGModel fadeLogInnerPowerStatus = null;
        private static MSGModel fadeLogInnerSyncCounter = null;         

        private static string Rx_LowFreKey = "169-69";
        private static string Rx_HighFreKey = "169-73";

        static LogDataParser()
        {
            fadeLogInnerFlowSwith = new MSGModel();
            fadeLogInnerFlowSwith.NodeAddress = 200;
            fadeLogInnerFlowSwith.Command = 1;
            fadeLogInnerFlowSwith.DataName = "Flow Status";

            fadeLogInnerRotatingSwith = new MSGModel();
            fadeLogInnerRotatingSwith.NodeAddress = 200;
            fadeLogInnerRotatingSwith.Command = 2;
            fadeLogInnerRotatingSwith.DataName = "Rotating Status";

            fadeLogInnerPowerStatus = new MSGModel();
            fadeLogInnerPowerStatus.NodeAddress = 200;
            fadeLogInnerPowerStatus.Command = 3;
            fadeLogInnerPowerStatus.DataName = "Power Status";

            fadeLogInnerSyncCounter = new MSGModel();
            fadeLogInnerSyncCounter.NodeAddress = 200;
            fadeLogInnerSyncCounter.Command = 4;
            fadeLogInnerSyncCounter.DataName = "Sync Counter";

            EVOAppSettings evoSetting = new EVOAppSettings(LoggingDownloadForm.AppSettingFileName, false);
            isShowParserTestMsg = (evoSetting["isShowParserTestMsg"] == "true");
            isValidToCorrectTime = (evoSetting["isValidToCorrectTime"] == "true");
            isValidTimeToUPLine = (evoSetting["isValidTimeToUPLine"] == "true");

        }


        /// <summary>
        /// delegate for parse process value
        /// </summary>
        /// <param name="processBalue"></param>
        public delegate void ParseProcessBarDelegate( int processBalue );
        /// <summary>
        /// event to notify
        /// </summary>
        public static event ParseProcessBarDelegate ParseProcessBarDelegateEvent;


        #region test  code.

        private static Dictionary<string, int> test_totalTestMap = new Dictionary<string, int>();

        private static void test_resetTotalMessage()
        {
            test_totalTestMap.Clear();
        }
        private static void test_printTotalMessage()
        {
            if (isShowParserTestMsg)
            {
                foreach (string key in test_totalTestMap.Keys)
                {
                    Console.WriteLine(key + "  : " + test_totalTestMap[key]);
                }
            }
        }
        private static void test_upTotal(string key)
        {
            if (isShowParserTestMsg)
            {
                if (test_totalTestMap.ContainsKey(key))
                {
                    test_totalTestMap[key] = (test_totalTestMap[key] + 1);
                }
                else
                {
                    test_totalTestMap[key] = 1;
                }
            }
        }

        private static StreamWriter testFileStrem = null;
        private static void WriteDebugMessage(string message)
        {
            if ( !isShowParserTestMsg ) return;
            try
            {
                if ( testFileStrem == null )
                {
                    string folder = @"C:\TEMP";
                    if ( !Directory.Exists( folder ) )
                    {
                        Directory.CreateDirectory(folder);
                    }
                    testFileStrem = new StreamWriter( folder + @"\logParseTset" + DateTime.Now.ToString( "yyyy-MM-dd-HHmmss" ) + ".csv" );
                }
                if ( testFileStrem == null )
                {
                    return;
                }
                testFileStrem.WriteLine( message );
                testFileStrem.Flush();
            }
            catch
            {
            }            
        }
        private static void CloseTestDebugStream()
        {
            if ( !isShowParserTestMsg ) return;
            try
            {
                if ( testFileStrem != null )
                {
                    testFileStrem.Close();
                    testFileStrem = null;
                }
            }
            catch
            {
            }
        }
        #endregion
        
        //if iscsv ,the save csv file, else save xls file.
        /// <summary>
        /// Parses the log data.
        /// </summary>
        /// <param name="form">The form.</param>
        /// <param name="sourceStream">The source stream.</param>
        /// <param name="destStream">The dest stream.</param>
        /// <param name="dbModels">The db models.</param>
        /// <param name="isCSV">if set to <c>true</c> [is CSV].</param>
        public static bool parseLogData(Stream sourceStream, string destFilePath, Dictionary<string, MSGModel> dbModels, bool isCSV)
        {
            if (destFilePath == null || destFilePath == null) return false;
            if (sourceStream.Length < spareBytes) return false;

            bool parseDataFlag = true;
            Stream destStream = new FileStream(destFilePath, FileMode.Create);
            
            try
            {
                byte[] nodeByte = new byte[1];
                sourceStream.Read(nodeByte,0,1);
                int nodeId = nodeByte[0];
                //skip spare
                sourceStream.Position = spareBytes;
                DateTime baseDateTime = GetBaseTime();
                List<MSGModel> logModels = new List<MSGModel>();
                CSVRowData fullRowHeader = new CSVRowData(1); // row type = 1 //title row

                Dictionary<string, MSGModel> msgsDict = new Dictionary<string, MSGModel>();
                List<CSVRowData> rows_normal = new List<CSVRowData>();
                List<CSVRowData> rows_RxLowRaw = new List<CSVRowData>();
                List<CSVRowData> rows_RxHighRaw = new List<CSVRowData>();
                

                string fadeKeySynCounter = fadeLogInnerSyncCounter.key;
                msgsDict.Add( fadeKeySynCounter, fadeLogInnerSyncCounter );
                fullRowHeader.partValue.Add( fadeKeySynCounter, fadeKeySynCounter );
                logModels.Add( fadeLogInnerSyncCounter );
                
                string fadeKeyFlowSwitch = fadeLogInnerFlowSwith.key;
                msgsDict.Add( fadeKeyFlowSwitch, fadeLogInnerFlowSwith );
                fullRowHeader.partValue.Add( fadeKeyFlowSwitch, fadeKeyFlowSwitch );
                logModels.Add( fadeLogInnerFlowSwith );

                
                string fadeKeyRotating = fadeLogInnerRotatingSwith.key;
                msgsDict.Add( fadeKeyRotating, fadeLogInnerRotatingSwith );
                fullRowHeader.partValue.Add( fadeKeyRotating, fadeKeyRotating );
                logModels.Add( fadeLogInnerRotatingSwith );
                
                logModels.Add( fadeLogInnerPowerStatus );
                string fadeKeyPowerStatus = fadeLogInnerPowerStatus.key;
                msgsDict.Add( fadeKeyPowerStatus, fadeLogInnerPowerStatus );
                fullRowHeader.partValue.Add( fadeKeyPowerStatus, fadeKeyPowerStatus );

                CSVRowData nodeRow = new CSVRowData(2);
                nodeRow.otherTypeFistValue = ConstInfo.getNodeStrName(nodeId); // add node name
                rows_normal.Add(nodeRow);

                //CSVRowData lowHeader = new CSVRowData( 1 );
               // lowHeader.partValue.Add( Rx_LowFreKey, Rx_LowFreKey );

                //CSVRowData highHeader = new CSVRowData( 1 );
              //  highHeader.partValue.Add( Rx_HighFreKey, Rx_HighFreKey );

                // for rx low/frequency raw data.
                rows_RxLowRaw.Add( nodeRow );
               // rows_RxLowRaw.Add( lowHeader );

                rows_RxHighRaw.Add( nodeRow );
              //  rows_RxHighRaw.Add( highHeader );

                bool isNameRowIgnore = true;// to ignor the first name row

                rows_normal.Add(fullRowHeader);
                byte[] bytes = new byte[4];
                int processValue = 0;

                while (sourceStream.Read(bytes, 0, 4) == 4) // when the return value is not 4, data is parsed over.
                {
                    byte IDByte = bytes[0];
                    // just deal all config file .
                    if (IDByte == flag_config)// config file
                    {
                        //set process bar to show ..
                        if (processValue < 4)
                        {
                            processValue++;
                            setForCovertProcessBar(processValue);
                        }
                                               
                        sourceStream.Position -= 2;
                        sourceStream.Read(bytes, 0, 4);
                        uint se = BitConverter.ToUInt32(bytes, 0);// bytes[0] | bytes[1] << 8 | bytes[2] << 16 | ((long)bytes[3] << 24);
                                                
                        //MemoryStream configStream = new MemoryStream();
                        byte[] configBytes = new byte[646];
                        sourceStream.Read(configBytes, 0, configBytes.Length);                      

                        //skip spare
                        // in logging, there is no configspare place.20141112
                        int configSpare = 0;//1024 - 2 - 4 - 646; 
                        sourceStream.Position += configSpare;

                        //List<MSGModel> currentConfigModels = parseMessageModel(configStream);
                        FrameMessageSettingModel logModel = null;
                        try
                        {
                           logModel = ConfigFileSerialization.ParseMessageModel(configBytes);
                        }
                        catch (Exception e)
                        {
                            logModel = new FrameMessageSettingModel();
                            parseDataFlag = false;
                        }
                        List<MSGModel> currentConfigModels = logModel.Messages;
                        baseDateTime = GetBaseTime();
                        baseDateTime = baseDateTime.AddSeconds( se );
                                                
                        CSVRowData rd = new CSVRowData(1);
                        rd.partValue.Add( fadeKeySynCounter, fadeKeySynCounter );
                        rd.partValue.Add( fadeKeyFlowSwitch, fadeKeyFlowSwitch );
                        rd.partValue.Add( fadeKeyRotating, fadeKeyRotating );
                        rd.partValue.Add( fadeKeyPowerStatus, fadeKeyPowerStatus );
                        if (isNameRowIgnore) // the first row title use the full name row.   (the first row is node name row,  the seconde row is full name row)
                        {
                            isNameRowIgnore = false;
                        }
                        else
                        {
                            rows_normal.Add(rd);
                        }
                       

                        StringBuilder sb1 = new StringBuilder();
                        StringBuilder sb2 = new StringBuilder();
                        StringBuilder sb3 = new StringBuilder();

                        //logModels
                        foreach (MSGModel model in currentConfigModels)
                        {
                            string tpKep = model.NodeAddress + "-" + model.Command;
                            if ( tpKep == Rx_LowFreKey && model.MessageLogType == (int)MessageLogTypeEnum.Tx)//rx low frequency 
                            {
                                CSVRowData rowRow = new CSVRowData( 1 );
                                rowRow.partValue.Add( tpKep, tpKep );
                                rows_RxLowRaw.Add( rowRow );
                            }
                            else if(tpKep == Rx_HighFreKey && model.MessageLogType == (int)MessageLogTypeEnum.Tx ) // rx high frequency 
                            {
                                CSVRowData rowRow = new CSVRowData( 1 );
                                rowRow.partValue.Add( tpKep, tpKep );
                                rows_RxHighRaw.Add( rowRow );
                            }

                            if ( msgsDict.ContainsKey( tpKep ) )
                            {
                                // why do this, i think it doesn't no need , because we just use the data name  after we make this list.
                                for ( int i = 0; i < logModels.Count; i++ )
                                {
                                    MSGModel m = logModels[ i ];
                                    if ( m.NodeAddress == model.NodeAddress && m.Command == model.Command )
                                    {
                                        logModels[ i ] = model;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                logModels.Add( model );
                            }
                            msgsDict[ tpKep ] = model;
                            rd.partValue[ tpKep ] = tpKep;
                            fullRowHeader.partValue[ tpKep ] = tpKep;


                            if ( isShowParserTestMsg )
                            {
                                sb1.Append( tpKep + "," );
                                sb2.Append( ( model.MessageLogType == 3 ? ( model.IntervalTime + " - " + ( model.LogFlowStatusType == 0 ? "on" : ( model.LogFlowStatusType == 1 ? "off" : "both" ) ) ) : "" ) + "," );
                                sb3.Append( model.DataName + "," );
                            }
                        }
                        if ( isShowParserTestMsg )
                        {
                            WriteDebugMessage( "Log base time," + se + "," + baseDateTime.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                            Console.WriteLine( "--------------begin config file-------------@@@" );
                            test_printTotalMessage();
                            test_resetTotalMessage();
                            Console.WriteLine( "absolute second:" + se );
                            Console.WriteLine( "absolute time:" + baseDateTime.ToLongTimeString() );

                            Console.WriteLine( "--------------end config file-------------@@@@" );

                            WriteDebugMessage( sb1.ToString() );
                            WriteDebugMessage( sb2.ToString() );
                            WriteDebugMessage( sb3.ToString() );
                        }

                        ParseLogPartData( sourceStream, baseDateTime, rows_normal,rows_RxLowRaw,rows_RxHighRaw, msgsDict, logModel );
                    }

                    else//data
                    {
                      //other data.
                    }
                }

                //for test.
                CloseTestDebugStream();
                if (isShowParserTestMsg)
                {
                    Console.WriteLine("--------------begin last time-------------");
                    test_printTotalMessage();
                    test_resetTotalMessage();
                    Console.WriteLine("--------------end last time-------------");
                }


                if (isCSV)
                {
                    parseDataFlag = LogCSVSaveAction( destStream, logModels, rows_normal, processValue );
                    
                }
                else
                {
                    parseDataFlag = LogXSSSaveAction( destStream, logModels, rows_normal, processValue );
                }

                
                //save Rx raw data
                if ( rows_RxLowRaw.Count > 1 )                     
                {
                    int dotIndex = destFilePath.LastIndexOf(".");
                    string LowRawFilePath = destFilePath.Substring( 0, dotIndex ) + ( "_RxLowRaw" ) + destFilePath.Substring( dotIndex );
                    Stream lowRawDestStream = null;
                    try
                    {
                        lowRawDestStream = new FileStream( LowRawFilePath, FileMode.Create );
                        List<MSGModel> lowModeL = new List<MSGModel>();
                        if ( ConstInfo.GetMessagesMap().ContainsKey( Rx_LowFreKey ) )
                        {
                            lowModeL.Add( ConstInfo.GetMessagesMap()[ Rx_LowFreKey ] );
                            if ( isCSV )
                            {
                                parseDataFlag |= LogCSVSaveAction( lowRawDestStream, lowModeL, rows_RxLowRaw, processValue );

                            }
                            else
                            {
                                parseDataFlag |= LogXSSSaveAction( lowRawDestStream, lowModeL, rows_RxLowRaw, processValue );
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        try
                        {
                            if ( lowRawDestStream != null )
                            {
                                lowRawDestStream.Flush();
                                lowRawDestStream.Close();
                            }
                        }
                        catch { }

                    }
                }


                if ( rows_RxHighRaw.Count > 1 )
                {
                    int dotIndex = destFilePath.LastIndexOf( "." );
                    string highRawFilePath = destFilePath.Substring( 0, dotIndex ) + ( "_RxHighRaw" ) + destFilePath.Substring( dotIndex );
                    Stream highRawDestStream = null;
                    try
                    {
                        highRawDestStream = new FileStream( highRawFilePath, FileMode.Create );
                        List<MSGModel> highModeL = new List<MSGModel>();
                        if ( ConstInfo.GetMessagesMap().ContainsKey( Rx_HighFreKey ) )
                        {
                            highModeL.Add( ConstInfo.GetMessagesMap()[ Rx_HighFreKey ] );
                            if ( isCSV )
                            {
                                parseDataFlag |= LogCSVSaveAction( highRawDestStream, highModeL, rows_RxHighRaw, processValue );

                            }
                            else
                            {
                                parseDataFlag |= LogXSSSaveAction( highRawDestStream, highModeL, rows_RxHighRaw, processValue );
                            }
                        }
                    }
                    catch
                    {
                    }
                    finally
                    {
                        try
                        {
                            if ( highRawDestStream != null )
                            {
                                highRawDestStream.Flush();
                                highRawDestStream.Close();
                            }
                        }
                        catch { }
                    }
                }
                setForCovertProcessBar(100);
            }
            catch(Exception e)
            {
                parseDataFlag = false;
            }
            finally
            {
                try
                {
                    destStream.Close();
                }
                catch { }

                try
                {
                    sourceStream.Close();
                }
                catch { }                
            }
            return parseDataFlag;
        }
        

         /// <summary>
        /// Parse log file
        /// </summary>
        /// <param name="sourceStream"></param>
        private static void ParseLogPartData( Stream sourceStream, DateTime baseDateTime, List<CSVRowData> rows_normal,List<CSVRowData> rows_RxLowRaw,List<CSVRowData> rows_RxHighRaw, Dictionary<string, MSGModel> msgsDict, FrameMessageSettingModel logModel )
        {
            int logFormatVersion = logModel.IdleTxTime; // in logging binary format . The idleTxTime position is used for saving log format version which need firmware to fill.
            if ( logFormatVersion <= 2 )
            {
                ParseLogInV2( sourceStream, baseDateTime, rows_normal,rows_RxLowRaw,rows_RxHighRaw, msgsDict, logModel );
            }
            else if ( logFormatVersion >= 3 )
            {
                ParseLogInV3( sourceStream, baseDateTime, rows_normal,rows_RxLowRaw,rows_RxHighRaw, msgsDict, logModel );
            }
            else
            {
                // no way run to here.
            }
        }

        
        /// <summary>
        /// Parse log file  Version 3
        /// </summary>
        /// <param name="sourceStream"></param>
        private static void ParseLogInV3( Stream sourceStream, DateTime baseDateTime, List<CSVRowData> rows, List<CSVRowData> rows_RxLowRaw, List<CSVRowData> rows_RxHighRaw, Dictionary<string, MSGModel> msgsDict, FrameMessageSettingModel logModel )
        {
            int logFormatVersion = logModel.IdleTxTime;
            byte[] bytes = new byte[ 4 ];            
            CSVRowData rowData = new CSVRowData();
            rowData.rowTime = baseDateTime;
            DateTime latestRowDateTime = baseDateTime;
            DateTime lastTimeSychonization = baseDateTime;
            DateTime lastTimeRxLowRawSychonization = baseDateTime;
            DateTime lastTimeRxHighRawSychonization = baseDateTime;
            List<MSGModel> logMessageList = logModel.Messages;

            int rxHighRawRowIndex = 0;
            

            while ( sourceStream.Read( bytes, 0, 4 ) == 4 ) // when the return value is not 4, data is parsed over.
            {
                byte address = bytes[ 0 ];
                if( address == timestamp_baseTime )
                {
                    //update baseTime for data
                     int seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                     lastTimeSychonization = baseDateTime.AddSeconds( seconds );
                     if ( isShowParserTestMsg )
                     {
                         WriteDebugMessage( "time base Line df," + seconds + "," + lastTimeSychonization.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                     }
                }
                else if ( address == timestamp_RxLowRaw )
                {
                    // update Rx low data base time.
                     int seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                     lastTimeRxLowRawSychonization = baseDateTime.AddSeconds( seconds );
                     //rxHighRawRowIndex = 0;
                     //lastTimeRxHighRawSychonization = baseDateTime.AddSeconds( seconds );
                     if ( isShowParserTestMsg )
                     {
                         WriteDebugMessage( "Rx Low time base Line df," + seconds + "," + lastTimeRxLowRawSychonization.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                     }
                }
                else if ( address == timestamp_RxHighRaw )
                {
                    // update Rx high data base time.
                    int seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                    rxHighRawRowIndex = 0;
                    lastTimeRxHighRawSychonization = baseDateTime.AddSeconds( seconds );
                    if ( isShowParserTestMsg )
                    {
                        WriteDebugMessage( "Rx high time base Line df," + seconds + "," + lastTimeRxHighRawSychonization.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                    }
                }
                else if ( address == flag_config )// config file
                {
                    //finish parsing this part long data.
                    //reset read positon back to before config file flag.
                    sourceStream.Position -= 4;
                    break;
                }
                else//data
                {
                    string key = null;//key
                    object rv_str = null;//value
                    int seconds = 0; ;
                    DateTime timestamp = baseDateTime;
                    if ( address == timestamp_flowON || address == timestamp_flowOFF ) // flow on fade data.
                    {
                        key = fadeLogInnerFlowSwith.key;
                        rv_str = (address == timestamp_flowON)?"ON":"OFF";
                        seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                        timestamp = baseDateTime.AddSeconds(seconds);
                    }
                    else if( address == timestamp_RotatingON || address == timestamp_RotatingOFF ) // rotating on fade data
                    {
                        key = fadeLogInnerRotatingSwith.key;
                        rv_str = ( address == timestamp_RotatingON ) ? "ON" : "OFF";
                        seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                        timestamp = baseDateTime.AddSeconds( seconds );
                    }
                    else if ( address == timestamp_PowerON || address == timestamp_PowerOFF ) // rotating on fade data
                    {
                        key = fadeLogInnerPowerStatus.key;
                        rv_str = ( address == timestamp_PowerON ) ? "ON" : "OFF";
                        seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;
                        timestamp = baseDateTime.AddSeconds( seconds );
                    }
                    else if ( address == flag_synCounter )
                    {
                        key = fadeLogInnerSyncCounter.key;
                        seconds = bytes[ 1 ];
                        timestamp = lastTimeSychonization.AddSeconds( seconds );
                        rv_str = (bytes[ 2 ] | bytes[ 3 ] << 8).ToString();
                    }
                    else
                    {
                        int index = address - 17;
                        if ( index >= 0 && index < logMessageList.Count )
                        {
                            MSGModel model = logMessageList[ index ];
                            key = model.key;
                            rv_str = ParseDataValue( logFormatVersion, bytes, model, sourceStream );
                            seconds = bytes[ 1 ];

                            if ( key == Rx_LowFreKey && model.MessageLogType == (int)MessageLogTypeEnum.Tx ||//rx low frequency 
                                key == Rx_HighFreKey && model.MessageLogType == (int)MessageLogTypeEnum.Tx ) // rx high frequency 
                            {
                                //timestamp = lastTimeRxLowRawSychonization.AddSeconds( seconds );
                                CSVRowData rawRow = new CSVRowData();
                                rawRow.isWriteTimeFlag = false;
                                rawRow.partValue.Add( key, rv_str );

                                if ( key == Rx_LowFreKey ) //low frequency.
                                {
                                    if ( lastTimeRxLowRawSychonization.Millisecond == 0 )
                                    {
                                        rawRow.isWriteTimeFlag = true;
                                    }
                                    lastTimeRxLowRawSychonization = lastTimeRxLowRawSychonization.AddMilliseconds( 50 );
                                    rawRow.rowTime = lastTimeRxLowRawSychonization;
                                    rows_RxLowRaw.Add( rawRow );
                                }
                                else
                                {
                                    rxHighRawRowIndex++;
                                    if ( rxHighRawRowIndex % 2 == 0 )
                                    {
                                        lastTimeRxHighRawSychonization = lastTimeRxHighRawSychonization.AddMilliseconds( 1 );
                                    }
                                    else if ( lastTimeRxHighRawSychonization.Millisecond == 0 )
                                    {
                                        rawRow.isWriteTimeFlag = true;
                                    }
                                    //Console.WriteLine( lastTimeRxHighRawSychonization.ToString("yyyy-MM-dd HH:mm:ss ms"));
                                    rawRow.rowTime = lastTimeRxHighRawSychonization;
                                    rows_RxHighRaw.Add( rawRow );
                                }
                                key = null;
                            }
                            else
                            {
                                timestamp = lastTimeSychonization.AddSeconds( seconds );
                            }
                        }
                    }

                    if ( key != null )
                    {
                        if ( rowData.rowTime != timestamp || rowData.partValue.ContainsKey(key))
                        {
                            rows.Add( rowData );
                            rowData = new CSVRowData();                            
                        }
                        rowData.partValue.Add( key, rv_str );
                        rowData.rowTime = timestamp;
                        rowData.isWriteTimeFlag = true;


                        if ( isShowParserTestMsg )
                        {
                            WriteDebugMessage( key + "," + rv_str + ",  " + seconds + "," + timestamp.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                        }
                    }
                }
            }
            //add the last rowData
            rows.Add( rowData );
        }

        /// <summary>
        /// Parse log file  Version 2  or befor Version 2.
        /// </summary>
        /// <param name="sourceStream"></param>
        private static void ParseLogInV2( Stream sourceStream, DateTime baseDateTime, List<CSVRowData> rows, List<CSVRowData> rows_RxLowRaw, List<CSVRowData> rows_RxHighRaw, Dictionary<string, MSGModel> msgsDict, FrameMessageSettingModel logModel )
        {
            int logFormatVersion = logModel.IdleTxTime;
            byte[] bytes = new byte[ 4 ];

            //in firmware there 3 seconds delay // add 20141112  by Alex Men.
            baseDateTime = baseDateTime.AddSeconds(3);

            //get all interval time log.
            IntervalTimeList intervals = new IntervalTimeList();
            List<MSGModel> currentConfigModels = logModel.Messages;
            intervals.setIntervalTimeModel( currentConfigModels );
            intervals.resetRealTime( baseDateTime, 0 );//set real time..

          

            bool isHasFirstTime = false;
            CSVRowData rowData = new CSVRowData();
            rowData.rowTime = baseDateTime;
            DateTime latestRowDateTime = DateTime.Now;

            while ( sourceStream.Read( bytes, 0, 4 ) == 4 ) // when the return value is not 4, data is parsed over.
            {
                byte address = bytes[ 0 ];
                if ( address == timestamp_flowON )// date time.
                {
                    int seconds = bytes[ 1 ] | bytes[ 2 ] << 8 | bytes[ 3 ] << 16;

                    if ( seconds <= 3 )
                    {
                        continue;
                    }
                    else if ( seconds > 3 )
                    {
                        //  data have been write on 0 second. 
                        intervals.FinishFirstDataTreat();
                    }
                    //in firmware there 3 seconds delay // add 20141112  by Alex Men.
                    seconds -= 3;

                    DateTime t = baseDateTime.AddSeconds( seconds );

                    intervals.resetRealTime( t, seconds );
                    isHasFirstTime = false;

                    rows.Add( rowData );
                    rowData = new CSVRowData();
                    rowData.rowTime = t;

                    if ( isShowParserTestMsg )
                    {
                        WriteDebugMessage( "Date df," + seconds + "," + t.ToString( "yyyy-MM-dd HH:mm:ss" ) );
                        Console.WriteLine( "--------------begin relation time-------------" );
                        test_printTotalMessage();
                        test_resetTotalMessage();
                        Console.WriteLine( "relationTime:" + seconds );
                        Console.WriteLine( "absolute time:" + t.ToLongTimeString() );
                        Console.WriteLine( "--------------end relation time-------------" );
                    }                  
                }
                else if ( address == flag_config )// config file
                {
                    //finish parsing this part long data.
                    //reset read positon back to before config file flag.
                    sourceStream.Position -= 4;
                    break;
                }
                else//data
                {
                    string key = address + "-" + bytes[ 1 ];
                    if ( !msgsDict.ContainsKey( key ) )
                    {
                        continue; // not  in config file , it's not a correct data
                    }
                    MSGModel model = msgsDict[ key ];
                    object rv_str = ParseDataValue( logFormatVersion, bytes, model, sourceStream );
                    if ( rv_str == null ) break; // there is nothing left .
                    InterValTimeObj tItem = intervals.getIntervalObj( key );
                    if ( tItem == null ) //it's not a interval time log message
                    {
                        if ( rowData.partValue.ContainsKey( key ) )
                        {
                            rows.Add( rowData );
                            rowData = new CSVRowData( rowData.hasIntervalTimeItem, false, rowData.rowTime );
                        }
                    }
                    else
                    {
                        test_upTotal( tItem.key );//to record data number.
                        tItem.timeUP( isValidToCorrectTime );
                        if ( isValidTimeToUPLine )
                        {
                            if ( isHasFirstTime )
                            {
                                while ( tItem.DataTime < latestRowDateTime )
                                {
                                    tItem.timeUP( isValidToCorrectTime );
                                }
                            }
                            else
                            {
                                isHasFirstTime = true;
                            }
                            latestRowDateTime = tItem.DataTime;
                        }

                        if ( rowData.rowTime != tItem.DataTime )
                        {
                            rows.Add( rowData );
                            rowData = new CSVRowData();
                        }
                        else
                        {
                            if ( rowData.partValue.ContainsKey( key ) )
                            {
                                rows.Add( rowData );
                                rowData = new CSVRowData( rowData.hasIntervalTimeItem, false, rowData.rowTime );
                            }
                        }

                        tItem.UpRepeatCount();
                        rowData.rowTime = tItem.DataTime;
                        rowData.hasIntervalTimeItem = true;
                    }
                    rowData.partValue.Add( key, rv_str );


                    if ( isShowParserTestMsg )
                    {
                        WriteDebugMessage( key + "," + model.DataName + "," + ( tItem != null ? tItem.repeatedTimes.ToString() : "" ) );
                    }                    
                }
            }
            //add the last rowData
            rows.Add( rowData );
        }

        /// <summary>
        /// Parse data value.
        /// </summary>
        /// <param name="logFormatVersion"></param>
        /// <param name="bytes"></param>
        /// <param name="model"></param>
        /// <param name="sourceStream"></param>
        /// <returns></returns>
        private static object ParseDataValue(int logFormatVersion,byte[] bytes,MSGModel model, Stream sourceStream )
        {
            int bits = model.RealBits;

            int v = 0; // bytes[2] | (bytes[3] << 8);
            if ( logFormatVersion >= 2 )
            {
                if ( bits < 16 ) bits = 16;//
            }
            if ( bits > 16 ) // the bit num is bigger than 16 , so use another 4 bytes to save high part.
            {
                v = bytes[ 2 ] | ( bytes[ 3 ] << 8 );
                int len = sourceStream.Read( bytes, 0, 4 );
                if ( len == 4 )
                {
                    v += bytes[ 2 ] << 16 | ( bytes[ 3 ] << 24 );
                }
                else
                {
                    return null;

                }
            }
            else
            {
                //v = BitConverter.ToInt16(bytes, 2);
                v = BitConverter.ToUInt16( bytes, 2 );
            }
            double rv = v * ( model.DataMax - model.DataMin ) / ( Math.Pow( 2, bits ) - 1 ) + model.DataMin;
            return rv;
        }

        /// <summary>
        /// save pared rows to xss file.
        /// </summary>
        /// <param name="destStream"></param>
        /// <param name="logModels"></param>
        /// <param name="rows"></param>
        /// <param name="processValue"></param>
        /// <returns></returns>
        private static bool LogXSSSaveAction( Stream destStream, List<MSGModel> logModels, List<CSVRowData> rows, int processValue )
        {
            bool isSaved = false;
            try
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet sheet = workbook.CreateSheet( "LogData" );

                cellStyle_center = workbook.CreateCellStyle();
                cellStyle_center.Alignment = HorizontalAlignment.CENTER;

                cellStyle_right = workbook.CreateCellStyle();
                cellStyle_right.Alignment = HorizontalAlignment.RIGHT;
                cellStyle_right.DataFormat = workbook.CreateDataFormat().GetFormat( doubleFormat );

                int nextRowIndex = 0;
                //add title header.
                IRow row;// = createRowFormSheet(sheet, nextRowIndex);
                // fillXLSRow(row, logModels, fullRowHeader);

                /*
                ICellStyle ds =  workbook.CreateCellStyle();
                ds.Alignment = HorizontalAlignment.RIGHT;
                for (int i = 0; i <= logModels.Count; i++)
                {
                    //sheet.SetDefaultColumnStyle(i, ds);
                }
                //*/

                //freeze pane  ( canceled ) 20130905- by Alex
                //sheet.CreateFreezePane(logModels.Count + 1, 1);

                // nextRowIndex++;
                //add row ..
                for ( int i = 0; i < rows.Count; i++ )
                {
                    int pv = i * 100 / rows.Count;
                    if ( pv > processValue )
                    {
                        processValue = pv;
                        setForCovertProcessBar( processValue );
                    }
                    CSVRowData rd = rows[ i ];
                    /* if (!getFirstTitle)
                     {
                         if (rd.RowType == 1)
                         {
                             getFirstTitle = true;
                             continue;
                         }
                     }*/
                    if ( rd.RowType == 0 && rd.partValue.Count <= 0 ) continue;
                    row = createRowFormSheet( sheet, nextRowIndex );//add a new method to poi , this method with no max row number check.
                    nextRowIndex++;
                    fillXLSRow( row, logModels, rd );
                }
                workbook.Write( destStream );
                isSaved = true;
            }
            catch ( Exception e )
            {
                isSaved = false;
            }
            finally
            {

            }
            return isSaved;
        }
        /// <summary>
        /// Save pared rows to CVS file.
        /// </summary>
        /// <param name="destStream"></param>
        /// <param name="logModels"></param>
        /// <param name="rows"></param>
        /// <param name="processValue"></param>
        /// <returns></returns>
        private static bool LogCSVSaveAction( Stream destStream, List<MSGModel> logModels, List<CSVRowData> rows, int processValue )
        {
            bool isSaved = false;
            StreamWriter fsWriter = null;
            try
            {
                fsWriter = new StreamWriter( destStream );
                //write total header..
                // writeCSVRowData(fsWriter, logModels, fullRowHeader);

                //write row Data.                
                for ( int i = 0; i < rows.Count; i++ )
                {
                    int pv = i * 100 / rows.Count;
                    if ( pv > processValue )
                    {
                        processValue = pv;
                        setForCovertProcessBar( processValue );
                    }
                    CSVRowData rd = rows[ i ];                    
                    writeCSVRowData( fsWriter, logModels, rd );
                }
                isSaved = true;

            }
            catch ( Exception e )
            {
                isSaved = false;
            }
            finally
            {
                try
                {
                    if ( fsWriter != null )
                    {
                        fsWriter.Flush();
                        fsWriter.Close();
                    }
                }
                catch { }
            }
            return isSaved;
        }

        private static void setForCovertProcessBar(int value)
        {
            if (ParseProcessBarDelegateEvent != null)
            {
                ParseProcessBarDelegateEvent(value);
            }
        }

        /// <summary>
        /// Pulser Timer Movemnents Dictionary
        /// </summary>
        private static Dictionary<int, string> PulserTimerMovemnents;

        /// <summary>
        /// Creates the row form sheet.
        /// </summary>
        /// <param name="sheet">The sheet.</param>
        /// <param name="rowIndex">Index of the row.</param>
        /// <returns>IRow.</returns>
        private static IRow createRowFormSheet(ISheet sheet, int rowIndex)
        {
            //sheet.CreateRow(nextRowIndex);
            return sheet.CreateLastRow(rowIndex);//add a new method to poi by alex , this method with no max row number check.
        }
        /*
        //update columns's dataName , this dataName is show in every date block begin as title.
        /// <summary>
        /// Updates the name of the model data.
        /// </summary>
        /// <param name="columns">The columns.</param>
        /// <param name="DBModels">The DB models.</param>
        private static void updateModelDataName(List<MSGModel> columns, Dictionary<string, MSGModel> DBModels)
        {
            if (columns == null || DBModels == null || DBModels.Count <= 0)
            {
                return;
            }
            foreach (MSGModel model in columns)
            {
                string key = model.NodeAddress +"-"+model.Command;

                if (DBModels.ContainsKey(key))
                {
                    model.DataName = DBModels[key].DataName;

                }
                else
                {
                    if (model.DataName == null || model.DataName.Length <= 0)
                    {
                        model.DataName = key;
                    }
                }
            }               
        }*/
        private static DateTime GetBaseTime()
        {
            return new DateTime(2000, 1, 1);
        }

        /// <summary>
        /// Fills the XLS row.
        /// </summary>
        /// <param name="xlsRow">The XLS row.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rowData">The row data.</param>
        private static void fillXLSRow(IRow xlsRow, List<MSGModel> columns, CSVRowData rowData)
        {
            Dictionary<string, object> partValue = rowData.partValue;
            ICell iCell = null;
            if (rowData.RowType == 0 && partValue.Count <= 0)
            {
                return;
            }
            int nexColIndex = 0;
            if (rowData.RowType ==1)
            {
                xlsRow.CreateCell(nexColIndex).SetCellValue("Date");
            }
            else if (rowData.RowType == 2)
            {
                xlsRow.CreateCell(nexColIndex).SetCellValue(rowData.otherTypeFistValue == null ? "" : rowData.otherTypeFistValue);
            }
            else
            {
                if (rowData.isWriteTimeFlag)
                {
                    DateTime time = rowData.rowTime;
                    if (time != null)
                    {
                        iCell = xlsRow.CreateCell(nexColIndex);
                        iCell.SetCellValue(time.ToString(" yyyy-MM-dd HH:mm:ss"));
                    }
                }
            }
            nexColIndex++;

            for (; nexColIndex <= columns.Count; nexColIndex++)
            {
                MSGModel model = columns[nexColIndex - 1];
                string key = model.NodeAddress + "-" + model.Command;
                if (partValue.ContainsKey(key))
                {
                    if (rowData.RowType == 1)
                    {
                        iCell = xlsRow.CreateCell(nexColIndex);
                        iCell.CellStyle = cellStyle_center;
                        if (model.DataName != null && model.DataName.Length > 0)
                        {
                            iCell.SetCellValue(model.DataName);
                        }
                        else
                        {
                            iCell.SetCellValue(key);
                        }
                    }
                    else if (rowData.RowType == 2)
                    {
                        iCell = xlsRow.CreateCell(nexColIndex);
                        iCell.CellStyle = cellStyle_center;
                        iCell.SetCellValue(rowData.otherTypeFistValue == null ? "" : rowData.otherTypeFistValue);
                    }
                    else
                    {
                        iCell = xlsRow.CreateCell(nexColIndex);
                        iCell.CellStyle = cellStyle_right;
                        object vo = partValue[key];
                        if (typeof(double) == vo.GetType())
                        {
                            iCell.SetCellValue((double)vo);
                        }
                        else if (typeof(int) == vo.GetType())
                        {
                            iCell.SetCellValue((int)vo);
                        }
                        else if (typeof(float) == vo.GetType())
                        {
                            iCell.SetCellValue((float)vo);
                        }
                        else
                        {
                            iCell.SetCellValue(vo.ToString());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Writes the CSV row data.
        /// </summary>
        /// <param name="writer">The writer.</param>
        /// <param name="columns">The columns.</param>
        /// <param name="rowData">The row data.</param>
        private static void writeCSVRowData(StreamWriter writer, List<MSGModel> columns, CSVRowData rowData)
        {
            Dictionary<string, object> partValue = rowData.partValue;
            if (rowData.RowType == 0 && partValue.Count <= 0)
            {
                return;
            }
            StringBuilder builder = new StringBuilder();
            if (rowData.RowType == 1)
            {
                builder.Append("Date");
            }
            else if (rowData.RowType == 2)
            {
                builder.Append(rowData.otherTypeFistValue == null ? "" : rowData.otherTypeFistValue);
            }
            else
            {
                if (rowData.isWriteTimeFlag)
                {
                    DateTime time = rowData.rowTime;
                    if (time != null)
                    {
                        builder.Append(time.ToString(" yyyy-MM-dd HH:mm:ss"));
                    }
                }
            }
            builder.Append(",");
            for (int i = 0; i < columns.Count; i++)
            {
                MSGModel model = columns[i];
                string key = model.NodeAddress + "-" + model.Command;
                if (partValue.ContainsKey(key))
                {
                    if (rowData.RowType == 1)
                    {
                        if (model.DataName != null && model.DataName.Length > 0)
                        {
                            builder.Append(model.DataName.Replace("\r", "").Replace("\n", ""));
                        }
                        else
                        {
                            builder.Append(key);
                        }
                    }
                    else
                    {
                        object vo = partValue[key];
                        if (typeof(double) == vo.GetType())
                        {
                            builder.Append(((double)vo).ToString(doubleFormat));
                        }
                        else
                        {
                            builder.Append(vo.ToString());
                        }
                    }
                }
                builder.Append(",");
            }

            writer.WriteLine(builder.ToString());
        }
        /*
        /// <summary>
        /// Parses the message model.
        /// </summary>
        /// <param name="stream">The stream.</param>
        /// <returns>List{MSGModel}.</returns>
        private static List<MSGModel> parseMessageModel(Stream stream)
        {
            stream.Position = 0;
            List<MSGModel> logModels = new List<MSGModel>();            
            byte[] messageHeader = new byte[6];
            if (stream.Read(messageHeader, 0, 6) != 6) return logModels;

            int messageCount = messageHeader[5];// message count number

            int perMessageLen = 20;
            byte[] msgBytes = new byte[perMessageLen];
            for (int i = 0; i < messageCount; i++)
            {
                MSGModel model = new MSGModel();
                stream.Read(msgBytes, 0, perMessageLen);

                model.NodeAddress = msgBytes[0];
                model.Command = msgBytes[1];
                model.IntervalTime = msgBytes[2];
                model.intervalTimeLogType = msgBytes[3];

                //model.DataMax = msgBytes[4] | msgBytes[5] << 8 | msgBytes[6] << 16 | msgBytes[7]<<24;
                //model.DataMin = msgBytes[8] | msgBytes[9] << 8 | msgBytes[10] << 16 | msgBytes[11] << 24;
                model.DataMax = BitConverter.ToSingle(msgBytes, 4);
                model.DataMin = BitConverter.ToSingle(msgBytes, 8);
                model.DataMax += model.DataMin;

                model.MessageLogType = msgBytes[12];
                model.RealBits = msgBytes[13];
                model.AcqTime = msgBytes[14] | msgBytes[15] << 8;
                
                // with 4 spare bytes

                logModels.Add(model);
            }
            return logModels;

        }*/

    }




    /// <summary>
    /// Class IntervalTimeList
    /// </summary>
    class IntervalTimeList
    {
        //inner container
        /// <summary>
        /// The interval times
        /// </summary>
        private List<InterValTimeObj> intervalTimes = new List<InterValTimeObj>();

        /// <summary>
        /// Sets the interval time model.
        /// </summary>
        /// <param name="logModels">The log models.</param>
        public void setIntervalTimeModel(List<MSGModel> logModels)
        {
            this.intervalTimes.Clear();
            foreach (MSGModel model in logModels)
            {
                if (model.MessageLogType == 3 && model.IntervalTime > 0) // interval time log
                {
                    InterValTimeObj obj = new InterValTimeObj();
                    obj.model = model;
                    obj.updateKey();
                    this.intervalTimes.Add(obj);
                }
            }
        }
        /// <summary>
        /// Delegate Comparison
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="x">The x.</param>
        /// <param name="y">The y.</param>
        /// <returns>System.Int32.</returns>
        public delegate int Comparison<T>(T x, T y);

        /// <summary>
        /// record if the tiem is corrected
        /// </summary>
        //public bool isCorrectedTime = false;
        /// <summary>
        /// relativeTime
        /// </summary>
        private int relativeTime = 0;

        private DateTime dataTime = DateTime.Now;

        public DateTime DataTime1
        {
            get
            {
                return this.dataTime;
            }
        }

        public void FinishFirstDataTreat()
        {
            foreach ( InterValTimeObj item in intervalTimes )
            {
                item.FinishFirstDataTreat();
            }
        }

        /// <summary>
        /// Resets the real time.
        /// </summary>
        /// <param name="time">The time.</param>
        public void resetRealTime(DateTime time,int timeSpanInSec)
        {
            this.relativeTime = timeSpanInSec;

            this.dataTime = time;
            foreach (InterValTimeObj iobj in intervalTimes)
            {
                iobj.DataTime = time;
                iobj.dateTimeSpanInSecond = timeSpanInSec;
                iobj.isCorrectedTime = false;
            }
        }
     
        /// <summary>
        /// Gets the interval obj.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>InterValTimeObj.</returns>
        public InterValTimeObj getIntervalObj(string key)
        {
            foreach (InterValTimeObj ito in this.intervalTimes)
            {
                if (ito.key == key)
                {
                    return ito;
                }
            }
            return null;
        }

    }

    /// <summary>
    /// Class InterValTimeObj
    /// </summary>
    class InterValTimeObj
    {
        /// <summary>
        /// The key
        /// </summary>
        public string key;
        /// <summary>
        /// The model
        /// </summary>
        public MSGModel model;       
        /// <summary>
        /// The data time
        /// </summary>
        private DateTime dataTime;//data's time

        /// <summary>
        /// passed date time span in second
        /// </summary>
        public int dateTimeSpanInSecond = 0;

        /// <summary>
        /// record if the tiem is corrected
        /// </summary>
        public bool isCorrectedTime = false;

        /// <summary>
        /// If this is the firt time up.
        /// </summary>
        public bool isFirstItemTime = true;

        /// <summary>
        /// special log times on the zero time.
        /// 0 or 1.
        /// </summary>
        private int startLogTimes = 0;

        /// <summary>
        /// what's reported number.
        /// </summary>
        public int repeatedTimes = 0;

        public void UpRepeatCount()
        {
            this.repeatedTimes++;
        }

        public DateTime DataTime
        {
            get
            {
                return this.dataTime;
            }

            set
            {
                this.dataTime = value;
            }
        }

        /// <summary>
        /// to correct data time to previous time.
        /// </summary>
        /// <param name="reduceSeconds"></param>
        private void CorrectDataTime()
        {
        }

        /// <summary>
        /// Updates the key.
        /// </summary>
        public void updateKey()
        {
            this.key = this.model.NodeAddress + "-" + this.model.Command;
        }

        /// <summary>
        /// Gets the inter val time_original.
        /// </summary>
        /// <value>The inter val time_original.</value>
        public int interValTime_original
        {
            get
            {
                return this.model.IntervalTime;

            }
        }
        /// <summary>
        /// finishi first time up treat.
        /// </summary>
        public void FinishFirstDataTreat()
        {
            if ( this.isFirstItemTime )
            {
                this.startLogTimes = 1;
            }
            this.isFirstItemTime = false;
        }

      
        /// <summary>
        /// Times the UP.
        /// </summary>
        public void timeUP(bool isValidTimeAutoCorrect)
        {
            if ( isFirstItemTime )
            {
                this.FinishFirstDataTreat();
                return;
            }
            synchronizeTimestamp( isValidTimeAutoCorrect );
            this.dataTime = this.dataTime.AddSeconds( this.interValTime_original );         
        }

        /// <summary>
        /// synchoronize timestamp.
        /// </summary>
        /// <param name="isValidTimeAutoCorrect"></param>
        private void synchronizeTimestamp( bool isValidTimeAutoCorrect )
        {
            if ( this.isCorrectedTime ) return;            
            if ( isValidTimeAutoCorrect && this.dateTimeSpanInSecond > 0 && this.interValTime_original > 0 )
            {
                int pastTime = this.dateTimeSpanInSecond % this.interValTime_original;
                if ( pastTime > 0 )
                {
                    int reduceSeconds = pastTime; //this.interValTime_original - pastTime;
                    this.dataTime = this.dataTime.AddSeconds( reduceSeconds * ( -1 ) );
                }
                else if ( pastTime == 0 )
                {
                    int calNumber = this.dateTimeSpanInSecond / this.interValTime_original + startLogTimes;
                    int realPassNumber = this.repeatedTimes;

                    if ( calNumber > realPassNumber )
                    {
                        this.dataTime = this.dataTime.AddSeconds( this.interValTime_original * ( -1 ) );
                    }
                    else if ( calNumber == realPassNumber )
                    {
                        // must be not reduce the time.
                    }
                    else
                    {
                        //do not need to correct.
                    }

                }
            }

            this.isCorrectedTime = true;
            
        }


    }

    /// <summary>
    /// Class CSVRowData
    /// </summary>
    class CSVRowData
    {
        /// <summary>
        /// The is column title
        /// </summary>
        private int rowType = 0;// 0: data ; 1: ColumnTitle; 2: other message

        public int RowType
        {
            get
            {
                return this.rowType;
            }
            set
            {
                if (value < 0) value = 0;
                if (value > 2) value = 2;
                this.rowType = value;
            }
        }

        /// <summary>
        /// The value of fist cell in "other message" type row
        /// </summary>
        public string otherTypeFistValue= "";
        /// <summary>
        /// The part value
        /// </summary>
        public Dictionary<string, object> partValue = new Dictionary<string, object>();                
        /// <summary>
        /// The row time
        /// </summary>
        public DateTime rowTime;
        /// <summary>
        /// The has interval time item
        /// </summary>
        public bool hasIntervalTimeItem = false;
        /// <summary>
        /// The is write time flag
        /// </summary>
        public bool isWriteTimeFlag = true;


        /// <summary>
        /// Initializes a new instance of the <see cref="CSVRowData"/> class.
        /// </summary>
        public CSVRowData():this(0)
        {            
        }
        /// <summary>
        /// Initializes a new instance of the <see cref="CSVRowData"/> class.
        /// </summary>
        /// <param name="isColumnTitle">if set to <c>true</c> [is column title].</param>
        public CSVRowData(int rowType)
        {
            this.RowType = rowType;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CSVRowData"/> class.
        /// </summary>
        /// <param name="hasIntervalTimeItem">if set to <c>true</c> [has interval time item].</param>
        /// <param name="isWriteTimeFlag">if set to <c>true</c> [is write time flag].</param>
        /// <param name="rowTime">The row time.</param>
        public CSVRowData(bool hasIntervalTimeItem, bool isWriteTimeFlag, DateTime rowTime)
        {
            this.hasIntervalTimeItem = hasIntervalTimeItem;
            this.isWriteTimeFlag = isWriteTimeFlag;
            this.rowTime = rowTime;
        }


        /// <summary>
        /// Clears this instance.
        /// </summary>
        public void Clear()
        {
            this.partValue.Clear();
        }


    }
}
