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
        #region GitHub Renk Paleti (Professional Dark)
        private static readonly Color BgMain = Color.FromArgb(13, 17, 23);       // Ana Arka Plan
        private static readonly Color BgHeader = Color.FromArgb(22, 27, 34);     // Üst Bar
        private static readonly Color GhGreen = Color.FromArgb(35, 134, 54);     // GitHub Buton Yeşili
        private static readonly Color GhGreenHover = Color.FromArgb(46, 160, 67); // Buton Hover
        private static readonly Color BorderColor = Color.FromArgb(48, 54, 61);   // Çerçeve
        private static readonly Color TextMain = Color.FromArgb(201, 209, 217);  // Ana Yazı
        private static readonly Color TextMuted = Color.FromArgb(139, 148, 158); // Sönük Yazı (Loglar için)
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

            // Başlık Paneli (Sürükleme Alanı)
            Panel pnlHeader = new Panel { Dock = DockStyle.Top, Height = 45, BackColor = BgHeader };
            Label lblTitle = new Label { 
                Text = " Argus.pro / Wentra-Client-v1.0", 
                Location = new Point(15, 13), 
                AutoSize = true, 
                Font = new Font("Segoe UI Semibold", 10f) 
            };
            
            // Çıkış Butonu (GitHub Tarzı)
            Button btnExit = new Button { 
                Text = "×", 
                Size = new Size(35, 35), 
                Location = new Point(510, 5), 
                FlatStyle = FlatStyle.Flat, 
                ForeColor = Color.White,
                Font = new Font("Arial", 14f)
            };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();
            
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(btnExit);
            this.Controls.Add(pnlHeader);

            // Log Paneli (Terminal Alanı)
            // Not: ListBox yerine RichTextBox kullanırsanız renkli log yapabilirsiniz.
            lstLogs.BackColor = Color.FromArgb(1, 4, 9); // Daha koyu terminal iç rengi
            lstLogs.BorderStyle = BorderStyle.FixedSingle;
            lstLogs.ForeColor = TextMuted;
            lstLogs.Font = new Font("Consolas", 9f);
            lstLogs.Location = new Point(15, 60);
            lstLogs.Size = new Size(520, 260);

            // GitHub "Launch" Butonu
            btnLaunch.BackColor = GhGreen;
            btnLaunch.FlatStyle = FlatStyle.Flat;
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.ForeColor = Color.White;
            btnLaunch.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnLaunch.Text = "🚀 Initialize Deployment";
            btnLaunch.Size = new Size(520, 45);
            btnLaunch.Location = new Point(15, 335);
            btnLaunch.Cursor = Cursors.Hand;

            // Buton Hover Efekti
            btnLaunch.MouseEnter += (s, e) => btnLaunch.BackColor = GhGreenHover;
            btnLaunch.MouseLeave += (s, e) => btnLaunch.BackColor = GhGreen;
        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            btnLaunch.Enabled = false;
            btnLaunch.Text = "⌛ Running Workflow...";

            AddLog("--- STARTING ARGUS DEPLOYMENT WORKFLOW ---");
            await Task.Delay(600);

            // 1. Yol Taraması
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jarPath = Path.Combine(desktop, "hile", "WentraClient.jar");
            string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

            AddLog("Checking repository files: WentraClient.jar");
            if (!File.Exists(jarPath))
            {
                AddLog("::error:: Binary not found in 'Desktop/hile/'");
                ResetButton();
                return;
            }
            AddLog("::success:: Binary located successfully.");

            // 2. JVM & Agent Hazırlığı
            AddLog("Hooking into Java Virtual Machine...");
            await Task.Delay(800);
            AddLog("Injecting javaagent: " + Path.GetFileName(jarPath));

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gamePath,
                    Arguments = $"-javaagent:\"{jarPath}\"",
                    WorkingDirectory = Path.GetDirectoryName(gamePath),
                    UseShellExecute = false
                };

                AddLog("Executing bootstrapper: CraftRise.exe");
                Process.Start(startInfo);

                AddLog("-------------------------------------------");
                AddLog("DEPLOYMENT SUCCESSFUL. Client detached.");
                AddLog("Closing loader in 3 seconds...");
                
                await Task.Delay(3000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                AddLog("::fatal:: Exception during deployment: " + ex.Message);
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
            // Cross-thread güvenliği ve terminal stili loglama
            this.Invoke((MethodInvoker)delegate {
                lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
                lstLogs.SelectedIndex = lstLogs.Items.Count - 1;
            });
        }

        #region Sürükleme Mantığı (Header Panel için)
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
