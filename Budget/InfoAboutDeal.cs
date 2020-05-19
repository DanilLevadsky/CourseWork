using System.IO;
using System;

namespace Budget
{
    public static class InfoAboutDeal
    {
        public static void LogToFile(string bankId, string name, decimal value, BankAccount card)
        {
            var fileName = Path.Combine(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.FullName,$"BankCards/{card._id}_info.log");
            
            void WriteToFile(TextWriter sw)
            {
                
                sw.Write($"{DateTime.Now}\t{name} ${value}\n");
                sw.Write($"Current balance: {card.Balance}\n\n");
                
            }
            if (!File.Exists(fileName))
            {
                using var sw = new StreamWriter(fileName);
                sw.WriteLine($"Created date: {DateTime.Now} for bank account with ID: {bankId}\n");
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