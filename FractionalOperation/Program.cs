using System;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;

namespace FractionalOperations
{
    class Program
    {
        static void Main(string[] args)
        {
            // Valid test cases
            //string input = "2_3/8 * 9/8";
            //string input = "23/8 - 9_3/4";
            //string input = "5 / 8/5";
            //string input = "1_1/5 + 8/5";

            // Invalid inputs
            //string input = "1_0/5 + 8/5";
            //string input = "1_1/5 + 8/0";
            //string input = "1/1_5 + 8/0";
            //string input = "1_1/5 + 8_0";

            Console.Write("Please enter expression: ");
            string input = Console.ReadLine();

            // d+[_]?\d*[\/]?\d* - Regex for fractional numbers
            // [ ]+([\/*+-])[ ]+ - Regex for operators. Operator must be surrounded by spaces
            Regex numberRegex = new Regex(@"\d+[_]?\d*[\/]?\d*|[ ]+([\/*+-])[ ]+");
            MatchCollection matches = numberRegex.Matches(input);

            try
            {
                if (matches.Count != 3)
                    throw new Exception("Input must contain exactly two numbers separated by an operator");

                string inputExpression =
                    String.Format("{0} {1} {2}", ConvertToDecimalString(matches[0].ToString()),  // Number 1
                                                 ValidateOperator(matches[1].ToString()),        // Operator
                                                 ConvertToDecimalString(matches[2].ToString())); // Number 2

                Console.WriteLine(String.Format("Interpreting input as: {0}", inputExpression));

                double evaluatedExpression = EvaluateExpression(inputExpression);
                Console.WriteLine(String.Format("Calculated value: {0}", evaluatedExpression));

                string fractionalResult = GetFactionalResult(evaluatedExpression);
                Console.WriteLine(String.Format("Fractional representation: {0}", fractionalResult));
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.Message);
            }
        }

        // Takes a string representing a whole number and a fraction and converts it to a string representing
        // the equivalent decimal value. e.x: "2_1/2" => "2.5"
        public static string ConvertToDecimalString(string number)
        {
            decimal returnValue;
            
            // If the number is a plain number (i.e. something like 123), convert to double and return
            if (!number.Contains('/') && !number.Contains('_'))
                returnValue = Convert.ToDecimal(number);

            // Return decimal equivalent of fractions like 1/2
            else if (number.Contains('/') && !number.Contains('_'))
                returnValue = GetFractionalValue(number);

            // Return decimal equivalent of a number and a fraction, like 3_3/4
            else if (number.Contains('/') && number.Contains('_'))
            {
                string[] numberArray = number.Split('_');
                returnValue = Convert.ToDecimal(numberArray[0]) + GetFractionalValue(numberArray[1]);
            }
            else
                // If number does not match the expected pattern, disregard it    
                throw new Exception(String.Format("Invalid input: '{0}' does not match the expected format", number));
            
            return returnValue.ToString();
        }

        // Takes a string representing a fraction and converts it to a decimal. e.x: "3/4" => 0.75
        public static decimal GetFractionalValue(string number)
        {
            string[] numberArray = number.Split('/');

            if ((numberArray[0] == "") || (numberArray[1] == ""))
                throw new Exception(String.Format("Invalid input: '{0}'. Both numerator and denominator must be specified", number));

            decimal numerator = Convert.ToDecimal(numberArray[0]);
            decimal denominator = Convert.ToDecimal(numberArray[1]);

            if(denominator == 0)
                throw new Exception(String.Format("Invalid input: '{0}'. Denominator cannot be 0", number));

            if (numerator == denominator)
                throw new Exception(String.Format("Invalid input: '{0}'. Numerator cannot equal denominator", number));

            return numerator / denominator;
        }

        // Verifies that the supplied operator is one of the permitted operators
        public static string ValidateOperator(string sOperator)
        {
            string[] validOperators = { "/", "*", "+", "-" };
            sOperator = sOperator.Trim();

            if(!validOperators.Contains(sOperator))
                throw new Exception(String.Format("'{0}' is not a valid operator", sOperator));
            
            return sOperator;
        }

        // Takes a string representing an expression and evaluates it. e.x. "0.5 - 0.75" => -0.25
        public static double EvaluateExpression(string inputExpression)
        {
            DataTable table = new DataTable();
            table.Columns.Add("expression", typeof(string), inputExpression);

            DataRow row = table.NewRow();
            table.Rows.Add(row);

            return double.Parse((string)row["expression"]);
        }

        // Converts a double to it's fractional representation. For example 1.25 => "1_1/4"
        public static string GetFactionalResult(double expression)
        {
            bool isPositive = true;
            if(expression < 0)
            {
                isPositive = false;
                expression *= -1;
            }

            // Get the portion of the number before the decimal
            double integral = Math.Truncate(expression);

            // Get the portion of the number after the decimal
            double decValue = expression - integral;

            if (expression.Equals(integral))
                return integral.ToString();

            //Convert to a fractional representatoin: 0.25 => "25/100"
            int numerator = Convert.ToInt32(Math.Round((decValue * 100), 2));
            int denominator = 100;
            int gcd = GCD(numerator, denominator);

            // Divide numerator and denominator by their greatest common divisor to reduce the fractoin
            // e.x. For 25/100, GCD is 25. Division by 25 reduces the fraction to 1/4
            numerator /= gcd;
            denominator /= gcd;

            string outputString = isPositive ? "" : "-";

            if (integral > 0)
                outputString = outputString + integral + "_";

            outputString = outputString + numerator + "/" + denominator;

            return outputString;
        }

        // Calculate the greatest common divisor using Euclid's algorithm
        private static int GCD(int num1, int num2)
        {
            while (num1 != 0 && num2 != 0)
            {
                if (num1 > num2)
                    num1 %= num2;
                else
                    num2 %= num1;
            }

            return num1 == 0 ? num2 : num1;
        }
    }
}
