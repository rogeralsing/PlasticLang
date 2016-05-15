using System.Threading.Tasks;
using PlasticLang.Ast;

namespace PlasticLang
{
    public delegate Task<object> PlasticMacro(PlasticContext context, IExpression[] args);
}