using System;
using System.Collections.Generic;
using System.Linq;
using Budget;
using LogsAndExceptions;

namespace Users
{
    public class User
    {
        private readonly string _name;
        
        public List<Budget.BankAccount> Cards = new List<BankAccount>();

        public int Count => Cards.Count;
        
        public User(string name)
        {
            this._name = name;
        }

        public void AddCard(Budget.BankAccount card)
        {
            this.Cards.Add(card);
        }

        public BankAccount this[int index]
        {
            get
            {
                try
                {
                    return this.Cards[index];
                }
                catch (Exception e)
                {
                    Logs.LogException(e);
                }

                return null;
            }
        }

        public BankAccount[] ChooseTwoCards()
        {
            var cards = new BankAccount[2];
            if (this.Count <= 1)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Недостаточно карт в кошельке.");
                Console.ResetColor();
                throw new Exception("Имеется недостаточно карт.");
            }

            string fst, scd;
            var flag1 = true;
            var flag2 = true;
            while (flag1)
            {
                Console.WriteLine("Введите ID первой карты: ");
                fst = Console.ReadLine();
                foreach (var card in this.Cards.Where(card => card._id == fst))
                {
                    cards[0] = card;
                    flag1 = false;
                    break;
                }

                if (flag1 == true)
                {
                    Console.WriteLine("Карта не найдена. Попробуйте еще раз.");
                } 
            }
            while (flag2)
            {
                Console.WriteLine("Введите ID второй карты: ");
                scd = Console.ReadLine();
                foreach (var card in this.Cards.Where(card => card._id == scd))
                {
                    cards[1] = card;
                    flag2 = false;
                    break;
                }

                if (flag2 == true)
                {
                    Console.WriteLine("Карта не найдена. Попробуйте еще раз.");
                } 
            }
            return cards;
        }
        public BankAccount ChooseCard()
        {
            if (this.Count < 1)
            {
                Logs.LogException(new Exception("У вас нет карт."));
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Error.WriteLine("Недостаточно карт.");
                Console.ResetColor();
                throw new Exception("Not enough cards."); 
            }
            BankAccount card = null;
            var flag = true;
            while (flag)
            {
                Console.WriteLine("Введите ID карты: ");
                var temp = Console.ReadLine();
                foreach (var c in this.Cards.Where(card => card._id == temp))
                {
                    card = c;
                    flag = false;
                    break;
                }

                if (flag != true) continue;
                Console.WriteLine("Карта не найдена. Попробуйте еще раз.");
                return null;
            }

            return card;
        }
    }
}