using System;
using System.Linq;

namespace OfflineAnalyzer
{
    internal class Program
    {
        [STAThread]
        private static void Main(string[] args)
        {
            if (args.Length != 0 || args.Contains("help"))
                ShowHelp();
            else
                FileOpenHelpers.FileOpener.BeginAnalyzation();

            Console.WriteLine("* Press any key to exit.");
            Console.ReadKey();
        }

        private static void ShowHelp()
        {
            Console.WriteLine("HumanMobility offline data analyzer help:");
            Console.WriteLine("* Acceptable argument: 'help'.");
            Console.WriteLine("* Open client side database (from device) or CSV file from dialog.");
            Console.WriteLine("*");
            Console.WriteLine("* After the data has been processed, you have to chose the save type");
            Console.WriteLine("** text file");
            Console.WriteLine("** chart (image file)");
        }
    }
}
