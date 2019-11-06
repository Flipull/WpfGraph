using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace WpfGraph.Formula
{
    abstract class FormulaPart
    {
        static string[] functions = { "", "sin", "cos", "abs" };
        static string[] operators = { "^", "*", "/", "+", "-", "%" };
        static string[] variables = { "x" };
        
        public abstract double Evaluate(List<Tuple<string, double>> variables);

        static public FormulaPart ProcessString(string text)
        {
            if (text == "")
                throw new ArgumentException("formula content empty");

            string breakable_text = text;
            FormulaPart root = null;
            FormulaPart last_oper = null;
            FormulaPart last_part = null;
            while (breakable_text != "")
            {

                
                if (isFunction(breakable_text))
                {
                    Tuple<FormulaPart_Function, string> result;
                    try
                    {
                        result = getFunction(breakable_text);
                    }
                    catch (ArgumentException ex)
                    {
                        throw (new ArgumentException(ex.Message + " -> Builder"));
                    }
                    
                    if (root == null)
                    {
                        root = result.Item1;
                    }
                    else
                        if (last_oper is FormulaPart_Operator)
                        {
                            (last_oper as FormulaPart_Operator).Right = result.Item1;
                        }
                        else
                            throw new ArgumentException("Function found, Operator expected @ " + breakable_text);
                    last_part = result.Item1;
                    breakable_text = result.Item2;
                    continue;
                }


                if (isOperator(breakable_text))
                {
                    if (last_part is FormulaPart_Operator)
                        throw new ArgumentException("Operator found, Operand expected @ " + breakable_text);

                    Tuple<FormulaPart_Operator, string> result;
                    try
                    {
                        result = getOperator(breakable_text);
                    }
                    catch (ArgumentException ex)
                    {
                        throw (new ArgumentException(ex.Message + " -> Builder"));
                    }

                    if (root is FormulaPart_Operator)
                    {
                        if ((root as FormulaPart_Operator).GetPrecedence() >= result.Item1.GetPrecedence())
                        {
                            result.Item1.Left = root;
                            root = result.Item1;
                        }
                        else
                        {
                            result.Item1.Left = (root as FormulaPart_Operator).Right;
                            (root as FormulaPart_Operator).Right = result.Item1;
                        }
                    }
                    else
                        if (root != null)
                        {
                            result.Item1.Left = root;
                            root = result.Item1;
                        }
                        else
                            root = result.Item1;
                    last_oper = result.Item1;
                    last_part = result.Item1;

                    breakable_text = result.Item2;
                    continue;
                }


                if (isVariable(breakable_text))
                {
                    Tuple<FormulaPart, string> result = getVariable(breakable_text);
                    if (root == null)
                    {
                        root = result.Item1;
                    }
                    else
                        if (last_oper is FormulaPart_Operator)
                        {
                            (last_oper as FormulaPart_Operator).Right = result.Item1;
                        }
                        else
                        throw new ArgumentException("Variable found, Operator expected @ " + breakable_text);
                    last_part = result.Item1;

                    breakable_text = result.Item2;
                    continue;
                }


                if (isValue(breakable_text))
                {
                    Tuple<FormulaPart, string> result = getValue(breakable_text);

                    if (root == null)
                    {
                        root = result.Item1;
                    }
                    else
                        if (last_oper is FormulaPart_Operator)
                        {
                            (last_oper as FormulaPart_Operator).Right = result.Item1;
                        }
                        else
                        throw new ArgumentException("Function found, Operator expected @ " + breakable_text);
                    last_part = result.Item1;

                    breakable_text = result.Item2;
                    continue;
                }

                throw new ArgumentException("Statement not found @ " + breakable_text);
            }
            return root;
        }
        
        static public bool isValue(string text)
        {
            Match regex_match = Regex.Match(text, @"\A\G\d*\.?\d+");
            if (regex_match.Success && regex_match.Index == 0)
                return true;
            else return false;
        }

        static public Tuple<FormulaPart, string> getValue(string text)
        {
            if (isValue(text))
            {
                Match regex_match = Regex.Match(text, @"\A\G\d*\.?\d+");
                if (regex_match.Success && regex_match.Index == 0)
                {
                    return new Tuple<FormulaPart, string>(
                            new FormulaPart_Value(double.Parse(regex_match.Value, CultureInfo.InvariantCulture)),
                            text.Substring(regex_match.Value.Length).Trim()
                        );
                }
            }

            return null;
        }

        static public bool isFunction(string text)
        {
            foreach (string fnc in functions)
            {
                if (text.IndexOf(fnc + "(") == 0)
                    return true;
            }
            return false;
        }

        
        static private FormulaPart_Function getFunctionObject(string func_name, string content_text)
        {
            FormulaPart contentpart;
            try
            {
                contentpart = ProcessString(content_text);

                if (func_name == "")
                    return new FormulaPart_FncHks(contentpart);

                if (func_name == "sin")
                    return new FormulaPart_FncSin(contentpart);
            }
            catch (ArgumentException ex)
            {

                throw new ArgumentException(ex.Message + " -> in " + func_name);
            }
            
            throw new ArgumentException(" function " + func_name + " not implemented");
        }
        static public Tuple<FormulaPart_Function, string> getFunction(string text)
        {
            if (isFunction(text) && IsBalancedString(text))
            {
                foreach (string fnc in functions)
                {
                    if (text.IndexOf(fnc + "(") == 0)
                    {
                        int index = BalancedStringIndex(text);
                        string function = text.Substring(0, fnc.Length);
                        string function_content = text.Substring(fnc.Length + 1, index - (fnc.Length + 1));
                        string rest_of_text = text.Substring(index + 1).Trim();

                        FormulaPart_Function subpart;
                        try
                        {
                            subpart = getFunctionObject(fnc, function_content);
                        } catch (ArgumentException ex)
                        {
                            throw;//throw new ArgumentException(ex.Message + " -> getFunction");
                        }
                        
                        
                        return new Tuple<FormulaPart_Function, string>(
                            subpart,
                            rest_of_text
                            );
                    }
                }
            }
            throw new ArgumentException("Invalid function @ " + text);
        }

        static public bool isOperator(string text)
        {
            foreach (string op in operators)
            {
                if (text.IndexOf(op) == 0)
                    return true;
            }
            return false;
        }

        static private FormulaPart_Operator getOperatorObject(string text)
        {
            if (text == "+")
                return new FormulaPart_OpAdd();
            if (text == "-")
                return new FormulaPart_OpSub();
            if (text == "*")
                return new FormulaPart_OpMul();
            if (text == "/")
                return new FormulaPart_OpDiv();
            if (text == "^")
                return new FormulaPart_OpExp();

            throw new ArgumentException(" operator " + text + " not implemented");
        }
        static public Tuple<FormulaPart_Operator, string> getOperator(string text)
        {
            if (isOperator(text))
            {
                foreach( string op in operators )
                {
                    if (text.IndexOf(op) == 0)
                    {
                        FormulaPart_Operator newpart = getOperatorObject(op );
                        string rest_of_text = text.Substring(op.Length).Trim();
                        
                        return new Tuple<FormulaPart_Operator, string>(newpart, rest_of_text);
                    }
                }
            }
            throw new ArgumentException("Operator not implemented");
        }

        static private FormulaPart_Variable getVariableObject(string var_name)
        {
            return new FormulaPart_Variable(var_name);
        }
        
        static public bool isVariable(string text)
        {
            foreach (string var in variables)
            {
                if (text.IndexOf(var) == 0)
                    return true;
            }
            return false;
        }

        static public Tuple<FormulaPart, string> getVariable(string text)
        {
            if (isVariable(text))
            {
                foreach (string var in variables)
                {
                    if (text.IndexOf(var) == 0)
                    {
                        FormulaPart_Variable newpart = getVariableObject(var);
                        string rest_of_text = text.Substring(var.Length).Trim();
                        
                        return new Tuple<FormulaPart, string>(newpart, rest_of_text);
                    }
                }

            }
            return null;
        }

        static private bool IsBalancedString(string text)
        {
            if (text.Count(f => f == '(') == text.Count(f => f == ')') )
                return true;
            else return false;
        }

        static private int BalancedStringIndex(string text)
        {
            if (IsBalancedString(text))
            {
                int charnr = text.IndexOf('(');
                int open = 1;
                while (open > 0)
                {
                    charnr++;
                    if (text[charnr] == '(')
                    {
                        open++;
                    }
                    if (text[charnr] == ')')
                    {
                        open--;
                    }
                }
                return charnr;
            }
            else
                throw new ArgumentException("Inbalanced formula: " + text);
        }

    }
}
