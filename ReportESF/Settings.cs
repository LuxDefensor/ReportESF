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
        public static readonly string SettingsFile = "esf_report.ini";
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
            result.AppendLine(ex?.Message);
            result.AppendLine(new string('=', 30));
            result.AppendLine(ex?.StackTrace);
            return result.ToString();
        }
    }
}
