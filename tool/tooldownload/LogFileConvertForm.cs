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
    public partial class LogFileConvertForm : Form
    {
        /// <summary>
        /// Constructor
        /// </summary>
        public LogFileConvertForm()
        {
            InitializeComponent();
            Initial();
            this.Icon = LoggingDownloadForm.AppFormIcon();
            this.StartPosition = FormStartPosition.CenterScreen;
        }
        #region variables definition
        /// <summary>
        /// Background worker of onverting data to CSV or Hex 
        /// </summary>
        private BackgroundWorker convertDataBGWork;
        /// <summary>
        /// seleted convert format
        /// </summary>
        private int saveDataType = -1; //1: hex 2:scv 3: xls
        /// <summary>
        /// Seleted log file path.
        /// </summary>
        private string logDataFilePath;
        /// <summary>
        /// Export destination file path
        /// </summary>
        private string exportDestFilePath = null;
        /// <summary>
        /// Messages dictionary
        /// </summary>
        private Dictionary<string, MSGModel> AllMessageModels = null;
        /// <summary>
        /// If the messages preparing
        /// </summary>
        private bool GetAllMessages = true;
        /// <summary>
        /// Flag of background worker working status
        /// </summary>
        private bool isBKGWorking = false;
        /// <summary>
        /// suffix of log file.
        /// </summary>
        private string evoLog_suffix = ".evoLog";   

        #endregion  end of variables definition
        /// <summary>
        /// Initialize
        /// </summary>
        private void Initial()
        {  
            PrepareAllMessageList();
            this.radioButton_csv.Checked = true;

            Utilities.ResizeButtons(this);

            convertDataBGWork = new BackgroundWorker();
            convertDataBGWork.DoWork += new DoWorkEventHandler(LogConvertProcessEntrance);
            convertDataBGWork.RunWorkerCompleted += new RunWorkerCompletedEventHandler(ConvertBackgroundWorkerComplete);
        }
        
        /// <summary>
        /// Prepare all messages list
        /// </summary>
        private void PrepareAllMessageList()
        {
            AllMessageModels = new Dictionary<string,MSGModel>();
            List<MSGModel> MsgsList = new List<MSGModel>();

            Thread oThread = new Thread(delegate()
            {
                try
                {
                    bool isConnectedDB = ConstInfo.GetAllValidMessages(ref MsgsList);
                    foreach (MSGModel m in MsgsList)
                    {
                        AllMessageModels[m.NodeAddress + "-" + m.Command] = m;
                    }
                    if (!isConnectedDB)
                    {
                        this.Invoke((ThreadStart)delegate()
                        {
                            MessageBoxEX.Show( this, "Failed to get all messages definition.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                        });
                    }
                }
                catch{}
                this.GetAllMessages = false;
            });
            oThread.Start();
        }
        
        /// <summary>
        /// Previous work for background worker
        /// </summary>
        private void PreworkForBGW()
        {
            this.isBKGWorking = true;
            this.Invoke((ThreadStart)delegate()
               {
                   this.panel_operation2.Enabled = false;
                   this.progressBar_convert.Value = 0;
                   this.label_leftTime.Text = "";
               });
        }
        /// <summary>
        /// Post work for background worker
        /// </summary>
        private void PostworkForBGW()
        {    
            try
            {
                this.Invoke((ThreadStart)delegate()
               {                   
                   this.progressBar_convert.Value = 0;           
                   this.panel_operation2.Enabled = true;
                   this.label_leftTime.Text = "";
               });                
            }
            catch { }
            this.isBKGWorking = false;
        }        
        /// <summary>
        /// set progress bar value
        /// </summary>
        /// <param name="value"></param>
        public void setProgressBarValue(int value)
        {
            if (value > 100) value = 100;
            this.Invoke((ThreadStart)delegate()
            {
                this.progressBar_convert.Value = value;                
            });
        }
              
        /// <summary>
        /// Log convert background worker complete event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConvertBackgroundWorkerComplete(object sender, RunWorkerCompletedEventArgs e)
        {
            this.PostworkForBGW();
        }      
            

        /// <summary>
        /// Form closing event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogFileParseFormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = true;
            this.closeForm();
        }

     
        /// <summary>
        /// Close form
        /// </summary>
        private void closeForm()
        {
            if (this.isBKGWorking)
            {
                MessageBoxEX.Show( this, "Please wait for log convert process to complete.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
            }
            else
            {
                this.Visible = false;
                this.Dispose();
            }
        }

        
        /// <summary>
        /// Export button click event
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ExportButton_Click(object sender, EventArgs e)
        {
            this.setProgressBarValue(0);

            if (this.radioButton_hex.Checked)
            {
                this.saveDataType = 1;
            }
            else if (this.radioButton_csv.Checked)
            {
                this.saveDataType = 2;
            }
            else
            {
                this.saveDataType = 3;
            }
            /* test begin
            LogDataParser.parseLogData(null, null);
            return;
            //test end */
            if (this.textBox_logDataFile.Text.Length <= 0)
            {
                MessageBoxEX.Show( this, "Please select log data file first.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
                return;
            }
            logDataFilePath = this.textBox_logDataFile.Text;
            int bi = logDataFilePath.LastIndexOf("\\");
            int bi2 = logDataFilePath.LastIndexOf("/");
            if (bi2 > bi) bi = bi2;
            string resultPath = logDataFilePath.Substring(0, bi);

            int ei = logDataFilePath.LastIndexOf(".");
            string saveFileName = "EVOLogDataFile";
            if (ei - bi > 1)
            {
               saveFileName = logDataFilePath.Substring(bi + 1, ei - bi -1);
            }
            string fix = "xlsx";
            if (this.saveDataType == 1)
            {
                fix = "datahex";
            }
            else if (this.saveDataType == 2)
            {
                fix = "csv";
            }

            //DateTime time = DateTime.Now;
            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = "log data save to file";
            dialog.Filter = "EVO log File(*." + fix + ")|*." + fix;
            dialog.FileName = saveFileName;
            dialog.CheckPathExists = true;
            dialog.InitialDirectory = resultPath;

            DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                this.exportDestFilePath = dialog.FileName;
                //this.destFileWriter = new FileStream(this.destFilePath, FileMode.Create);

                #region start process info showing
                this.PreworkForBGW();
                #endregion

                this.convertDataBGWork.RunWorkerAsync();            
            }
        }
       
        /// <summary>
        /// Log convert process entrance.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogConvertProcessEntrance(object sender, DoWorkEventArgs e)
        {
            this.PreworkForBGW();

            bool isSuccessful = true;
            string errorMsg = "";
            FileStream sourceStream = new FileStream(logDataFilePath, FileMode.Open);
            try
            {
                if (saveDataType == 1)
                {
                    int len = 1000;
                    int readNum = 0;
                    byte[] bytes = new byte[len];
                    StreamWriter writer = new StreamWriter(this.exportDestFilePath);
                    int wl = 0;
                    writer.Write("00000---");

                    int processValue = 1;
                    long all = sourceStream.Length;
                    while ((readNum = sourceStream.Read(bytes, 0, len)) > 0)
                    {
                        for (int i = 0; i < readNum; i++)
                        {
                            writer.Write(bytes[i].ToString("X2"));
                            writer.Write("  ");
                            wl++;
                            if (wl % 10 == 0)
                            {
                                writer.WriteLine();
                                writer.Write(wl.ToString("X5") + "---");
                            }
                        }
                        long persent = sourceStream.Position * 100 / all;
                        if (persent > processValue)
                        {
                            processValue = (int)persent;
                            this.setProgressBarValue(processValue);
                        }
                    }

                }
                else
                {
                    while (this.GetAllMessages)
                    {
                        Thread.Sleep(1);
                    }
                    LogDataParser.ParseProcessBarDelegateEvent += this.setProgressBarValue;
                    isSuccessful = LogDataParser.parseLogData(sourceStream, this.exportDestFilePath, this.AllMessageModels, (saveDataType == 2));
                    LogDataParser.ParseProcessBarDelegateEvent -= this.setProgressBarValue;
                }
                
            }
            catch (Exception ee)
            {
                isSuccessful = false;
                errorMsg += ee.Message;
            }
            try
            {
                this.Invoke((ThreadStart)delegate()
                {
                    if (isSuccessful)
                    {
                        this.setProgressBarValue(100);
                        MessageBoxEX.Show( this, "Log data converted successfully.", "Information", MessageBoxButtons.OK, MessageBoxIcon.Information );
                    }
                    else
                    {
                        MessageBoxEX.Show( this, "Failed to convert log data: " + errorMsg, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                    }
                });
            }
            catch
            {
            }
            sourceStream.Close();
            //this.destFileWriter.Close();
            //this.destFileWriter = null;

            this.setProgressBarValue(0);
        }

        /// <summary>
        /// Click event of log file selecte button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogfileSelectButtion_ClickEvent(object sender, EventArgs e)
        {
            //this.markeloadingInfoDisplay();

            OpenFileDialog dialog = new OpenFileDialog();
            dialog.Title = "Select log data to parse.";
            dialog.Filter = "EVO log File(*"+evoLog_suffix+")|*"+evoLog_suffix;
            dialog.CheckPathExists = true;
            dialog.CheckFileExists = true;
            dialog.Multiselect = false;

            string lastLogSavePathKey = "LastLogSavePath";
            EVOAppSettings evoSetting = new EVOAppSettings(LoggingDownloadForm.AppSettingFileName,false);
            string logSavePath = evoSetting[lastLogSavePathKey];
            if (logSavePath != null)
            {
                dialog.InitialDirectory = logSavePath;
            }

            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string fn = dialog.FileName;
                int bi = fn.LastIndexOf("\\");
                //Utilities.ConfigSet(lastLogSavePathKey, fn.Substring(0, bi));
                evoSetting[lastLogSavePathKey] = fn.Substring(0, bi);

                this.textBox_logDataFile.Text = fn;

            }
        }


       /// <summary>
       /// Form loaded.
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        private void LogFileParseForm_Load(object sender, EventArgs e)
        {
            
        }

      
    }
}
