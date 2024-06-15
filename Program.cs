using System;
using System.Diagnostics;
using System.Management;
using System.Net;
using System.Net.Mail;
using System.Threading;
using Telegram.Bot;
using Telegram.Bot.Exceptions;

class SystemMonitor
{
    private static readonly string botToken = "<<< write your botToken id >>>";
    private static readonly string chatId = "<<< write your chat id >>>";
    private static readonly TelegramBotClient botClient = new TelegramBotClient(botToken);
    private static readonly string smtpHost = "smtp-mail.outlook.com";
    private static readonly int smtpPort = 587;
    private static readonly string smtpUser = "<<< write your e-mail address >>>";
    private static readonly string smtpPassword = "<<< write your e-mail password >>>";

    static void Main(string[] args)
    {
        PerformanceCounter cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
        (double currentClockSpeed, double maxClockSpeed) = GetClockSpeeds();

        cpuCounter.NextValue();
        Thread.Sleep(1000);

        while (true)
        {
            double cpuUsagePercentage = cpuCounter.NextValue();
            double cpuFrequencyUsagePercentage = (currentClockSpeed / maxClockSpeed) * 100;
            double ramAvailable = GetAvailableMemory();
            double ramUsed = GetTotalPhysicalMemory() - ramAvailable;
            double ramUsagePercentage = (ramUsed / GetTotalPhysicalMemory()) * 100;
            double totalPhysicalMemory = GetTotalPhysicalMemory();



            if (cpuUsagePercentage > 80 || cpuFrequencyUsagePercentage > 80 || ramUsagePercentage > 80)
            {
                // Send a message with the current CPU and RAM usage.
                string message = $"⚠️ Alert: High Resource Usage ⚠️\nCPU Usage: {Math.Round(cpuUsagePercentage, 2)}%\n" +
                                 $"RAM Usage: {Math.Round(ramUsagePercentage, 2)}% - " +
                                 $"{Math.Round(ramUsed / (1024 * 1024 * 1024), 2)} GB used of " +
                                 $"{Math.Round(totalPhysicalMemory / (1024 * 1024 * 1024), 2)} GB total";
                Console.WriteLine(message);


                try
                {
                    botClient.SendTextMessageAsync(chatId, message).Wait();
                    SendEmailAsync(message).Wait();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to send notification: {ex.Message}");
                }
            }

            currentClockSpeed = GetCurrentClockSpeed();
            Thread.Sleep(10000);
        }
    }

    static async Task SendEmailAsync(string message)
    {
        using (var smtpClient = new SmtpClient(smtpHost, smtpPort)
        {
            Credentials = new NetworkCredential(smtpUser, smtpPassword),
            EnableSsl = true,
        })
        {
            var mailMessage = new MailMessage
            {
                From = new MailAddress(smtpUser),
                To = { smtpUser },
                Subject = "System Alert: High Resource Usage",
                Body = message,
                IsBodyHtml = true
            };

            try
            {
                await smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to send email: {ex.Message}");
            }
        }
    }

    static (double currentClockSpeed, double maxClockSpeed) GetClockSpeeds()
    {
        double currentSpeed = 0;
        double maxSpeed = 0;
        using (var searcher = new ManagementObjectSearcher("SELECT CurrentClockSpeed, MaxClockSpeed FROM Win32_Processor"))
        {
            foreach (var obj in searcher.Get())
            {
                currentSpeed = Convert.ToDouble(obj["CurrentClockSpeed"]);
                maxSpeed = Convert.ToDouble(obj["MaxClockSpeed"]);
            }
        }
        return (currentSpeed, maxSpeed);
    }

    static double GetCurrentClockSpeed()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT CurrentClockSpeed FROM Win32_Processor"))
        {
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["CurrentClockSpeed"]);
            }
        }
        return 0;
    }

    static double GetTotalPhysicalMemory()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem"))
        {
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["TotalPhysicalMemory"]);
            }
        }
        return 0;
    }

    static double GetAvailableMemory()
    {
        using (var searcher = new ManagementObjectSearcher("SELECT FreePhysicalMemory FROM Win32_OperatingSystem"))
        {
            foreach (var obj in searcher.Get())
            {
                return Convert.ToDouble(obj["FreePhysicalMemory"]) * 1024;
            }
        }
        return 0;
    }
}
