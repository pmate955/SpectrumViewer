using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using Un4seen.Bass;
using Un4seen.BassWasapi;
using System.Windows.Threading;
using System.Text;
using System.Diagnostics;
using System.Net.Sockets;
using System.IO;
using System.Net;

namespace SpectrumViewer
{
    class Analyser
    {
        public Analyser(Chart ch, ComboBox devicelist)
        {
            this._fftData = new float[1024];
            this._lastlevel = 0;
            this._hanctr = 0;
            this._t = new DispatcherTimer();
            this._t.Tick += _t_Tick;
            this._t.Interval = TimeSpan.FromMilliseconds(20); //40hz refresh rate
            this._t.IsEnabled = false;
            this._process = new WASAPIPROC(Process);
            this._spectrumdata = new List<byte>();
            this._devicelist = devicelist;
            this._initialized = false;
            this._chart = ch;
            this._isRealMode = false;
            this._variableColor = 0;
            this._sb = new StringBuilder(91);
            this._max = new byte[this._lines];
            this._isDotMode = false;
            this._brightness = 50;
            this._udpData = new List<byte>();
            Init();
        }

        // Serial port for arduino output
        public SerialPort Port { get; set; }

        public UdpClient UdpClient { get; set; }


        // flag for display enable
        public bool DisplayEnable { get; set; }

        private bool _isEnabled;            //enabled status
        private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fftData;           //buffer for fft data
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private ComboBox _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index
        private byte[] _max;                 //Store peak values
        private bool _isRealMode;           //Realtime visualization
        private int _lines = 64;            //number of spectrum lines (channels)
        private Chart _chart;               //ChartView for Form
        private StringBuilder _sb;          //Output stringbuilder for Serial
        private int _variableColor;         //Bar color depends on level

        /// <summary>
        /// Is the dot mode enabled
        /// </summary>
        private bool _isDotMode;

        /// <summary>
        /// Color percent
        /// </summary>
        private int _brightness;

        private List<byte> _udpData;

        public void SetColor(int value)
        {
            _variableColor = value;
            if (_variableColor < 8)
            {
                _chart.Series[0].BackGradientStyle = GradientStyle.None;
            }
            else
            {
                _chart.Series[0].BackGradientStyle = GradientStyle.TopBottom;
            }

            if (_variableColor == 0) _chart.Series[0].Color = System.Drawing.Color.Blue;
            else if (_variableColor == 1) _chart.Series[0].Color = System.Drawing.Color.Lime;
            else if (_variableColor == 2) _chart.Series[0].Color = System.Drawing.Color.Red;
            else if (_variableColor == 3) _chart.Series[0].Color = System.Drawing.Color.Cyan;
            else if (_variableColor == 4) _chart.Series[0].Color = System.Drawing.Color.OrangeRed;

        }

        public void SetBrightness(int value)
        {
            if (value > 10 && value <= 100)
            {
                this._brightness = value;
            }
        }

        public void SetDotMode(bool isDotMode)
        {
            this._isDotMode = isDotMode;
        }

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get { return _isEnabled; }
            set
            {
                _isEnabled = value;
                if (value)
                {
                    if (!_initialized)
                    {
                        var array = (_devicelist.Items[_devicelist.SelectedIndex] as string).Split(' ');
                        devindex = Convert.ToInt32(array[0]);
                        bool result = BassWasapi.BASS_WASAPI_Init(devindex, 0, 0, BASSWASAPIInit.BASS_WASAPI_BUFFER, 1f, 0.05f, _process, IntPtr.Zero);
                        if (!result)
                        {
                            var error = Bass.BASS_ErrorGetCode();
                            //MessageBox.Show(error.ToString());
                        }
                        else
                        {
                            _initialized = true;
                            _devicelist.Enabled = false;
                        }
                    }
                    BassWasapi.BASS_WASAPI_Start();
                }
                else BassWasapi.BASS_WASAPI_Stop(true);
                System.Threading.Thread.Sleep(500);
                _t.IsEnabled = value;
            }
        }

        // initialization
        private void Init()
        {
            bool result = false;
            for (int i = 0; i < BassWasapi.BASS_WASAPI_GetDeviceCount(); i++)
            {
                var device = BassWasapi.BASS_WASAPI_GetDeviceInfo(i);
                if (device.IsEnabled && (device.IsInput || device.IsLoopback))// && device.IsLoopback)
                {
                    _devicelist.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            _devicelist.SelectedIndex = (_devicelist.Items.Count > 0 ? 1 : 0);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }

        public void changeRealMode()
        {
            _isRealMode = !_isRealMode;
        }

        //timer 
        private void _t_Tick(object sender, EventArgs e)
        {

            int ret = BassWasapi.BASS_WASAPI_GetData(_fftData, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x = 0; x < _lines; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (; b0 < b1; b0++)
                {
                    if (peak < _fftData[1 + b0]) peak = _fftData[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 200 - 4);
                if (y > 255) y = 255;
                if (y < 0) y = 0;
                _spectrumdata.Add((byte)y);
            }

            if (DisplayEnable)
            {
                _chart.Series[0].Points.Clear();
            }

            _sb.Clear();
            _sb.Append('_');
            

            for (int i = 0; i < _spectrumdata.Count; i++)
            {
                if (!_isRealMode)
                {
                    if (_spectrumdata[i] > _max[i]) _max[i] = _spectrumdata[i];               //if the new level is greater than original, then set to max
                    else if (_max[i] >= 12 && _spectrumdata[i] < _max[i] - 5) _max[i] -= 12;     //if greater than 12 and difference is bigger than 5, decrease by 12
                    else if (_max[i] > 0) _max[i]--;                                          //else decrease by one
                }
                else
                {
                    _max[i] = _spectrumdata[i];
                }
                if (DisplayEnable)
                {
                    _chart.Series[0].Points.AddXY(i + 1, _max[i]);
                    if (_variableColor == 5)                                    //Dynamic pink
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(_max[i], 0, 150);
                    }
                    else if (_variableColor == 6)                              //Default dynamic
                    {
                        if (_max[i] < 65) _chart.Series[0].Points[i].Color = System.Drawing.Color.Lime;
                        else if (_max[i] < 130) _chart.Series[0].Points[i].Color = System.Drawing.Color.Yellow;
                        else if (_max[i] < 189) _chart.Series[0].Points[i].Color = System.Drawing.Color.Orange;
                        else _chart.Series[0].Points[i].Color = System.Drawing.Color.Red;
                    }
                    else if (_variableColor == 7)                              //Dynamic cyan
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(0, _max[i], 150);
                    }
                    else if (_variableColor == 8)                              //Dynamic Fire
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(_max[i], 0, 0);
                        _chart.Series[0].Points[i].BackSecondaryColor = System.Drawing.Color.FromArgb(250, _max[i], 31); ;
                    }
                    else if (_variableColor == 9)                              //Dynamic sth
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(_max[i], 0, 140);
                        _chart.Series[0].Points[i].BackSecondaryColor = System.Drawing.Color.FromArgb(0, _max[i], 255);
                    }
                }
                char c = _getLevelCharacter(_max[i]);
                _sb.Append(c);
            }

            this._sendDotMode(_udpData);
            this._sendColor(_udpData);
            if (Port != null) Port.Write(_sb.ToString()); //Serial.Write(output);

            if (UdpClient != null)
            {
                ASCIIEncoding asen = new ASCIIEncoding();
                _udpData.AddRange(asen.GetBytes(_sb.ToString()));
                byte[] buffer = _udpData.ToArray();
                UdpClient.Send(buffer, buffer.Length);
            }

            _udpData.Clear();
            _spectrumdata.Clear();

            int level = BassWasapi.BASS_WASAPI_GetLevel();
            if (level == _lastlevel && level != 0) _hanctr++;
            _lastlevel = level;

            //Required, because some programs hang the output. If the output hangs for a 75ms
            //this piece of code re initializes the output so it doesn't make a gliched sound for long.
            /*
            if (_hanctr > 3)
            {
                Console.WriteLine("error");

                _hanctr = 0;
                _l.Value = 0;
                _r.Value = 0;
                Free();
                Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
                _initialized = false;
                Enable = true;
            }
            */

        }

        /// <summary>
        /// Returns the character for given level
        /// </summary>
        /// <param name="i">Value between 0-255</param>
        /// <returns></returns>
        private char _getLevelCharacter(byte b)
        {
            char c;
            if (b < 8) c = 'a';
            else if (b < 16) c = 'b';
            else if (b < 24) c = 'c';
            else if (b < 32) c = 'd';
            else if (b < 40) c = 'e';
            else if (b < 48) c = 'f';
            else if (b < 56) c = 'g';
            else if (b < 64) c = 'h';
            else if (b < 72) c = 'i';
            else if (b < 80) c = 'j';
            else if (b < 88) c = 'k';
            else if (b < 96) c = 'l';
            else if (b < 104) c = 'm';
            else if (b < 112) c = 'n';
            else if (b < 120) c = 'o';
            else if (b < 128) c = 'q';
            else if (b < 136) c = 'r';
            else if (b < 144) c = 's';
            else if (b < 152) c = 't';
            else if (b < 160) c = 'u';
            else if (b < 168) c = 'v';
            else if (b < 176) c = 'w';
            else if (b < 184) c = 'x';
            else if (b < 192) c = 'y';
            else if (b < 200) c = 'z';
            else if (b < 208) c = 'A';
            else if (b < 216) c = 'B';
            else if (b < 224) c = 'C';
            else if (b < 232) c = 'D';
            else if (b < 240) c = 'E';
            else if (b < 248) c = 'F';
            else c = 'G';
            return c;
        }

        /// <summary>
        /// It sends a dot if we are in dot mode
        /// </summary>
        private void _sendDotMode(List<byte> udpData)
        {
            if (Port != null && this._isDotMode)
            {
                Port.Write(".");
            }

            if (UdpClient != null && this._isDotMode)
            {
                udpData.Add(46);
            }
        }

        /// <summary>
        /// It sends the graph color to Serial
        /// </summary>
        private void _sendColor(List<byte> udpData)
        {

            byte r = 0, g = 0, b = 0, maxChannels = 0;
            if (_variableColor == 0)
            {
                b = 0xff;
            }
            else if (this._variableColor == 1)
            {
                g = 0xff;
            }
            else if (this._variableColor == 2)
            {
                r = 0xff;
            }
            else if (this._variableColor == 3)
            {
                g = 0xff;
                b = 0xff;
            }
            else if (this._variableColor == 4)
            {
                r = 0xff;
                g = 0x45;
            }
            else if (this._variableColor == 5)
            {
                b = 150;
                maxChannels = 4;
            }
            else if (this._variableColor == 6)
            {
                g = 150;
                maxChannels = 5;
            }
            else if (this._variableColor == 7)
            {
                b = 150;
                maxChannels = 2;
            }
            else if (this._variableColor == 8)
            {
                r = 250;
                maxChannels = 2;
                b = 31;
            }
            else
            {
                r = 0xff;
                g = 0xff;
                b = 0xff;
            }

            r = (byte)(r > 0 ? (byte)(r * (this._brightness / 100.0)) : 0);
            g = (byte)(g > 0 ? (byte)(g * (this._brightness / 100.0)) : 0);
            b = (byte)(b > 0 ? (byte)(b * (this._brightness / 100.0)) : 0);

            if (r < 0 || r > 255)
                r = 0;
            if (g < 0 || g > 255)
                g = 0;
            if (b < 0 || b > 255)
                b = 0;

            if (Port != null)
            {
                Port.Write(";");
                Port.Write(new byte[] { r, g, b, maxChannels }, 0, 4);
                Port.Write(";");
            }

            if (UdpClient != null)
            {
                byte[] buffer = new byte[] { 59, r, g, b, maxChannels, 59 };
                udpData.AddRange(buffer);
            }
        }


        // WASAPI callback, required for continuous recording
        private int Process(IntPtr buffer, int length, IntPtr user)
        {
            return length;
        }

        //cleanup
        public void Free()
        {
            BassWasapi.BASS_WASAPI_Free();
            Bass.BASS_Free();
        }
    }
}
