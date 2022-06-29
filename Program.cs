using static System.Console;
using static Test.Calculator;

namespace Test
{
    public class Program
    {
        #region Главный метод, в котором содержится меню калькулятора
        static void Main()
        {
            while (true)
            {
                WriteLine("Menu:\n" +
                          "1. a - Write the expression for calculation.\n" +
                          "2. Any other symbol to exit from the program.\n" +
                          "Importantly: you can write only mathematical elements, such as:\n" +
                          @"+ | - | * | / | ( | ) | , | numbers 0-9" + "\n");

                Write("Write the symbol: ");
                string str = ReadLine().ToLower();
                WriteLine();

                if (str == "a")
                    InputOutputWithCheck();
                else
                    return;
            }
        }
        #endregion

        #region Метод для ввода-вывода выражения и результата
        private static void InputOutputWithCheck()
        {
            string? expression;
            WriteLine("Write the expression:\n");

            expression = ReadLine();
            WriteLine();

            try
            {
                WriteLine($"The result of the expression is: {CalculateResult(expression)}\n");
                WriteLine("Enter the symbol to continue.\n");
                ReadKey();
                Clear();
            }
            catch (Exception ex)
            {
                WriteLine(ex.Message + '\n');
                WriteLine("Enter the symbol to continue.\n");
                ReadKey();
                Clear();
            }
        }
        #endregion
    }
}