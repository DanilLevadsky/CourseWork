using System;
using System.Linq;
using Budget;
using LogsAndExceptions;
using Users;

namespace CourseWork
{
    internal class Program
    {
        private const string menu = "Выберите что сделать:\n" +
                                    "0 - Выход из программы\n" +
                                    "1 - Добавить новую карту\n" +
                                    "2 - Объединить карты\n" +
                                    "3 - Перевести с карты на карту\n" +
                                    "4 - Потратить деньги\n" +
                                    "5 - Вывести деньги\n" +
                                    "6 - Получить деньги\n" +
                                    "7 - Информация о картах";


        private static void Main(string[] args)
        {
            var name = string.Empty;
            while (name?.Trim(' ') == "")
            {
                Console.Write("Введите ваше имя: ");
                name = Console.ReadLine();
                if (name?.Trim(' ') != "") continue;
                Console.WriteLine("Неверные данные.");
                Logs.LogException(new ArgumentNullException("Empty input."));
            }

            var usr = new User(name);

            while (true)
            {
                Console.WriteLine(menu);
                int key;
                try
                {
                    Console.Write("Ваш выбор: ");
                    key = int.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Logs.LogException(e);
                    continue;
                }

                ActivityOrProduct obj;
                BankAccount card;
                switch (key)
                {
                    case 0:
                        WriteInfo($"Программа завершена. До свидания, {name}");
                        Environment.Exit(0);
                        break;
                    case 1:
                        card = CreateNewCard(usr);
                        if (card != null) usr.AddCard(card);
                        break;

                    case 2:
                        WriteInfo(usr.GetListOfCards());
                        try
                        {
                            WriteInfo("Выберите карты: ");
                            var cards = usr.ChooseTwoCards();
                            cards[0] = cards[0] + cards[1];
                            break;
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                            break;
                        }
                    case 3:
                        WriteInfo(usr.GetListOfCards());
                        try
                        {
                            var temp = usr.ChooseTwoCards();
                            WriteInfo("Сколько денег хотите передать: ");
                            var money = Convert.ToDecimal(Console.ReadLine());
                            BankAccount.Transfer(temp[0], temp[1], money);
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }

                        break;
                    case 4:
                        WriteInfo(usr.GetListOfCards());
                        try
                        {
                            WriteInfo("Выберите карту: ");
                            var c = usr.ChooseCard();
                            obj = CreateNewThing();
                            if (obj == null)
                            {
                                PrintError("Не удалось создать объект. Неверные данные.");
                                break;
                            }

                            c.SpendMoney(obj);
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }

                        break;
                    case 5:
                        decimal m = 0;
                        WriteInfo($"{name} сколько денег Вы хотите вывести?");
                        try
                        {
                            m = Convert.ToDecimal(Console.ReadLine());
                            if (m.GetType() != typeof(decimal))
                            {
                                Logs.LogException(new ArgumentException("Сумма должна быть числом"));
                                break;
                            }

                            if (m < 0)
                            {
                                PrintError($"{name}, нельзя вывести отрицательную сумму.");
                                Logs.LogException(new InvalidAmountException("You can`t withdraw negative sum."));
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                            break;
                        }
                        WriteInfo(usr.GetListOfCards());
                        WriteInfo("Выберите карту");
                        var crd = usr.ChooseCard();
                        try
                        {
                            crd.GetMoney(m);
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }

                        break;
                    case 6:
                        try
                        {
                            WriteInfo($"На какую карту будут зачислены деньги, {name}?");
                            card = usr.ChooseCard();
                            if (usr.Cards.Any(x => x._id == card._id))
                            {
                                Console.WriteLine("Укажите источник денег.");
                                obj = CreateNewThing();


                                if (obj == null) break;
                                card.AddMoney(obj);
                                break;
                            }

                            PrintError("Карта не найдена");
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }

                        break;
                    case 7:
                        WriteInfo(usr.GetListOfCards());
                        break;
                }
            }
        }

        private static BankAccount? CreateNewCard(User usr)
        {
            WriteInfo(usr.GetListOfCards());
            Console.Write("Введите ID карты: ");
            var id = Console.ReadLine();
            if (id?.Trim(' ') != string.Empty) return new BankAccount(0, id);
            PrintError("Не удалось создать карту.");
            return null;
        }

        private static ActivityOrProduct? CreateNewThing()
        {
            string nameOfActivity;
            decimal price;
            Console.Write("Введите название предмета/активности: ");
            nameOfActivity = Console.ReadLine();
            if (nameOfActivity?.Trim(' ') == string.Empty)
            {
                PrintError("Не удалось идентифицировать предмет/услугу");
                return null;
            }

            Console.Write("Введите цену услуги: ");
            price = Convert.ToDecimal(Console.ReadLine());
            if (price < 0) Logs.LogException(new InvalidAmountException("Negative price detected."));
            return new ActivityOrProduct(nameOfActivity, price);
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }

        private static void WriteInfo(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }
    }
}