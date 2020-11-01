using BarRaider.SdTools;
using System.Runtime.InteropServices;

namespace OSDSidekick
{
    class Program
    {
        public static int nbMonitors;
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int Gen_init();

        // Token: 0x06000188 RID: 392
        [DllImport("GenLib.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool Gen_Close();

        static void Main(string[] args)
        {
            // Uncomment this line of code to allow for debugging
            //while (!System.Diagnostics.Debugger.IsAttached) { System.Threading.Thread.Sleep(100); }
            Program.nbMonitors = Gen_init();

            SDWrapper.Run(args);

            Gen_Close();
        }
    }
}
