using System;
using System.Runtime.Loader;
using System.Threading;

using FluentScheduler;

using Microsoft.Extensions.Configuration;

using TeamSpeakNotifier.Senders;

namespace TeamSpeakNotifier
{
    public class Program
    {
        public static AppSettings Settings { get; set; }
        public static bool Close { get; set; }

        private static void Main(string[] args)
        {
            Init();

            JobManager.Initialize(new TaskRegistry(Settings));

            while (!Close)
            {
                Thread.Sleep(1000);
            }
        }

        private static void Init()
        {
            AssemblyLoadContext.Default.Unloading += SigTermEventHandler; //register sigterm event handler. 
            Console.CancelKeyPress += CancelHandler; //register sigint event handler

            InitSettings();

            MessageSender.RegisterSender(new ConsoleSender(Settings));
            MessageSender.RegisterSender(new EmailSender(Settings));

            Console.WriteLine($"TeamSpeak user Monitor ({Settings.Environment})");
            Console.WriteLine("======================");
            MessageSender.SendMessage<ConsoleSender>("TeamSpeak Monitor Started");
        }

        private static void InitSettings()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

            IConfiguration config = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", false, false)
                .AddJsonFile($"appsettings.{env}.json", true, false)
                .Build();

            Settings = config.Get<AppSettings>();
        }

        private static void CancelHandler(object sender, ConsoleCancelEventArgs e)
        {
            e.Cancel = true;
            JobManager.Stop();
            Close = true;
        }

        private static void SigTermEventHandler(AssemblyLoadContext obj)
        {
            Console.WriteLine("Shutting Down");
        }
    }
}