using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace WentraLoader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            
            // Basit ve şık karanlık tema ayarları
            this.BackColor = Color.FromArgb(13, 17, 23);
            this.ForeColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Argus.pro / Wentra Injector";
            this.Size = new Size(350, 200);

            // Başlatma Butonu
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
            
            // Butona tıklama olayı
            btnLaunch.Click += BtnLaunch_Click;
            this.Controls.Add(btnLaunch);
        }

        private void BtnLaunch_Click(object sender, EventArgs e)
        {
            // 1. Masaüstündeki hile klasörünün yolunu bulur
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string jarPath = Path.Combine(desktopPath, "hile", "WentraClient.jar");

            // 2. CraftRise'ın sistemdeki yüklü olduğu yolu bulur
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string gamePath = Path.Combine(appData, ".craftrise", "CraftRise.exe");

            // 3. Dosyaların gerçekten orada olup olmadığını kontrol eder
            if (!File.Exists(jarPath))
            {
                MessageBox.Show("HATA: JAR dosyası bulunamadı!\n\nLütfen hilenizi şu yola koyun:\n" + jarPath, "Argus.pro - Dosya Eksik", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!File.Exists(gamePath))
            {
                MessageBox.Show("HATA: CraftRise.exe bulunamadı!\nOyunun yüklü olduğundan emin olun.", "Argus.pro - Oyun Bulunamadı", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            // 4. Oyunu JAR dosyası ile birlikte (Agent olarak) başlatır
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = gamePath;
                
                // JAR dosyasını EXE'nin içine gömmeden dışarıdan enjekte eden komut
                startInfo.Arguments = $"-javaagent:\"{jarPath}\"";
                startInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
                
                Process.Start(startInfo);
                
                MessageBox.Show("WentraClient başarıyla CraftRise'a enjekte edildi!", "Argus.pro - Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // İşlem bitince loader kendini kapatır
                Application.Exit();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Enjekte sırasında kritik bir hata oluştu:\n" + ex.Message, "Argus.pro - Kritik Hata", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
