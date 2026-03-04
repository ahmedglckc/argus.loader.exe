private async void btnLaunch_Click(object sender, EventArgs e)
{
    // 1. Dosya Yollarını Tanımla
    string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
    string jarPath = Path.Combine(desktopPath, "hile", "WentraClient.jar");
    
    // CraftRise'ın bilgisayarınızdaki yüklü olduğu yer
    string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    string gamePath = Path.Combine(appData, ".craftrise", "CraftRise.exe");

    // 2. Kontrolleri Yap
    if (!File.Exists(jarPath))
    {
        MessageBox.Show("HATA: Masaüstündeki 'hile' klasöründe WentraClient.jar bulunamadı!", "Argus.pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    if (!File.Exists(gamePath))
    {
        MessageBox.Show("HATA: CraftRise yüklü değil veya yolu hatalı!", "Argus.pro", MessageBoxButtons.OK, MessageBoxIcon.Error);
        return;
    }

    // 3. Oyunu Ajan (javaagent) ile Başlat
    try
    {
        ProcessStartInfo startInfo = new ProcessStartInfo();
        startInfo.FileName = gamePath;
        
        // Bu parametre hileyi oyunun içine sessizce sızdırır
        startInfo.Arguments = $"-javaagent:\"{jarPath}\"";
        
        startInfo.WorkingDirectory = Path.GetDirectoryName(gamePath);
        
        Process.Start(startInfo);

        // Başarılı mesajı ve 2 saniye sonra kapanış
        MessageBox.Show("Wentra Client Başarıyla Enjekte Edildi!", "Başarılı", MessageBoxButtons.OK, MessageBoxIcon.Information);
        await Task.Delay(2000);
        Application.Exit();
    }
    catch (Exception ex)
    {
        MessageBox.Show("Kritik Hata: " + ex.Message);
    }
}
