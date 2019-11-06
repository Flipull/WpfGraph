using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_FncHks: FormulaPart_Function
    {
        public FormulaPart_FncHks(FormulaPart sub): base(sub)
        {
        }
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            return EvaluateSubFormula(variables);
        }
    }
}
