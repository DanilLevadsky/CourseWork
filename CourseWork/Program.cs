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
                                    "2 - Перевести с карты на карту\n" +
                                    "3 - Потратить деньги\n" +
                                    "4 - Вывести деньги\n" +
                                    "5 - Получить деньги\n" +
                                    "6 - Информация о картах";


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
                try
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
                            if (card != null)
                            {
                                if (usr.IfExist(card))
                                {
                                    PrintError("Такая карта уже существуем.");
                                    break;
                                }

                                usr.AddCard(card);
                                WriteInfo("Карта успешно создана.");
                            }

                            break;
                        case 2:
                            WriteInfo(usr.GetListOfCards());

                            var temp = usr.ChooseTwoCards();
                            WriteInfo("Сколько денег хотите передать: ");
                            var money = Convert.ToDecimal(Console.ReadLine());
                            BankAccount.Transfer(temp[0], temp[1], money);

                            break;
                        case 3:
                            WriteInfo(usr.GetListOfCards());
                            WriteInfo("Выберите карту: ");
                            var c = usr.ChooseCard();
                            obj = CreateNewThing();
                            if (obj == null)
                            {
                                PrintError("Не удалось создать объект. Неверные данные.");
                                break;
                            }

                            c.SpendMoney(obj);

                            break;
                        case 4:
                            decimal m = 0;
                            WriteInfo($"{name} сколько денег Вы хотите вывести?");
                            m = Convert.ToDecimal(Console.ReadLine());
                            if (m.GetType() != typeof(decimal)) throw new ArgumentException("Сумма должна быть числом");

                            if (m < 0) throw new InvalidObjectException("Нельзя вывести отрицательную сумму.");

                            WriteInfo(usr.GetListOfCards());
                            WriteInfo("Выберите карту");
                            var crd = usr.ChooseCard();

                            crd.GetMoney(m);


                            break;
                        case 5:
                            if (usr.Count == 0)
                            {
                                PrintError("У вас нет карт");
                                break;
                            }

                            WriteInfo($"На какую карту будут зачислены деньги, {name}?");
                            WriteInfo(usr.GetListOfCards());
                            card = usr.ChooseCard();
                            if (usr.Cards.Any(x => x._id == card._id))
                            {
                                Console.WriteLine("Укажите источник денег.");
                                obj = CreateNewThing();
                                if (obj == null) break;
                                card.AddMoney(obj);
                            }

                            break;
                        case 6:
                            WriteInfo(usr.GetListOfCards());
                            break;
                    }
                }
                catch (InvalidCardOperationException e)
                {
                    Logs.LogException(e);
                }
                catch (InvalidObjectException e)
                {
                    PrintError(e.Message);
                    Logs.LogException(e);
                }
                catch (NullReferenceException e)
                {
                    Logs.LogException(e);
                }
                catch (ArgumentNullException e)
                {
                    Logs.LogException(e);
                }
                catch (Exception e)
                {
                    Logs.LogException(e);
                }
        }

        private static BankAccount CreateNewCard(User usr)
        {
            WriteInfo(usr.GetListOfCards());
            Console.Write("Введите ID карты: ");
            var id = Console.ReadLine();
            if (id?.Trim(' ') != string.Empty) return new BankAccount(0, id);
            throw new InvalidObjectException("Не удалось создать карту");
        }

        private static ActivityOrProduct CreateNewThing()
        {
            string nameOfActivity;
            string strPrice;
            decimal price;
            Console.Write("Введите название предмета/активности: ");
            nameOfActivity = Console.ReadLine();
            if (nameOfActivity?.Trim(' ') == string.Empty)
                throw new InvalidObjectException("Не удалось идентифицировать предмет/услугу");
            Console.Write("Введите цену услуги: ");
            strPrice = Console.ReadLine();
            if (strPrice?.Trim(' ') == string.Empty) throw new InvalidObjectException("Цена должна быть числом");
            price = Convert.ToDecimal(strPrice);
            if (price >= 0) return new ActivityOrProduct(nameOfActivity, price);
            Logs.LogException(new InvalidObjectException("Цена не может быть отрицательной"));
            throw new InvalidObjectException("Цена не может быть отрицательной");
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