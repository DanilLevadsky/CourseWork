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
                                    "6 - Получить деньги";
        
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
                    key = int.Parse(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Logs.LogException(e);
                    continue;
                }
                ActivityOrProduct obj;
                switch (key)
                {
                    case 0:
                        Console.WriteLine($"Программа завершена. Хорошего вечера, {name}");
                        Environment.Exit(0);
                        break;
                    case 1:
                        var card = CreateNewCard(usr);
                        usr.AddCard(card);
                        break;
                    case 2:
                        try
                        {
                            Console.WriteLine("Выберите карты: ");
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
                        try
                        {
                            var temp = usr.ChooseTwoCards();
                            Console.Write("Сколько денег хотите передать: ");
                            var money = Convert.ToDecimal(Console.ReadLine());
                            BankAccount.Transfer(temp[0], temp[1], money);
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }
                        break;
                    case 4:
                        try
                        {
                            Console.WriteLine("Выберите карту: ");
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
                        Console.WriteLine($"{name} сколько денег Вы хотите вывести?");
                        try
                        {
                            m = Convert.ToDecimal(Console.ReadLine());
                            if (m.GetType() != typeof(decimal))
                            {
                                break;
                            }

                            if (m < 0)
                            {
                                PrintError($"{name}, нельзя вывести отрицательную сумму.");
                                Logs.LogException(new ArgumentException("You can`t withdraw negative sum."));
                                break;
                            }
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                            break;
                        }
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
                            Console.WriteLine($"На какую карту будут зачислены деньги, {name}?");
                            card = usr.ChooseCard();
                            Console.WriteLine("Укажите источник денег.");
                            obj = CreateNewThing();
                            if (obj == null)
                            {
                                PrintError("Не удалось создать объект. Неверные данные.");
                                break;
                            }
                            card.AddMoney(obj);
                        }
                        catch (Exception e)
                        {
                            Logs.LogException(e);
                        }
                        break;
                    default:
                        break;
                }    
            }
        }

        private static BankAccount CreateNewCard(User usr)
        {
            var id = string.Empty;
            while (id == "")
            {
                Console.Write("Enter your card ID: ");
                id = Console.ReadLine();
                foreach (var c in usr.Cards.Where(c => c._id == id))
                {
                    id = "";
                    PrintError("Не удалось создать карту.");
                }
            }
            return new BankAccount(0, id);
        }

        private static ActivityOrProduct CreateNewThing()
        {
            string nameOfActivity;
            decimal price;
            try
            {
                Console.Write("Введите название предмета/активности: ");
                nameOfActivity = Console.ReadLine();
                Console.Write("Введите цену услуги: ");
                price = Convert.ToDecimal(Console.ReadLine());
                if (price >= 0) return new ActivityOrProduct(nameOfActivity, price);
                Logs.LogException(new ArgumentException("Negative price detected."));
                return null;
            }
            catch (Exception e)
            {
                Logs.LogException(e);
                PrintError("Не удалось идентифицировать предмет/услугу.");
                return null;
            }
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(message);
            Console.ResetColor();
        } 
    }
}