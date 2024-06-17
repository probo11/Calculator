using Microsoft.AspNetCore.Mvc;
using System.Collections;
using System.Linq;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Calculator.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CalculatorController : ControllerBase
    {
        static List<string> history = new();

        // GET: <CalculatorController>
        [HttpGet(Name = "GetCalculator")]
        public IEnumerable<string> Get(string calculation)
        {
            string[] operators = { "^", "/", "*", "-", "+" };

            List<string> tokens = PrepareTokens(calculation);
            Queue<string> shunting = ShuntingAlgorithm(tokens, operators);
            if (shunting.Peek() == "Invalid input")
                return ["Invalid input"];
            string calculatedOutput = ReturnOutputCalculation(shunting, operators);
            history.Add(calculation + " = " + calculatedOutput);

            return [calculatedOutput];
        }

        [HttpGet]
        [Route("/history")]
        public IEnumerable<string> GetHistory()
        {
            return history;
        }

        [HttpGet]
        [Route("/history/clear")]
        public IEnumerable<string> ClearHistory()
        {
            history.Clear();
            return history;
        }

        List<string> PrepareTokens(string calculation)
        {
            List<string> calculationNumbers = [];
            string temp = "";

            for (int i = 0; i < calculation.Length; i++)
            {
                // if negative number
                if ((calculation[i] == '-' && i == 0) || (calculation[i] == '-' && "^/*-+()".Contains(calculation[i - 1])))
                {
                    temp += calculation[i];
                }
                // if item is number
                else if (!"^/*-+()".Contains(calculation[i]))
                {
                    temp += calculation[i];
                }
                else
                {
                    if (temp.Length > 0)
                        calculationNumbers.Add(temp);
                    calculationNumbers.Add(calculation[i].ToString());
                    temp = "";
                }
                // parse last number to array
                if (calculation.Length - 1 == i && temp.Length > 0)
                {
                    calculationNumbers.Add(temp);
                }
            }
            return calculationNumbers;
        }

        Queue<string> ShuntingAlgorithm(List<string> calculationTokens, string[] operators)
        {
            Stack<string> operatorStack = new();
            Queue<string> output = new();

            foreach (var item in calculationTokens)
            {
                // if number push to output
                if (double.TryParse(item.ToString(), out _))
                {
                    output.Enqueue(item);
                } // if operator push to stack
                else if (operators.Contains(item))
                {
                    // if stack is empty push to stack
                    // while there is a operator on stack (o2) which is not ( AND o2 has greater precedence than o1 OR o1 and o2 have the same and o1 is left-associative
                    // pop o2 from stack to output and push o1 to stack
                    while (operatorStack.Count != 0 && operatorStack.Peek() != "(" &&
                        (GetPrecedence(operatorStack.Peek()) > GetPrecedence(item) ||
                        (GetPrecedence(operatorStack.Peek()) == GetPrecedence(item) && GetAssociativity(item) == "left")))
                    {
                        output.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Push(item);

                }
                // if item ( push to stack
                if (item == "(")
                {
                    operatorStack.Push(item);
                }
                // while operator at the top of the stack is not ( pop stack into the output
                else if (item == ")")
                {
                    if (operatorStack.Count == 0)
                    {
                        Queue<string> a = new();
                        a.Enqueue("Invalid input");
                        return a;
                    }
                    while (operatorStack.Peek() != "(")
                    {
                        // if mismatched parentheses throw error
                        
                        output.Enqueue(operatorStack.Pop());
                    }
                    operatorStack.Pop();
                }
            }
            // while there are tokens on the operator stack
            while (operatorStack.Count != 0)
            {
                output.Enqueue(operatorStack.Pop());
            }
            return output;
        }

        string ReturnOutputCalculation(Queue<string> output, string[] operators)
        {
            // apply rpe to calculate the result
            int j = 0;
            string op = "";
            double num1 = 0;
            double num2 = 0;
            List<string> newOutput = [.. output];
            while (newOutput.Count > 1)
            {
                if (operators.Contains(newOutput[j]))
                {
                    op = newOutput[j];
                    newOutput.RemoveAt(j);
                    try
                    {
                        num1 = double.Parse(newOutput[j - 1]);
                        newOutput.RemoveAt(j - 1);
                    }
                    catch (Exception)
                    {
                        return "Invalid input";
                        throw;
                    }

                    try
                    {
                        num2 = double.Parse(newOutput[j - 2]);
                        newOutput.RemoveAt(j - 2);
                    }
                    catch (Exception)
                    {
                        return "Invalid input";
                        throw;
                    }
                    newOutput.Insert(j - 2, Calculations(op, num1, num2).ToString());
                    j = 0;
                }
                j++;
            }
            return newOutput[0];
        }

        double Calculations(string s, double n1, double n2)
        {
            if (s == "+")
                return n2 + n1;
            if (s == "-")
                return n2 - n1;
            if (s == "/")
                return n2 / n1;
            if (s == "*")
                return n2 * n1;
            if (s == "^")
                return Math.Pow(n2, n1);
            else
                return 0;
        }

        int GetPrecedence(string s)
        {
            if (s == "^")
                return 4;
            if (s == "*" || s == "/")
                return 3;
            if (s == "+" || s == "-")
                return 2;
            else
                return 0;
        }

        string GetAssociativity(string s)
        {
            if (s == "^")
                return "right";
            if (s == "*" || s == "/")
                return "left";
            if (s == "+" || s == "-")
                return "left";
            else
                return "null";
        }
    }
}
