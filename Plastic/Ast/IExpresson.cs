using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public abstract record Syntax
    {
        public abstract ValueTask<object> Eval(PlasticContext context);
    }
}