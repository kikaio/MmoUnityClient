using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public static class FileUtils
{

    //특정 shell 실행가능?
    public static async Task RunShell (string _cmd, bool _isShell, string _args = null)
    {
        string projDir = Directory.GetCurrentDirectory();
        _cmd = $"{projDir}/{_cmd}";

        UnityEngine.Debug.Log($"[{DateTime.Now.ToShortTimeString()}] run cmd : {_cmd.ToString()}");

        var shellThread = new Thread(()=> {
            ProcessStartInfo ps = new ProcessStartInfo(_cmd);
            using (var p = new Process())
            {
                ps.UseShellExecute = _isShell;
                if (_isShell == false)
                {
                    ps.RedirectStandardError = true;
                    ps.RedirectStandardOutput = true;
                    ps.StandardOutputEncoding = System.Text.Encoding.UTF8;
                }
                if (_args != null && _args != "")
                {
                    ps.Arguments = _args;
                }
                p.StartInfo = ps;
                p.Start();
                p.WaitForExit();
                if (_isShell)
                {
                    string ret = p.StandardOutput.ReadToEnd().Trim();
                    if (string.IsNullOrEmpty(ret) == false)
                        UnityEngine.Debug.Log($"[{DateTime.Now.ToShortTimeString()}]{ret}");
                    string errors = p.StandardError.ReadToEnd().Trim();
                    if(string.IsNullOrEmpty(errors) == false)
                        UnityEngine.Debug.LogError($"[{DateTime.Now.ToShortTimeString()}]{errors}");
                }
            }
        });

        shellThread.Start();
    }

    public static async Task KillProcessByName(string _name)
    {
        foreach (var proc in Process.GetProcesses())
        {
            if (proc.ProcessName == _name)
                proc.Kill();
        }
    }

    public static async Task RunServerProcess(string _path, string _name, int _portNo)
    {
        try
        {
            if (_portNo < 0)
            {
                UnityEngine.Debug.LogError($"Server port set is invalid");
                return;
            }

            await KillProcessByName(_name);
            string curPath = System.Environment.CurrentDirectory;
            string processPath = $@"{_path}\{_name}";
            
            using (var newPs = Process.Start(processPath, $"{_portNo}"))
            {

                newPs.Exited += (sender, e) =>
                {
                    UnityEngine.Debug.Log("process is exited");
                };
            }
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError($"{e.ToString()}");
            throw;
        }
        return;
    }
}
