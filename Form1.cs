﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace SpectrumViewer
{
    public partial class Form1 : Form
    {
        private Analyser _analyser;                 //Analyser DLL
        private SerialPort _port;                   //Serial port to communication
        private Boolean _isHided;                   //Hide controllers
        private int windowWidth, windowHeight;      //Window size

        public Form1()      
        {
            InitializeComponent();
            _analyser = new Analyser( chart1, deviceBox);
            _isHided = false;
            windowWidth = 545;
            windowHeight = 200;
            colorBox.SelectedIndex = 1;
            readDatas();
        }

        private void readDatas()
        {
            try
            {
                string[] lines = System.IO.File.ReadAllLines("conf.txt");
                for (int i = 0; i < lines.Length; i++)
                {
                    string[] columns = lines[i].Split(' ');
                    if (columns[0].Equals("SourceIndex"))
                    {
                        try
                        {
                            int num = Int32.Parse(columns[1]);
                            if (deviceBox.Items.Count > num) deviceBox.SelectedIndex = num;
                        }
                        catch (Exception e)
                        {

                        }
                    }
                    else if (columns[0].Equals("DotMode"))
                    {
                        if (columns[1].Equals("1")) DotModeBtn.Checked=true;
                        
                    }
                    else if (columns[0].Equals("RealMode"))
                    {
                        if (columns[1].Equals("1")) RealBtn.Checked = true;
                    }
                    else if (columns[0].Equals("Display"))
                    {
                        if (columns[1].Equals("1")) displayEnabled.Checked = true;
                        else displayEnabled.Checked = false;
                    }
                    else if (columns[0].Equals("OnTop"))
                    {
                        if (columns[1].Equals("1"))
                        {
                            alwaysOnTopCb.Checked = true;
                        }
                    } else if (columns[0].Equals("ColorIndex"))
                    {
                        try
                        {
                            int num = Int32.Parse(columns[1]);
                            colorBox.SelectedIndex = num;
                        } catch (Exception n)
                        {

                        }

                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("No source file");
            }
        }

        private void saveData()
        {
            
           // System.IO.File.WriteAllLines("conf.txt", lines);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        


        private void ckbEnable_CheckedChanged(object sender, EventArgs e)               //Enable checkbutton
        {
            if (ckbEnable.Checked == true)
            {
                ckbEnable.Text = "Disable";
                _analyser.Enable = true;
                _analyser.DisplayEnable = (bool)displayEnabled.Checked;

            }
            else
            {
                _analyser.Enable = false;
                ckbEnable.Text = "Enable";
            }
        }

        private void displayEnabled_CheckedChanged(object sender, EventArgs e)      //GUI display change
        {
            _analyser.DisplayEnable = (bool)displayEnabled.Checked;
        }

        private void portBox_DropDown(object sender, EventArgs e)                   //Refresh Serial port list
        {
            portBox.Items.Clear();
            var ports = SerialPort.GetPortNames();
            foreach (var port in ports) portBox.Items.Add(port);
        }

        private void serialEnabled_CheckedChanged(object sender, EventArgs e)       //Serial data sending
        {
            try
            {
                if (serialEnabled.Checked == true)
                {
                    portBox.Enabled = false;                    
                    _port = new SerialPort((portBox.Items[portBox.SelectedIndex] as string));
                    _port.BaudRate = 57600;                                                         //Baudrate
                    _port.StopBits = StopBits.One;
                    _port.Parity = Parity.None;
                    _port.DataBits = 8;
                    _port.DtrEnable = true;
                    _port.Open();
                    _analyser.Serial = _port;
                    
                    
                }
                else
                {
                    portBox.Enabled = true;
                    _analyser.Serial = null;
                    if (_port != null)
                    {
                      _port.Close();
                      _port.Dispose();
                      _port = null;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error");
            }
        }

        private void alwaysOnTopCb_CheckedChanged(object sender, EventArgs e)           //Always on Top
        {
            this.TopMost = alwaysOnTopCb.Checked;
        }
        
        

        private void Form1_MouseDoubleClick(object sender, MouseEventArgs e)            //Hide/Show controls 
        {
            if (!_isHided)                                         //Hide controls
            {
                _isHided = true;
                this.Height = windowHeight;
                this.Width = windowWidth;
                chart1.Height = windowHeight-40;
                chart1.Width = windowWidth-15;
                deviceBox.Visible = false;
                portBox.Visible = false;
                alwaysOnTopCb.Visible = false;      //
                serialEnabled.Visible = false;
                displayEnabled.Visible = false;
                DotModeBtn.Visible = false;
                RealBtn.Visible = false;
                colorBox.Visible = false;
                chart1.Location = new Point(0, 0);                
                this.FormBorderStyle = FormBorderStyle.Sizable;
            }
            else
            {                                           //Show controls
                windowWidth = this.Width;
                windowHeight = this.Height;
                _isHided = false;
                if (this.Height < 211)
                {
                    this.Height = 211;
                    chart1.Height = windowHeight - 40;
                }
                this.Width = chart1.Width + 162;
                deviceBox.Visible = true;
                portBox.Visible = true;
                DotModeBtn.Visible = true;
                RealBtn.Visible = true;
                alwaysOnTopCb.Visible = true;      //
                serialEnabled.Visible = true;
                displayEnabled.Visible = true;
                colorBox.Visible = true;
                chart1.Location = new Point(145, 0);                
                this.FormBorderStyle = FormBorderStyle.FixedSingle;
            }
        }

        private void RealBtn_CheckedChanged(object sender, EventArgs e)             //Realtime mode 
        {
            _analyser.changeRealMode();
        }

        private void DotModeBtn_CheckedChanged(object sender, EventArgs e)          //Dot mode
        {
            if (_port != null)
            {
                _port.Write("x");
            }
            if (chart1.Series[0].ChartType == System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point) chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Column;
            else chart1.Series[0].ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Point;

        }

        private void colorBox_SelectedIndexChanged(object sender, EventArgs e)      //Select color
        {
            _analyser.set_variableColor(colorBox.SelectedIndex);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            string[] lines = new string[6];
            lines[0] = "SourceIndex " + deviceBox.SelectedIndex;
            lines[1] = "DotMode " + (DotModeBtn.Checked ? "1" : "0");
            lines[2] = "RealMode " + (RealBtn.Checked ? "1" : "0");
            lines[3] = "Display " + (displayEnabled.Checked ? "1" : "0");
            lines[4] = "OnTop " + (alwaysOnTopCb.Checked ? "1" : "0");
            lines[5] = "ColorIndex " + colorBox.SelectedIndex;
            try
            {
                System.IO.File.WriteAllLines("conf.txt", lines);
            } catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }

        private void Form1_SizeChanged(object sender, EventArgs e)
        {
            if (_isHided)
            {
                chart1.Width = this.Width - 15;
                chart1.Height = this.Height - 40;
            }          
        }
    }
    
}
