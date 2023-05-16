using System.Globalization;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SmartCalc.Core.Interfaces;
using SmartCalc.Core.Models;

namespace SmartCalc.Core;

public class SmartCalcService : ISmartCalcService
{
    [DllImport("libsmartcalc")]
    private static extern double Calc(Node[] nodes, int size);

    public double Evaluate(string expression, double xValue = 1)
    {
        expression = ReplaceConventions(expression);

        if (IsExpressionCorrect(expression) is false)
            return double.NaN;

        var nodes = Parse(expression, xValue).ToArray();

        return Calc(nodes, nodes.Length);
    }

    public Point[] CalcPlotData(string expression, int xMin, int xMax, int yMin, int yMax)
    {
        var points = new List<Point>();
        if (string.IsNullOrWhiteSpace(expression))
            return points.ToArray();

        for (double x = xMin; x <= xMax; x += 0.3)
        {
            var y = Evaluate(expression, x);
            if (double.IsNormal(y) && y >= yMin && y <= yMax)
                points.Add(new Point(x, y));
            else
                points.Add(new Point());
        }
        return points.ToArray();
    }

    private static string ReplaceConventions(string expression)
    {
        expression = Regex.Replace(expression, @"\s+", "");

        expression = expression.Replace(")(", ")*(");

        expression = Regex.Replace(expression, "mod|sqrt|cos|acos|sin|asin|tan|atan|ln|log",
            match => CutFunction(match.Value));

        // 1e2 -> 1*10^(2)
        expression = Regex.Replace(expression, @"[eE](?<arg>(\d+|x)|([+-](\d+|x)))",
            match => "*10^(" + match.Groups["arg"].Value + ")");

        // -2+(-x) -> (0-1)*2+((0-1)*x)
        string temp;
        do
        {
            temp = expression;
            expression = Regex.Replace(expression, @"(?<pre>^|\()(?<op>[+-])(?<value>([\dA-z(]))",
                match => match.Groups["pre"].Value + "(0" +
                         match.Groups["op"].Value + "1)*" +
                         match.Groups["value"].Value);
        } while (!expression.Equals(temp));

        return expression;
    }

    private static bool IsExpressionCorrect(string expression)
    {
        var regexps = new[]
        {
            @"[eE]",
            @"(^|\D)\.",
            @"\d+\.\d+\.",
            @"([0-9]|[x.)])\(",
            @"([x)]|[a-w])[0-9]",
            @"([0-9]|[.)x])[a-w]",
            @"([0-9]|[a-z]|[.)])x",
            @"([.(*/^%+-]|[a-w])$",
            @"(^|[.(*/^+-]|[a-w])%",
            @"([.+/*^%-]|[a-w])[+-]",
            @"(^|[.(+/*^%-]|[a-w])\)",
            @"(^|[.(*/^%+-]|[a-w])[*/^%]"
        };

        return IsBracketAmountCorrect(expression) &&
               regexps.All(regexp => !Regex.IsMatch(expression, regexp));
    }

    private static bool IsBracketAmountCorrect(IEnumerable<char> expression)
    {
        var openBracketCount = 0;
        var closeBracketCount = 0;
        foreach (var syllable in expression)
        {
            if (syllable == '(') openBracketCount++;
            if (syllable == ')') closeBracketCount++;
        }

        return openBracketCount == closeBracketCount;
    }

    private static IEnumerable<Node> Parse(IEnumerable<char> expression, double xValue)
    {
        var nodes = new List<Node>();

        var numberBuffer = string.Empty;
        var decimalSeparator = CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator;

        foreach (var syllable in expression)
        {
            if (char.IsDigit(syllable) || decimalSeparator.Contains(syllable))
            {
                numberBuffer += syllable;
            }
            else
            {
                if (numberBuffer.Length > 0)
                {
                    nodes.Add(new Node { Operand = double.Parse(numberBuffer) });
                    numberBuffer = string.Empty;
                }

                nodes.Add(SyllableToNode(syllable, xValue));
            }
        }

        if (numberBuffer.Length > 0)
            nodes.Add(new Node { Operand = double.Parse(numberBuffer) });

        return nodes.ToArray();
    }

    private static string CutFunction(string value) =>
        value switch
        {
            "sqrt" => "q",
            "sin" => "s",
            "asin" => "S",
            "cos" => "c",
            "acos" => "C",
            "tan" => "t",
            "atan" => "T",
            "log" => "l",
            "ln" => "L",
            "mod" => "%",
            _ => throw new ArgumentException("Unknown function")
        };

    private static Node SyllableToNode(char value, double xValue) =>
        value switch
        {
            'x' => new Node { Operand = xValue },
            '(' => new Node { Bracket = Bracket.OpenBracket },
            ')' => new Node { Bracket = Bracket.CloseBracket },
            '^' => new Node { Operation = Operation.Pow },
            '*' => new Node { Operation = Operation.Mul },
            '/' => new Node { Operation = Operation.Div },
            '%' => new Node { Operation = Operation.Mod },
            '+' => new Node { Operation = Operation.Add },
            '-' => new Node { Operation = Operation.Sub },
            'q' => new Node { Function = Function.Sqrt },
            'c' => new Node { Function = Function.Cos },
            'C' => new Node { Function = Function.Acos },
            's' => new Node { Function = Function.Sin },
            'S' => new Node { Function = Function.Asin },
            't' => new Node { Function = Function.Tan },
            'T' => new Node { Function = Function.Atan },
            'l' => new Node { Function = Function.Log10 },
            'L' => new Node { Function = Function.Log },
            _ => throw new ArgumentException("Unknown syllable")
        };
}