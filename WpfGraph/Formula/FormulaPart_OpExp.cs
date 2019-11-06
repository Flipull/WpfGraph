using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_OpExp : FormulaPart_Operator
    {
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            return Math.Pow( Left.Evaluate(variables), Right.Evaluate(variables) );
        }

        public const byte Precedence = 2;
        public override byte GetPrecedence()
        {
            return Precedence;
        }
    }
}
