private async void btnLaunch_Click(object sender, EventArgs e)
{
    btnLaunch.Enabled = false;
    btnLaunch.Text = "⌛ Deploying...";

    AddLog(">> Argus deployment initiated...");
    await Task.Delay(400);

    // 1. EXE'nin yanındaki 'hile' klasörü
    string currentDir = AppDomain.CurrentDomain.BaseDirectory;
    string jarPath = Path.Combine(currentDir, "hile", "WentraClient.jar");
    
    // CraftRise yolu
    string gamePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".craftrise", "CraftRise.exe");

    // 2. Kontroller
    if (!File.Exists(jarPath))
    {
        AddLog("[!] ERROR: .\\hile\\WentraClient.jar missing.");
        ResetButton();
        return;
    }

    AddLog("[+] Binary verified at local path.");

    try
    {
        ProcessStartInfo startInfo = new ProcessStartInfo
        {
            FileName = gamePath,
            Arguments = $"-javaagent:\"{jarPath}\"", // Ban riskini azaltan ajan enjeksiyonu
            WorkingDirectory = Path.GetDirectoryName(gamePath),
            UseShellExecute = false
        };

        AddLog("[+] Executing CraftRise with agent...");
        Process.Start(startInfo);

        AddLog(">> Success. Closing terminal.");
        await Task.Delay(2000);
        Application.Exit();
    }
    catch (Exception ex)
    {
        AddLog("[!] Fatal: " + ex.Message);
        ResetButton();
    }
}
