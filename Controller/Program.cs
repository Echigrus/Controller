using System;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;
using System.Windows.Automation;

namespace Controller
{
    class Program
    {
		static string[] paths = {
				@"\1c\Carousel.exe",
				@"\2c\Carousel.exe",
				@"\3c\Carousel.exe",
				@"\4c\Carousel.exe",
				@"\5c\Carousel.exe"};
		static Process[] programms = new Process[5];
		
		// https://docs.microsoft.com/en-us/windows/console/setconsolectrlhandler?WT.mc_id=DT-MVP-5003978
		[DllImport("Kernel32")]
		private static extern bool SetConsoleCtrlHandler(SetConsoleCtrlEventHandler handler, bool add);

		// https://docs.microsoft.com/en-us/windows/console/handlerroutine?WT.mc_id=DT-MVP-5003978
		private delegate bool SetConsoleCtrlEventHandler(CtrlType sig);

		private enum CtrlType
		{
			CTRL_C_EVENT = 0,
			CTRL_BREAK_EVENT = 1,
			CTRL_CLOSE_EVENT = 2,
			CTRL_LOGOFF_EVENT = 5,
			CTRL_SHUTDOWN_EVENT = 6
		}

		static void Main(string[] args)
        {
			SetConsoleCtrlHandler(Handler, true);
            IntPtr hwndChild = IntPtr.Zero;
			string programName, curDir = Directory.GetCurrentDirectory();

			for(int i = 0; i < 5; i++)
            {
				programms[i] = new Process();
				programms[i].StartInfo.FileName = curDir+paths[i];
				programms[i].StartInfo.WorkingDirectory = Path.GetDirectoryName(curDir+paths[i]);
			}
			
            while (true)
            {
				for (int i = 0; i < 5; i++)
				{
					programName = "Carousel" + (i + 1).ToString();
					var window = AutomationElement.RootElement.FindFirst(
						TreeScope.Children,
						new PropertyCondition(AutomationElement.NameProperty, programName));
					if (window == null)
					{
						programms[i].Start();
						Thread.Sleep(1000);
						//var startButton = window.FindFirst(TreeScope.Descendants, new PropertyCondition(AutomationElement.NameProperty, "Старт"));
						//var invokePattern = startButton.GetCurrentPattern(InvokePattern.Pattern) as InvokePattern;
						//invokePattern.Invoke();

						Console.WriteLine("Restarting bot №" + (i + 1).ToString() + " at " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
					}
                    else {
						Console.WriteLine("Working bot №" + (i + 1).ToString() + ", checked at " + DateTime.Now.Hour + ":" + DateTime.Now.Minute + ":" + DateTime.Now.Second);
					}
				}
				Thread.Sleep(TimeSpan.FromMinutes(10));
			}
		}

		private static bool Handler(CtrlType signal)
		{
			switch (signal)
			{
				case CtrlType.CTRL_BREAK_EVENT:
				case CtrlType.CTRL_C_EVENT:
				case CtrlType.CTRL_LOGOFF_EVENT:
				case CtrlType.CTRL_SHUTDOWN_EVENT:
				case CtrlType.CTRL_CLOSE_EVENT:
					Console.WriteLine("Closing");
					// TODO Cleanup resources
					for(int i = 0; i < 5; i++)
                    {
						programms[i].Kill();
                    }
					Environment.Exit(0);
					return false;

				default:
					return false;
			}
		}
	}
}
