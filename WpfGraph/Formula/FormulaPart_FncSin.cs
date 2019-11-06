using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_FncSin : FormulaPart_Function
    {
        public FormulaPart_FncSin(FormulaPart sub): base(sub)
        {
        }
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            return Math.Sin(EvaluateSubFormula(variables));
        }
    }
}
