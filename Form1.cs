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
        // GitHub Dark Tema Renkleri
        Color ghBg = Color.FromArgb(13, 17, 23);
        Color ghGreen = Color.FromArgb(35, 134, 54);
        Color ghText = Color.FromArgb(201, 209, 217);

        public Form1()
        {
            InitializeComponent();
            ApplyGitHubStyle();
        }

        private void ApplyGitHubStyle()
        {
            this.BackColor = ghBg;
            this.ForeColor = ghText;
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Size = new Size(500, 350);

            // Başlık Çubuğu Taklidi
            Label title = new Label() { Text = "Argus.pro / Wentra-Client-Loader", Location = new Point(10, 10), AutoSize = true, Font = new Font("Segoe UI Semibold", 10) };
            this.Controls.Add(title);

            // Çıkış Butonu
            Button btnExit = new Button() { Text = "X", Size = new Size(30, 30), Location = new Point(460, 5), FlatStyle = FlatStyle.Flat, ForeColor = Color.White };
            btnExit.FlatAppearance.BorderSize = 0;
            btnExit.Click += (s, e) => Application.Exit();
            this.Controls.Add(btnExit);
        }

        private async void btnLaunch_Click(object sender, EventArgs e)
        {
            AddLog("Checking environment... [OK]");
            await Task.Delay(500);
            
            // Masaüstündeki hile klasörü yolu
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jarPath = Path.Combine(desktop, "hile", "WentraClient.jar");
            
            // CraftRise yolu (Kullanıcı adına göre dinamik)
            string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

            if (!File.Exists(jarPath))
            {
                AddLog("[ERROR] Failed to locate: " + Path.GetFileName(jarPath));
                AddLog("[HINT] Create a folder named 'hile' on Desktop and put the JAR inside.");
                return;
            }

            AddLog("Injecting JavaAgent parameters into bootstrapper...");
            await Task.Delay(800);

            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = gamePath;
                
                // Efendim, burası hilenin ajan olarak sızmasını sağlayan en önemli satır:
                startInfo.Arguments = $"-javaagent:\"{jarPath}\"";
                
                startInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
                
                AddLog("Success! Launching CraftRise...");
                Process.Start(startInfo);

                AddLog("Wentra successfully detached. Closing loader in 3 seconds.");
                await Task.Delay(3000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                AddLog("[FATAL ERROR] " + ex.Message);
            }
        }

        private void AddLog(string message)
        {
            // Terminal tarzı loglama
            lstLogs.Items.Add($"[{DateTime.Now:HH:mm:ss}] {message}");
            lstLogs.SelectedIndex = lstLogs.Items.Count - 1;
        }

        // Formu sürüklemek için gerekli Windows API dokunuşu
        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);
            if (m.Msg == 0x84) m.Result = (IntPtr)0x2;
        }
    }
}
