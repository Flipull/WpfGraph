using System;
using System.Collections.Generic;

namespace WpfGraph.Formula
{
    class FormulaPart_OpMul : FormulaPart_Operator
    {
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            return Left.Evaluate(variables) * Right.Evaluate(variables);
        }

        public const byte Precedence = 1;
        public override byte GetPrecedence()
        {
            return Precedence;
        }

    }
}
