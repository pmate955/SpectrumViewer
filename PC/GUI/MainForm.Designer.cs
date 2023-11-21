namespace SpectrumViewer
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.ckbEnable = new System.Windows.Forms.CheckBox();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.deviceBox = new System.Windows.Forms.ComboBox();
            this.displayEnabled = new System.Windows.Forms.CheckBox();
            this.portBox = new System.Windows.Forms.ComboBox();
            this.serialEnabled = new System.Windows.Forms.CheckBox();
            this.alwaysOnTopCb = new System.Windows.Forms.CheckBox();
            this.RealBtn = new System.Windows.Forms.CheckBox();
            this.DotModeBtn = new System.Windows.Forms.CheckBox();
            this.colorBox = new System.Windows.Forms.ComboBox();
            this.ButtonToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.brightnessBar = new System.Windows.Forms.TrackBar();
            this.cbNetwork = new System.Windows.Forms.CheckBox();
            this.tbIpAddress = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).BeginInit();
            this.SuspendLayout();
            // 
            // ckbEnable
            // 
            this.ckbEnable.Appearance = System.Windows.Forms.Appearance.Button;
            this.ckbEnable.Location = new System.Drawing.Point(13, 27);
            this.ckbEnable.Name = "ckbEnable";
            this.ckbEnable.Size = new System.Drawing.Size(127, 23);
            this.ckbEnable.TabIndex = 2;
            this.ckbEnable.Text = "Enable";
            this.ckbEnable.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonToolTip.SetToolTip(this.ckbEnable, "Enable Spectrum Viewer");
            this.ckbEnable.UseVisualStyleBackColor = true;
            this.ckbEnable.CheckedChanged += new System.EventHandler(this.ckbEnable_CheckedChanged);
            // 
            // chart1
            // 
            chartArea1.AxisX.Maximum = 64D;
            chartArea1.AxisX.Minimum = 0D;
            chartArea1.AxisY.Maximum = 255D;
            chartArea1.AxisY.Minimum = 0D;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 100F;
            chartArea1.InnerPlotPosition.Width = 100F;
            chartArea1.Name = "ChartArea1";
            chartArea1.Position.Auto = false;
            chartArea1.Position.Height = 100F;
            chartArea1.Position.Width = 100F;
            this.chart1.ChartAreas.Add(chartArea1);
            this.chart1.Location = new System.Drawing.Point(146, 0);
            this.chart1.Name = "chart1";
            series1.ChartArea = "ChartArea1";
            series1.Color = System.Drawing.Color.Lime;
            series1.EmptyPointStyle.IsVisibleInLegend = false;
            series1.IsVisibleInLegend = false;
            series1.Name = "Series1";
            this.chart1.Series.Add(series1);
            this.chart1.Size = new System.Drawing.Size(545, 238);
            this.chart1.TabIndex = 3;
            this.chart1.Text = "chart1";
            this.chart1.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDoubleClick);
            // 
            // deviceBox
            // 
            this.deviceBox.FormattingEnabled = true;
            this.deviceBox.Location = new System.Drawing.Point(13, 0);
            this.deviceBox.Name = "deviceBox";
            this.deviceBox.Size = new System.Drawing.Size(127, 21);
            this.deviceBox.TabIndex = 4;
            this.ButtonToolTip.SetToolTip(this.deviceBox, "Audio source");
            // 
            // displayEnabled
            // 
            this.displayEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.displayEnabled.AutoSize = true;
            this.displayEnabled.Checked = true;
            this.displayEnabled.CheckState = System.Windows.Forms.CheckState.Checked;
            this.displayEnabled.Location = new System.Drawing.Point(13, 85);
            this.displayEnabled.Name = "displayEnabled";
            this.displayEnabled.Size = new System.Drawing.Size(51, 23);
            this.displayEnabled.TabIndex = 5;
            this.displayEnabled.Text = "Display";
            this.ButtonToolTip.SetToolTip(this.displayEnabled, "Enable GUI display");
            this.displayEnabled.UseVisualStyleBackColor = true;
            this.displayEnabled.CheckedChanged += new System.EventHandler(this.displayEnabled_CheckedChanged);
            // 
            // portBox
            // 
            this.portBox.FormattingEnabled = true;
            this.portBox.Location = new System.Drawing.Point(71, 116);
            this.portBox.Name = "portBox";
            this.portBox.Size = new System.Drawing.Size(69, 21);
            this.portBox.TabIndex = 6;
            this.ButtonToolTip.SetToolTip(this.portBox, "Serial port");
            this.portBox.DropDown += new System.EventHandler(this.portBox_DropDown);
            // 
            // serialEnabled
            // 
            this.serialEnabled.Appearance = System.Windows.Forms.Appearance.Button;
            this.serialEnabled.Location = new System.Drawing.Point(13, 114);
            this.serialEnabled.Name = "serialEnabled";
            this.serialEnabled.Size = new System.Drawing.Size(52, 23);
            this.serialEnabled.TabIndex = 7;
            this.serialEnabled.Text = "Serial ";
            this.serialEnabled.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonToolTip.SetToolTip(this.serialEnabled, "Enable serial data sending");
            this.serialEnabled.UseVisualStyleBackColor = true;
            this.serialEnabled.CheckedChanged += new System.EventHandler(this.serialEnabled_CheckedChanged);
            // 
            // alwaysOnTopCb
            // 
            this.alwaysOnTopCb.Appearance = System.Windows.Forms.Appearance.Button;
            this.alwaysOnTopCb.Location = new System.Drawing.Point(70, 85);
            this.alwaysOnTopCb.Name = "alwaysOnTopCb";
            this.alwaysOnTopCb.Size = new System.Drawing.Size(70, 23);
            this.alwaysOnTopCb.TabIndex = 8;
            this.alwaysOnTopCb.Text = "On Top";
            this.alwaysOnTopCb.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonToolTip.SetToolTip(this.alwaysOnTopCb, "Always on Top");
            this.alwaysOnTopCb.UseVisualStyleBackColor = true;
            this.alwaysOnTopCb.CheckedChanged += new System.EventHandler(this.alwaysOnTopCb_CheckedChanged);
            // 
            // RealBtn
            // 
            this.RealBtn.Appearance = System.Windows.Forms.Appearance.Button;
            this.RealBtn.Location = new System.Drawing.Point(70, 56);
            this.RealBtn.Name = "RealBtn";
            this.RealBtn.Size = new System.Drawing.Size(70, 24);
            this.RealBtn.TabIndex = 10;
            this.RealBtn.Text = "Real ";
            this.RealBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonToolTip.SetToolTip(this.RealBtn, "Realtime spectrum");
            this.RealBtn.UseVisualStyleBackColor = true;
            this.RealBtn.CheckedChanged += new System.EventHandler(this.RealBtn_CheckedChanged);
            // 
            // DotModeBtn
            // 
            this.DotModeBtn.Appearance = System.Windows.Forms.Appearance.Button;
            this.DotModeBtn.Location = new System.Drawing.Point(13, 56);
            this.DotModeBtn.Name = "DotModeBtn";
            this.DotModeBtn.Size = new System.Drawing.Size(51, 24);
            this.DotModeBtn.TabIndex = 11;
            this.DotModeBtn.Text = "Dot ";
            this.DotModeBtn.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ButtonToolTip.SetToolTip(this.DotModeBtn, "Dot mode");
            this.DotModeBtn.UseVisualStyleBackColor = true;
            this.DotModeBtn.CheckedChanged += new System.EventHandler(this.DotModeBtn_CheckedChanged);
            // 
            // colorBox
            // 
            this.colorBox.FormattingEnabled = true;
            this.colorBox.Items.AddRange(new object[] {
            "Blue",
            "Green",
            "Red",
            "Light Blue",
            "Orange",
            "Dynamic Pink",
            "Dynamic Default",
            "Dynamic Cyan",
            "Dynamic Fire",
            "Dynamic sth"});
            this.colorBox.Location = new System.Drawing.Point(12, 176);
            this.colorBox.Name = "colorBox";
            this.colorBox.Size = new System.Drawing.Size(113, 21);
            this.colorBox.TabIndex = 12;
            this.ButtonToolTip.SetToolTip(this.colorBox, "Chart color");
            this.colorBox.SelectedIndexChanged += new System.EventHandler(this.colorBox_SelectedIndexChanged);
            // 
            // brightnessBar
            // 
            this.brightnessBar.Location = new System.Drawing.Point(13, 203);
            this.brightnessBar.Maximum = 100;
            this.brightnessBar.Minimum = 10;
            this.brightnessBar.Name = "brightnessBar";
            this.brightnessBar.Size = new System.Drawing.Size(113, 45);
            this.brightnessBar.TabIndex = 13;
            this.ButtonToolTip.SetToolTip(this.brightnessBar, "Brightness");
            this.brightnessBar.Value = 50;
            this.brightnessBar.MouseUp += new System.Windows.Forms.MouseEventHandler(this.brightnessBar_ValueChanged);
            // 
            // cbNetwork
            // 
            this.cbNetwork.Appearance = System.Windows.Forms.Appearance.Button;
            this.cbNetwork.Location = new System.Drawing.Point(13, 142);
            this.cbNetwork.Name = "cbNetwork";
            this.cbNetwork.Size = new System.Drawing.Size(51, 23);
            this.cbNetwork.TabIndex = 14;
            this.cbNetwork.Text = "UDP";
            this.cbNetwork.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.cbNetwork.UseVisualStyleBackColor = true;
            this.cbNetwork.CheckedChanged += new System.EventHandler(this.cbNetwork_CheckedChanged);
            // 
            // tbIpAddress
            // 
            this.tbIpAddress.Location = new System.Drawing.Point(71, 144);
            this.tbIpAddress.Name = "tbIpAddress";
            this.tbIpAddress.Size = new System.Drawing.Size(69, 20);
            this.tbIpAddress.TabIndex = 15;
            this.tbIpAddress.Text = "192.168.0.12";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(691, 250);
            this.Controls.Add(this.tbIpAddress);
            this.Controls.Add(this.cbNetwork);
            this.Controls.Add(this.brightnessBar);
            this.Controls.Add(this.colorBox);
            this.Controls.Add(this.DotModeBtn);
            this.Controls.Add(this.RealBtn);
            this.Controls.Add(this.alwaysOnTopCb);
            this.Controls.Add(this.serialEnabled);
            this.Controls.Add(this.portBox);
            this.Controls.Add(this.displayEnabled);
            this.Controls.Add(this.deviceBox);
            this.Controls.Add(this.chart1);
            this.Controls.Add(this.ckbEnable);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "MainForm";
            this.Text = "Spectrum Viewer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.SizeChanged += new System.EventHandler(this.Form1_SizeChanged);
            this.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseDoubleClick);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.brightnessBar)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckBox ckbEnable;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.ComboBox deviceBox;
        private System.Windows.Forms.CheckBox displayEnabled;
        private System.Windows.Forms.ComboBox portBox;
        private System.Windows.Forms.CheckBox serialEnabled;
        private System.Windows.Forms.CheckBox alwaysOnTopCb;
        private System.Windows.Forms.CheckBox RealBtn;
        private System.Windows.Forms.CheckBox DotModeBtn;
        private System.Windows.Forms.ComboBox colorBox;
        private System.Windows.Forms.ToolTip ButtonToolTip;
        private System.Windows.Forms.TrackBar brightnessBar;
        private System.Windows.Forms.CheckBox cbNetwork;
        private System.Windows.Forms.TextBox tbIpAddress;
    }
}

