private async void btnLaunch_Click(object sender, EventArgs e)
{
    // Arayüzü kilitleyerek kullanıcının ardışık tıklamalarını engelliyoruz.
    btnLaunch.Enabled = false;
    btnLaunch.Text = "⌛ Deploying...";

    // Kullanıcıya işlemin başladığını terminal üzerinden bildiriyoruz.
    AddLog(">> Argus deployment initiated...");
    await Task.Delay(400);

    // 1. DİNAMİK YOL TESPİTİ
    // EXE'nin çalıştığı ana dizini tespit ediyoruz.
    string currentDir = AppDomain.CurrentDomain.BaseDirectory;
    
    // Yanındaki 'hile' klasörünün ve içindeki JAR dosyasının yolunu birleştiriyoruz.
    string jarPath = Path.Combine(currentDir, "hile", "WentraClient.jar");
    
    // Oyunun sistemdeki standart kurulum yolunu belirliyoruz.
    string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

    // 2. KRİTİK KONTROLLER (PRE-FLIGHT CHECKS)
    
    // A. Hile dosyası kontrolü
    if (!File.Exists(jarPath))
    {
        AddLog("[!] ERROR: .\\hile\\WentraClient.jar is missing.");
        AddLog("[!] ACTION: Lütfen JAR dosyasını hile klasörüne koyun.");
        ResetButton();
        return;
    }

    // B. Oyun dosyası kontrolü (EKLEDİĞİMİZ GÜVENLİK ADIMI)
    if (!File.Exists(gamePath))
    {
        AddLog("[!] ERROR: CraftRise.exe not found in AppData.");
        AddLog("[!] ACTION: Lütfen oyunun yüklü olduğundan emin olun.");
        ResetButton();
        return;
    }

    AddLog("[+] Binary and target executables verified.");
    await Task.Delay(300);

    // 3. ENJEKSİYON VE BAŞLATMA
    try
    {
        AddLog("[+] Preparing JVM arguments...");
        
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = gamePath,
            // Hilenin kalbi: javaagent parametresi ile JAR'ı oyuna gömüyoruz.
            Arguments = $"-javaagent:\"{jarPath}\"", 
            WorkingDirectory = Path.GetDirectoryName(gamePath),
            UseShellExecute = false
        };

        AddLog("[+] Executing CraftRise with Argus Agent...");
        Process.Start(startInfo);

        AddLog("-------------------------------------------");
        AddLog(">> DEPLOYMENT SUCCESSFUL.");
        AddLog(">> Argus session detaching. Closing terminal...");
        
        // İşlem bittikten sonra iz bırakmamak için 2 saniye bekleyip kapanıyor.
        await Task.Delay(2000);
        Application.Exit();
    }
    catch (Exception ex)
    {
        // Beklenmeyen sistem hatalarını (yetki eksikliği vb.) yakalıyoruz.
        AddLog("[!] FATAL EXCEPTION: " + ex.Message);
        ResetButton();
    }
}
