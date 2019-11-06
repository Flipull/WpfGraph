using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_Value : FormulaPart
    {
        private double Value;

        public FormulaPart_Value(double value)
        {
            Value = value;
        }

        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            return Value;
        }
    }
}
