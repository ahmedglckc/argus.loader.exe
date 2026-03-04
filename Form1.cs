using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArgusLoader
{
    public partial class Form1 : Form
    {
        #region GitHub Pro Dark Palette
        private static readonly Color BgMain = Color.FromArgb(13, 17, 23);       // Ana Arka Plan
        private static readonly Color BgHeader = Color.FromArgb(22, 27, 34);     // Üst Bar
        private static readonly Color GhGreen = Color.FromArgb(35, 134, 54);     // GitHub Yeşili
        private static readonly Color GhGreenHover = Color.FromArgb(46, 160, 67); // Hover Yeşili
        private static readonly Color TextMain = Color.FromArgb(201, 209, 217);  // Ana Yazı
        private static readonly Color TextMuted = Color.FromArgb(139, 148, 158); // Log Yazısı
        #endregion

        public Form1()
        {
            InitializeComponent();
            SetupProfessionalUI();
        }

        private void SetupProfessionalUI()
        {
            // Form Temel Ayarları
            this.BackColor = BgMain;
            this.ForeColor = TextMain;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(550, 400);

            // Log Alanı (ListBox) Ayarları
            lstLogs.BackColor = Color.FromArgb(1, 4, 9);
            lstLogs.BorderStyle = BorderStyle.None;
            lstLogs.ForeColor = TextMuted;
            lstLogs.Font = new Font("Consolas", 9.5f);
            lstLogs.ItemHeight = 20;

            // Buton Tasarımı
            btnLaunch.BackColor = GhGreen;
            btnLaunch.FlatStyle = FlatStyle.Flat;
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.Cursor = Cursors.Hand;
            btnLaunch.Font = new Font("Segoe UI", 10f, FontStyle.Bold);

            AddLog("System initialized. Local environment ready.");
        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            btnLaunch.Enabled = false;
            btnLaunch.Text = "⌛ DEPLOYING...";

            AddLog("-------------------------------------------");
            AddLog(">> STARTING ARGUS DEPLOYMENT WORKFLOW");
            await Task.Delay(400);

            // 1. Dinamik Yol Tespiti (EXE'nin yanındaki 'hile' klasörü)
            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            string hileDir = Path.Combine(rootDir, "hile");
            string jarPath = Path.Combine(hileDir, "WentraClient.jar");
            
            // Oyun Yolu
            string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

            AddLog($">> Scanning local directory: .\\hile\\");

            // 2. Klasör ve Dosya Kontrolü
            if (!Directory.Exists(hileDir))
            {
                AddLog("::error:: Directory 'hile' not found beside loader.");
                ResetButton();
                return;
            }

            if (!File.Exists(jarPath))
            {
                AddLog("::error:: 'WentraClient.jar' not found in .\\hile\\");
                ResetButton();
                return;
            }

            AddLog("::success:: Binary assets verified locally.");
            await Task.Delay(500);

            // 3. Başlatma ve Enjeksiyon
            try
            {
                AddLog("Hooking into Java Virtual Machine...");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gamePath,
                    // JAR dosyasını Ajan olarak sisteme fırlatıyoruz
                    Arguments = $"-javaagent:\"{jarPath}\"",
                    WorkingDirectory = Path.GetDirectoryName(gamePath),
                    UseShellExecute = false
                };

                AddLog("Executing bootstrapper: CraftRise.exe");
                Process.Start(startInfo);

                AddLog("-------------------------------------------");
                AddLog("DEPLOYMENT SUCCESSFUL. Session detached.");
                AddLog("Closing terminal in 3 seconds...");
                
                await Task.Delay(3000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                AddLog("::fatal:: Exception during launch: " + ex.Message);
                ResetButton();
            }
        }

        private void ResetButton()
        {
            btnLaunch.Enabled = true;
            btnLaunch.Text = "🚀 Initialize Deployment";
        }

        private void AddLog(string message)
        {
            this.Invoke((MethodInvoker)delegate {
                lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                lstLogs.SelectedIndex = lstLogs.Items.Count - 1; // Otomatik kaydırma
            });
        }

        #region Form Sürükleme Mantığı
        [DllImport("user32.dll")]
        public static extern bool ReleaseCapture();
        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam);

        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, 0xA1, (IntPtr)0x2, IntPtr.Zero);
            }
        }
        #endregion
    }
}
