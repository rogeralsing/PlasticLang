using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLangLabb1.Ast
{
    public class ArgsAndBody
    {
        public ArgsAndBody(TupleValue args, Statements body)
        {
            Args = args;
            Body = body;
        }

        public TupleValue Args { get; set; }
        public Statements Body { get; set; }
    }
}
