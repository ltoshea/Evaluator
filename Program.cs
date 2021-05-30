using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace EvaluatorC
{
    class Program
    {
        public const string filePath = @"C:\Users\Liam\Downloads\Instruction Evaluator Exercise\basic_input.txt";
        public static SortedDictionary<int, string> lineMap = new SortedDictionary<int,string>();
        public static Stack stack = new Stack();
        static void Main(string[] args)
        {
            LoadAndValidateFile(filePath);
            buildStack(getFirstIndex());
            var result = getResult();
            Console.WriteLine(result);
            Console.ReadKey();
        }

        /// <summary>
        /// Goes through resultant stack performing the arithmetic for final result
        /// </summary>
        /// <returns></returns>
        private static int getResult()
        {
            List<int> numbers = new List<int>();
            int number;
            int result = 0;
            while (stack.Count > 0)
            {
                if (int.TryParse((string)stack.Peek(), out number))
                {
                    numbers.Add(number);
                    stack.Pop();
                }
                else
                {
                    var operand = stack.Pop();
                    
                    if(operand.ToString().Equals("ADD", StringComparison.InvariantCultureIgnoreCase))
                    {
                        for(int i=0; i<numbers.Count-1; i++)
                        {
                            result = result + numbers[i] + numbers[i + 1];
                        }
                    }
                    else if (operand.ToString().Equals("MULT", StringComparison.InvariantCultureIgnoreCase))
                    {
                        for (int i = 0; i < numbers.Count - 1; i++)
                        {
                            result = result + numbers[i] * numbers[i + 1];
                        }
                    }
                    else
                    {
                        throw new ArithmeticException("Unexpected Operand");
                    }
                    numbers.Clear();
                }
            }
            return result;
        }

        private static int getFirstIndex()
        {
            return lineMap.Keys.First();
        }
        
        /// <summary>
        /// Builds instruction stack
        /// </summary>
        /// <param name="index"></param>
        private static void buildStack(int index)
        {
            var tokens = lineMap[index].Split(' ');
            if (tokens[0] == "VALUE")
            {
                stack.Push(tokens[1]);
            }
            else
            {
                stack.Push(tokens[0]);
                for (int i = 1; i < tokens.Length; i++)
                {
                    buildStack(int.Parse(tokens[i]));
                }
            }
        }
        /// <summary>
        /// Checks each line in file matches expected format and adds each line to dictionary
        /// </summary>
        /// <param name="filePath"></param>
        private static void LoadAndValidateFile(string filePath)
        {
            string validationPattern = @"^([0-9]+): ((ADD|VALUE|MULT)(\s-?[0-9]+)*$)";
            Regex regex = new Regex(validationPattern);
            MatchCollection matches; 
            foreach (string line in File.ReadLines(filePath))
            {
                matches = regex.Matches(line.ToUpper());
                if (matches.Count > 0)
                {
                    lineMap.Add(int.Parse(matches[0].Groups[1].Value), matches[0].Groups[2].Value);
                    continue;
                }
                else
                {
                    throw new InvalidDataException("Invalid File input");
                }
            }
            return;
        }
    }
}
