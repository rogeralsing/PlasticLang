using System.Diagnostics;

namespace PlasticLangLabb1.Ast
{
    [DebuggerDisplay("{Name}")]
    public class Identifier : IExpression
    {
        public Identifier(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public object Eval(PlasticContext context)
        {
            return context[Name];
        }
    }
}