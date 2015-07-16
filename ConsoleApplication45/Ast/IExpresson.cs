using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public interface IExpression
    {
        object Eval(PlasticContext context);
    }
}
