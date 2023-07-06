using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace t6._1
{
    public partial class Form1 : Form
    {
        private Timer timer;
        private WasapiCapture capture;
        private bool isMicrophoneInUse;
        private string activeApplication;

        public Form1()
        {
            InitializeComponent();

            // Inițializare timer
            timer = new Timer();
            timer.Interval = 5000;
            timer.Tick += Timer_Tick;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            StartMicrophoneMonitoring();
            timer.Start();
            this.Height = 0;
            this.Width = 0;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (isMicrophoneInUse)
            {
                // Afișează un MessageBox că microfonul este pornit și este utilizat de: {activeApplication}
                MessageBox.Show($"Microfonul este pornit și este utilizat de: {activeApplication}");
            }
            else
            {
                // Afișează un MessageBox că microfonul este oprit sau nu este utilizat de nicio aplicație.
                MessageBox.Show("Microfonul este oprit sau nu este utilizat de nicio aplicație.");
            }
        }

        private void InitializeMicrophoneCapture()
        {
            capture = new WasapiCapture();
            capture.DataAvailable += Capture_DataAvailable;
        }

        private void StartMicrophoneMonitoring()
        {
            InitializeMicrophoneCapture();
            capture.StartRecording();
        }

        private void StopMicrophoneMonitoring()
        {
            capture.StopRecording();
            capture.Dispose();
        }

        private void Capture_DataAvailable(object sender, WaveInEventArgs e)
        {
            isMicrophoneInUse = true;
            activeApplication = GetActiveApplicationName();
        }

        private string GetActiveApplicationName()
        {
            IntPtr foregroundWindow = GetForegroundWindow();
            uint processId;
            GetWindowThreadProcessId(foregroundWindow, out processId);
            Process process = Process.GetProcessById((int)processId);
            return process.ProcessName;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            StopMicrophoneMonitoring();
        }

        #region Windows API Definitions

        [DllImport("user32.dll")]
        public static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", SetLastError = true)]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        #endregion
    }
}
