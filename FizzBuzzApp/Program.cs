using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FizzBuzzApp
{
    class Program
    {
        private static Screens CurrentScreen;
        private static bool DoPrintMenu;
        private static FileConfig fileConfig;
        private static string ConfigFileName = "config";
        static void Main(string[] args)
        {
            CurrentScreen = Screens.Menu;
            DoPrintMenu = true;
            var option = "";
            fileConfig = GetConfig();

            while(option != "Q" && option != "q")
            {
                if(DoPrintMenu)
                PrintRequiredMenu();

                option = Console.ReadLine();
                DoPrintMenu = true;
                ExecuteRequiredAction(option);

            }

            Console.WriteLine("The application has ended because user entered Q as an option");
            Console.WriteLine("Enter any value to exit the console.");
            Console.ReadLine();
        }

        public static void PrintRequiredMenu()
        {
            switch (CurrentScreen)
            {
                case Screens.Menu:
                    PrintMenu();
                    break;
                case Screens.FizzBuzzMenu:
                    PrintFizzBuzzMenu();
                    break;
                case Screens.ConfigureFilePath:
                    PrintConfigureNamePathMenu();
                    break;
                case Screens.ConfigureLogPath:
                    PrintConfigureLogPathMenu();
                    break;
            }
            Log("Menu was printed");
        }

        public static void ExecuteRequiredAction(string option)
        {
            switch (CurrentScreen)
            {
                case Screens.Menu:
                    ReadMenuOption(option);
                    break;
                case Screens.FizzBuzzMenu:
                    ReadFizzBuzzOption(option);
                    break;
                case Screens.ConfigureFilePath:
                    ReadConfigureFilePathOption(option);
                    break;
                case Screens.ConfigureLogPath:
                    ReadConfigureLogPathOption(option);
                    break;
                case Screens.FizzBuzzCompleted:
                    CurrentScreen = Screens.FizzBuzzMenu;
                    break;
                case Screens.ConfigureFilePathCompleted:
                    CurrentScreen = Screens.Menu;
                    break;
                case Screens.ConfigureLogPathCompleted:
                    CurrentScreen = Screens.Menu;
                    break;
            }
            Log("Action was executed");
        }

        private static void ReadConfigureLogPathOption(string option)
        {
            var uri = option;
            var wellFormed = Uri.IsWellFormedUriString(option, UriKind.RelativeOrAbsolute);
            if (wellFormed)
            {
                var path = Path.GetDirectoryName(option);
                var fileName = Path.GetFileName(option);
                fileConfig.LogName = fileName;
                fileConfig.LogPath = path;
                SaveConfig();
                Log("Log Path has been changed");
                CurrentScreen = Screens.ConfigureFilePathCompleted;
                Console.WriteLine("The configuration has been changed successfully. Please press enter to return to the main menu.");
            }
            else
            {
                Console.WriteLine("The entered Path is not valid. Please supply a valid path or filename.");
                AvoidMenu();
                return;
            }

        }

        private static void SaveConfig()
        {
            var file = ConfigFileName;
            StreamWriter resultFile;

            resultFile = new StreamWriter(file);
            var configJson = JsonConvert.SerializeObject(fileConfig);
            resultFile.WriteLine(configJson);

            resultFile.Close();
            Log("Configuration was changed");
        }

        private static FileConfig ReadConfig()
        {
            FileConfig result = new FileConfig();

            if (File.Exists(ConfigFileName))
            {
                var jsonStr = File.ReadAllText(ConfigFileName);
                result = JsonConvert.DeserializeObject<FileConfig>(jsonStr);
            } 

            if (result == null) result = new FileConfig();

            return result;
        }

        private static void ReadConfigureFilePathOption(string option)
        {
            var uri = option;
            var wellFormed = Uri.IsWellFormedUriString(option, UriKind.RelativeOrAbsolute);
            if (wellFormed)
            {
                var path = Path.GetDirectoryName(option);
                var fileName = Path.GetFileName(option);
                fileConfig.FileName = fileName;
                fileConfig.FilePath = path;
                SaveConfig();
                CurrentScreen = Screens.ConfigureLogPathCompleted;
                Console.WriteLine("The configuration has been changed successfully. Please press enter to return to the main menu.");
                Log("File path was changed");
            }
            else
            {
                Console.WriteLine("The entered Path is not valid. Please supply a valid path or filename.");
                AvoidMenu();
                return;
            }
        }

        public static void ReadMenuOption(string option)
        {
            int opt;
            var parsed = int.TryParse(option, out opt);
            if (parsed)
            {
                if(opt >= 1 && opt <= 3)
                {
                    switch (opt)
                    {
                        case 1:
                            CurrentScreen = Screens.FizzBuzzMenu;
                            break;

                        case 2:
                            CurrentScreen = Screens.ConfigureFilePath;
                            break;

                        case 3:
                            CurrentScreen = Screens.ConfigureLogPath;
                            break;
                    }
                }
                else
                {
                    Console.WriteLine("The entered value was not available. Please only enter numeric available values from the menu.");
                }
            }
            else
            {
                Console.WriteLine("The entered value was not numeric. Please only enter numeric available values from the menu.");
            }
        }

        public static void ReadFizzBuzzOption(string option)
        {
            if(option == "R" || option == "r")
            {
                CurrentScreen = Screens.Menu;
                return;
            }
            var options = option.Trim().Split('|').ToList(); ;
            int start = 0;
            int end = 0;
            if(options.Count != 2)
            {
                Console.WriteLine("The input was not well formed. Remember to include one (and only one) | between the start and end.");
                AvoidMenu();
                return;
            }
            var parsed = int.TryParse(options[0], out start) && int.TryParse(options[1], out end);
            if (parsed)
            {
                if(end > start)
                {
                    var str = FizzBuzz(start, end);
                    SaveFizzBuzzResult(str);
                    Console.WriteLine("Fizz buzz was successfull and saved onto the following file: " + fileConfig.FilePath + fileConfig.FileName);
                    Console.WriteLine("Press any key (but Q) to continue.");
                    CurrentScreen = Screens.FizzBuzzCompleted;
                }
                else
                {
                    Console.WriteLine("Remember that the end number (the second one) must be greater than the start number (the first one). Please try again");
                    AvoidMenu();
                    return;
                }
            }
            else
            {
                Console.WriteLine("The input didn't have the required amount of numbers in it. Remember to add two numbers, one before the dash (|) and one after. i.e 10|15");
                AvoidMenu();
                return;
            }

        }

        public static void AvoidMenu()
        {
            DoPrintMenu = false;
        }

        public static void PrintMenu()
        {
            Console.WriteLine("Welcome to the Fizzbuzzer. Please select an option:");
            Console.WriteLine("1) Start The Fizzbuzzer");
            Console.WriteLine("2) Configure Name and Path of File where the fizzbuzzer will be saved.");
            Console.WriteLine("3) Configure the Name and Path of the Log File");
            
        }

        public static void PrintFizzBuzzMenu()
        {
            Console.WriteLine("Excellent choice!");
            Console.WriteLine("To Start FizzBuzzing please enter the start and end separated by a dash (|) i.e 10|30. Remember that the first number must be greater than the second one");
            Console.WriteLine("To return to the menu Screen type R");
        }

        public static void PrintConfigureNamePathMenu()
        {
            Console.WriteLine("Please provide the full desired path including the name of the file");
        }

        public static void PrintConfigureLogPathMenu()
        {
            Console.WriteLine("Please provide the full desired path including the name of the file");
        }

        private static string FizzBuzz(int start, int end)
        {
            string result = "";
            for(int i = start; i <= end; i++)
            {
                result += GetFizzBuzzValue(i);
                result += "\n";
            }
            Log(String.Format("FizzBuzz was played with start of {0} and end of {1}", start, end));
            return result;
        }

        private static string GetFizzBuzzValue(int number)
        {
            return (number % 3 == 0 && number % 5 == 0 ? "FizzBuzz" :
                number % 3 == 0 ? "Fizz" :
                number % 5 == 0 ? "Buzz" : number.ToString());
        }

        public static void SaveFizzBuzzResult(string res)
        {
            var rs = "FizBuzz Generated on " + DateTime.Now;
            rs += "\n";
            rs += "-------------------------";
            rs += "\n";
            rs += res;
            rs += "-------------------------";
            WriteFizzBuzz(rs);
        }

        public static FileConfig GetConfig()
        {
            return ReadConfig();
        }

        public class FileConfig
        {
            public string FileName { get; set; }
            public string FilePath { get; set; }
            public string LogName
            {
                get; set;
            }
            public string LogPath { get; set; }

            public FileConfig()
            {
                FileName = "output.txt";
                FilePath = "";
                LogName = "log.txt";
                LogPath = "";
            }
        }

        public static void Log(string logText)
        {
            var str = "[" + DateTime.Now.ToString() + "] " + logText; 
            var file = Path.Combine(fileConfig.LogPath, fileConfig.LogName);
            StreamWriter log;

            if (!File.Exists(file))
            {
                log = new StreamWriter(file);
            }
            else
            {
                log = File.AppendText(file);
            }

            log.WriteLine(str);
            log.WriteLine();

            log.Close();
        }
        
        public static void WriteFizzBuzz(string text)
        {
            var file = Path.Combine(fileConfig.FilePath, fileConfig.FileName);
            StreamWriter resultFile;

            if (!File.Exists(file))
            {
                resultFile = new StreamWriter(file);
            }
            else
            {
                resultFile = File.AppendText(file);
            }

            resultFile.Write(text);
            resultFile.WriteLine();

            resultFile.Close();
        }
    }

    public enum Screens
    {
        Menu = 0,
        FizzBuzzMenu = 1,
        FizzBuzzCompleted = 2,
        ConfigureFilePath = 3,
        ConfigureLogPath = 4,
        ConfigureFilePathCompleted = 5,
        ConfigureLogPathCompleted = 6
    }
}
