using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace ReportESF
{
    static class Settings
    {
        private static string fileName = "esf_report.ini";

        /// <summary>
        /// Checks whether the ini-file exists and if not
        /// then this methods creates the file with default settings
        /// </summary>
        /// <returns></returns>
        public static void CheckINIFile()
        {
            if (!File.Exists(fileName))
            {
                string[] lines = new string[4];
                lines[0] = "server=localhost";
                lines[1] = "database=askue_stavropolenergo";
                lines[2] = "user=unknown";
                lines[3] = "password=password";
                lines[4] = "1";
                File.WriteAllLines(fileName, lines);
            }
        }

        public static Dictionary<string, string> Entries
        {
            get
            {
                string[] lines = File.ReadAllLines(fileName);
                Dictionary<string, string> s =
                    lines.ToDictionary<string, string, string>(
                        (string inp) => inp.Split('=')[0].Trim(),
                        (string el) => el.Split('=')[1].Trim());
                return s;
            }
        }

        public static string GetSetting(string settingName)
        {
            var entries = Entries;
            if (entries.ContainsKey(settingName))
            {
                return entries[settingName];
            }
            else
            {
                throw new Exception("Нет такого ключа в настройках: " + settingName);
            }
        }

        public static void SaveSetting(string key, string value)
        {
            Dictionary<string, string> entries = Entries;
            if (entries.ContainsKey(key))
                entries[key] = value;
            else
            {
                throw new Exception("Нет такого ключа в настройках: " + key);
            }
            SaveSettings(entries);
        }

        public static void SaveSettings(Dictionary<string, string> newSettings)
        {
            try
            {
                File.WriteAllLines(fileName, newSettings.Select(
                    kvp => string.Format("{0}={1}", kvp.Key, kvp.Value)).ToArray());
            }
            catch (Exception ex)
            {
                formError err = new formError("Ошибка при сохранении настроек в ini-файле",
                    "Ошибка!", ErrorInfo(ex, "Settings.SaveSettings"));
                err.ShowDialog();
            }
        }

        public static string ErrorInfo(Exception ex, string place)
        {
            DateTime errorTime = DateTime.Now;
            string ErrorPreface = "Нижеследующий текст необходимо отправить разработчику по электронной почте" +
                Environment.NewLine + "staskue@stavre.elektra.ru" + Environment.NewLine +
                "телефон: 8(8793) 36-34-08" + Environment.NewLine +
                "         23-79";
            StringBuilder result = new StringBuilder(2048);
            result.AppendLine(ErrorPreface);
            result.AppendLine();
            result.AppendLine("Computer name = " + Environment.MachineName);
            result.AppendFormat("OS = {0}, ver. {1}",
                Environment.OSVersion.Platform.ToString(),
                Environment.OSVersion.VersionString);
            result.AppendLine();
            result.AppendLine("tc=" + errorTime.ToString("yyyyMMddHHmmss"));
            result.AppendLine();
            result.AppendLine(new string('=', 30));
            result.AppendLine("Error in " + place);
            result.AppendLine(ex.Message);
            result.AppendLine(new string('=', 30));
            result.AppendLine(ex.StackTrace);
            return result.ToString();
        }
    }
}
