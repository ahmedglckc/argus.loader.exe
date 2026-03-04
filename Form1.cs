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
        public Form1()
        {
            InitializeComponent();
            SetupUI();
        }

        private void SetupUI()
        {
            // Temiz, karanlık ve profesyonel tasarım
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Argus.pro / Wentra Injector";
            this.Size = new Size(350, 200);

            // Başlat Butonu
            Button btnLaunch = new Button();
            btnLaunch.Text = "🚀 Hileyi Enjekte Et";
            btnLaunch.Size = new Size(200, 50);
            btnLaunch.Location = new Point(65, 50);
            btnLaunch.BackColor = Color.FromArgb(35, 134, 54);
            btnLaunch.ForeColor = Color.White;
            btnLaunch.FlatStyle = FlatStyle.Flat;
            btnLaunch.FlatAppearance.BorderSize = 0;
            btnLaunch.Font = new Font("Segoe UI", 10f, FontStyle.Bold);
            btnLaunch.Cursor = Cursors.Hand;
            btnLaunch.Click += BtnLaunch_Click;
            
            this.Controls.Add(btnLaunch);
        }

        private async void BtnLaunch_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            btn.Enabled = false;
            btn.Text = "⌛ Bekleyin...";

            // 1. Masaüstü/hile/WentraClient.jar yolunu belirle
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jarPath = Path.Combine(desktopPath, "hile", "WentraClient.jar");
            
            // 2. Oyunun yolunu belirle
            string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

            // 3. Dosya Kontrolleri
            if (!File.Exists(jarPath))
            {
                MessageBox.Show("HATA: JAR dosyası bulunamadı!\nLütfen hilenizi şu yola koyun:\n" + jarPath, "Argus.pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn.Enabled = true;
                btn.Text = "🚀 Hileyi Enjekte Et";
                return;
            }

            if (!File.Exists(gamePath))
            {
                MessageBox.Show("HATA: CraftRise.exe bulunamadı!\nOyunun yüklü olduğundan emin olun.", "Argus.pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn.Enabled = true;
                btn.Text = "🚀 Hileyi Enjekte Et";
                return;
            }

            // 4. Oyunu Ajan (javaagent) parametresiyle başlat
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = gamePath,
                    Arguments = $"-javaagent:\"{jarPath}\"",
                    WorkingDirectory = Path.GetDirectoryName(gamePath),
                    UseShellExecute = false
                };
                
                Process.Start(startInfo);
                
                MessageBox.Show("WentraClient başarıyla başlatıldı! İyi oyunlar efendim.", "Argus.pro", MessageBoxButtons.OK, MessageBoxIcon.Information);
                await Task.Delay(1000);
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Kritik Hata:\n" + ex.Message, "Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btn.Enabled = true;
                btn.Text = "🚀 Hileyi Enjekte Et";
            }
        }
    }
}
