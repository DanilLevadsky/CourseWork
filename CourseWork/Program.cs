using System;
using System.Linq;
using Budget;
using LogsAndExceptions;
using Users;

namespace CourseWork
{
    internal static class Program
    {
        private const string Menu = "Выберите что сделать:\n" +
                                    "0 - Выход из программы\n" +
                                    "1 - Добавить новую карту\n" +
                                    "2 - Перевести с карты на карту\n" +
                                    "3 - Потратить деньги\n" +
                                    "4 - Вывести деньги\n" +
                                    "5 - Получить деньги\n" +
                                    "6 - Информация о картах\n" +
                                    "7 - Просмотреть балланс";


        private static void Main(string[] args)
        {
            
            var name = string.Empty;
            while (name?.Trim(' ') == "")
            {
                Console.Write("Введите ваше имя: ");
                name = Console.ReadLine();
                if (name?.Trim(' ') != "") continue;
                PrintError("Неверные данные.");
                Logs.LogException(new ArgumentNullException("Пустые входные данные."));
            }

            var usr = new User(name);


            while (true)
                try
                {
                    Console.WriteLine(Menu);
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
                                    PrintError("Карта с таким ID уже существует.");
                                    break;
                                }

                                usr.AddCard(card);
                                WriteInfo("Карта успешно создана.");
                            }

                            break;
                        case 2:
                            WriteInfo(usr.GetListOfCards());
                            var temp = usr.ChooseTwoCards();
                            WriteInfo($"Текущий балланс карты {temp[0]._id} - {temp[0].Balance}");
                            WriteInfo($"Текущий балланс карты {temp[1]._id} - {temp[1].Balance}");
                            WriteInfo("Сколько денег хотите передать: ");
                            var money = Convert.ToDecimal(Console.ReadLine());
                            BankAccount.Transfer(temp[0], temp[1], money);
                            break;
                        case 3:
                            WriteInfo(usr.GetListOfCards());
                            WriteInfo("Выберите карту: ");
                            card = usr.ChooseCard();
                            WriteInfo($"Текущий балланс карты - {card.Balance}");
                            obj = CreateNewThing();
                            if (obj == null)
                            {
                                PrintError("Не удалось создать объект. Неверные данные.");
                                break;
                            }

                            card.SpendMoney(obj);

                            break;
                        case 4:
                            WriteInfo(usr.GetListOfCards());
                            WriteInfo("Выберите карту");
                            card = usr.ChooseCard();
                            WriteInfo($"Текущий балланс карты - {card.Balance}");
                            decimal m = 0;
                            WriteInfo($"{name}, сколько денег Вы хотите вывести?");
                            m = Convert.ToDecimal(Console.ReadLine());
                            if (m.GetType() != typeof(decimal)) throw new ArgumentException("Сумма должна быть числом");
                            if (m < 0) throw new InvalidObjectException("Нельзя вывести отрицательную сумму.");
                            card.GetMoney(m);
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
                                WriteInfo($"Текущий балланс карты - {card.Balance}");
                                Console.WriteLine("Укажите источник денег.");
                                obj = CreateNewThing();
                                if (obj == null) break;
                                card.AddMoney(obj);
                            }

                            break;
                        case 6:
                            WriteInfo(usr.GetListOfCards());
                            break;
                        case 7:
                            WriteInfo(usr.GetListOfCards());
                            WriteInfo("Выберите карту: ");
                            card = usr.ChooseCard();
                            WriteInfo($"Текущий балланс карты {card._id} - {card.Balance}");
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
                catch (FormatException e)
                {
                    PrintError("Неверные данные.");
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
            if (nameOfActivity?.Trim(' ') == "")
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