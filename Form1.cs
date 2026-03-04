using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WentraLoader
{
    public partial class Form1 : Form
    {
        // GitHub Teması Renkleri
        Color ghBg = Color.FromArgb(13, 17, 23);
        Color ghGreen = Color.FromArgb(35, 134, 54);
        Color ghBorder = Color.FromArgb(48, 54, 61);

        public Form1()
        {
            InitializeComponent();
            SetupGithubUI();
        }

        private void SetupGithubUI()
        {
            this.BackColor = ghBg;
            this.FormBorderStyle = FormBorderStyle.None;
            // Butonun tasarımını GitHub "New" butonu gibi yapalım
            btnLaunch.BackColor = ghGreen;
            btnLaunch.FlatStyle = FlatStyle.Flat;
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.ForeColor = Color.White;
            btnLaunch.Text = "🚀 Initialize Wentra Client";
            
            lblStatus.Text = "Status: Idle - Standing by for Argus.pro/Wentra";
            lstLogs.BackColor = Color.FromArgb(22, 27, 34);
            lstLogs.ForeColor = Color.FromArgb(139, 148, 158);
        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            AddLog("Fetching repository: Argus.pro/Wentra...");
            await Task.Delay(800);

            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jarPath = Path.Combine(desktop, "hile", "WentraClient.jar");
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gamePath = Path.Combine(appData, ".craftrise", "CraftRise.exe");

            // Dosya Kontrolü
            if (!File.Exists(jarPath))
            {
                AddLog("[ERROR] Failed to locate local build: WentraClient.jar");
                MessageBox.Show("Masaüstünde 'hile' klasörü ve JAR bulunamadı!");
                return;
            }

            AddLog("Checking local environment: Java JDK 8 Detected.");
            await Task.Delay(500);
            AddLog("Injecting javaagent parameters...");

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = gamePath;
                // En güvenli yöntem: Java Agent
                startInfo.Arguments = $"-javaagent:\"{jarPath}\"";
                startInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);

                AddLog("Launching CraftRise with Wentra.jar...");
                Process.Start(startInfo);

                AddLog("Success! Client successfully detached from loader.");
                lblStatus.Text = "Status: Success - Running";
                
                await Task.Delay(2000);
                Application.Exit(); // Loader işini bitirdi
            }
            catch (Exception ex)
            {
                AddLog("[FATAL ERROR] " + ex.Message);
            }
        }

        private void AddLog(string msg)
        {
            // Terminal havası katan log fonksiyonu
            lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}] {msg}");
            lstLogs.SelectedIndex = lstLogs.Items.Count - 1;
        }

        // Formu sürüklemek için küçük bir dokunuş
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x84) m.Result = (IntPtr)0x2;
        }
    }
}
