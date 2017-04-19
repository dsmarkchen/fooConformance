using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Diagnostics;  // for stopwatch
using System.IO;
using System.IO.Ports;
using System.Threading;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Runtime.InteropServices; //for dllimport


namespace FooConformance
{
    #pragma warning disable 414
    public partial class FooConformanceForm : Form
    {
        /// <summary>
        /// back ground worker for running.
        /// </summary>
        BackgroundWorker _bw_run_all;
        PageState _page_state1 = new PageState();
        PageState _page_state2 = new PageState();
        PageState _page_state3 = new PageState();

        LogTo _log = new LogTo();

        public FooConformanceForm()
        {
            InitializeComponent();
            this.Icon = GammaIconExt.GetIco();
            this.StartPosition = FormStartPosition.CenterScreen;            
        }


        private void Datagridview_setup(DataGridView View1, DataSet ds)
        {

            View1.ReadOnly = true;
            View1.RowHeadersVisible = false;
            //View1.RowHeadersVisible = true;
            View1.RowHeadersWidth = 25;
            View1.RowHeadersDefaultCellStyle.BackColor = Color.Blue;
            View1.EnableHeadersVisualStyles = true;
            View1.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;

            View1.ColumnHeadersVisible = false;
            View1.BackgroundColor = Color.LightGray;
            View1.BorderStyle = BorderStyle.Fixed3D;
            View1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
            View1.AllowUserToResizeColumns = false;
            View1.AllowUserToResizeRows = false;
            View1.AllowUserToAddRows = false;
            View1.AllowUserToDeleteRows = false;
            View1.AllowUserToOrderColumns = false;
            //View1.RowsDefaultCellStyle.SelectionBackColor = System.Drawing.Color.Transparent;
            View1.AutoGenerateColumns = true;
            View1.DataSource = ds.Tables[0];
            View1.AutoGenerateColumns = false;
            View1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            View1.MultiSelect = false;
            View1.Columns[0].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            View1.Columns[1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            View1.Columns[2].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            View1.Columns[2].Visible = false;
            View1.Columns[3].Visible = false;
            View1.Columns[4].Visible = false;

            DataGridViewImageColumn img = new DataGridViewImageColumn();
            Icon icon1 = new Icon(SystemIcons.Exclamation, 8, 8);
            img.Icon = icon1;
            img.DefaultCellStyle.NullValue = null;
            View1.Columns.Add(img);

            int i = 0;
            foreach (DataGridViewColumn column in View1.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;

                if (i % 2 == 0) column.DefaultCellStyle.Font = new Font(View1.Font.FontFamily, 8, FontStyle.Bold);

                i++;
            }

        }
        private void Datagridview_add_image_column(DataGridView view)
        {

            DataGridViewImageColumn dgv_column = new DataGridViewImageColumn();
            Icon icon1 = new Icon(SystemIcons.Exclamation, 8, 8);
            dgv_column.Icon = icon1;
            dgv_column.Name = "Result";
            dgv_column.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgv_column.DefaultCellStyle.BackColor = Color.Transparent;
            dgv_column.DefaultCellStyle.SelectionBackColor = Color.Transparent;

            view.Columns.Add(dgv_column);
            
#if true
            // remove default [x] image for data DataGridViewImageColumn columns
            foreach (DataGridViewColumn column in view.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                if (column is DataGridViewImageColumn)
                    (column as DataGridViewImageColumn).DefaultCellStyle.NullValue = null;
            }
#endif
        }

        private DataSet _gamma_TestDS = null;
        String _filename_gamma_test = System.Windows.Forms.Application.StartupPath + "\\foo_verification_input_table.xml";

       
        SymbolicProcessor _sym_engine = null;

        private void Form1_Load(object sender, EventArgs e)
        {
            #region init symbolic processor
            _sym_engine = new SymbolicProcessor();
            SymbolicProcessor.del_delay = this.delay;
            SymbolicProcessor.del_gamma_const = this.Sensor_read_gamma_consts;
            SymbolicProcessor.del_wait_prompt = this.Wait_prompt;
            SymbolicProcessor.del_gamma_cps = this.Sensor_read_gamma_cps;
            #endregion

            #region load verification xml
            try
            {
                this._gamma_TestDS = new DataSet();
                _gamma_TestDS.ReadXmlSchema("test_input.xsd");
                _gamma_TestDS.ReadXml(_filename_gamma_test);

            }
            catch (Exception ex)
            {
                MessageBoxEX.Show(this, "Error -- " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                Console.WriteLine(ex.Message);
            }
            _log.GeneralLog("formload", "load ds done", "");

            Datagridview_setup( dataGridView1, _gamma_TestDS);

            tabPage1.Tag = 0;
           

            #endregion

           

       
            
#if false
            datagridview_add_image_column(dataGridView1);
            datagridview_add_image_column(dataGridView2);
            datagridview_add_image_column(dataGridView3);
#endif
            #region show date and tester
            buttonAbort.Enabled = false;
            labelDate.Text = DateTime.Now.ToString("yyyy-MMM-dd");
            #endregion

            #region setup "run" background worker
            _bw_run_all = new BackgroundWorker();
            _bw_run_all.DoWork += new DoWorkEventHandler(try_run_all_func);
            _bw_run_all.RunWorkerCompleted += new RunWorkerCompletedEventHandler(Run_all_complete_func);
            #endregion

        }
        #region try_run_all_func
        private void try_run_all_func(object sender, DoWorkEventArgs e)
        {
            try
            {
                run_all_func(sender, e);
            }
            catch (Exception ex)
            {
                this.Invoke((ThreadStart)delegate()
                {
                    if (e.Argument == null) {
                    }
                    MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
                });
            }
            finally
            {
                //this.finishTestTime = DateTime.Now;
            }
        }
        private void run_all_func(object sender, DoWorkEventArgs e)
        {
                if (e.Argument == null)
                {
                    int selectIndex = -1;
                    this.Invoke((ThreadStart)delegate()
                    {
                        selectIndex = this.tabControl1.SelectedIndex;
                        current_page = selectIndex;
                    });

                    datagridview_run_all(selectIndex);

                }
                else
                {
                    datagridview_run_item((object[])(e.Argument));
                }
        }

        bool is_verification_done = false;
        bool is_conformance_done = false;
        bool is_calibration_done = false;
        int start_index = 0;
        int current_page = 0;
        private void datagridview_run_all(int selectIndex)
        {
            DataGridView view;
            this._sym_engine.IsStopProcessing = false;
            

            GammaControlExt.IsCompleted = false;

            
                
                    
            is_verification_done = false;
                    
            view = this.dataGridView1;
               
            int jj = 0;
            foreach (DataGridViewRow row in view.Rows)
            {
                if (jj <= start_index)
                {
                    jj++;
                    continue;
                }
                row.Cells[1].Value = "";
                row.Cells[1].Style.BackColor = Color.White;
                row.Cells[5].Value = null;
                row.Cells[5].Style.BackColor = Color.White;
                jj++;
            }

            for (int index = start_index; index < view.Rows.Count; index++)
            {
            case_enter:
                if (this._sym_engine.IsStopProcessing) break;
                this.Invoke((ThreadStart)delegate()
                {
                    view.Rows[index].Selected = true;
                    view.CurrentCell = view[0, index];
                });
                Datagridview_row_bgcolor(view, index);
                String s = view.Rows[index].Cells[0].Value.ToString();

                if (s == null) break;
                run(s, ref view, index);
                start_index = index;
                if (this._sym_engine.IsRetryProcessing)
                {
                    this._sym_engine.IsRetryProcessing = false;
                    Datagridview_row_clear(view, index);
                    Datagridview_row_bgcolor_default(view, index);
                    index -= 1;
                    Datagridview_row_clear(view, index);
                    Datagridview_row_bgcolor_default(view, index);
                    index -= 1;
                    Datagridview_row_clear(view, index);
                    Datagridview_row_bgcolor_default(view, index);
                    goto case_enter;
                }
            }

            if (this._sym_engine.IsStopProcessing) return;
            switch (selectIndex)
            {
                case 0:
                    this._page_state1.set_default();
                    this.start_index = 0;
                    is_verification_done = true;break;


                case 1:
                    this.start_index = 0;
                    this._page_state2.set_default();
                    is_conformance_done = true;break;

                default:
                    this.start_index = 0;
                    this._page_state3.set_default();
                    is_calibration_done = true;break;

            }


            GammaControlExt.IsCompleted = true;
        }
        private void datagridview_run_item(object[] parameters)
        {
            //object[] parameters = (object[])e.Argument;
            int ri = (int)parameters[1];
            DataGridView rv = (DataGridView)parameters[0];
            String s = rv.Rows[ri].Cells[0].Value.ToString();
            run(s, ref rv, ri);
        }
        #endregion


        private void Run_all_complete_func(object sender, RunWorkerCompletedEventArgs e)
        {
            buttonRun.Enabled = true;
            resetCOMPortToolStripMenuItem.Enabled = true;
            GammaControlExt.IsRunning = false;
            this.finishTestTime = DateTime.Now;

            if (GammaControlExt.IsCompleted)
            {
                buttonRun.Text = "Start";
                buttonAbort.Text = "Stop";
                buttonAbort.Enabled = false;
                
            }
            else 
                this.buttonAbort.Text = "Continue";
        }

      

        private void datagridview_update_row(DataGridView View1, int RowIndex, string misc, bool res)
        {
            this.Invoke((ThreadStart)delegate()
               {
                   if (res == true)
                   {
                       View1.Rows[RowIndex].HeaderCell.Value = "o"; // Properties.Resources.Image1;
                       try
                       {
                           DataGridViewImageCell cell = View1.Rows[RowIndex].Cells[5] as DataGridViewImageCell;
                           cell.Value = FooConformance.Properties.Resources.ok;
                       }
                       catch (Exception)
                       { 
                       }

                   }
                   else
                   {
                       View1.Rows[RowIndex].HeaderCell.Value = "x";
                       try
                       {
                           DataGridViewImageCell cell = View1.Rows[RowIndex].Cells[5] as DataGridViewImageCell;
                           cell.Value = FooConformance.Properties.Resources.wrong;
                       }
                       catch (Exception)
                       { 
                       }
                   }
                   View1.Rows[RowIndex].Cells[1].Value = misc;


                   View1.Rows[RowIndex].Cells[2].Value = res.ToString();
               });
        }

        static DataGridView delayView1;
        static int delay_row;

        private void run(string s, ref DataGridView View1, int RowIndex)
        {
            if (GammaControlExt.IsRunning == false) throw new Exception("Stopped!");
            
            if ((s.IndexOf("API/CPS") >= 0)) 
            {
                if (RowIndex < 15) {
                    datagridview_update_row(View1, RowIndex, "N/A", true);
                    return;
                }
                Datagridview_row_clear(View1, RowIndex);
                bool res = true;
                double v1 = 0;
                double v = -1d;
                try
                {
                    if (this._sym_engine.dict.ContainsKey("net_cps"))
                    {
                        double.TryParse(this._sym_engine.dict["net_cps"], out v1);
                    }
                    Console.WriteLine(FooConformance.Properties.Settings.Default.APIFactor.ToString("0.00"));
                    v = FooConformance.Properties.Settings.Default.APIFactor / v1;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    res = false;
                }
                if (res == true)
                {
                    bool b1, b2, b3;
                    bool.TryParse(this._sym_engine.dict["step1ok"], out b1);
                    bool.TryParse(this._sym_engine.dict["step2ok"], out b2);
                    bool.TryParse(this._sym_engine.dict["step3ok"], out b3);
                    if (b1 == true && b2 == true && b3 == true) res = true;
                    else res = false;
                }
                #region api range checking (5 percent close to 1.0)
                if (res == true) {
                    if ((v <= 0.95d) || (v >= 1.05d)) res = false;
                }
                #endregion
                datagridview_update_row(View1, RowIndex, v.ToString("F2"), res);
                return;
            }

            if (s.IndexOf("Net CPS") >= 0)
            {
                Datagridview_row_clear(View1, RowIndex);
                bool res = true;
                double v1 = 0;
                double v2, v3, v;
                try
                {
                    if (this._sym_engine.dict.ContainsKey("step1")) double.TryParse(this._sym_engine.dict["step1"], out v1);
                    double.TryParse(this._sym_engine.dict["step2"], out v2);
                    double.TryParse(this._sym_engine.dict["step3"], out v3);
                    v = v2 - (v1 + v3) / 2;
                }
                catch (Exception ex)
                {
                    res = false;
                    datagridview_update_row(View1, RowIndex, ex.Message, res);
                    return;
                }
                try
                {
                    this._sym_engine.dict.Add("net_cps", v.ToString());
                }
                catch (ArgumentException)
                {
                    this._sym_engine.dict["net_cps"] = v.ToString();
                }
                bool b1, b2, b3;
                bool.TryParse(this._sym_engine.dict["step1ok"], out b1);
                bool.TryParse(this._sym_engine.dict["step2ok"], out b2);
                bool.TryParse(this._sym_engine.dict["step3ok"], out b3);
                if (b1 == true && b2 == true && b3 == true) res = true;
                else res = false;

                datagridview_update_row(View1, RowIndex, v.ToString("F2"), res);
                return;
            }
            #region symbol process one row
            Datagridview_row_clear(View1, RowIndex);
            List<string> ssss = new List<string>();
            string smisc = "";
            bool bres = true;
            string sexp = View1.Rows[RowIndex].Cells[3].Value.ToString();
            bool proc_ret = this._sym_engine.TrySymbolProcess(sexp, View1, RowIndex, ref smisc, ref ssss, ref bres);
            if (ssss.Count > 0) smisc = ssss[0];
            datagridview_update_row(View1, RowIndex, smisc, bres);




            #endregion

            if ((s.IndexOf("Pulseout Mode") >= 0) && (bres == false) && (smisc.IndexOf("Uncorrected") >= 0))
            {
                System.Windows.Forms.DialogResult dlgresult = System.Windows.Forms.DialogResult.Yes;
                this.Invoke((ThreadStart)delegate()
                {
                    dlgresult = MessageBoxEX.Show(this, "Pulseout=Uncorrected. Correct it now?", "Alert",
                        MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                        
                });
                if (dlgresult == System.Windows.Forms.DialogResult.No) return;


                


            }
        }


        //Dictionary<string, string> dict = new Dictionary<string, string>();
        private bool check_valid_string(string s)
        {
            bool res = false;
            if (s != null && s != "") res = true;
            return res;
        }
             
        private void View_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (FooConformance.Properties.Settings.Default.EnableForEngineeringUse == false) return;


            if (e.RowIndex > -1)
            {
                this.buttonRun.Enabled = false;
                if (!GammaControlExt.IsRunning)
                {
                    GammaControlExt.IsRunning = true;
                    this._bw_run_all.RunWorkerAsync(new object[] { (DataGridView)sender, e.RowIndex });
                }

                return;
            }

        }
        private void Datagridview_row_bgcolor(DataGridView View1, int row_index)
        {
#if true
            this.Invoke((ThreadStart)delegate()
            {
                View1.Rows[row_index].Cells[5].Style.BackColor = Color.LightGray;
                View1.Rows[row_index].Cells[1].Style.BackColor = Color.LightGray;// Color.Cornsilk;
                View1.Refresh();
            });
#endif
        }
        private void Datagridview_row_bgcolor_default(DataGridView View1, int row_index)
        {
            this.Invoke((ThreadStart)delegate()
            {
                View1.Rows[row_index].Cells[5].Style.BackColor = Color.White;
                View1.Rows[row_index].Cells[1].Style.BackColor = Color.White;
                View1.Refresh();
            });
        }

        
        private void Datagridview_row_clear(DataGridView View1, int row_index)
        {
            this.Invoke((ThreadStart)delegate()
               {
                   View1.Rows[row_index].HeaderCell.Value = "--";
                   View1.Rows[row_index].Cells[1].Value = "";
                   View1.Rows[row_index].Cells[2].Value = "";
                   View1.Rows[row_index].Cells[5].Value = null;
                   //View1.Rows[row_index].Cells[5].Style.BackColor = Color.White;
                   View1.Refresh();
               });
        }

        string gamma_comport = null;

        static delay_cp_t del_delay = null; // delay
        static foo_consts_cp_t del_gamma_const = null ; 
        static wait_prompt_cp_t del_wait_prompt = null;
        static bool is_delay_running = false;


        private void delay(ref DataGridView View1, int RowIndex, ref bool res, string exec_arg1)
        {
            is_delay_running = true;                        
            delayView1 = View1;
            delay_row = RowIndex;
#if DEBUG
            exec_arg1 = "3";
#endif

            int.TryParse(exec_arg1, out  delay_val);
            while (delay_val != 0)
            {
                Thread.Sleep(1000);
                this.Invoke((ThreadStart)delegate()
                {
                    Console.WriteLine("xxx: " + delay_val);
                    
                    delay_val--;
                    delayView1.Rows[RowIndex].Cells[1].Value = "Wait " + delay_val + " Seconds";
                    delayView1.Update();
                    delayView1.Refresh();
                }
                );
                if (GammaControlExt.IsRunning == false) break;
                
            }
            is_delay_running = false;

        }

        #region callback functions used by symbolic processor

        private void Wait_prompt(ref DataGridView View1, int row_index, ref bool res, string exec_arg1)
        {
            WaitForm form = new WaitForm(exec_arg1);
            form.Icon = this.Icon;
            bool result = false;
            this.Invoke((ThreadStart)delegate()
                {
                    DialogResult dr = form.ShowDialog();
                    if (dr == DialogResult.OK) result = true;
                });
            res = result;
        }
        private void Sensor_read_gamma_consts(ref DataGridView View1, int row_index, ref bool res, out List<string> output)
        {

            output = null;

        }
        string calib_count;
        string calib_rate;
        string calib_temp;


        private void Sensor_read_gamma_cps(ref DataGridView View1, int row_index, ref string misc, ref bool res, out List<string> output)
        {

            output = null;

        }
        #endregion

        DateTime finishTestTime = DateTime.Now;
        #region reporting

        /// <summary>
        /// truncate string to fixed size
        /// </summary>
        /// <param name="value"></param>
        /// <param name="maxLength"></param>
        /// <param name="use_tail">an option to add ".." at the end of string if truncated</param>
        /// <returns></returns>
        public static string StringTruncate(string value, int maxLength, bool use_tail = false)
        {
            string tail = (use_tail == true) ? ".." : "";
            if (string.IsNullOrEmpty(value)) return value;
            return value.Length <= maxLength ? value : value.Substring(0, maxLength) + tail;
        }
        
        private void buttonPrint_Click(object sender, EventArgs e)
        {
            


        }
        #endregion


        private int delay_maximum = 10;
        static private int delay_val = 10;
        private void timer1_Tick(object sender, EventArgs e)
        {
            delay_val--;
            if (delay_val == 0) timer1.Stop();
        }

        private void buttonRun_Click(object sender, EventArgs e)
        {
            #region validating if I need to continue the run
           
            if (this.tabControl1.SelectedIndex == 1)
            {
                int i;
                string run, abort;
                this._page_state3.get(out i, out run, out abort);
                if (i > 0) {
                    MessageBoxEX.Show(this,"Please run calibration!");
                    return;
                }
            
            }
            if (this.tabControl1.SelectedIndex == 2)
            {
                int i;
                string run, abort;
                this._page_state2.get(out i, out run, out abort);
                if (i > 0)
                {
                    MessageBoxEX.Show(this,"Please run conformance!");
                    return;
                }

            }
            #endregion

            #region run
            start_index = 0;
            this._sym_engine.init();
            buttonRun.Enabled = false;
            buttonRun.Text = "Restart";
            resetCOMPortToolStripMenuItem.Enabled = false;
            buttonAbort.Enabled = true;
            if (buttonAbort.Text == "Continue")
            {
                buttonAbort.Text = "Stop";
            }

            if (!GammaControlExt.IsRunning)
            {
                GammaControlExt.IsRunning = true;
                if(!_bw_run_all.IsBusy)
                    _bw_run_all.RunWorkerAsync();
            }
            #endregion
        }
        private void buttonAbort_Click(object sender, EventArgs e)
        {
            if (buttonAbort.Text == "Continue") {
                if (buttonRun.Enabled == true) buttonRun.Enabled = false;
                buttonAbort.Text = "Stop";
                if (!GammaControlExt.IsRunning)
                {
                    GammaControlExt.IsRunning = true;
                    if (!_bw_run_all.IsBusy)
                        _bw_run_all.RunWorkerAsync();
                }
                return;
            }
            if (GammaControlExt.IsRunning) GammaControlExt.IsRunning = false;
            if (buttonRun.Enabled == false) buttonRun.Enabled = true;

        }

        private void dataGridView_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            #region databindcomplete processing

            DataGridView view = sender as DataGridView;


            foreach (DataGridViewRow rrr in view.Rows)
            {
                try
                {
                    bool use = false;
                    DataGridViewCellStyle style2 = new DataGridViewCellStyle();
                    
                    string s = rrr.Cells[4].Value.ToString();
                    if (Regex.Match(s, "AliceBlue", RegexOptions.IgnoreCase).Success)
                    {
                        use = true;
                        style2.BackColor = Color.FromArgb(240, 248, 255);
                        
                    }
                    if (Regex.Match(s, "LightGreen", RegexOptions.IgnoreCase).Success)
                    {
                        use = true;
                        style2.BackColor = Color.FromArgb(204, 255, 204); // offwhitegreen
                        // 180 238 180
                        //style2.BackColor = Color.FromArgb(180, 238, 180);

                    }

                    if (Regex.Match(s, "Aquamarine", RegexOptions.IgnoreCase).Success)
                    {
                        use = true;
                        style2.BackColor = Color.FromArgb(127,255,212);

                    }
                    if (Regex.Match(s, "DarkSeaGreen", RegexOptions.IgnoreCase).Success)
                    {
                        use = true;
                        style2.BackColor = Color.FromArgb(143, 188, 143);

                        // 180 238 180
                        //style2.BackColor = Color.FromArgb(180, 238, 180);

                    }
                    string ss = Regex.Replace(s, ".*fontsize=([0123456789]+).*", "$1");
                    
                    if (use)
                    {
                        
                        DataGridViewCell cell = rrr.Cells[0];
                            FontStyle fs = FontStyle.Bold;
                            

                            int h = 0; int.TryParse(ss, out h);
                            if (h > 0)
                            {
                                use = true;
                                style2.Font = new Font(view.Font.FontFamily, h, fs);
                            }

                            
                            
                            cell.Style.ApplyStyle(style2);
                        
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                try
                {
                    DataGridViewCellStyle style3 = new DataGridViewCellStyle();
                    bool use = false;
                    string s = rrr.Cells[4].Value.ToString();
                    if (Regex.Match(s, "Beige", RegexOptions.IgnoreCase).Success)
                    {
                        use = true;
                        style3.BackColor = Color.FromArgb(245, 245, 220);
                    }
                    string ss = Regex.Replace(s, ".*fontsize=([0123456789]+).*", "$1");
                    if (use)
                    {
                            DataGridViewCell cell = rrr.Cells[0];
                            FontStyle fs = FontStyle.Bold;
                            int h = 0; int.TryParse(ss, out h);
                            if (h > 0)
                            {
                                use = true;
                                style3.Font = new Font(view.Font.FontFamily, h, fs);
                            }

                            cell.Style.ApplyStyle(style3);
                                                
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }//*/
            #endregion
        }

        private void dataGridView1_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectIndex = this.tabControl1.SelectedIndex;
            int i = 0;
            string run = "", abort = "";
            if (selectIndex != current_page)
            {
                i = start_index;
                run = buttonRun.Text;
                abort = buttonAbort.Text;


                if (current_page == 0)
                {
                    _page_state1.set(i, run, abort);
                }
                if (current_page == 1)
                {
                    _page_state2.set(i, run, abort);
                }
                if (current_page == 2)
                {
                    _page_state3.set(i, run, abort);
                }

            }



            if (selectIndex == 0)
            { 
                _page_state1.get(out i, out run, out abort);
            }
            if (selectIndex == 1)
            {
                _page_state2.get(out i, out run, out abort);
            }
            if (selectIndex == 2)
            {
                _page_state3.get(out i, out run, out abort);
            }
            current_page = selectIndex;
            buttonRun.Text = run;
            buttonAbort.Text = abort;
            start_index = i;
            
        }

  

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedCells.Count > 0)
                dataGridView1.ClearSelection();
        }

      

        private void resetCOMPortToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (gamma_comport == null) return;

            if (!GammaControlExt.IsRunning)
            {
                gamma_comport = null;
                MessageBox.Show(this, "Reset COM port is done.", "Alert");
            }
            
        }

        private void GammaConformanceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (GammaControlExt.IsRunning)
            {
                if (MessageBox.Show(this, "It is running, please confirm to exit?", "Alert", 
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                    == System.Windows.Forms.DialogResult.No)
                    e.Cancel = true;
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }
    }


    /// <summary>
    /// gamma conformance page state
    /// </summary>
    class PageState
    {
        #region constructors
        public PageState()
        {
            set_default();      
        }
        #endregion

        #region methods
        public void set_default()
        {
            _index = 0;
            _button_run = "Start";
            _button_abort = "Stop";
        }
        public void set(int i, string run, string abort)
        {
            _index = i;
            _button_run = run;
            _button_abort = abort;
        }
        public void get(out int i, out string run, out string abort)
        {
            i = _index;
            run = _button_run;
            abort = _button_abort;
        }
        #endregion

        #region variables
        string _button_run;
        string _button_abort;
        int _index;
        #endregion
    }

    /// <summary>
    /// customized icon for gamma conformance
    /// </summary>
    public static class GammaIconExt
    {
        #region variables
        private static Icon _displayIco = null;
        #endregion
        #region methods
        /// <summary>
        /// Gets the tool config ico.
        /// </summary>
        /// <returns>Icon.</returns>
        public static Icon GetIco()
        {
            if (_displayIco == null)
            {
                FileStream stream = new FileStream("./gamma.ico", FileMode.Open);
                _displayIco = new Icon(stream);
                stream.Close();
            }
            return _displayIco;
        }
        #endregion 
    }

    /// <summary>
    /// static flag for running or completed
    /// </summary>
    public static class GammaControlExt
    {
        #region variables
        static bool _is_completed = false;
        static bool _is_running = false;
        #endregion

        #region property methods
        public static bool IsCompleted {
            get { return _is_completed; }
            set { _is_completed = value; }
        }
        public static bool IsRunning {
            get { return _is_running; }
            set { _is_running = value; }
        }
        #endregion
    }
    public delegate void delay_cp_t(ref DataGridView view, int row_index, ref bool res, string arg1);
    public delegate void wait_prompt_cp_t(ref DataGridView view, int row_index, ref bool res, string arg1);
    public delegate void foo_consts_cp_t(ref DataGridView view, int row_index, ref bool res, out List<String> ooo);
    public delegate void foo_cps_cp_t(ref DataGridView view, int row_index, ref string misc, ref bool res, out List<String> ooo);

    public class SymbolicProcessor
    {
        public Dictionary<string, string> dict = new Dictionary<string, string>();

        static public delay_cp_t del_delay = null; // delay
        static public foo_consts_cp_t del_gamma_const = null;
        static public foo_cps_cp_t del_gamma_cps = null;
        static public wait_prompt_cp_t del_wait_prompt = null;

        public SymbolicProcessor()
        {
        }

        private bool check_valid_string(string s)
        {
            bool res = false;
            if (s != null && s != "") res = true;
            return res;
        }

        public void init()
        {
            dict.Clear();
        }
        public bool TrySymbolProcess(string exp, DataGridView View1, int RowIndex, ref string misc, ref List<string> iii, ref bool res)
        {
            bool ret = true;
            try
            {
                ret = symbol_process(exp, View1, RowIndex, ref misc, ref iii, ref res);
                if (ret == false) misc = "";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);

                res = false;
            }
            return ret;
        }
        bool retry_processing = false;
        public bool IsRetryProcessing
        {
            get { return retry_processing; }
            set
            {
                retry_processing = value;
            }
        }

        bool stop_processing = false;
        public bool IsStopProcessing
        {
            get { return stop_processing; }
            set
            {
                stop_processing = value;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="View1"></param>
        /// <param name="RowIndex"></param>
        /// <param name="misc"></param>
        /// <param name="iii"></param>
        /// <param name="res"></param>
        /// <returns> is show or not</returns>
        public bool symbol_process(string exp, DataGridView View1, int RowIndex, ref string misc, ref List<string> iii, ref bool res)
        {

            List<string> ooo = new List<string>();
            bool is_show = false;
            //string exp = View1.Rows[RowIndex].Cells[3].Value.ToString();
            bool has_exp = check_valid_string(exp);
            if (has_exp != false)
            {
                List<string> expArr = exp.Split(';').ToList<string>();
                foreach (string s in expArr)
                {
                    Console.WriteLine(s);
                    string s0 = s.TrimStart(' ');
                    string[] argArr = UtilityArgs.CommandLineToArgs(s0);
                    string op = argArr[0];
                    string arg1 = "";
                    string arg2 = "";
                    string arg3 = "";
                    string arg4 = "";
                    string arg5 = "";
                    if (argArr.Length == 2) arg1 = argArr[1];
                    if (argArr.Length == 3)
                    {
                        arg1 = argArr[1]; arg2 = argArr[2];
                    }
                    if (argArr.Length == 4)
                    {
                        arg1 = argArr[1]; arg2 = argArr[2]; arg3 = argArr[3];
                    }
                    if (argArr.Length == 5)
                    {
                        arg1 = argArr[1]; arg2 = argArr[2]; arg3 = argArr[3]; arg4 = argArr[4];
                    }
                    if (argArr.Length == 6)
                    {
                        arg1 = argArr[1]; arg2 = argArr[2]; arg3 = argArr[3]; arg4 = argArr[4]; arg5 = argArr[5];
                    }


                    Match m = Regex.Match(op, "show");
                    if (m.Success)
                    {
                        is_show = true;
                    }
                    m = Regex.Match(op, "get");
                    if (m.Success)
                    {
                        double v;
                        string o = null;
                        try
                        {
                            v = double.Parse(dict[arg1]);
                            iii.Add(v.ToString());
                        }
                        catch (Exception)
                        {
                            o = dict[arg1];
                            if (o == "") res = false;
                            iii.Add(o);

                        }

                    }
                    m = Regex.Match(op, "setres");
                    if (m.Success)
                    {
                        try
                        {
                            dict.Add(arg1, res.ToString());
                        }
                        catch (ArgumentException)
                        {
                            dict[arg1] = res.ToString();
                        }
                        continue;
                    }

                    m = Regex.Match(op, "set");
                    if (m.Success)
                    {

                        if (iii.Count > 0)
                        {
                            double v = -1;
                            double.TryParse(iii[0], out v);

                            try
                            {
                                dict.Add(arg1, v.ToString());
                            }
                            catch (ArgumentException)
                            {
                                dict[arg1] = v.ToString();
                            }

                            if (iii.Count >= 2)
                            {
                                double.TryParse(iii[1], out v);
                                try
                                {
                                    dict.Add(arg2, v.ToString());
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg2] = v.ToString();
                                }

                            }
                            if (iii.Count >= 3)
                            {
                                double.TryParse(iii[2], out v);
                                try
                                {
                                    dict.Add(arg3, v.ToString());
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg3] = v.ToString();
                                }

                            }
                            if (iii.Count >= 4)
                            {
                                double.TryParse(iii[3], out v);
                                try
                                {
                                    dict.Add(arg4, v.ToString());
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg4] = v.ToString();
                                }

                            }
                        }
                        else if (ooo.Count > 0)
                        {
                            //double v = -1;
                            //double.TryParse(ooo[0], out v);

                            try
                            {
                                dict.Add(arg1, ooo[0]);
                            }
                            catch (ArgumentException)
                            {
                                dict[arg1] = ooo[0];
                            }

                            if (ooo.Count >= 2)
                            {
                                ///double.TryParse(ooo[1], out v);
                                try
                                {
                                    dict.Add(arg2, ooo[1]);
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg2] = ooo[1];
                                }

                            }
                            if (ooo.Count >= 3)
                            {
                                ///double.TryParse(ooo[1], out v);
                                try
                                {
                                    dict.Add(arg3, ooo[2]);
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg3] = ooo[2];
                                }

                            }
                            if (ooo.Count >= 4)
                            {
                                ///double.TryParse(ooo[1], out v);
                                try
                                {
                                    dict.Add(arg4, ooo[3]);
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg4] = ooo[3];
                                }

                            }

                            if (ooo.Count >= 5)
                            {
                                try
                                {
                                    dict.Add(arg5, ooo[4]);
                                }
                                catch (ArgumentException)
                                {
                                    dict[arg5] = ooo[4];
                                }

                            }
                        }

                    }

                    m = Regex.Match(op, "exec");
                    if (m.Success)
                    {
                        string exec = arg1;
                        string exec_arg1 = arg2;
                        string exec_arg2 = arg3;
                        string exec_arg3 = arg4;
                        if (exec == "delay")
                        {
                            int vmin;
                            int.TryParse(exec_arg1, out vmin);
                            vmin = vmin / 60;
                            if (vmin == 1) misc = vmin.ToString() + " minute";
                            else misc = vmin.ToString() + " minutes";
                            if (del_delay != null)
                                del_delay(ref View1, RowIndex, ref res, exec_arg1);
                        }
                        if (exec == "prompt")
                        {
                            //if (del_wait_prompt == null) del_wait_prompt = this.wait_prompt;
                            del_wait_prompt(ref View1, RowIndex, ref res, exec_arg1);

                        }
                        if (exec == "msgbox" || exec == "popup")
                        {
                            #region manually handle replace @"\\" to @"\" (I could not use replace function)

                            String msg0 = exec_arg1;

                            byte[] array = Encoding.ASCII.GetBytes(exec_arg1);
                            byte[] array_out = new byte[array.Length];
                            int jj = 0;
                            for (int ii = 0; ii < array.Length; ii++)
                            {
                                if (array[ii] == 92 && array[ii + 1] == 110)
                                {
                                    array_out[jj++] = 10;
                                    ii++;
                                    continue;

                                }
                                array_out[jj++] = array[ii];
                            }
                            string b = Encoding.ASCII.GetString(array_out, 0, jj);
                            #endregion

                            try
                            {
                                Form form = (Form)Form.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
                                form.Invoke((ThreadStart)delegate()
                                {
                                    MessageBox.Show(form, b, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                });
                            }
                            catch
                            {
                                MessageBox.Show(b, "Warning", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            }
                        }
                        if (exec == "retry_if")
                        {
                           
                        }
                        if (exec == "error_if")
                        {
                            if (res == false)
                            {
                                Form form = (Form)Form.FromHandle(Process.GetCurrentProcess().MainWindowHandle);
                                DialogResult result = DialogResult.Abort;
                                string misc_2 = misc;
                                form.Invoke((ThreadStart)delegate()
                                {
                                    result = MessageBox.Show(form, exec_arg1, misc_2, MessageBoxButtons.AbortRetryIgnore, MessageBoxIcon.Error);
                                });
                                if (result == DialogResult.Abort)
                                {
                                    stop_processing = true;
                                    break;
                                }
                                if (result == DialogResult.Retry)
                                {
                                    retry_processing = true;
                                    break;
                                }

                            }
                        }
                        if (exec == "check_tolerance")
                        {
                            double d, d1;
                            double d_tolerance;
                            if (iii.Count != 0)
                            {
                                double.TryParse(iii[0], out d);
                            }
                            else
                            {
                                double.TryParse(ooo[0], out d);
                            }
                            double.TryParse(dict[exec_arg1], out d1);
                            double.TryParse(exec_arg2, out d_tolerance);
                            if (Math.Abs(d - d1) < d_tolerance)
                            {
                                res = true;
                            }
                            else
                                res = false;
                        }
                        if (exec == "check_true3")
                        {
                            bool b1, b2, b3;
                            bool.TryParse(dict[exec_arg1], out b1);
                            bool.TryParse(dict[exec_arg2], out b2);
                            bool.TryParse(dict[exec_arg3], out b3);
                            if (b1 && b2 && b3) res = true;
                            else
                                res = false;
                        }
                        if (exec == "check")
                        {
                            double min, max, d;
                            if (iii.Count != 0)
                            {
                                double.TryParse(iii[0], out d);
                            }
                            else
                            {
                                double.TryParse(ooo[0], out d);
                            }
                            double.TryParse(exec_arg1, out min);
                            double.TryParse(exec_arg2, out max);
                            misc = d.ToString("F2") /*+ " " + "Expected: " + min.ToString() + ", " + max.ToString()*/;
                            if (d >= min && d <= max) res = true;
                            else
                            {
                                res = false;
                            }

                        }
                        if (exec == "check_match")
                        {
                            string str;
                            if (iii.Count != 0)
                            {
                                str = iii[0];
                            }
                            else
                            {
                                str = ooo[0];
                            }
                            Match mat = Regex.Match(str, exec_arg1);
                            if (mat.Success)
                            {
                                res = true;
                            }
                            else
                                res = false;

                        }
                        if (exec == "gamma_consts")
                        {
                            del_gamma_const(ref View1, RowIndex, ref res, out ooo);
                        }
                        if (exec == "gamma_cps")
                        {
                            del_gamma_cps(ref View1, RowIndex, ref misc, ref res, out ooo);
                        }
                    }


                }
            }
            return is_show;
        }
    }

    public class UtilityArgs
    {
        [DllImport("shell32.dll", SetLastError = true)]
        static extern IntPtr CommandLineToArgvW(
            [MarshalAs(UnmanagedType.LPWStr)] string lpCmdLine, out int pNumArgs);

        public static string[] CommandLineToArgs(string commandLine)
        {
            int argc;
            var argv = CommandLineToArgvW(commandLine, out argc);
            if (argv == IntPtr.Zero)
                throw new System.ComponentModel.Win32Exception();
            try
            {
                var args = new string[argc];
                for (var i = 0; i < args.Length; i++)
                {
                    var p = Marshal.ReadIntPtr(argv, i * IntPtr.Size);
                    args[i] = Marshal.PtrToStringUni(p);
                }

                return args;
            }
            finally
            {
                Marshal.FreeHGlobal(argv);
            }
        }
    }

    public class MessageBoxEX
    {
        private static IWin32Window _owner;
        private static HookProc _hookProc;
        private static IntPtr _hHook;

        public static DialogResult Show(string text)
        {
            Initialize();
            return MessageBox.Show(text);
        }

        public static DialogResult Show(string text, string caption)
        {
            Initialize();
            return MessageBox.Show(text, caption);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options)
        {
            Initialize();
            return MessageBox.Show(text, caption, buttons, icon, defButton, options);
        }

        public static DialogResult Show(IWin32Window owner, string text)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon, defButton);
        }

        public static DialogResult Show(IWin32Window owner, string text, string caption, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defButton, MessageBoxOptions options)
        {
            _owner = owner;
            Initialize();
            return MessageBox.Show(owner, text, caption, buttons, icon,
                                   defButton, options);
        }

        public delegate IntPtr HookProc(int nCode, IntPtr wParam, IntPtr lParam);

        public delegate void TimerProc(IntPtr hWnd, uint uMsg, UIntPtr nIDEvent, uint dwTime);

        public const int WH_CALLWNDPROCRET = 12;

        public enum CbtHookAction : int
        {
            HCBT_MOVESIZE = 0,
            HCBT_MINMAX = 1,
            HCBT_QS = 2,
            HCBT_CREATEWND = 3,
            HCBT_DESTROYWND = 4,
            HCBT_ACTIVATE = 5,
            HCBT_CLICKSKIPPED = 6,
            HCBT_KEYSKIPPED = 7,
            HCBT_SYSCOMMAND = 8,
            HCBT_SETFOCUS = 9
        }

        [DllImport("user32.dll")]
        private static extern bool GetWindowRect(IntPtr hWnd, ref Rectangle lpRect);

        [DllImport("user32.dll")]
        private static extern int MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, bool bRepaint);

        [DllImport("User32.dll")]
        public static extern UIntPtr SetTimer(IntPtr hWnd, UIntPtr nIDEvent, uint uElapse, TimerProc lpTimerFunc);

        [DllImport("User32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr SetWindowsHookEx(int idHook, HookProc lpfn, IntPtr hInstance, int threadId);

        [DllImport("user32.dll")]
        public static extern int UnhookWindowsHookEx(IntPtr idHook);

        [DllImport("user32.dll")]
        public static extern IntPtr CallNextHookEx(IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll")]
        public static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int maxLength);

        [DllImport("user32.dll")]
        public static extern int EndDialog(IntPtr hDlg, IntPtr nResult);

        [DllImport("Kernel32", EntryPoint = "GetCurrentThreadId", ExactSpelling = true)]
        public static extern Int32 GetCurrentWin32ThreadId();

        [StructLayout(LayoutKind.Sequential)]
        public struct CWPRETSTRUCT
        {
            public IntPtr lResult;
            public IntPtr lParam;
            public IntPtr wParam;
            public uint message;
            public IntPtr hwnd;
        } ;

        static MessageBoxEX()
        {
            _hookProc = new HookProc(MessageBoxHookProc);
            _hHook = IntPtr.Zero;
        }

        private static void Initialize()
        {
            if (_hHook != IntPtr.Zero)
            {
                throw new NotSupportedException("Multiple calls are not supported");
            }

            if (_owner != null)
            {
                _hHook = SetWindowsHookEx(WH_CALLWNDPROCRET, _hookProc, IntPtr.Zero,
                    GetCurrentWin32ThreadId());
            }
        }

        private static IntPtr MessageBoxHookProc(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode < 0)
            {
                return CallNextHookEx(_hHook, nCode, wParam, lParam);
            }

            CWPRETSTRUCT msg = (CWPRETSTRUCT)Marshal.PtrToStructure(lParam, typeof(CWPRETSTRUCT));
            IntPtr hook = _hHook;

            if (msg.message == (int)CbtHookAction.HCBT_ACTIVATE)
            {
                try
                {
                    CenterWindow(msg.hwnd);
                }
                finally
                {
                    UnhookWindowsHookEx(_hHook);
                    _hHook = IntPtr.Zero;
                }
            }

            return CallNextHookEx(hook, nCode, wParam, lParam);
        }

        private static void CenterWindow(IntPtr hChildWnd)
        {

            Rectangle recChild = new Rectangle(0, 0, 0, 0);
            bool success = GetWindowRect(hChildWnd, ref recChild);

            int width = recChild.Width - recChild.X;
            int height = recChild.Height - recChild.Y;

            Rectangle recParent = new Rectangle(0, 0, 0, 0);
            success = GetWindowRect(_owner.Handle, ref recParent);

            Point ptCenter = new Point(0, 0);
            ptCenter.X = recParent.X + ((recParent.Width - recParent.X) / 2);
            ptCenter.Y = recParent.Y + ((recParent.Height - recParent.Y) / 2);


            Point ptStart = new Point(0, 0);
            ptStart.X = (ptCenter.X - (width / 2));
            ptStart.Y = (ptCenter.Y - (height / 2));


            Screen[] screens = Screen.AllScreens;
            if (GetLocatedScreen(screens, ptStart.X, ptStart.Y) == null
                || GetLocatedScreen(screens, ptStart.X + width, ptStart.Y + height) == null)
            {
                Screen parentScreen = GetLocatedScreen(screens, recParent.X, recParent.Y); // start.
                if (parentScreen == null)
                {
                    parentScreen = GetLocatedScreen(screens, recParent.Width, recParent.Y);
                }
                if (parentScreen == null)
                {
                    parentScreen = GetLocatedScreen(screens, recParent.Width, recParent.Height);
                }
                if (parentScreen == null)
                {
                    parentScreen = GetLocatedScreen(screens, recParent.X, recParent.Height);
                }
                if (parentScreen == null)
                {
                    parentScreen = Screen.PrimaryScreen;
                }

                recParent = parentScreen.Bounds;

                ptStart.X = recParent.X + (recParent.Width - width) / 2; //recParent.X +  recParent.Width/2 - width/2; 
                ptStart.Y = recParent.Y + (recParent.Height - height) / 2;
            }

            int result = MoveWindow(hChildWnd, ptStart.X, ptStart.Y, width, height, false);
        }

        private static Screen GetLocatedScreen(Screen[] screens, int x, int y)
        {
            if (screens == null || screens.Length <= 0) return null;
            foreach (Screen screen in screens)
            {
                Rectangle rec = screen.Bounds;
                if (x >= rec.X && y >= rec.Y && x < rec.X + rec.Width && y < rec.Y + rec.Height)
                {
                    return screen;
                }
            }
            return null;
        }

    }



}
