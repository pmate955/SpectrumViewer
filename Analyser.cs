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

namespace SpectrumViewer
{
    class Analyser
    {
        private bool _enable;               //enabled status
        private DispatcherTimer _t;         //timer that refreshes the display
        private float[] _fft;               //buffer for fft data
        private WASAPIPROC _process;        //callback function to obtain data
        private int _lastlevel;             //last output level
        private int _hanctr;                //last output level counter
        private List<byte> _spectrumdata;   //spectrum data buffer
        private ComboBox _devicelist;       //device list
        private bool _initialized;          //initialized flag
        private int devindex;               //used device index
        private byte[] max;                 //Store peak values
        private bool _realMode;          //Realtime visualization
        private char[] output;              //char data for serial
        private int _lines = 90;            //number of spectrum lines (channels)
        private Chart _chart;               //ChartView for Form
        private StringBuilder _sb;          //Output stringbuilder for Serial
        private int _variableColor;     //Bar color depends on level

        public Analyser(Chart ch, ComboBox devicelist)
        {
            _fft = new float[1024];
            output = new char[_lines];
            _lastlevel = 0;
            _hanctr = 0;
            _t = new DispatcherTimer();
            _t.Tick += _t_Tick;
            _t.Interval = TimeSpan.FromMilliseconds(20); //40hz refresh rate
            _t.IsEnabled = false;           
            _process = new WASAPIPROC(Process);
            _spectrumdata = new List<byte>();
            _devicelist = devicelist;
            _initialized = false;
            _chart = ch;
            _realMode = false;
            _variableColor = 0;
            _sb = new StringBuilder(91);
            max = new byte[90];
            Init();
        }
        
        // Serial port for arduino output
        public SerialPort Serial { get; set; }
        

        // flag for display enable
        public bool DisplayEnable { get; set; }

        public void set_variableColor(int value) {
            _variableColor = value;
            if(_variableColor < 8)
            {
                _chart.Series[0].BackGradientStyle = GradientStyle.None;
            } else
            {
                _chart.Series[0].BackGradientStyle = GradientStyle.TopBottom;
            }

            if(_variableColor == 0) _chart.Series[0].Color = System.Drawing.Color.Blue;
            else if(_variableColor == 1) _chart.Series[0].Color = System.Drawing.Color.Lime;
            else if (_variableColor == 2) _chart.Series[0].Color = System.Drawing.Color.Red;
            else if (_variableColor == 3) _chart.Series[0].Color = System.Drawing.Color.Cyan;
            else if (_variableColor == 4) _chart.Series[0].Color = System.Drawing.Color.OrangeRed;

        }

        //flag for enabling and disabling program functionality
        public bool Enable
        {
            get { return _enable; }
            set
            {
                _enable = value;
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
                if (device.IsEnabled &&(device.IsInput || device.IsLoopback))// && device.IsLoopback)
                {
                    _devicelist.Items.Add(string.Format("{0} - {1}", i, device.name));
                }
            }
            _devicelist.SelectedIndex = (_devicelist.Items.Count>0?1:0);
            Bass.BASS_SetConfig(BASSConfig.BASS_CONFIG_UPDATETHREADS, false);
            result = Bass.BASS_Init(0, 44100, BASSInit.BASS_DEVICE_DEFAULT, IntPtr.Zero);
            if (!result) throw new Exception("Init Error");
        }

        public void changeRealMode()
        {
            _realMode = !_realMode;
        }

       

        //timer 
        private void _t_Tick(object sender, EventArgs e)
        {
            
            int ret = BassWasapi.BASS_WASAPI_GetData(_fft, (int)BASSData.BASS_DATA_FFT2048); //get channel fft data
            if (ret < -1) return;
            int x, y;
            int b0 = 0;

            //computes the spectrum data, the code is taken from a bass_wasapi sample.
            for (x=0; x<_lines; x++)
            {
                float peak = 0;
                int b1 = (int)Math.Pow(2, x * 10.0 / (_lines - 1));
                if (b1 > 1023) b1 = 1023;
                if (b1 <= b0) b1 = b0 + 1;
                for (;b0<b1;b0++)
                {
                    if (peak < _fft[1 + b0]) peak = _fft[1 + b0];
                }
                y = (int)(Math.Sqrt(peak) * 3 * 255 - 4);
                if (y > 255) y = 255;
                if (y < 0) y = 0;
                _spectrumdata.Add((byte)y);
            }

            if (DisplayEnable)
            {
                _chart.Series[0].Points.Clear();
            }
            
            _sb.Clear();

            for (int i = 0; i < _spectrumdata.Count; i++)
           {
                if (!_realMode)
                {
                    if (_spectrumdata[i] > max[i]) max[i] = _spectrumdata[i];               //if the new level is greater than original, then set to max
                    else if (max[i] >= 12 && _spectrumdata[i] < max[i] - 5) max[i] -= 12;     //if greater than 12 and difference is bigger than 5, decrease by 12
                    else if (max[i] > 0) max[i]--;                                          //else decrease by one
                } else
                {
                    max[i] = _spectrumdata[i];
                }
                if (DisplayEnable)
                {
                    _chart.Series[0].Points.AddXY(i + 1, max[i]);
                    if (_variableColor == 5)                                    //Dynamic pink
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(max[i],0, 150);
                    } else if(_variableColor == 6)                              //Default dynamic
                    {
                        if (max[i] < 65) _chart.Series[0].Points[i].Color = System.Drawing.Color.Lime;
                        else if(max[i] < 130 ) _chart.Series[0].Points[i].Color = System.Drawing.Color.Yellow;
                        else if (max[i] < 189) _chart.Series[0].Points[i].Color = System.Drawing.Color.Orange;
                        else _chart.Series[0].Points[i].Color = System.Drawing.Color.Red;
                    } else if(_variableColor == 7)                              //Dynamic cyan
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(0, max[i], 150);
                    } else if(_variableColor == 8)                              //Dynamic Fire
                    {
                        //_chart.Series[0].Color = System.Drawing.Color.Red;
                        //_chart.Series[0].BackSecondaryColor = System.Drawing.Color.Yellow;
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(max[i], 0, 0);
                        _chart.Series[0].Points[i].BackSecondaryColor = System.Drawing.Color.FromArgb(250, max[i],31); ;
                    } else if(_variableColor == 9)                              //Dynamic sth
                    {
                        _chart.Series[0].Points[i].Color = System.Drawing.Color.FromArgb(max[i], 0, 140);
                        _chart.Series[0].Points[i].BackSecondaryColor = System.Drawing.Color.FromArgb(0,max[i], 255);
                    }
                }
                char c = 'a';
                byte b = max[i];
                if (b < 5) c = 'a';
                else if (b < 17) c = 'b';
                else if (b < 39) c = 'c';
                else if (b < 51) c = 'd';
                else if (b < 63) c = 'e';
                else if (b < 75) c = 'f';
                else if (b < 87) c = 'g';
                else if (b < 99) c = 'h';
                else if (b < 111) c = 'i';
                else if (b < 123) c = 'j';
                else if (b < 135) c = 'k';
                else if (b < 147) c = 'l';
                else if (b < 159) c = 'm';
                else if (b < 171) c = 'n';
                else if (b < 183) c = 'o';
                else if (b < 195) c = 'q';
                else if (b < 207) c = 'r';
                else if (b < 219) c = 's';
                else if (b < 231) c = 't';
                else if (b < 243) c = 'u';
                else c = 'v';
                int num = c - 'a';
                _sb.Append(c);
          }

            //Console.WriteLine(output);
            if (Serial != null) Serial.Write(_sb.ToString()); //Serial.Write(output);
            //Console.WriteLine();

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
