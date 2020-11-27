using System.Threading.Tasks;
using PlasticLang.Ast;

namespace PlasticLang
{
    public delegate ValueTask<object> PlasticMacro(PlasticContext context, Syntax[] args);
}