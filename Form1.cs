using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Linq;

namespace ArgusLoader
{
    public partial class Form1 : Form
    {
        #region GitHub Pro Dark Palette
        private static readonly Color BgMain = Color.FromArgb(13, 17, 23);
        private static readonly Color BgHeader = Color.FromArgb(22, 27, 34);
        private static readonly Color GhGreen = Color.FromArgb(35, 134, 54);
        private static readonly Color GhGreenHover = Color.FromArgb(46, 160, 67);
        private static readonly Color GhRed = Color.FromArgb(248, 81, 73);
        private static readonly Color TextMain = Color.FromArgb(201, 209, 217);
        private static readonly Color TextMuted = Color.FromArgb(139, 148, 158);
        #endregion

        public Form1()
        {
            InitializeComponent();
            InitializeAdvancedUI();
        }

        private void InitializeAdvancedUI()
        {
            // Ana Form Ayarları
            this.BackColor = BgMain;
            this.ForeColor = TextMain;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(580, 420);
            this.Text = "Argus Deployment Terminal";

            // ListBox (Log Alanı) İyileştirmesi
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

            AddLog("System initialized. Awaiting user authorization...");
        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            SetInterfaceState(false);
            
            AddLog("-------------------------------------------");
            AddLog(">> INITIALIZING DEPLOYMENT WORKFLOW");
            await Task.Delay(400);

            // 1. Dinamik Yol Tespiti (Exe'nin yanındaki 'hile' klasörü)
            string rootDir = AppDomain.CurrentDomain.BaseDirectory;
            string hileDir = Path.Combine(rootDir, "hile");
            string jarPath = Path.Combine(hileDir, "WentraClient.jar");
            string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

            // 2. Ön Kontroller (Pre-flight Checks)
            if (!Directory.Exists(hileDir))
            {
                AddLog("[!] ERROR: 'hile' directory is missing in root.");
                ResetInterface();
                return;
            }

            if (!File.Exists(jarPath))
            {
                AddLog($"[!] ERROR: Binary target not found: {Path.GetFileName(jarPath)}");
                ResetInterface();
                return;
            }

            // 3. Oyun Süreci Kontrolü (Opsiyonel ama profesyonel bir dokunuş)
            if (Process.GetProcessesByName("javaw").Length > 0)
            {
                AddLog("[*] WARN: JVM instance already detected. Procedural hook starting...");
            }

            AddLog("[+] Verifying local repository assets...");
            await Task.Delay(500);

            // 4. Deployment Başlatma
            try
            {
                AddLog("[+] Constructing JavaAgent arguments...");
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gamePath,
                    // Efendim, hileyi ajan yapan sihirli parametre:
                    Arguments = $"-javaagent:\"{jarPath}\"",
                    WorkingDirectory = Path.GetDirectoryName(gamePath),
                    UseShellExecute = false,
                    RedirectStandardOutput = false
                };

                AddLog("[+] Executing bootstrapper: CraftRise.exe");
                Process.Start(startInfo);

                AddLog(">> DEPLOYMENT SUCCESSFUL.");
                AddLog(">> Closing Argus session in 3 seconds...");
                
                await Task.Delay(3000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                AddLog("[!] FATAL: " + ex.Message);
                ResetInterface();
            }
        }

        private void SetInterfaceState(bool enabled)
        {
            btnLaunch.Enabled = enabled;
            btnLaunch.Text = enabled ? "🚀 Initialize Deployment" : "⌛ Running...";
            btnLaunch.BackColor = enabled ? GhGreen : GhHeader;
        }

        private void ResetInterface()
        {
            this.Invoke((MethodInvoker)delegate {
                SetInterfaceState(true);
                AddLog(">> Session suspended. Awaiting retry.");
            });
        }

        private void AddLog(string message)
        {
            if (lstLogs.InvokeRequired)
            {
                lstLogs.Invoke(new Action<string>(AddLog), message);
                return;
            }

            string timestamp = DateTime.Now.ToString("HH:mm:ss");
            lstLogs.Items.Add($"[{timestamp}] {message}");
            
            // Otomatik Kaydırma
            lstLogs.TopIndex = lstLogs.Items.Count - 1;
        }

        #region Windows API - Drag Mechanism
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
