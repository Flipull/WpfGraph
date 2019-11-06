using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    abstract class FormulaPart_Function: FormulaPart
    {
        static string[] functions = { "", "sin", "cos", "abs" };

        public FormulaPart subFormula { get; set; }


        public FormulaPart_Function(FormulaPart sub)
        {
            if (sub == null)
            {
                throw new ArgumentException("invalid formulapart");
            }

            subFormula = sub;
        }


        public double EvaluateSubFormula(List<Tuple<string, double>> variables)
        {
            return subFormula.Evaluate(variables);
        }
    }
}
