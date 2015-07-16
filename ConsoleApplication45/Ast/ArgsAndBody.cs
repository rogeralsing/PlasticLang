using PlasticLang.Ast;

namespace PlasticLang.Ast
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