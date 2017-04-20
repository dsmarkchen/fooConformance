using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Threading;
using Peak.Can.Basic;
using CommonConfiguration.ConfigTools;
using CommonConfiguration.CommonConfiguration;
using ToolConnector;
using System.Configuration;
using EVO;
using CommonConfiguration;

namespace CommanSenderSpace
{
    /// <summary>
    /// Log uploading main form
    /// </summary>
    public partial class LoggingDownloadForm : Form
    {   
        /// <summary>
        /// Constructor
        /// </summary>
        public LoggingDownloadForm()
        {          
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Initial();
        }

        #region variable definition
        
        /// <summary>
        /// Log uploading background worker
        /// </summary>
        private BackgroundWorker logUploadingBGWorker;
        /// <summary>
        /// Log uploading process bar update background worker
        /// </summary>
        private BackgroundWorker logUploadingProcessBarUpdateBGWorker;
        /// <summary>
        /// border information request background worker
        /// </summary>
        private BackgroundWorker borderInformationRequestBGWorker;
        /// <summary>
        /// Application running flag
        /// </summary>
        private bool isRunning = true;        
        /// <summary>
        /// use new erase function or not. 
        /// </summary>
        private bool isDisableNewEraseFunction = false;

        /// <summary>
        /// The interface_start time
        /// </summary>
        private double interface_startTime = 0; // interface need how many time to prepare working..        
        /// <summary>
        /// The power O n_ time
        /// </summary>
        private DateTime powerON_Time = DateTime.Now;
        /// <summary>
        /// The power process time
        /// </summary>
        private double powerProcessTime = 0;
        /// <summary>
        /// File name of appliation config setting.
        /// </summary>
        public static string AppSettingFileName = "AppSetting-ToolsLogUpload.xml";

        /// <summary>
        /// Can bus tranmission manager handler.
        /// </summary>
        private TransmissionManager transmissionManager;
        /// <summary>
        /// per node erase default spend time in second
        /// </summary>
        private int perNodeEraseDefaultSpendTimeInSecond = 240;

        /// <summary>
        /// Can bus message for tool reset delay.
        /// </summary>
        private TPCANMsg toolResetDelayCanMsg = CanbusManager.CreateToolResetDelayCanMsg;

        #endregion

        /// <summary>
        /// Form icon
        /// </summary>
        private static Icon _appFormIcon = null;
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Icon AppFormIcon()
        {
            if (_appFormIcon == null)
            {
                string basePaht = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                FileStream stream = new FileStream(basePaht+"toolLogDataDownload.ico", FileMode.Open);
                _appFormIcon = new Icon(stream);
                stream.Close();
            }
            return _appFormIcon;
        }

        #region const config
        /// <summary>
        /// Logging request common definition
        /// </summary>
        private static int command_loggingRequest = 4;//xxx node
        /// <summary>
        /// Logging data beging transmission command definition
        /// </summary>
        private const int command_dataBegin = 17;//24 node
        /// <summary>
        /// Log data command definition
        /// </summary>
        private const int command_data = 18;//24 node
        /// <summary>
        /// Log package CRC command definition
        /// </summary>
        private const int command_CRC = 19;//24 node
        /// <summary>
        /// Log data end command definition
        /// </summary>
        private const int command_dataEnd = 21;//24 node
        /// <summary>
        /// Long data erase command definition
        /// </summary>
        private const int command_logDataErase = 28;//send command to clear log data.
        /// <summary>
        /// Node board information command definition.
        /// </summary>
        private const int Command_boardInfoID = 5;

        /// <summary>
        /// Node flash size command.
        /// </summary>
        private const int Command_flashSize = 0xC7;
        /// <summary>
        ///  Avalid nodes list.
        /// </summary>
        private List<int> _avalidNodesList = new List<int>(); 
        
        /// <summary>
        /// Node select checkbox dictionary
        /// </summary>
        private  Dictionary<int,CheckBox> _nodeSelectCheckBox = null;
        /// <summary>
        /// Node information boxes dictionary
        /// </summary>
        private Dictionary<int, Label[]> _infoShowBoxes;

        /// <summary>
        /// Index of node name
        /// </summary>
        private int _indexName = 0;
        /// <summary>
        /// Index of node connected status
        /// </summary>
        private int _indexConnect = 1;    
        /// <summary>
        /// Index of node log size
        /// </summary>
        private int _indexFlashSize = 2;
        /// <summary>
        /// Index of log uploading estimated time
        /// </summary>
        private int _indexFlashUploadingEstimateTime = 3;
        /// <summary>
        /// Index of node operation status. 
        /// </summary>
        private int _indexStatus = 4;

        /// <summary>
        /// Id of sumary row index in _infoShowBoxes.
        /// </summary>
        private int _dudmyTotalSummaryId = 999;

        #endregion
        /// <summary>
        /// Initialize the default setting.
        /// </summary>
        private void Initial()
        {
            //user double buffer for table layou panel updating.
            this.tableLayoutPanel_message.GetType().GetProperty( "DoubleBuffered", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic ).SetValue( this.tableLayoutPanel_message, true, null );

            #region Initial UI and variables
            //initialize UI 
            this.button_check.Tag = 1;
            this.button_uncheck.Tag = 0;
            this.Icon = AppFormIcon();
            this.label_remainingTime.Text = "";
            this.label_remainingTime_total.Text = "";
            this.radioButton_upload.Checked = true;
            this.button_action.Enabled = false;

            //init node display list.
            NodeIdentityEnum[] nodes = ConstInfo.ToolNodeList;
            //new int[] { ConstInfo.node_battery, ConstInfo.node_interface, ConstInfo.node_em, ConstInfo.node_pulser, ConstInfo.node_sensor, ConstInfo.node_rx };
            this._nodeSelectCheckBox = new Dictionary<int, CheckBox>();
            _infoShowBoxes = new Dictionary<int, Label[]>();



            this.Height -= this.listBox_Info.Height;
            this.listBox_Info.Visible = false;
            this.tableLayoutPanel_process.RowStyles[ 1 ].Height = 0;

            Font labelFont = new System.Drawing.Font( "Segoe UI", 9f );
            for ( int i = 0; i < nodes.Length + 1; i++ ) // the first table layout row is column header.
            {
                int nodeId = -1;
                if ( i == nodes.Length )
                {
                    nodeId = _dudmyTotalSummaryId;
                }
                else
                {
                    nodeId = (int)nodes[ i ];
                    CheckBox cb = new CheckBox();
                    cb.Enabled = false;
                    cb.Tag = nodes[ i ];
                    cb.Dock = DockStyle.Fill;
                    cb.Anchor = AnchorStyles.Right;

                    this.tableLayoutPanel_message.Controls.Add( cb, 0, i + 1 );
                    _nodeSelectCheckBox.Add( nodeId, cb );
                }
                Label[] nodeDisplay = new Label[ 5 ];
                _infoShowBoxes[ nodeId ] = nodeDisplay;

                for ( int j = 1; j <= 5; j++ ) // the first table layout column is connection status.
                {
                    Label box = new Label();
                    box.Font = labelFont;
                    box.AutoSize = false;
                    box.Margin = new System.Windows.Forms.Padding( 2 );
                    nodeDisplay[ j - 1 ] = box;
                    box.TextAlign = ContentAlignment.MiddleCenter;
                    box.Anchor = AnchorStyles.Top | AnchorStyles.Bottom;
                    box.Dock = DockStyle.Fill;


                    box.BorderStyle = BorderStyle.None;
                    if ( j == this._indexFlashSize + 1 || j == this._indexFlashUploadingEstimateTime + 1 )
                    {
                        box.TextAlign = ContentAlignment.MiddleRight;
                    }
                    if ( i == nodes.Length )
                    {
                        box.BackColor = Color.LightCyan;
                    }
                    if ( j == 1 )
                    {
                        box.TextAlign = ContentAlignment.MiddleLeft;
                        if ( i == nodes.Length )
                        {
                            box.TextAlign = ContentAlignment.MiddleCenter;
                            box.Text = "Total".ToUpper();
                        }
                        else
                        {
                            box.TextAlign = ContentAlignment.MiddleLeft;
                            box.Text = ConstInfo.getNodeStrName( nodes[ i ] ).ToUpper();
                        }
                    }
                    this.tableLayoutPanel_message.Controls.Add( box, j, i + 1 );

                    if ( j - 1 == this._indexFlashSize )
                    {
                        box.TextChanged += LogTotalFlashSizeRecalcuateEvent;
                    }
                }

                foreach ( CheckBox ck in this._nodeSelectCheckBox.Values )
                {
                    ck.CheckedChanged += LogTotalFlashSizeRecalcuateEvent;
                }
            }

            EVOAppSettings evoSetting = new EVOAppSettings( LoggingDownloadForm.AppSettingFileName, false );

            string interfaceTime = evoSetting[ "interfaceStartTime" ];
            if ( interfaceTime != null )
            {
                try
                {
                    interface_startTime = double.Parse( interfaceTime );
                }
                catch { }
            }
            string newEraseFunDIsableStr = evoSetting[ "isDisableNewEraseFunction" ];
            if ( newEraseFunDIsableStr != null )
            {
                try
                {
                    this.isDisableNewEraseFunction = bool.Parse( newEraseFunDIsableStr );
                }
                catch { }
            }
            
            string powerTime = evoSetting[ "poweronProcessTime" ];
            if ( powerTime != null )
            {
                try
                {
                    this.powerProcessTime = double.Parse( powerTime );
                }
                catch { this.powerProcessTime = 1; }
            }
            else
            {
                this.powerProcessTime = 1;
            }
            
            Utilities.ResizeButtons(this);

            MessageDefinitionReadThread();
            #endregion

            #region Initial backgroud workers
            logUploadingBGWorker = new BackgroundWorker();
            logUploadingBGWorker.DoWork += new DoWorkEventHandler( logDownloadEntrance );
            logUploadingBGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( sendComplete );

            logUploadingProcessBarUpdateBGWorker = new BackgroundWorker();
            logUploadingProcessBarUpdateBGWorker.DoWork += new DoWorkEventHandler( logUploadProcessBar_doworker );
            //processBarForDataUploadingBGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler();

            bgwForInfo = new BackgroundWorker();
            bgwForInfo.DoWork += new DoWorkEventHandler( bgwInfo_dowork );
            bgwForInfo.RunWorkerCompleted += new RunWorkerCompletedEventHandler( bgwInfo_workCompleted );
            bgwForInfo.RunWorkerAsync();

            borderInformationRequestBGWorker = new BackgroundWorker();
            borderInformationRequestBGWorker.DoWork += new DoWorkEventHandler( BorderInformationRequestBGWork_dowork );
            borderInformationRequestBGWorker.RunWorkerCompleted += new RunWorkerCompletedEventHandler( BorderInformationRequest_workCompleted );

            #endregion

            //build can bus connection.
            this.transmissionManager = new TransmissionManager();

        }

        
        /// <summary>
        /// All Messages definition
        /// </summary>
        private Dictionary<string,MSGModel> allMessageModels= null;
        /// <summary>
        /// Is connected to DB.
        /// </summary>
        private bool isConnectedDB = false;

        /// <summary>
        /// Thread for message definition read.
        /// </summary>
        private void MessageDefinitionReadThread()
        {
            allMessageModels = new Dictionary<string,MSGModel>();
            List<MSGModel> DBMsgsList = new List<MSGModel>();

            Thread oThread = new Thread(delegate()
            {
                try
                {
                    this.isConnectedDB = ConstInfo.GetAllValidMessages(ref DBMsgsList);
                    foreach (MSGModel m in DBMsgsList)
                    {
                        allMessageModels[m.NodeAddress + "-" + m.Command] = m;
                    }
                    if (!this.isConnectedDB)
                    {
                        this.Invoke((ThreadStart)delegate()
                        {
                            MessageBoxEX.Show( this, "Cannot connect to the database. Config cannot display full message list.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        });
                    }
                }
                catch (Exception ee)
                {

                }
            });
            oThread.Start();
        }
              

        #region connect to canbus and get avalide node
        /// <summary>
        /// Board informations request backgroud worker  dowork event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BorderInformationRequestBGWork_dowork(object sender, DoWorkEventArgs e)
        {
            try
            {
                Thread.Sleep(15);
                this.ConnectTOCanBus();
                
            }
            catch (Exception ee)
            {
            }
        }
        /// <summary>
        /// Board informations request backgroud worker  workComplete event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BorderInformationRequest_workCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            this.EndBGWWorker();
        }
        #endregion

        private string openPath = System.Windows.Forms.Application.StartupPath + "\\EVOConfigFiles\\";
       
        private Dictionary<string,object[]> _filesDict = new Dictionary<string,object[]>();


        #region status info dispplay
        private BackgroundWorker bgwForInfo;

        private List<DisplayMessageInfo> messageCache = new List<DisplayMessageInfo>();
        private List<DisplayMessageInfo> messageCache_tp = new List<DisplayMessageInfo>();
        private void _showMessages(int destNode, string msg, bool notUsingParameter)
        {
            lock (messageCache)
            {
                messageCache.Add(new DisplayMessageInfo(destNode,msg,DateTime.Now));
            }           
           
        }

        private class DisplayMessageInfo
        {
            public int node;
            public string message;
            public DateTime time;

            public DisplayMessageInfo(int node, string message, DateTime time)
            {
                this.node = node;
                this.message = message;
                if (this.message == null)
                {
                    this.message = "";
                }
                this.time = time;
            }

            public string CombinMessage
            {
                get
                {
                    if (node != -1)
                    {
                        return ConstInfo.getNodeStrName(node) + " [id:" + node + " ]  " + message;
                    }
                    else
                    {
                        return message;
                    }
                }
            }
        }

        /// <summary>
        /// Prepare for backgroud worker start 
        /// </summary>
        private void StartBGWPreviousWork()
        {
            this.isBKGWorking = true;
            this._runStopFlag = false;            
            this.Invoke((ThreadStart)delegate()
               {
                   this.labelBox_logDir.Text = "";
                   this.button_action.Text = "Stop";
                   this.panel_operation.Enabled = false;
                   this.panel_operation2.Enabled = false;
                   this.panel_operation3.Enabled = false;
                   this.button_close.Enabled = false;
                   foreach (int node in this._avalidNodesList)
                   {
                       this._nodeSelectCheckBox[node].Enabled = false;
                   }

                   //this.button_eraseCancel.Visible = false;
                   this._dummyReceivedDataIndex = 0;
                   this.progressBar_part.Value = 0;
                   //this.label_progress.Text = "";
                   this.label_remainingTime.Text = "";      

                   this.progressBar_total.Value = 0;
                   //this.label_progress_total.Text = "";
                   this.label_remainingTime_total.Text = "";
               });
        }
        /// <summary>
        /// post work of background worker
        /// </summary>
        private void EndBGWWorker()
        {
            this._runStopFlag = true;
            this.transmissionManager.IsSoftwarePowerRunning = false; //true;
            try
            {
                this.Invoke((ThreadStart)delegate()
               {
                   this.button_close.Enabled = true;
                   this.progressBar_part.Value = 0;
                   //this.label_progress.Text = "";
                   this.label_remainingTime.Text = "";

                   this.progressBar_total.Value = 0;
                   //this.label_progress_total.Text = "";
                   this.label_remainingTime_total.Text = "";

                   //this.button_eraseCancel.Visible = false;
                   this.panel_operation.Enabled = true;
                   this.panel_operation2.Enabled = true;
                   this.panel_operation3.Enabled = true;
                   this.button_action.Enabled = true;
                   foreach (int node in this._avalidNodesList)
                   {
                       this._nodeSelectCheckBox[node].Enabled = true;
                   }
                   if (this.radioButton_erase.Checked)
                   {
                       this.button_action.Text = this.radioButton_erase.Text as string;
                   }
                   else
                   {
                       this.button_action.Text = this.radioButton_upload.Text as string;
                   }
               });                
            }
            catch { }
            this.isBKGWorking = false;

        }
        /// <summary>
        /// Dummy received data index. It used when the log size is failed to get.
        /// </summary>
        private int _dummyReceivedDataIndex = 0;
        /// <summary>
        /// Dummy log data total size. It used when the log size is failed to get.
        /// </summary>
        private int _dummmyLogTotalSize = 1000;
        /// <summary>
        /// set progress bar value
        /// </summary>
        private void setProgressBarValue()
        {
            if (this._flashSize_CurrentNode > 0)
            {               
                //current node process value
                int pValue = this._flashReceivedSize_currentNode * 100 / this._flashSize_CurrentNode;
                string leftTime = GetTimeString(this._flashSize_CurrentNode, this._flashReceivedSize_currentNode, this._flashUploadStartTime);
                if (pValue > 100) pValue = 100;
                //if (pValue < this.progressBar_p.Value) pValue = this.progressBar_p.Value;
                
                //total process value
                int receiveTotal = this._flashReceivedSize_total;
                if (this._flashReceivedSize_currentNode > 0)
                {
                    receiveTotal += this._flashReceivedSize_currentNode;
                }
                int pValue_total = receiveTotal * 100 / this._flashSize_total;
                string leftTime_total = GetTimeString(this._flashSize_total, receiveTotal, this.flashUploadStartTime_total);
                if (pValue_total < this.progressBar_total.Value) pValue_total = this.progressBar_total.Value;
                if (pValue_total > 100) pValue_total = 100;

                this.Invoke((ThreadStart)delegate()
                {
                    //set current process value
                    //this.label_progress.Text = pValue + "%";
                    this.progressBar_part.Value = pValue;
                    this.label_remainingTime.Text = leftTime+" remaining";

                    //set total process value
                    //this.label_progress_total.Text = pValue_total + "%";
                    this.progressBar_total.Value = pValue_total;
                    this.label_remainingTime_total.Text = leftTime_total + " remaining";
                });

                
            }
            else
            {
                int pValue = (this._dummyReceivedDataIndex + 1) * 100 / _dummmyLogTotalSize;
                this.Invoke((ThreadStart)delegate()
                {
                    //this.label_progress.Text = pValue + "%";
                    this.progressBar_part.Value = pValue;
                });
            }

        }
        /// <summary>
        /// Set progress bar value for parse thread
        /// </summary>
        /// <param name="value"></param>
        public void setParseProgressBarValue(int value)
        {
            if (value > 100) value = 100;
            this.Invoke((ThreadStart)delegate()
            {
                //this.label_progress.Text = value + "%";
                this.progressBar_part.Value = value;

               //this.label_progress_total.Text = value + "%";
                this.progressBar_total.Value = value;                
            });
        }

    
        /// <summary>
        /// If the background is working.
        /// </summary>
        private bool isBKGWorking = false;
        /// <summary>
        /// Information box row index
        /// </summary>
        private int informationBoxRowIndex = 1;

        /// <summary>
        /// information display background worker workCompleted evet.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgwInfo_workCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker currBGWorker = (BackgroundWorker)sender;
            if (currBGWorker.CancellationPending)
            {
                return;
            }
            try
            {
                if (this.messageCache.Count > 0)               
                {
                    this.messageCache_tp.Clear();
                    lock (this.messageCache)
                    {

                        this.messageCache_tp.AddRange(this.messageCache);
                        this.messageCache.Clear();
                    }
                    this.listBox_Info.SuspendLayout();

                    for (int i = 0; i < messageCache_tp.Count; i++)
                    {
                        DisplayMessageInfo info = this.messageCache_tp[i];                        
                        this.listBox_Info.Items.Add(informationBoxRowIndex + ":  " + info.time.ToLongTimeString() + " - " + info.CombinMessage);
                        informationBoxRowIndex++;                        
                    }
                    this.listBox_Info.SelectedIndex = this.listBox_Info.Items.Count - 1;
                    this.listBox_Info.ResumeLayout();
                }               
                if (this.isRunning)
                {
                    currBGWorker.RunWorkerAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        /// <summary>
        /// Information background worker doWork event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bgwInfo_dowork(object sender, DoWorkEventArgs e)
        {
            try
            {
                BackgroundWorker currBGWorker = (BackgroundWorker)sender;

                while (isRunning)
                {
                    if (currBGWorker.CancellationPending)
                    {
                        e.Cancel = true;
                        return;
                    }
                    Thread.Sleep(1000);
                    if (this.messageCache.Count <= 0)// && !this.isShowCartoon)
                    {
                        continue;
                    }
                    else
                    {
                        return;
                    }
                }
            }
            catch{}
        }
        #endregion end status info dispplay

        
        private bool isWorking = false;
        /// <summary>
        /// Command send complete evnent.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void sendComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.isWorking = false;
            this.button_action.Enabled = true;            
        }             
     
        private void button2_Click(object sender, EventArgs e)
        {
            this.ConnectTOCanBus();
        }

        private void ConnectTOCanBus()
        {            

            this.isRunning = true;            
            this.powerON_Time = DateTime.Now;
            /*
            //power on battery.. and get Enable node
            bool powerOk = this.transmissionManager.BatteryPowerControl(true);
            this.powerON_Time = DateTime.Now;

            this.Invoke((ThreadStart)delegate()
               {
                   if (powerOk)
                   {
                       this._showMessages(-1, "Tools power on successfuly", false);
                   }
                   else
                   {
                       this._showMessages(-1, "Tools power on with failure", false);
                       //MessageBox.Show(this, "Can't turn tool power on, Please check.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                       //return;
                   }
               });*/

            List<int> errorBINodes = checkAvalidNode();

            if (errorBINodes != null && errorBINodes.Count > 0)
            {
                StringBuilder invalidBoardInfoMsg = new StringBuilder();
                invalidBoardInfoMsg.Append("Nodes [");
                foreach (int nodeId in errorBINodes)
                {
                    invalidBoardInfoMsg.Append("  ");
                    invalidBoardInfoMsg.Append(ConstInfo.getNodeStrName(nodeId));
                }
                invalidBoardInfoMsg.Append(" ] cannot be connected.");
                this.Invoke((ThreadStart)delegate()
                {
                    MessageBoxEX.Show( this, invalidBoardInfoMsg.ToString(), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                });
            }
        }
        /*
        //when pow off, it will not wait for response.
        private bool BatteryPowerControl(bool isOn)
        {
            powerON_Time = DateTime.Now;
            if (!this.isControlPowerON || !isOn) return true;

            TPCANStatus stsResult;
            bool isOk = true;
            TPCANMsg pownOn = this.createMsg(node_battery, null, 0);
            pownOn.LEN = 3;
            pownOn.DATA[0] = (byte)commandPowon;
            pownOn.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
            if (isOn)
            {
                pownOn.DATA[2] = 1;
            }
            else
            {
                pownOn.DATA[2] = 0;
            }

            stsResult = sendPCANData(ref pownOn);
            if (isOn)
            {
                isOk = this.transmissionManager.isCanbusResponseContinue(ConstInfo.SurfaceCanBusAddr, node_battery, commandPowon, this.powerProcessTime);
            }
            Thread.Sleep(400);
            powerON_Time = DateTime.Now;
            return isOk;
        }*/
     
     
        /// <summary>
        /// 
        /// </summary>
        /// <returns>error borderinfo nodes list</returns>
        private List<int> checkAvalidNode()
        {
            this._avalidNodesList.Clear();          

            List<int> errorInfoNodes = new List<int>();

            TPCANStatus stsResult;
            TPCANMsg NodeAliveCheckMsg = this.createMsg(0, null, 0);
            NodeAliveCheckMsg.LEN = 2;
            NodeAliveCheckMsg.DATA[0] = Command_boardInfoID;
            NodeAliveCheckMsg.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
            for (int i = 0; i < ConstInfo.ToolNodeList.Length; i++)
            {
                int tryTime = 2;
                int nodeId = (int)ConstInfo.ToolNodeList[i];
                NodeAliveCheckMsg.ID = (uint)nodeId;
                if (nodeId == (int)NodeIdentityEnum.node_interface)
                {
                    DateTime now = DateTime.Now;
                    TimeSpan span = now - this.powerON_Time;
                    if (span.TotalMilliseconds < interface_startTime * 1000)
                    {
                        Thread.Sleep((int)(interface_startTime * 1000 - span.TotalMilliseconds));
                    }
                }
                while (tryTime > 0)
                {                    
                    stsResult = sendPCANData(ref NodeAliveCheckMsg);
                    BoardInformation  boarderInfoBytes = this.transmissionManager.receiveBoardInformation(ConstInfo.SurfaceCanBusAddr, nodeId, Command_boardInfoID);// this.receiveBoardInformation_new(nodeId);


                    /*/
                    if (boarderInfoBytes == null || !boarderInfoBytes.IsHaveData)
                    {
                        break;// no this node.
                    }
                    else //*/
                    if( boarderInfoBytes == null || !boarderInfoBytes.IsVaildBoardInfo)
                    {
                        //get message , but borderInfor error..
                        // try one more time..
                        tryTime--;
                        if (tryTime <= 0)
                        {
                            errorInfoNodes.Add(nodeId);
                            this.Invoke((ThreadStart)delegate()
                            {
                                this.tableLayoutPanel_message.SuspendLayout();
                                this._infoShowBoxes[nodeId][_indexConnect].Text = "×";
                                this._infoShowBoxes[nodeId][_indexConnect].BackColor = this.BackColor;
                                this._infoShowBoxes[nodeId][_indexName].BackColor = this.BackColor;
                                this._infoShowBoxes[nodeId][_indexFlashSize].Tag = -1;
                                this._infoShowBoxes[nodeId][_indexFlashSize].Text = "";
                                this._infoShowBoxes[nodeId][_indexFlashUploadingEstimateTime].Text = "";
                                this.tableLayoutPanel_message.ResumeLayout();

                                _nodeSelectCheckBox[nodeId].Enabled = false;
                                _nodeSelectCheckBox[nodeId].Checked = false;
                                
                            });
                            break;
                        }
                    }
                    else
                    {
                        // get borderinof correctly
                        this.boarderInformations[nodeId] = boarderInfoBytes;
                        this._avalidNodesList.Add(nodeId);
                        //to get flash size data..
                        int flashSize = this.GetFlashSize(nodeId);
                        string showMessage_size = null;
                        string showMessage_time = null;
                        if (flashSize < 0)
                        {
                            showMessage_size = "Unknown";
                            showMessage_time = "Unknown";
                        }
                        else
                        {
                            showMessage_size = this.GetSDataSizeString(flashSize);
                            showMessage_time = GetTimeString(flashSize, 0, DateTime.Now);
                        }
                       
                        this.Invoke((ThreadStart)delegate()
                        {
                            this.tableLayoutPanel_message.SuspendLayout();
                            this._infoShowBoxes[nodeId][_indexConnect].Text = "√";
                            this._infoShowBoxes[nodeId][_indexFlashSize].Tag = flashSize;
                            this._infoShowBoxes[nodeId][_indexFlashSize].Text = showMessage_size;                            
                            this._infoShowBoxes[nodeId][_indexFlashUploadingEstimateTime].Text = showMessage_time;
                            this._infoShowBoxes[nodeId][_indexConnect].BackColor = Color.LightGreen;
                            this._infoShowBoxes[nodeId][_indexName].BackColor = Color.LightGreen;
                            this.tableLayoutPanel_message.ResumeLayout();

                            _nodeSelectCheckBox[nodeId].Enabled = true;
                            _nodeSelectCheckBox[nodeId].Checked = true;
                        });
                        break;
                    }                   
                }              
            }
          
            return errorInfoNodes;
        }
        
        private void disConnectionToCanBus(object sender, EventArgs e)
        {
            this.transmissionManager.disconnectCanBus();
            this.transmissionManager.Dispose();
            this.isRunning = false;
            this.button_action.Enabled = false;
        }

        private void logUploadProcessBar_doworker(object sender, DoWorkEventArgs e)
        {
            while (this.isWorking && !this.IsDisposed)
            {
                this.setProgressBarValue();
                for (int i = 0; i < 3 && this.isWorking && !this.IsDisposed; i++)
                {
                    Thread.Sleep(1000);                    
                }
            }
        }

        /// <summary>
        /// dispaly on loguploading file.
        /// </summary>
        private string _fileTimeSuffix = null;
        /// <summary>
        /// log download  process.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void logDownloadEntrance(object sender, DoWorkEventArgs e)
        {           
            this.isWorking = true;
            
            this._showMessages(-1, "Beginning upload.", false);

            this._flashUploadStartTime = DateTime.Now;
            this.flashUploadStartTime_total = DateTime.Now;

            //process bar display update background worker to start.
            this.logUploadingProcessBarUpdateBGWorker.RunWorkerAsync();

            this._fileTimeSuffix = DateTime.Now.ToString("yyyyMMdd-HHmm");
            Random r = new Random(1000);
            string successUploadingStr = "";
            string failToConvertStr = "";
            
            string failedStr = "";
            List<NodeIdentityEnum> unfinishedNodes = new List<NodeIdentityEnum>();
            unfinishedNodes.AddRange( this.uploadingSelectedList );
            
            for ( int i = 0; i < this.uploadingSelectedList.Count && !this._runStopFlag; i++ )
            {                
                NodeIdentityEnum node = this.uploadingSelectedList[i];
                this._showMessages((int)node, "Start to upload log", false);
                this.Invoke((ThreadStart)delegate()
                {
                    this._infoShowBoxes[(int)node][_indexStatus].Text = "UPLOADING..";
                });
                NodeUploadStatus status = this.dataDownloadProcess(node,3);
                //isOk = (r.Next() % 2 == 0);
                //Thread.Sleep(10000);

                #region end process info showing
                string logMessage = null;
                string nodeName = ConstInfo.getNodeStrName(node);
                string nodeUploadingStatus = null;
                int currentFlashSize = this._flashSize_CurrentNode;
                this._flashSize_CurrentNode = -1;
                this._flashReceivedSize_currentNode = 0;

                if ( status.IsUploaded)
                {
                    logMessage = "[" + nodeName + "] log data has been uploaded successfully.";
                    nodeUploadingStatus = "UPLOADED";
                    if (successUploadingStr.Length > 0)
                    {
                        successUploadingStr += ",";
                    }
                    successUploadingStr += nodeName;

                    if ( status.IsConverted )
                    {
                        nodeUploadingStatus += " && CONVERTED";
                    }
                    else
                    {
                        if ( failToConvertStr.Length > 0 )
                        {
                            failToConvertStr += ",";
                        }
                        failToConvertStr += nodeName;
                        nodeUploadingStatus += " && FAIL to CONVERT";
                    }
                    unfinishedNodes.Remove( node );
                }
                else
                {
                    if ( status.IsCancelled )
                    {
                        nodeUploadingStatus = "CANCELLED";
                        logMessage = "[" + nodeName + "] log data uploading is cancelled.";
                    }
                    else
                    {
                        nodeUploadingStatus = "FAIL to UPLOAD";
                        logMessage = "Failed to upload " + "[" + nodeName + "] log data.";

                        if ( failedStr.Length > 0 )
                        {
                            failedStr += ","; ;
                        }
                        failedStr += nodeName;
                        unfinishedNodes.Remove( node );
                    }
                }

                if (currentFlashSize > 0)
                {
                    this._flashReceivedSize_total += currentFlashSize;
                }

                this.Invoke((ThreadStart)delegate()
                {
                    this._infoShowBoxes[(int)node][_indexStatus].Text = nodeUploadingStatus;
                });
               
                this._showMessages(-1, logMessage, false);
                ConstInfo.SaveLogToFile("LOG UPLOAD-" + logMessage);
                #endregion
            }
            if ( failedStr.Length <= 0 && unfinishedNodes.Count <= 0 )
            {
                //stop watch dog for tool reset delay
                this.SendToolResetDealyCommand( false );

            }

            // to pop all message to show list all uploading result.
            //this.popMessage(logMessage);
            string popMessage = "";
            if (successUploadingStr.Length > 0)
            {
                popMessage += "Successfully uploaded [" + successUploadingStr + "] log.\r\n";
                if (failToConvertStr.Length > 0)
                {
                    popMessage += "Failed to convert [" + failToConvertStr + "] log to CSV file.\r\n";
                }
            }
            if (failedStr.Length > 0)
            {
                popMessage += "Failed to upload [" + failedStr + "] log.\r\n";
            }

            if ( unfinishedNodes.Count > 0 )
            {
                string unfinishedstr = "";
                foreach ( NodeIdentityEnum unfinishedNode in unfinishedNodes )
                {
                    this.Invoke( (ThreadStart)delegate()
                    {
                        this._infoShowBoxes[ (int)unfinishedNode ][ _indexStatus ].Text = "CANCELLED";
                    } );

                    if ( unfinishedstr.Length != 0 )
                    {
                        unfinishedstr += ",";
                    }
                    unfinishedstr += ConstInfo.getNodeStrName( unfinishedNode );
                }
                popMessage += "Cancelled uploading for [" + unfinishedstr + "].\r\n";
            }

            this.EndBGWWorker();
            this._showMessages(-1, popMessage, false);
            this.PopUpInformationMessage(popMessage);            
        }

        private TPCANStatus sendPCANData(ref TPCANMsg msg)
        {
            return this.transmissionManager.sendPCANData(ref msg);
        }
      
        #region tool download data process

        private void PopUpInformationMessage(string messsgage)
        {
            this.Invoke((ThreadStart)delegate()
            {
                MessageBoxEX.Show( this, messsgage, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
            });
        }

        private void PopUpWarningMessage( string messsgage )
        {
            this.Invoke( (ThreadStart)delegate()
            {
                MessageBoxEX.Show( this, messsgage, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            } );
        }

        private void PopUpErrorMessage( string messsgage )
        {
            this.Invoke( (ThreadStart)delegate()
            {
                MessageBoxEX.Show( this, messsgage, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
            } );
        }

        /// <summary>
        /// data uploading process
        /// </summary>
        /// <param name="node"></param>
        /// <param name="retryTimes"></param>
        /// <param name="isParseOk"></param>
        /// <returns></returns>
        private NodeUploadStatus dataDownloadProcess( NodeIdentityEnum node, int retryTimes)        
        {
            NodeUploadStatus status = new NodeUploadStatus();

            this._flashSize_CurrentNode = -1;
            this._flashReceivedSize_currentNode = 0;            
            this._flashUploadStartTime = DateTime.Now;

            int leftTimes = retryTimes;
            if (leftTimes <= 0) return status;           
            
            int destAddress = (int)node;
            byte[] rdata;

            //keep watch dog for tool reset delay
            this.SendToolResetDealyCommand( true );

            #region request logging requset
            this._showMessages(destAddress, " logging request send", false);
            TPCANMsg begin = this.createMsg(destAddress, null, 0);
            begin.DATA[0] = (byte)command_loggingRequest;
            begin.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
            begin.LEN = 2;
            int reslen = 0;
            this.sendPCANData(ref begin);            
            byte[] beginRes = this.transmissionManager.getCanBusResponData(ConstInfo.SurfaceCanBusAddr, destAddress, command_loggingRequest, 0.5,ref reslen);
            if ( reslen <= 0 ) // can not connect to tool;
            {
                string mes = "Logging request error, please try again.";
                this.PopUpErrorMessage( mes );
                this._showMessages( destAddress, mes, false );
                return status;
            }
            else
            {
                //if (reslen >= 8)
                // {
                //    this.flashTotalSize = BitConverter.ToInt32(beginRes, 4);
                // }
                this._showMessages( destAddress, "logging request ok, ready to receive log data!", false );
            }
            #endregion

            string logFileName = "EVOLog_" + ConstInfo.getNodeStrName(node) + "_" + this._fileTimeSuffix;
            string logFileNameWithSuffix = logFileName + evoLog_suffix;
            string logDataSaveFilePath = this.logSaveFolder +"/" +logFileNameWithSuffix;
            //to save log data 
            FileStream logDataFileStream = new FileStream(logDataSaveFilePath, FileMode.OpenOrCreate);
            // to save log data in hidden folder for engineering.
            FileStream logDataFileStream_bak = null;
            #region create log data backup file stream
            string fd = this.bakupSecurityFilePath + this.bakupDirectory;
            if (!Directory.Exists(fd))
            {
                Directory.CreateDirectory(fd);
            }            
            DirectoryInfo info = new DirectoryInfo(fd);
            //if bakup folder is not hidden, set to hidden 
            if ((info.Attributes & FileAttributes.Hidden) != FileAttributes.Hidden)
            {
                info.Attributes = FileAttributes.Directory | FileAttributes.Hidden;
            }
            string[] files = Directory.GetFiles(fd);
            bool isFileExist = false;
            foreach (string name in files)
            {
                int bbi = name.LastIndexOf("\\");
                string fNamePart = name.Substring(bbi + 1, name.Length - bbi - 1);
                if (fNamePart == logFileNameWithSuffix)
                {
                    isFileExist = true;
                    break;
                }
            }
            string bakupFname = null;
            if (isFileExist)
            {
                bakupFname = logFileName + "-" + DateTime.Now.Ticks + evoLog_suffix;
            }
            else
            {
                bakupFname = logFileNameWithSuffix;
            }
            logDataFileStream_bak = new FileStream(fd + "/" + bakupFname, FileMode.OpenOrCreate);
            #endregion


            #region  bein to receie data
            this._showMessages(destAddress, "log data beging to receive", true);
            //bool isOk = this.transmissionManager.isCanbusResponseContinue(ConstInfo.SurfaceCanBusAddr,destAddress, command_dataBegin, 0.5);
            reslen = 0;

            byte[] beginData = this.transmissionManager.getCanBusResponData(ConstInfo.SurfaceCanBusAddr, destAddress, command_dataBegin, 0.5, ref reslen);

            if (reslen >= 8)
            {
                this._flashSize_CurrentNode = BitConverter.ToInt32(beginData, 4);
                if (this._flashSize_CurrentNode < 0)
                {
                    this._flashSize_CurrentNode = -1;
                }
                else
                {
                    this._showMessages(destAddress, "logging data size is " + this._flashSize_CurrentNode + "bytes", false);
                    int preReqFlashSize = (int)this._infoShowBoxes[destAddress][_indexFlashSize].Tag;
                    // to update flash size
                    if (preReqFlashSize > 0)
                    {
                        this._flashSize_total += (this._flashSize_CurrentNode - preReqFlashSize);
                    }
                    else
                    {
                        this._flashSize_total += (this._flashSize_CurrentNode - preReqFlashSize);
                    }
                    this.Invoke((ThreadStart)delegate()
                    {
                        this._infoShowBoxes[destAddress][_indexFlashSize].Text = GetSDataSizeString(this._flashSize_CurrentNode);
                        this._infoShowBoxes[destAddress][_indexFlashSize].Tag = this._flashSize_CurrentNode;
                        this._infoShowBoxes[destAddress][_indexFlashUploadingEstimateTime].Text = GetTimeString(this._flashSize_CurrentNode, 0, DateTime.Now);
                    });
                }
            }
            else
            {


            }
            #endregion
            #region  receie data
            TPCANMsg CRCCheckRes = this.createMsg(destAddress, null, 0);
            CRCCheckRes.DATA[0] = (byte)command_CRC;
            CRCCheckRes.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
            CRCCheckRes.LEN = 8;

            MemoryStream partDataStream = new MemoryStream();
            
            bool isSuccess = true;
            bool transing = true;
            int dataLen = 0;
            DateTime beginT = DateTime.Now;
            int testReceiveData = 1;

            int resetDelaySendPackageCounter = 0;

            while (transing)
            {
                if ( this._runStopFlag )
                {
                    isSuccess = false;
                    status.IsCancelled = true;
                    break;
                }
                rdata = this.transmissionManager.getCanBusResponData(ConstInfo.SurfaceCanBusAddr,destAddress, -1, 3,ref dataLen);//receive any data from this node
                if (rdata == null)
                {
                    //error
                    isSuccess = false;
                    this._showMessages(destAddress, "logging dowload time out, please check!", false);
                    break;
                }
                
                switch (rdata[0])
                {
                    case command_data:
                        testReceiveData++;
                        partDataStream.Write(rdata, 2, dataLen - 2);
                        break;
                    case command_CRC:
                        resetDelaySendPackageCounter++;
                        //send out one software power on command to make all tool alive.
                        this.transmissionManager.SendSoftwarePowerONCommandWioutRes();
                        if ( resetDelaySendPackageCounter >= 30 )
                        {
                            //keep watch dog for tool reset delay
                            this.SendToolResetDealyCommand( true );
                            resetDelaySendPackageCounter = 0;
                        }

                        int receiveCrc = rdata[2] | (rdata[3] << 8);
                        partDataStream.Flush();
                        ushort dataCRC = ConstInfo.getCRC16(partDataStream.ToArray());//calculate data CRC
                        if (receiveCrc == dataCRC)
                        {
                            CRCCheckRes.DATA[2] = 1;
                            partDataStream.WriteTo(logDataFileStream);
                            if (logDataFileStream_bak != null)
                            {
                                partDataStream.WriteTo(logDataFileStream_bak);
                            }
                            leftTimes = retryTimes;

                            //received flash log size.
                            this._flashReceivedSize_currentNode += (int)partDataStream.Length;                            
                        }
                        else
                        {
                            leftTimes--;
                            CRCCheckRes.DATA[2] = (byte)((leftTimes > 0) ? 2 : 3); //error 2: resend 3: error stop
                        }
                        this._showMessages(destAddress, "crc:"+receiveCrc + "@" + dataCRC, false);

                        byte[] lenBytes = BitConverter.GetBytes( (int)partDataStream.Length );
                        lenBytes.CopyTo( CRCCheckRes.DATA, 4 );
                        
                        partDataStream.SetLength(0);// clear data..
                        dataCRC = 0xFFFF;
                        this.sendPCANData(ref CRCCheckRes);
                        if (leftTimes <= 0)
                        {
                            isSuccess = false;
                            this._showMessages(destAddress, "CRC check retry times is over " + retryTimes + " times !", false);
                        }
                        //this.setProgressBarValue();
                        break;
                    case command_dataEnd:
                        transing = false;
                        break;
                }               
            }
            
            //keep watch dog for tool reset delay
            this.SendToolResetDealyCommand( true );

            status.IsUploaded = isSuccess;

            //save log for debug.
            DateTime endT = DateTime.Now;
            TimeSpan span = endT - beginT;
            StringBuilder str = new StringBuilder();
            str.AppendLine( "From "+ConstInfo.getNodeStrName(destAddress)+" receive data count : " + testReceiveData + "  totalTime:" + span.TotalMilliseconds);
            str.AppendLine("1K data : " + (span.TotalMilliseconds * 1024d / (6d * testReceiveData * 1000))+"s");
            str.AppendLine("1M data : " + (span.TotalMilliseconds * 1024d * 1024 / (6d * testReceiveData * 1000)) + "s");
            str.AppendLine("32.5M data : " + (span.TotalMilliseconds * 1024d * 1024 * 32.5 / (6d * testReceiveData * 1000)) + "s");         
            ConstInfo.SaveLogToFile(str.ToString()); // save it to log file. i want to know the log transmission speed.


            #region close used file stream.
            if (logDataFileStream != null)
            {
                try
                {
                    logDataFileStream.Flush();
                    logDataFileStream.Close();
                }
                catch { }
            }
            if (logDataFileStream_bak != null)
            {
                try
                {
                    logDataFileStream_bak.Flush();
                    logDataFileStream_bak.Close();
                }
                catch
                {
                }
            }
            #endregion

            if (status.IsUploaded)
            {
                this._showMessages(destAddress, "log data received successfully!", false);
                //parse file to csv.

                #region log data to convert
                string CSVFilePath = this.logSaveFolder + "/" + logFileName + ".CSV";                
                try
                {
                    FileStream sourceStream = new FileStream(logDataSaveFilePath, FileMode.Open);
                    status.IsConverted = LogDataParser.parseLogData(sourceStream, CSVFilePath, this.allMessageModels, true);
                }
                catch { }
                //string logFileName = "EVOLog_" + ConstInfo.getNodeStrName(node) + "_" + this._fileTimeSuffix;
                //string logFileNameWithSuffix = logFileName + evoLog_suffix;
                //string saveFilePath = this.logSaveFolder + "/" + logFileNameWithSuffix;
                // some day, i want to parse it automaticly.
                //Console.WriteLine(partDataStream.ToString());
                #endregion

                //keep watch dog for tool reset delay
                this.SendToolResetDealyCommand( true );
            }
            else if ( status.IsCancelled )
            {
                this._showMessages( destAddress, " log uploading is cancelled.", false );
            }
            else
            {
                this._showMessages( destAddress, " log failed to be uploaded.", false );
            }
            #endregion data receive end..

            

            return status;
        }
                
        #endregion end real boot load

        #region inner create canbus messge       

        private TPCANMsg createMsg(int dest,byte[] data, int start)
        {
            int len = 0;
            if (data != null)
            {
                len = data.Length - start;
            }
            if (len > 8) len = 8;

            TPCANMsg msg = new TPCANMsg();
            //msg.
            msg.MSGTYPE = TPCANMessageType.PCAN_MESSAGE_STANDARD;            
            msg.ID = (uint)dest;
            msg.DATA = new byte[8];
            msg.LEN = (byte)len;

            for (int i = 0; i < len; i++)
            {
                msg.DATA[i] = data[start + i];
            }
            
            return msg;
        }

        #endregion end inner create canbus messge

        private bool checkTextValid()
        {
            

            bool isValid = true;
            if (this._nodeSelectCheckBox == null || this._nodeSelectCheckBox.Count == 0)
            {
                isValid = false;
            }
            else
            {
                foreach (CheckBox box in this._nodeSelectCheckBox.Values)
                {
                    if (box.Checked)
                    {
                        isValid = true;
                        break;
                    }
                }
            }
            if (!isValid)
            {
                MessageBoxEX.Show( this, "Please select node first.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
            }

            return isValid;
        }

        private bool ValidateTextBox(TextBox box, string boxName)
        {
            if (box.Text.Length <= 0)
            {
                MessageBoxEX.Show( this, "Please input value to " + boxName + ".", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return false;
            }
            try
            {
                int value = int.Parse(box.Text);
            }
            catch (Exception e)
            {
                MessageBoxEX.Show( this, boxName + " must be a number.", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                return false;
            }
            return true;
        }

        private string evoLog_suffix = ".evoLog";
        /// <summary>
        /// telemetry tool dump logging data bakup directory.
        /// </summary>
        private string bakupSecurityFilePath = @"C:\JobArchive\";
        private string bakupDirectory = "ToolLogBackup";

        /// <summary>
        /// get node's flash log size
        /// </summary>
        /// <param name="nodeID"></param>
        /// <returns></returns>
        private int GetFlashSize(int nodeID)
        {
            byte Command_FlashSize = 213;
            //flash size request command.
            TPCANMsg flashSizeReqMsg = this.createMsg(0, null, 0);
            flashSizeReqMsg.LEN = 2;
            flashSizeReqMsg.DATA[0] = Command_FlashSize;
            flashSizeReqMsg.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
            flashSizeReqMsg.ID = (byte)nodeID;

            int resLen = 0;
            sendPCANData(ref flashSizeReqMsg);
            byte[] flashSizeBytes = this.transmissionManager.getCanBusResponData(ConstInfo.SurfaceCanBusAddr, nodeID, Command_FlashSize, 0.5, ref resLen);

            //did not get flash size from firmware
            if (resLen <= 0 || flashSizeBytes == null || flashSizeBytes.Length < 8) return -1;

            return BitConverter.ToInt32(flashSizeBytes, 4);
        }

        /// <summary>
        /// current node flash size need to upload for current node
        /// </summary>
        private int _flashSize_CurrentNode = -1;
        /// <summary>
        /// total flash size need to upload
        /// </summary>
        private int _flashSize_total = -1;
        /// <summary>
        /// received flash size from current node
        /// </summary>
        private int _flashReceivedSize_currentNode = 0;
        /// <summary>
        /// total received flash size.
        /// </summary>
        private int _flashReceivedSize_total = 0;
        
        /// <summary>
        /// the upload process start time for current node.
        /// </summary>
        private DateTime _flashUploadStartTime = DateTime.Now;

        /// <summary>
        /// Flash erase process start time.
        /// </summary>
        private DateTime _flashEraseStartTime = DateTime.Now; 

        /// <summary>
        /// the upload process start time for total
        /// </summary>
        private DateTime flashUploadStartTime_total = DateTime.Now;

        private string GetSDataSizeString(int totalSize)
        {
            if (totalSize <= 0) return "0 B";
            StringBuilder sb = new StringBuilder();
            int kleft = totalSize % 1024;
            int kNum = totalSize / 1024;
            int MLeft = kNum % 1024;
            int Mnum = kNum / 1024;

            if (Mnum > 0)
            {
                sb.Append(Mnum);
                sb.Append(" MB, ");
            }
            if (MLeft > 0 || sb.Length > 0)
            {
                string kb = MLeft + "";
                for (int i = 0; i < 4 - kb.Length; i++)
                {
                    sb.Append(" ");
                }
                sb.Append(kb);
                sb.Append(" KB, ");
            }
            if (kleft >= 0)
            {
                string bb = kleft + "";
                for (int i = 0; i < 4 - bb.Length; i++)
                {
                    sb.Append(" ");
                }
                
                sb.Append(bb);
                sb.Append(" B");
            }
            return sb.ToString();
        }

        private static string GetTimeString(int totalSize, int receivedSize, DateTime startTime)
        {
            if (receivedSize < 0) receivedSize = 0;
            if (totalSize <= 0)  totalSize = 0;

            int flashSize = totalSize - receivedSize;
            if (flashSize < 0) flashSize = 0;
            int st = 0;

            double spendSecond = (DateTime.Now - startTime).TotalSeconds;
            if (receivedSize < 1024 || spendSecond <= 1)
            {
                // can bus 125k bit/s  , one frame 8 bytes , 6 bytes used for log data. 0.75 valid rate // evaluate
               //st = (int)(Math.Ceiling(* / (125d * 1000 / 8 * (6d / 8) *0.75 )));               
                st = (int)(Math.Ceiling(flashSize / 1024d * 0.172)); //0.172s/K(new Firmware)   //0.56s/k is one evalution speed.
            }
            else
            {
                st = (int)(Math.Ceiling(flashSize * spendSecond/ receivedSize) );               
            }

            return FormatTimeString(st);
        }

        /// <summary>
        /// Format time in second to string.
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        private static string FormatTimeString(int st)
        {
            string showMessage = "";
            int hour = st / 3600;
            int minute = st % 3600 / 60;
            int second = st % 3600 % 60;

            if ( hour > 0 )
            {
                string hourStr = hour + "";
                if ( hourStr.Length < 2 )
                {
                    showMessage += " ";
                }
                showMessage += hourStr + " hr ";
            }
            if ( minute > 0 )
            {
                string minStr = minute + "";
                if ( minStr.Length < 2 )
                {
                    showMessage += " ";
                }

                showMessage += minStr + "  min ";
            }
            string secStr = second + "";
            if ( secStr.Length < 2 )
            {
                showMessage += " ";
            }

            showMessage += secStr + " sec";

            return showMessage;
        }

        /// <summary>
        /// selected node list which are going to upload flash.
        /// </summary>
        private List<NodeIdentityEnum> uploadingSelectedList = new List<NodeIdentityEnum>();
        /// <summary>
        /// folder to save uploaing log file.
        /// </summary>
        private string logSaveFolder = null;

         /// <summary>
        /// flash upload process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void messageSendFunction(object sender, EventArgs e)
        {
            string tag = this.button_action.Tag as string;
            if (tag == "Upload")
            {
                if (this._runStopFlag)
                {
                    this.LogUploaingEvent(sender, e);
                }
                else
                {
                      string confirmMsgToForceClose = "Are you sure you want to stop uploading?";
                      if ( MessageBoxEX.Show(this, confirmMsgToForceClose, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
                      {
                          if (!this._runStopFlag && isBKGWorking)
                          {
                              this.button_action.Enabled = false;
                              this._runStopFlag = true;
                              ConstInfo.SaveLogToFile("Log uploading CANCELLED.");
                          }
                      }
                }
                
            }
            else if (tag == "Erase")
            {
                if (this._runStopFlag)
                {
                    this.LogEraseEvent(sender, e);
                }
                else
                {
                    string confirmMsgToForceClose = "Erasing is not completed. If you stop erasing, the tool WILL NOT work properly!\r\n Do you still want to stop it?";
                    if ( MessageBoxEX.Show( this,confirmMsgToForceClose, "Log data erase stop confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) == DialogResult.Yes )
                    {
                        //this.button_eraseCancel.Visible = false;
                        string message = "Erasing is not completed. Tool will NOT work properly \r\n.You have to erase again before using.";

                        MessageBoxEX.Show( this, message, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning );
                        if (!this._runStopFlag && isBKGWorking)
                        {
                            this.button_action.Enabled = false;
                            this._runStopFlag = true;
                            ConstInfo.SaveLogToFile("LOG ERASE CANCELLED-" + message);
                        }
                    }                    
                }
            }
        }

        /// <summary>
        /// flash upload process
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogUploaingEvent(object sender, EventArgs e)
        {
            
            //check input valid
            if (!checkTextValid())
            {
                return;
            }
          
            //get all node need to uploading.
            this.uploadingSelectedList.Clear();
            bool _isBatteryChecked = false;
            foreach (CheckBox box in this._nodeSelectCheckBox.Values)
            {

                if (box.Checked)
                {
                    NodeIdentityEnum id = (NodeIdentityEnum)box.Tag;
                    if (id == NodeIdentityEnum.node_battery)
                    {
                        _isBatteryChecked = true;
                    }
                    else
                    {
                        this.uploadingSelectedList.Add(id);
                    }
                }
            }
            //add battery node to uploading selected list to the end if battery node is seleted.
            if (_isBatteryChecked) this.uploadingSelectedList.Add(NodeIdentityEnum.node_battery);

            if (this.uploadingSelectedList.Count == 0) return;

            int totalFlashSize = 0;
            StringBuilder flashSizeUnknow = new StringBuilder();
            foreach (NodeIdentityEnum nodea in this.uploadingSelectedList)
            {
                int fs = (int)this._infoShowBoxes[(int)nodea][_indexFlashSize].Tag;
                if (fs < 0)
                {
                    if (flashSizeUnknow.Length > 0)
                    {
                        flashSizeUnknow.Append(",");
                    }
                    flashSizeUnknow.Append(ConstInfo.getNodeStrName(nodea));
                }
                else
                {
                    totalFlashSize += fs;
                }
            }            
            
            //check upload time.
            string showMessage = "";
            if (flashSizeUnknow.Length > 0)
            {
                showMessage += "Can't get flash size from "+ flashSizeUnknow.ToString()+". Maybe the firmware doesn't support flash size function.\r\n";
            }
            this._flashSize_total = 0;
            this._flashReceivedSize_total = 0;
            if(totalFlashSize > 0)
            {
                this._flashSize_total = totalFlashSize;
                /*
                if (showMessage.Length > 0)
                {
                    showMessage += "And other nodes' estimated";
                }
                else
                {
                    showMessage += "Estimated";
                }*/
                showMessage += "Estimated log upload time is " + GetTimeString( totalFlashSize, 0, DateTime.Now ) + ". ";
            }



            showMessage += "Press Yes to continue, or No to cancel.";
            if ( MessageBoxEX.Show(this, showMessage, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) != DialogResult.Yes )
            {
                return;
            }

            try
            {
                if ( JobInfo.JobID != null && JobInfo.JobID != "-1" )
                {
                    using ( fmJobRun jobRun = new fmJobRun() )
                    {
                        jobRun.IsNewRun = false;
                        jobRun.AllowNewRun = false;
                        jobRun.StartPosition = FormStartPosition.CenterParent;
                        jobRun.ShowDialog( this );
                    }
                }
            }
            catch { }


            //*/            
            //select file folder.
            FolderBrowserDialog dialog = new FolderBrowserDialog();
            string lastLogSavePathKey = "LastLogSavePath";
            EVOAppSettings evoSetting = new EVOAppSettings(LoggingDownloadForm.AppSettingFileName,false);
            string _lastSavePath = evoSetting[lastLogSavePathKey];
            if (_lastSavePath != null)
            {
                dialog.SelectedPath = _lastSavePath;
            }
            
            DialogResult dialogResult = dialog.ShowDialog(this);
            if (dialogResult != System.Windows.Forms.DialogResult.OK) return;
            this.logSaveFolder = dialog.SelectedPath;

            if (_lastSavePath != this.logSaveFolder)
            {
                evoSetting[lastLogSavePathKey] = this.logSaveFolder;
            }

            

            #region start process info showing
            foreach (CheckBox box in this._nodeSelectCheckBox.Values)
            {
                NodeIdentityEnum id = (NodeIdentityEnum)box.Tag;
                if (box.Checked)
                {

                    this._infoShowBoxes[(int)id][_indexStatus].Text = "WAITING...";
                }
                else
                {
                    //this.infoShowBoxs[(int)id][_indexStatus].Text = "";
                }
            }

            this.StartBGWPreviousWork();
            this.labelBox_logDir.Text = "Log Path: " + this.logSaveFolder;
            this.transmissionManager.IsSoftwarePowerRunning = false;            
            //this.button_eraseCancel.Visible = false;
            #endregion

            this.process_display1.Visible = true;
            this.process_display2.Visible = true;
            this.logUploadingBGWorker.RunWorkerAsync();

        }
        
        private Dictionary<int, BoardInformation> boarderInformations = new Dictionary<int, BoardInformation>();          

        private void LogDownloadFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.closeFrame();
        }

        private void button6_Click(object sender, EventArgs e)
        {
            this.closeFrame();
        }

        private void closeFrame()
        {
            if (this.isBKGWorking)
            {
                MessageBoxEX.Show( this, "Please wait for logging upload process to complete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                this.Visible = false;
                this.transmissionManager.BatteryPowerControl(false,false);
                this.isRunning = false;
                if (this.transmissionManager != null)
                {
                    this.transmissionManager.disconnectCanBus();
                    this.transmissionManager.Dispose();
                }
                this.Dispose();
            }
        }


        /// <summary>
        /// node selete check box event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogTotalFlashSizeRecalcuateEvent(object sender, EventArgs e)
        {
            this.RecalcuteTotalLogDisplay();
            this.CalculateEraseEstimatedTime();
        }

        /// <summary>
        /// reset the total log size diaplsy.
        /// </summary>
        private void RecalcuteTotalLogDisplay()
        {
            int totalSelectedSize = 0;
            foreach (int nodeId in this._avalidNodesList)
            {

                if (this._nodeSelectCheckBox[nodeId].Checked)
                {
                    object tag = this._infoShowBoxes[nodeId][this._indexFlashSize].Tag;
                    if (tag == null) continue;
                    int logSize = (int)tag;
                    if (logSize > 0)
                    {
                        totalSelectedSize += logSize;
                    }
                }
            }


            this.Invoke((ThreadStart)delegate()
            {
                this._infoShowBoxes[_dudmyTotalSummaryId][this._indexFlashSize].Text = this.GetSDataSizeString(totalSelectedSize);
                this._infoShowBoxes[_dudmyTotalSummaryId][this._indexFlashUploadingEstimateTime].Text = GetTimeString(totalSelectedSize, 0, DateTime.Now);
            });

        }


        /// <summary>
        /// node container for log erase 
        /// </summary>
        List<NodeIdentityEnum> _eraseNodeList = new List<NodeIdentityEnum>();

        /// <summary>
        /// erase button event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogEraseEvent(object sender, EventArgs e)
        {         
            _eraseNodeList.Clear();
            if (!checkTextValid())
            {
                return;
            }

            foreach (NodeIdentityEnum node in ConstInfo.ToolNodeList)
            {
                if (this._nodeSelectCheckBox[(int)node].Checked)
                {
                    _eraseNodeList.Add(node);
                }
            }           
            if (_eraseNodeList.Count <= 0) return;
            string nodesStr = "";
            foreach (NodeIdentityEnum nodeE in _eraseNodeList)
            {
                if (nodesStr.Length != 0)
                {
                    nodesStr += ",";
                }
                nodesStr += ConstInfo.getNodeStrName(nodeE);
            }

            string confirmMsg = "Do you want to continue to erase [" + nodesStr + "] log?";
            if ( MessageBoxEX.Show(this, confirmMsg, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question ) != DialogResult.Yes )
            {
                return;
            }
          
            if (clearCommandProcessBarBackGroundWorker == null)
            {
                clearCommandProcessBarBackGroundWorker = new BackgroundWorker();
                clearCommandProcessBarBackGroundWorker.DoWork += this.dowork_EraseProcessUpdate;
                clearCommandProcessBarBackGroundWorker.RunWorkerCompleted += this.sendComplete;
            }

            //this.clearNodeAddress = address;

            this.StartBGWPreviousWork();
            //this.button_eraseCancel.Visible = true;

            this._eraseProcessNotConnectedNodeRecord.Clear();

            this.isBKGWorking = true;

            TPCANStatus stsResult;
            //board infomation request  to make sure the node is alive or not
            TPCANMsg NodeAliveCheckMsg = this.createMsg(0, null, 0);
            NodeAliveCheckMsg.LEN = 2;
            NodeAliveCheckMsg.DATA[0] = Command_boardInfoID;
            NodeAliveCheckMsg.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;

            TPCANMsg flashSizeToEraseMsg = this.createMsg( 0, null, 0 );
            flashSizeToEraseMsg.LEN = 2;
            flashSizeToEraseMsg.DATA[ 0 ] = Command_flashSize;
            flashSizeToEraseMsg.DATA[ 1 ] = (byte)ConstInfo.SurfaceCanBusAddr;

            erasingPercentageRecord.Clear();
            List<NodeIdentityEnum> eraseCommandSendNodes = new List<NodeIdentityEnum>();
            //send all other node to erase flag  except battery node.
            foreach (NodeIdentityEnum nodeE in _eraseNodeList)
            {
                
                NodeAliveCheckMsg.ID = (uint)nodeE;
                stsResult = sendPCANData(ref NodeAliveCheckMsg);
                BoardInformation boarderInfoBytes = this.transmissionManager.receiveBoardInformation(ConstInfo.SurfaceCanBusAddr, (int)nodeE, Command_boardInfoID);// this.receiveBoardInformation_new(nodeId);
                if (boarderInfoBytes == null || !boarderInfoBytes.IsHaveData)
                {
                    // no this node. to do something.                   
                    _eraseProcessNotConnectedNodeRecord.Add(nodeE);
                    this._showMessages((int)nodeE, " cannot be connected!", false);
                    this._infoShowBoxes[(int)nodeE][_indexStatus].Text = "FAIL to ERASE (DISCONNECTED)";
                }
                else
                {
                    flashSizeToEraseMsg.ID = (uint)nodeE;
                    sendPCANData( ref flashSizeToEraseMsg );
                    int resLen = 0;
                    byte[] res = this.transmissionManager.getCanBusResponData( ConstInfo.SurfaceCanBusAddr, (int)nodeE, Command_flashSize, 0.5, ref resLen );
                    int flashTotal = 0x1FF5;
                    if ( resLen >= 8 )
                    {
                        int sz = res[ 6 ] + ( res[ 7 ] << 8 );
                        if ( sz > 0 )
                        {
                            flashTotal = sz;
                        }                            
                    }
                    erasingPercentageRecord.Add( (int)nodeE, new int[] { flashTotal,0} );

                    if (nodeE != NodeIdentityEnum.node_battery)
                    {
                        eraseCommandSendNodes.Add(nodeE);

                        this._infoShowBoxes[(int)nodeE][_indexStatus].Text = "ERASING...";
                    }
                    else
                    {
                        this._infoShowBoxes[(int)nodeE][_indexStatus].Text = "WAITING...";
                    }
                }
            }



            //this.sendPCANData(ref clearLogCommand);
            Thread resThread = new Thread(LogClearResponseDetect);
            resThread.Start();

            //set as erase start time.
            this._flashEraseStartTime = DateTime.Now;

            TPCANMsg clearLogCommand = this.createMsg(0, null, 0);
            clearLogCommand.LEN = 8;
            clearLogCommand.DATA[0] = command_logDataErase;
            clearLogCommand.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;           

            foreach (NodeIdentityEnum nodeE in eraseCommandSendNodes)
            {
                if (nodeE == NodeIdentityEnum.node_battery) continue;
                clearLogCommand.ID = (byte)nodeE;
                this._showMessages((int)nodeE, " begin to erase flash log.", false);
                this.sendPCANData(ref clearLogCommand);
                Thread.Sleep(20);// to wait for 20ms.
            }

            this.process_display1.Visible = false;
            this.process_display2.Visible = true;

            clearCommandProcessBarBackGroundWorker.RunWorkerAsync();
        }

        
        /// <summary>
        /// when do log earse, the node which can be connected will saved in this list.
        /// </summary>
        private List<NodeIdentityEnum> _eraseProcessNotConnectedNodeRecord = new List<NodeIdentityEnum>();
        /// <summary>
        /// Erasing process percentage.
        /// </summary>
        private Dictionary<int, int[]> erasingPercentageRecord = new Dictionary<int, int[]>();

        /// <summary>
        /// to force stop erasing flash log.
        /// </summary>
        private bool _runStopFlag = false;

        /// <summary>
        /// To waint clear response command
        /// </summary>
        private void LogClearResponseDetect()
        {
            this.isBKGWorking = true;
            bool isContainBattery = false;
            this._runStopFlag = false;

            Dictionary<NodeIdentityEnum, bool> eraseIDResult = new Dictionary<NodeIdentityEnum, bool>();
            Dictionary<NodeIdentityEnum, bool> eraseIDs = new Dictionary<NodeIdentityEnum, bool>();
            foreach (NodeIdentityEnum nodeE in _eraseNodeList)
            {
                if (nodeE == NodeIdentityEnum.node_battery)
                {
                    isContainBattery = true;
                }
                else
                {
                    eraseIDs.Add(nodeE,false);
                }
            }

            if (this._eraseProcessNotConnectedNodeRecord.Count > 0)
            {
                foreach (NodeIdentityEnum ne in _eraseProcessNotConnectedNodeRecord)
                {
                    eraseIDs.Remove(ne);
                }
            }
            DateTime endTime = DateTime.Now.AddSeconds(clearCommandTimeOut);            

            string successMsg = " flash log erased successfully.";
            string failMsg =  " failed to erase flash log.";
            while (!_runStopFlag &&DateTime.Now < endTime && eraseIDs.Count > 0)
            {
                int len = 0;
                byte[] data = this.transmissionManager.getCanBusResponData(ConstInfo.SurfaceCanBusAddr, -1, -1, 1,ref len);
                if (len >= 3)
                {
                    int sourceID = data[ 1 ];
                    if ( data[ 0 ] == command_logDataErase )
                    {
                        NodeIdentityEnum source = (NodeIdentityEnum)sourceID;
                        if ( eraseIDs.ContainsKey( source ) )
                        {
                            bool isOk = ( data[ 2 ] == 1 );

                            if ( this.erasingPercentageRecord.ContainsKey( sourceID ) )
                            {
                                int[] percentage = this.erasingPercentageRecord[ sourceID ];
                                percentage[ 1 ] = percentage[ 0 ];
                            }

                            this._showMessages( (int)source, ( isOk ? successMsg : failMsg ), false );
                            eraseIDResult.Add( source, isOk );
                            eraseIDs.Remove( source );
                            this.Invoke( (ThreadStart)delegate()
                            {
                                this._infoShowBoxes[ (int)source ][ _indexStatus ].Text = ( isOk ? "ERASED" : "FAIL to ERASE" );
                                if ( isOk )
                                {
                                    this._infoShowBoxes[ (int)source ][ _indexFlashSize ].Tag = 0;
                                    this._infoShowBoxes[ (int)source ][ _indexFlashSize ].Text = "0 KB";
                                    this._infoShowBoxes[ (int)source ][ _indexFlashUploadingEstimateTime ].Text = "0 S";
                                }

                            } );
                        }
                    }
                    else if ( data[ 0 ] == Command_flashSize )
                    {                        
                        if ( len >= 6 )
                        {
                            if ( this.erasingPercentageRecord.ContainsKey( sourceID ) )
                            {
                                this.erasingPercentageRecord[ sourceID ][ 1 ] = (data[ 4 ] | (data[ 5 ] << 8));
                            }
                        }
                    }
                }
            }
            if (eraseIDs.Count > 0)
            {
                foreach (NodeIdentityEnum nodeE in eraseIDs.Keys)
                {
                    this._showMessages((int)nodeE, failMsg, false);
                    eraseIDResult.Add(nodeE, false);
                    this.Invoke((ThreadStart)delegate()
                    {
                        this._infoShowBoxes[(int)nodeE][_indexStatus].Text = (_runStopFlag ? "CANCELLED" : "FAIL to ERASE");
                    });
                }
            }

            if (isContainBattery)
            {                
                bool needSendClear = true;
                foreach (NodeIdentityEnum en in this._eraseProcessNotConnectedNodeRecord)
                {
                    if (en == NodeIdentityEnum.node_battery)
                    {
                        needSendClear = false;
                        break;
                    }
                }

                if (needSendClear)
                {
                    this._flashEraseStartTime = DateTime.Now;
                    bool isOk = false;
                    this.Invoke((ThreadStart)delegate()
                    {
                        this._infoShowBoxes[(int)NodeIdentityEnum.node_battery][_indexStatus].Text = "ERASING...";
                    });
                    if (!_runStopFlag)
                    {
                        TPCANMsg clearLogCommand = this.createMsg(0, null, 0);
                        clearLogCommand.LEN = 8;
                        clearLogCommand.ID = (byte)NodeIdentityEnum.node_battery;
                        clearLogCommand.DATA[0] = command_logDataErase;
                        clearLogCommand.DATA[1] = (byte)ConstInfo.SurfaceCanBusAddr;
                        this._showMessages((int)NodeIdentityEnum.node_battery, " begins to erase flash log.", false);
                        this.sendPCANData(ref clearLogCommand);
                        endTime = DateTime.Now.AddSeconds(clearCommandTimeOut);
                        int batteryId = (int)NodeIdentityEnum.node_battery;

                        int[] batteryErasePercentage = this.erasingPercentageRecord[batteryId];

                        while (!_runStopFlag && DateTime.Now < endTime)
                        {
                            int len = 0;
                            byte[] data = this.transmissionManager.getCanBusResponData( ConstInfo.SurfaceCanBusAddr, batteryId, -1, 1, ref len );
                            if (len >= 3)
                            {
                                if ( data[ 0 ] == command_logDataErase )
                                {
                                    NodeIdentityEnum source = (NodeIdentityEnum)data[ 1 ];
                                    isOk = ( data[ 2 ] == 1 );
                                    batteryErasePercentage[ 1 ] = batteryErasePercentage[ 0 ];
                                    break;
                                }
                                else if ( data[ 0 ] == Command_flashSize )
                                {
                                    if ( len >= 6 )
                                    {
                                        batteryErasePercentage[ 1 ] = ( data[ 4 ] | ( data[ 5 ] << 8 ) );
                                    }
                                }
                            }
                        }
                    }
                    this.Invoke((ThreadStart)delegate()
                    {
                        this._infoShowBoxes[(int)NodeIdentityEnum.node_battery][_indexStatus].Text = isOk ? "ERASED" : ((_runStopFlag ? "CANCELLED" : "FAIL to ERASE"));

                        if ( isOk )
                        {
                            this._infoShowBoxes[ (int)NodeIdentityEnum.node_battery ][ _indexFlashSize ].Tag = 0;
                            this._infoShowBoxes[ (int)NodeIdentityEnum.node_battery ][ _indexFlashSize ].Text = "0 KB";
                            this._infoShowBoxes[ (int)NodeIdentityEnum.node_battery ][ _indexFlashUploadingEstimateTime ].Text = "0 S";
                        }

                    });

                    this._showMessages((int)NodeIdentityEnum.node_battery, (isOk ? successMsg : failMsg), false);
                    eraseIDResult.Add(NodeIdentityEnum.node_battery, isOk);
                }
            }

            this.isBKGWorking = false ;
            this.Invoke((ThreadStart)delegate()
            {
                this.EndBGWWorker();
                this.setParseProgressBarValue(100);
                StringBuilder successNode = new StringBuilder();
                StringBuilder failNode = new StringBuilder();

                successNode.Append("[");
                failNode.Append("[");
                foreach (NodeIdentityEnum nodeE in eraseIDResult.Keys)
                {
                    bool isOk = eraseIDResult[nodeE];
                    if (isOk)
                    {
                        if (successNode.Length > 1)
                        {
                            successNode.Append(" , ");
                        }
                        successNode.Append(ConstInfo.getNodeStrName(nodeE));

                    }
                    else
                    {
                        if (failNode.Length > 1)
                        {
                            failNode.Append(" , ");
                        }
                        failNode.Append(ConstInfo.getNodeStrName(nodeE));
                    }
                }

                string message = "";
                if (successNode.Length > 1)
                {
                    successNode.Append("]");
                    message += "The flash log of " + successNode + " have been erased successfully.\r\n";
                    this._showMessages(-1, message, false);
                }
                if (failNode.Length > 1)
                {
                    failNode.Append("]");
                    if (message.Length > 0)
                    {
                        message += "\r\n";
                    }
                    string messageB = "Failed to erase flash log of " + failNode + " !";
                    this._showMessages(-1, messageB, false);
                    message += messageB;
                }
                if (this._eraseProcessNotConnectedNodeRecord.Count > 0)
                {
                    if (message.Length > 0)
                    {
                        message += "\r\n";
                    }
                    String lostConnectNode = "";
                    foreach (NodeIdentityEnum ne in this._eraseProcessNotConnectedNodeRecord)
                    {
                        if (lostConnectNode.Length > 0)
                        {
                            lostConnectNode += ",";
                        }
                        lostConnectNode += ConstInfo.getNodeStrName(ne);
                    }
                    string messageC = "Failed to connect to [ " + lostConnectNode + " ] for log erasing.";
                    this._showMessages(-1, messageC, false);
                    message += messageC;
                }

                ConstInfo.SaveLogToFile("LOG ERASE-"+message);
                MessageBoxEX.Show( this, message, "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
            });
        }
        
        private int clearCommandTimeOut = 410;
        /// <summary>
        /// background worker used for sending log data clear command to update the process bar.
        /// </summary>
        private BackgroundWorker clearCommandProcessBarBackGroundWorker = null;

        private void dowork_EraseProcessUpdate(object sender, DoWorkEventArgs e)
        {
            EraseProcessUpdate( sender, e );
        }

        private void EraseProcessUpdate( object sender, DoWorkEventArgs e )
        {
            int flashNeedEraseTotalSize = 0;
            bool isBatteryWithIn = false;
            bool isBatteryOnly = false;
            int batteryID = (int) NodeIdentityEnum.node_battery;
            foreach ( int key in this.erasingPercentageRecord.Keys )
            {
                if ( key == batteryID )
                {
                    isBatteryWithIn = true;
                }
                else
                {
                    flashNeedEraseTotalSize += this.erasingPercentageRecord[ key ][ 0 ];
                }
            }
            isBatteryOnly = isBatteryWithIn && ( this.erasingPercentageRecord.Count == 1 );
            
            if ( flashNeedEraseTotalSize == 0) flashNeedEraseTotalSize = 0x1FF5; ;

            int sleepTime = 1000;            
            while ( this.isBKGWorking && !this._runStopFlag )
            {
                int totalCleared = 0;
                double batteryHalfPercentage = 0;
                foreach ( int key in this.erasingPercentageRecord.Keys )
                {
                    int[] op = this.erasingPercentageRecord[ key ];
                    if ( key == batteryID )
                    {
                        if ( op[ 0 ] > 0 )
                        {
                            batteryHalfPercentage = ( op[ 1 ] * 50d ) / op[ 0 ];
                        }
                    }
                    else
                    {
                        totalCleared += op[ 1 ];
                    }
                }
                int erasingProcessPercentage = 0;
                string remainingTime = null;

                DateTime now = DateTime.Now;
                double tSpanInSecond = (now - this._flashEraseStartTime).TotalSeconds;
                if ( isBatteryOnly )
                {
                    erasingProcessPercentage = (int)( batteryHalfPercentage * 2 );
                    if ( batteryHalfPercentage <= 1 )
                    {
                        remainingTime = FormatTimeString( perNodeEraseDefaultSpendTimeInSecond );
                    }
                    else
                    {
                        remainingTime =FormatTimeString( (int)( tSpanInSecond * ( 50 - batteryHalfPercentage ) / batteryHalfPercentage ));
                    }

                }
                else if ( isBatteryWithIn )
                {
                    double finishiedPercentage = ( totalCleared * 100d / flashNeedEraseTotalSize );
                    erasingProcessPercentage = (int)( finishiedPercentage *0.5 + batteryHalfPercentage );
                    if ( finishiedPercentage <= 2 ) // not start.
                    {
                        remainingTime = FormatTimeString( perNodeEraseDefaultSpendTimeInSecond * 2);
                    }
                    else if ( totalCleared == flashNeedEraseTotalSize ) // EM/MP/.../ finined
                    {
                        if ( batteryHalfPercentage <= 1 ) //half
                        {
                            remainingTime = FormatTimeString( perNodeEraseDefaultSpendTimeInSecond );
                        }
                        else
                        {
                            remainingTime = FormatTimeString( (int)( tSpanInSecond * ( 50 - batteryHalfPercentage ) / batteryHalfPercentage ) );
                        }
                    }
                    else // EM/MP..  in erasing process.
                    {
                        remainingTime = FormatTimeString( (int)( ( tSpanInSecond * ( 100 - finishiedPercentage ) / finishiedPercentage ) ) + perNodeEraseDefaultSpendTimeInSecond );
                    }

                }
                else
                {
                    double finishiedPercentage =  ( totalCleared * 100d / flashNeedEraseTotalSize ) ;
                    erasingProcessPercentage = (int)finishiedPercentage;

                    if ( finishiedPercentage <= 2 )
                    {
                        remainingTime = FormatTimeString( perNodeEraseDefaultSpendTimeInSecond );
                    }
                    else
                    {
                        remainingTime = FormatTimeString( (int)( tSpanInSecond * ( 100 - finishiedPercentage ) / finishiedPercentage ) );
                    }
                }
                
                if ( erasingProcessPercentage < 0 ) erasingProcessPercentage = 0;
                if ( erasingProcessPercentage > 100 ) erasingProcessPercentage = 100;


                this.Invoke( (ThreadStart)delegate()
                {
                    this.setParseProgressBarValue( erasingProcessPercentage );
                    this.label_remainingTime_total.Text = remainingTime + " remaining";
                } );
                if ( erasingProcessPercentage > 100 )
                {
                    break;
                }
                Thread.Sleep( sleepTime );
            }

        }
                

        private void LoggingDownloadForm_Load(object sender, EventArgs e)
        {
            ReloadNodesInformation();            
        }

        private void RefreshButton_clickEvent(object sender, EventArgs e)
        {
            ReloadNodesInformation();
        }

        /// <summary>
        /// Get Nodes board information
        /// </summary>
        private void ReloadNodesInformation()
        {
            if (borderInformationRequestBGWorker.IsBusy) return;
            if (!this.transmissionManager.IsConnectToCanBus)
            {
                MessageBoxEX.Show( this, "Failed to connect to CAN bus.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                return;
            }
            this.button_action.Enabled = false;
            this.StartBGWPreviousWork();

            this.tableLayoutPanel_message.SuspendLayout();
            foreach ( NodeIdentityEnum node in ConstInfo.ToolNodeList )
            {
                Label[] lables = this._infoShowBoxes[(int)node];
                lables[ this._indexFlashSize ].Tag = 0;
                lables[ this._indexFlashSize ].Text = "";
                lables[ _indexConnect ].Text = "";
                lables[ _indexConnect ].BackColor = this.BackColor;
                lables[ _indexName ].BackColor = this.BackColor;
                lables[ _indexFlashSize ].Tag = -1;
                lables[ _indexFlashSize ].Text = "";
                lables[ _indexFlashUploadingEstimateTime ].Text = "";
                CheckBox cb = _nodeSelectCheckBox[ (int)node ];
                cb.Enabled = false;
                cb.Checked = false;
            }
            this.tableLayoutPanel_message.ResumeLayout();

            //this.button_eraseCancel.Visible = false;
            if ( isNeedInitRun )
            {
                //isNeedInitRun = false;
                borderInformationRequestBGWorker.RunWorkerAsync();
            }
        }

        bool isNeedInitRun = true;
        
        /// <summary>
        /// Node select checkbox click event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CheckEvent_Click_1(object sender, EventArgs e)
        {
            Button b = (Button)sender;
            int tag = (int)b.Tag;
            switch (tag)
            {
                case 0://uncheck all
                    foreach (CheckBox box in this._nodeSelectCheckBox.Values)
                    {
                        box.Checked = false;
                    }
                    break;
                case 1:// check all valid
                    foreach (int nid in this._avalidNodesList)
                    {
                        this._nodeSelectCheckBox[nid].Checked = true;
                    }
                    break;
            }
        }
        /// <summary>
        /// Log file convert form.
        /// </summary>
        private LogFileConvertForm parseForm = null;
        /// <summary>
        /// Convert tool strip menu item clik.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void converterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (parseForm == null || parseForm.IsDisposed)
            {
                parseForm = new LogFileConvertForm();
                //parseForm.Owner = this;
                parseForm.ShowInTaskbar = true;
                parseForm.StartPosition = FormStartPosition.CenterParent;

                Utilities.SetFormStartPositionToCenter( parseForm, this );

            }
            if (parseForm.WindowState == FormWindowState.Minimized)
            {
                parseForm.WindowState = FormWindowState.Normal;
            }
            parseForm.Show();
            parseForm.BringToFront();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.closeFrame();
        }

        private void infomationBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (this.listBox_Info.Visible)
            {
                this.Height -= this.listBox_Info.Height;
                this.listBox_Info.Visible = false;
                this.tableLayoutPanel_process.RowStyles[1].Height = 0;
            }
            else
            {
                this.Height += this.listBox_Info.Height;
                this.listBox_Info.Visible = true;
                this.tableLayoutPanel_process.RowStyles[1].Height = this.listBox_Info.Height;
            }
        }

        /// <summary>
        /// event for radio button upload
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_upload_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_upload.Checked)
            {
                this.button_action.Text = this.radioButton_upload.Text;
                this.button_action.Tag = this.radioButton_upload.Text;
                this.labelBox_logDir.Text = "";
            }
        }
        /// <summary>
        /// event for radio button erase
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_erase_CheckedChanged(object sender, EventArgs e)
        {
            if (this.radioButton_erase.Checked)
            {
                this.button_action.Text = this.radioButton_erase.Text;
                this.button_action.Tag = this.radioButton_erase.Text;
                CalculateEraseEstimatedTime();
            }
        }

        /// <summary>
        /// Calculate Erase estimated time.
        /// </summary>
        private void CalculateEraseEstimatedTime()
        {
            if ( !this.radioButton_erase.Checked ) return;
            if ( this._nodeSelectCheckBox == null ) return;

            bool isSelectBattery = false;
            bool isSelectOther = false;

            foreach ( CheckBox box in this._nodeSelectCheckBox.Values )
            {
                if ( isSelectBattery && isSelectOther ) break;
                if ( box.Checked )
                {
                    NodeIdentityEnum id = (NodeIdentityEnum)box.Tag;
                    if ( id == NodeIdentityEnum.node_battery )
                    {
                        isSelectBattery = true;
                    }
                    else
                    {
                        isSelectOther = true;
                    }
                }
            }

            int eraseTotalTime = this.perNodeEraseDefaultSpendTimeInSecond; ;
            if ( isSelectBattery && isSelectOther )
            {
                eraseTotalTime *= 2;
            }
            if ( isSelectBattery || isSelectOther )
            {
                this.labelBox_logDir.Text = "Estimated log erase time is " + FormatTimeString( eraseTotalTime ) + ".";
            }
            else
            {
                this.labelBox_logDir.Text = "";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        /// <summary>
        ///  send out tool reset delay canbus message.
        /// </summary>
        /// <param name="isEnable"></param>
        private void SendToolResetDealyCommand( bool isEnable )
        {
            this.toolResetDelayCanMsg.DATA[ 2 ] = (byte)( isEnable ? 1 : 0 );
            this.sendPCANData( ref this.toolResetDelayCanMsg );
        }
      
    }

    /// <summary>
    /// It is used for node uploading flag.
    /// </summary>
    class NodeUploadStatus
    {
        /// <summary>
        /// is upload all the data
        /// </summary>
        public bool IsUploaded
        {
            get;
            set;
        }

        /// <summary>
        /// Did the log convert to a CSV file
        /// </summary>
        public bool IsConverted
        {
            get;
            set;
        }

        /// <summary>
        /// is the uploading process cancelled
        /// </summary>
        public bool IsCancelled
        {
            get;
            set;
        }
    }
}
