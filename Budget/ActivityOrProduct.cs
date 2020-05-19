using System;

namespace Budget
{
    public class ActivityOrProduct
    {
        private readonly string _name;
        private readonly decimal _price;
        public delegate void NegativePrice(object sender, string message);
        public event NegativePrice PriceBelowZero;
        
        public decimal Price => _price;
        public string Name => _name;

        public ActivityOrProduct(string name, decimal price)
        {
            
            if (price < 0)
            {
                throw new ArgumentException("Price can`t be a negative number");
            }

            this.PriceBelowZero = SendMessage;
            this._name = name;
            this._price = price;
            if (price < 0)
            {
                PriceBelowZero?.Invoke(this, "Price can`t be a negative.");
            }
        }
        

        private static void SendMessage(object sender, string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}