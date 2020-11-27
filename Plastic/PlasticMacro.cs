using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang
{
    public delegate ValueTask<object?> PlasticMacro(PlasticContext context, Syntax[] args);
}