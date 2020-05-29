using System;
using LogsAndExceptions;

namespace Budget
{
    public class ActivityOrProduct
    {
        public ActivityOrProduct(string name, decimal price)
        {
            PriceBelowZero = SendMessage;
            if (price < 0)
            {
                PriceBelowZero?.Invoke(this, "Price can`t be a negative.");
                Logs.LogException(new InvalidAmountException("Negative price detected."));
                return;
            }

            Name = name;
            Price = price;
        }

        public decimal Price { get; }

        public string Name { get; }

        private event NegativePrice PriceBelowZero;

        private static void SendMessage(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }

        private delegate void NegativePrice(object sender, string message);
    }
}