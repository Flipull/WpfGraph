using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfGraph.Formula
{
    class FormulaPart_OpDiv: FormulaPart_Operator
    {
        public override double Evaluate(List<Tuple<string, double>> variables)
        {
            //if (Right.Evaluate(variables) == 0)
            //    throw new ArgumentException("Division by 0");
            return Left.Evaluate(variables) / Right.Evaluate(variables);
        }
        
        public const byte Precedence = 1;
        public override byte GetPrecedence()
        {
            return Precedence;
        }
    }

}
