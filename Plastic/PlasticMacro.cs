using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang
{
    public delegate ValueTask<dynamic?> PlasticMacro(PlasticContext context, Syntax[] args);
}