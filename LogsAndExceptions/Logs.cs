using System;
using System.IO;

namespace LogsAndExceptions
{
    public static class Logs
    {

        public static void LogException(params Exception[] e)
        {
            var fileName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,"exceptions.log");

            void WriteToFile(TextWriter sw)
            {
                foreach (var exc in e)
                {
                    sw.Write($"{DateTime.Now}: {exc.ToString()}\n\n");
                }
            }
            if (!File.Exists(fileName))
            {
                using var sw = new StreamWriter(fileName);
                sw.WriteLine($"Created date: {DateTime.Now}\n");
                WriteToFile(sw);
            }
            else
            {
                using var sw = File.AppendText(fileName);
                WriteToFile(sw);
            }
        }
    }
}