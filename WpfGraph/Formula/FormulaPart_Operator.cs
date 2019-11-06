using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    abstract class FormulaPart_Operator: FormulaPart
    {
        public FormulaPart Left { get; set; }
        public FormulaPart Right { get; set; }
        public FormulaPart_Operator()
        {
            Left = new FormulaPart_Value(0);
            Right = new FormulaPart_Value(0);

        }

        abstract public byte GetPrecedence();
        abstract public override double Evaluate(List<Tuple<string, double>> variables);
    }
}
