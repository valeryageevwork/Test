using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace Test
{
    public static class Calculator
    {
        #region Статические поля
        private static readonly char[] supported_operators = { '*', '/', '+', '-' }; // Поддерживаемые операторы
        private static readonly int[] priorities = { 0, 0, 1, 1 }; // Приоритет операций
        #endregion

        #region Результирующий метод для вычисления выражения
        public static decimal CalculateResult(string expression)
        {
            CheckMathematicalSymbols(expression);
            CheckDecimalNumbers(expression);
            CheckBrackets(expression);

            return CalculateBrackets(expression);
        }
        #endregion

        #region Проверки выражения
        /// <summary>
        /// Метод для проверки на математические символы
        /// </summary>
        /// <param name="expression"> Выражение </param>
        private static void CheckMathematicalSymbols(string expression)
        {
            if (String.IsNullOrWhiteSpace(expression)) // Проверка на null или на только на содержание символов разделителей
                throw new Exception("There is the null expression"); // Генерируем исключение

            foreach (var el in expression)
            {
                if (el > 47 && el < 58 || // Ищем только математические символы
                    el == '+' || el == '-' || el == '*' ||
                    el == '/' || el == '(' || el == ')' || el == ',' ||
                    el == ' ')
                {
                    continue;
                }
                else
                    throw new Exception("There is an excess symbol");
            }

        }
        /// <summary>
        /// Метод для проверки вещественных чисел
        /// </summary>
        /// <param name="expression"> Выражение </param>
        private static void CheckDecimalNumbers(string expression)
        {
            for (int i = 0; i < expression.Length; i++) // Проходим по строке
            {
                string str = "";

                while(expression[i] > 47 && expression[i] < 58 || expression[i] == ',') // Выделяем все, что является цифрой или запятой
                {
                    str += expression[i];
                    i++;

                    if (i == expression.Length)
                        break;
                }

                if (!decimal.TryParse(str, out decimal d_var) && str != "") // Конвертируем, в случае неудачи - генерируем исключение
                    throw new Exception("The decimal numbers are not correct.");
            }
        }
        /// <summary>
        /// Метод для проверки скобок
        /// </summary>
        /// <param name="expression"> Выражение </param>
        private static void CheckBrackets(string expression)
        {
            int i = 0;

            foreach (char ch in expression) // Пробегаем по выражению
            {
                switch (ch)
                {
                    case '(': 
                        i++; 
                        break;
                    case ')': 
                        i--; 
                        break;
                }

                if (i < 0) // Ошибка - закрывающих скобок больше
                    throw new Exception("There is not necessary bracket");
            }

            if (i > 0) // Ошибка - открывающих скобок больше
                throw new Exception("There is not necessary bracket");
        }
        #endregion

        #region Калькулятор
        /// <summary>
        /// Метод для поиска закрывающей скобки
        /// </summary>
        /// <param name="expression"> Выражение </param>
        /// <param name="start"> Начальный индекс для поиска </param>
        /// <returns> Возвращает позицию правой скобки </returns>
        private static int IndexOfRightBracket(string expression, int start)
        {
            int counter = 1;

            for (int i = start; i < expression.Length; i++)
            {
                switch (expression[i])
                {
                    case '(': 
                        counter++; 
                        break;
                    case ')': 
                        counter--; 
                        break;
                }

                if (counter == 0) 
                    return i;
            }

            return -1;
        }
        /// <summary>
        /// Вычисление выражений внутри скобок
        /// </summary>
        /// <param name="expression"> Выражение </param>
        /// <returns> Число, равное выражению внутри скобки или просто выражению </returns>
        private static decimal CalculateBrackets(string expression)
        {
            string new_expression = expression;
            while (new_expression.Contains('(')) // Пока содержит открывающую скобку
            {
                int start_point = new_expression.IndexOf('(') + 1; // Индекс для поиска и выделения
                int end_point = IndexOfRightBracket(new_expression, start_point); // Индекс правой скобки

                string inner_expression = new_expression.Substring(start_point, end_point - start_point); // Внутренее выражение (внутри скобок)
                new_expression = new_expression.Replace("(" + inner_expression + ")", CalculateBrackets(inner_expression).ToString());
                /*Рекурсивное выражение, которое доходит до самого последнего внутреннего выражения и поднимается вверх, заменяя
                  последнее выражение результатом вычисления*/
            }

            return ParseExpression(new_expression);
        }

        /// <summary>
        /// Конвертирование выражения в результат
        /// </summary>
        /// <param name="expression"> Выражение </param>
        /// <returns> Число-результат </returns>
        private static decimal ParseExpression(string expression)
        {
            List<char> ops = new List<char>(); // Коллекция операторов
            List<decimal> numbers = new List<decimal>(); // Коллекция чисел

            List<string> parts = new List<string>();
            /*Коллекция разбиения выражения на составляющие.
              Например, 123 + -4 будет разбиваться на
              {123, +, -4}*/

            Regex regex = new Regex(@"(\b[-+*/]|[+-]?\d+,\d+|[+-]?\d+)");
            // В выражении есть 3 альтернативы. 
            // \b[-+*/] - означает символ операции из квадратных скобок, который следует после алф.-цифр.выражения
            // [+-]?\d+,\d+ - означает вещественное число со знаком
            // [+-]?\d+ - означает целое число со знаком

            foreach (Match match in regex.Matches(expression.Replace(" ", ""))) // Находим соответствие рег. выр. в expression
                parts.Add(match.Value); // Записываем соответствие в коллекцию разбиений

            foreach (string part in parts) // Выделяем операторы и числа в отдельные коллекции
            {
                if (decimal.TryParse(part, out decimal number))
                    numbers.Add(number);
                else
                    ops.Add(part[0]);
            }

            if (ops.Count + 1 != numbers.Count) // Если чисел не на один больше, чем операторов, генерируем ошибку
                throw new Exception("The mismatch in the expression");

            foreach (int priority in priorities.Distinct()) // Проходим по двум значения 0,1
            {
                List<char> operators = new List<char>();

                for (int i = 0; i < priorities.Length; i++)
                    if (priorities[i] == priority) // Добавляем те операторы, приоритет которых равен текущему в цикле foreach {*/, а потом +-}
                        operators.Add(supported_operators[i]);

                for (int i = 0; i < ops.Count; i++)
                    if (operators.Contains(ops[i])) // Проверка на операторы высшего приоритета на данный момент
                    {
                        numbers[i] = Calculate(numbers[i], numbers[i + 1], ops[i]); // Вычисление значений, стоящих рядом с этим оператором
                        numbers.RemoveAt(i + 1); // Удаление лишнего числа
                        ops.RemoveAt(i); // Удаление лишнего оператора
                        i--; // Возвращение обратно на прошлый индекс (в цикле for счетчик увеличется)
                    }
            }

            return numbers[0]; // Возвращаем получившееся число, равное выражению
        }
        /// <summary>
        /// Метод для вычисления чисел для соответствующей операции
        /// </summary>
        /// <param name="left"> Левый аргумент </param>
        /// <param name="right"> Правый аргумент </param>
        /// <param name="op"> Оператор </param>
        /// <returns> Получившееся число </returns>
        private static decimal Calculate(decimal left, decimal right, char op)
        {
            switch (op)
            {
                case '*':
                    return left * right;
                case '/':
                    if (right == 0)
                        throw new Exception("Division by zero"); // При делении на ноль генерируем исключение
                    return left / right;
                case '+': 
                    return left + right;
                case '-': 
                    return left - right;
                default: 
                    throw new Exception("The not supported operator");
            }
        }
        #endregion 
    }
}
