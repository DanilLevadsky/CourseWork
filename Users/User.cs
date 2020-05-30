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
        public List<BankAccount> Cards = new List<BankAccount>();

        public User(string name)
        {
            _name = name;
            IncorrectUserOperations = UserHandler;
        }

        public int Count => Cards.Count;
        

        private event WrongData IncorrectUserOperations;

        public string GetListOfCards()
        {
            if (Count == 0) return "У вас пока нет карт.";
            var s = $"Список карт пользователя {_name}\n";
            var i = 1;
            foreach (var card in Cards)
            {
                s += $"{i} - {card._id}\n";
                i++;
            }

            return s.TrimEnd('\n');
        }

        public void AddCard(BankAccount card)
        {
            if (Cards.Any(x => x._id == card._id))
            {
                IncorrectUserOperations?.Invoke(this, new InputHandler("Карта уже существует."));
                return;
            }

            Cards.Add(card);
        }

        public BankAccount[] ChooseTwoCards()
        {
            var cards = new BankAccount[2];
            if (Count <= 1)
            {
                IncorrectUserOperations?.Invoke(this, new InputHandler("Недостаточно карт"));
                throw new InvalidCardOperationException("Имеется недостаточно карт.");
            }

            string fst, scd;
            var flag1 = true;
            var flag2 = true;
            while (flag1)
            {
                Console.WriteLine("Введите ID первой карты: ");
                fst = Console.ReadLine();
                foreach (var card in Cards.Where(card => card._id == fst))
                {
                    cards[0] = card;
                    flag1 = false;
                    break;
                }

                if (flag1)
                    IncorrectUserOperations?.Invoke(this, new InputHandler("Карта не найдена. Попробуйте еще раз."));
            }

            while (flag2)
            {
                Console.WriteLine("Введите ID второй карты: ");
                scd = Console.ReadLine();
                foreach (var card in Cards.Where(card => card._id == scd))
                {
                    cards[1] = card;
                    flag2 = false;
                    break;
                }

                if (flag2)
                    IncorrectUserOperations?.Invoke(this, new InputHandler("Карта не найдена. Попробуйте еще раз."));
            }

            if (cards[0]._id == cards[1]._id)
            {
                throw new InvalidObjectException("Выбраны одинаковые карты");
            }
            return cards;
        }

        public BankAccount ChooseCard()
        {
            if (Count < 1)
            {
                throw new InvalidObjectException("Недостаточно карт.");
            }

            BankAccount card = null;
            var flag = true;
            while (flag)
            {
                Console.WriteLine("Введите ID карты: ");
                var temp = Console.ReadLine();
                foreach (var c in Cards.Where(card => card._id == temp))
                {
                    card = c;
                    flag = false;
                    break;
                }

                if (flag != true) continue;
                IncorrectUserOperations?.Invoke(this, new InputHandler("Карта не найдена"));
                return null;
            }

            return card;
        }

        public bool IfExist(BankAccount card)
        {
            return Cards.Any(x => x._id == card._id);
        }

        private static void UserHandler(object sender, InputHandler handler)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(handler.Message);
            Console.ResetColor();
        }

        private delegate void WrongData(object sender, InputHandler handler);
    }
}