using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
namespace ST
{
    class Program
    {
        //Private Methods
        static void Main()
        {
            Security.Initialize();
            Menu();
        }
        private static void Menu()
        {
            Watermark();
            AuthInitialize();
            Console.WriteLine("1.) Login");
            Console.WriteLine("2.) Register");
            Console.WriteLine("Option: ");
            string option = Console.ReadLine();
            if (option == "1")
            {
                Login();
            }
            else if (option == "2")
            {
                Register();
            }
            else
            {
                MessageBox.Show("That isn't a valid option!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Console.Clear();
                Main();
            }
        }
        private static void Register()
        {
            Watermark();
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = Console.ReadLine();
            Console.WriteLine("Discord: ");
            string email = Console.ReadLine() + "@discord.com";
            Console.WriteLine("Token: ");
            string token = Console.ReadLine();
            if (AuthRegister(username, email, password, token))
            {
                MessageBox.Show($"Welcome, {username}!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Login();
            }
            else
            {
                MessageBox.Show("Error! Make sure you have all of your info right!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Main();
            }
        }
        private static void Login()
        {
            Watermark();
            Console.WriteLine("Username: ");
            string username = Console.ReadLine();
            Console.WriteLine("Password: ");
            string password = Console.ReadLine();
            if (AuthLogin(username, password))
            {
                MessageBox.Show($"Welcome, {username}!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
                CustomerMenu();
            }
            else
            {
                MessageBox.Show("Error! Make sure you have all of your info right!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Main();
            }
        }
        private static void CustomerMenu() //Main Menu
        {
            Watermark();
            MessageBox.Show("Spoofing!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Console.WriteLine("Loading Files...");
            Thread.Sleep(2000);
            Directory.CreateDirectory(@"C:\ProgramData\SpooferTemplate\");
            string mapper = @"C:\ProgramData\SpooferTemplate\Local64.exe";
            string driver1 = @"C:\ProgramData\SpooferTemplate\Local64.sys";
            string driver2 = @"C:\ProgramData\SpooferTemplate\Local64cfg.sys";
            var drivers = driver1 + driver2;
            WebClient files = new WebClient();
            Security.DownloadFile("https://cdn.discordapp.com/attachments/711016214998024225/787187597427474432/Local64.exe", mapper); //change all of these
            Security.DownloadFile("https://cdn.discordapp.com/attachments/711016214998024225/787187598908063754/Local64.sys", driver1);
            Security.DownloadFile("https://cdn.discordapp.com/attachments/711016214998024225/787187600540434442/Local64cfg.sys", driver2);
            Console.WriteLine("Done!");
            Thread.Sleep(2000);
            Console.WriteLine("Checking File Integrity...");
            Thread.Sleep(2000);
            Security.CheckHash(mapper, "mglxA3hd3qHp25fCrHQ0eZeRPnBbBWS+hwxEcEw2tgQ="); //change all of these
            Security.CheckHash(driver1, "/L5BeDLXcE169CwUIuIuyL9feodJ98lL5gKvncC9Wts=");
            Security.CheckHash(driver2, "MfTPtMcdpEEgdSchEDoWUSREwTwqwthXp+bxPLZ5tCc=");
            Console.WriteLine("Done!");
            Thread.Sleep(2000);
            Console.WriteLine("Mapping Drivers...");
            Thread.Sleep(2000);
            //Process.Start(mapper, drivers);
            Console.WriteLine("Done!");
            Thread.Sleep(2000);
            Console.WriteLine("Cleaning Up...");
            Thread.Sleep(2000);
            Security.CleanUp();
            Console.WriteLine("Done!");
            Thread.Sleep(2000);
            MessageBox.Show("Spoofed!", "Spoofer Template", MessageBoxButtons.OK, MessageBoxIcon.Information);
            Main();
        }

        //Public Methods
        public static void Watermark()
        {
            Console.Clear();
            Console.Title = "Spoofer Template | Simp#0174";
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("                                                     Spoofer Template");
            Console.WriteLine("                                                         Simp#0174");
            Console.WriteLine();
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void Wait() => Console.ReadKey();
        public static void AuthInitialize() => auth_instance.init();
        public static bool AuthLogin(string username, string password) => auth_instance.login(username, password);
        public static bool AuthRegister(string username, string email, string password, string token) => auth_instance.register(username, email, password, token);

        public static api auth_instance = new api("version", "program_key", "api_key", show_messages: false); //change this
    }
}