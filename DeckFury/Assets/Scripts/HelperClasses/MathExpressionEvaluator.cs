using System;
using System.Data;
using System.Text.RegularExpressions;

public static class MathExpressionEvaluator
{
    /// <summary>
    /// Validates and evaluates a mathematical expression from a string.
    /// Returns 0 if the expression is invalid or cannot be computed.
    /// </summary>
    /// <param name="expression">The string containing the mathematical expression.</param>
    /// <returns>The float result of the evaluated expression or 0 for invalid inputs.</returns>
    public static float Evaluate(string expression)
    {
        if (!IsValidExpression(expression))
        {
            Console.WriteLine("Expression contains invalid characters or sequence.");
            return 0;
        }

        try
        {
            DataTable table = new DataTable();
            object result = table.Compute(expression, string.Empty);
            return Convert.ToSingle(result);
        }
        catch (SyntaxErrorException)
        {
            Console.WriteLine("Syntax error in the expression.");
            return 0;
        }
        catch (EvaluateException)
        {
            Console.WriteLine("Error evaluating the expression.");
            return 0;
        }
        catch (Exception)
        {
            Console.WriteLine("An unexpected error occurred while evaluating the expression.");
            return 0;
        }
    }

    /// <summary>
    /// Checks if the expression is valid, containing only numbers, operators, parentheses, and spaces.
    /// </summary>
    /// <param name="expression">The expression to validate.</param>
    /// <returns>true if the expression is valid; otherwise, false.</returns>
    private static bool IsValidExpression(string expression)
    {
        // This pattern checks for allowed numbers, basic arithmetic operators, parentheses, and spaces.
        // Adjust the pattern as needed to fit more specific requirements.
        string pattern = @"^[\d+\-*/\(\) .]+$";
        return Regex.IsMatch(expression, pattern);
    }
}