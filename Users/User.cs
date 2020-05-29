﻿using System;
using System.Collections.Generic;
using System.Linq;
using Budget;
using LogsAndExceptions;

namespace Users
{
    public class User
    {
        private readonly string _name;
        private int Count => Cards.Count;
        public List<BankAccount> Cards = new List<BankAccount>();

        private delegate void WrongData(object sender, InputHandler handler);

        private event WrongData IncorrectUserOperations;
        
        public User(string name)
        {
            _name = name;
            IncorrectUserOperations = UserHandler;
        }

        public string GetListOfCards()
        {
            var s = $"Список карт пользователя {_name}\n";
            var i = 1;
            foreach (var card in Cards)
            {
                s += $"{i} - {card._id}";
                i++;
            }

            return s;
        }

        

        public BankAccount this[int index]
        {
            get
            {
                try
                {
                    return Cards[index];
                }
                catch (Exception e)
                {
                    Logs.LogException(e);
                }

                return null;
            }
        }

        public void AddCard(BankAccount card)
        {
            if (Cards.Any(x => x._id == card._id))
            {
                IncorrectUserOperations?.Invoke(this, new InputHandler("Карта уже существует"));
                return;
            }
            Cards.Add(card);
        }

        public BankAccount[] ChooseTwoCards()
        {
            Console.WriteLine(GetListOfCards());
            var cards = new BankAccount[2];
            if (Count <= 1)
            {
                IncorrectUserOperations?.Invoke(this, new InputHandler("Недостаточно карт"));
                throw new InvalidAmountException("Имеется недостаточно карт.");
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

                if (flag1) Console.WriteLine("Карта не найдена. Попробуйте еще раз.");
            }

            while (flag2)
            {
                Console.WriteLine(GetListOfCards());
                Console.WriteLine("Введите ID второй карты: ");
                scd = Console.ReadLine();
                foreach (var card in Cards.Where(card => card._id == scd))
                {
                    cards[1] = card;
                    flag2 = false;
                    break;
                }

                if (flag2) Console.WriteLine("Карта не найдена. Попробуйте еще раз.");
            }

            return cards;
        }

        public BankAccount ChooseCard()
        {
            if (Count < 1)
            {
                Logs.LogException(new Exception("У вас нет карт."));
                IncorrectUserOperations?.Invoke(this, new InputHandler("Недостаточно карт."));
                throw new InvalidAmountException("Not enough cards.");
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

        private static void UserHandler(object sender, InputHandler handler)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(handler.Message);
            Console.ResetColor();
        }
    }
    
}