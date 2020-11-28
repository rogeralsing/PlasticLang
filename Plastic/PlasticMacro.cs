using PlasticLang.Ast;
using PlasticLang.Contexts;

namespace PlasticLang
{
    public delegate object? PlasticMacro(PlasticContext context, Syntax[] args);
}