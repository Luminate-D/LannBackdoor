using System;
using System.ServiceProcess;

namespace LBService {
    internal class Program {
        private static Service service = new Service();

        public static void Main(string[] args) {
            if (!Environment.UserInteractive) ServiceBase.Run(service);
        }
    }
}