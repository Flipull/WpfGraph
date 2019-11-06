using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_Variable : FormulaPart
    {
        private string VariableName;
        public FormulaPart_Variable(string variable_name)
        {
            VariableName = variable_name;
        }
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            foreach (Tuple<string, double> var in variables)
            {
                if (var.Item1 == VariableName)
                {
                    return var.Item2;
                }
            }

            return double.NaN;
        }
    }
}
