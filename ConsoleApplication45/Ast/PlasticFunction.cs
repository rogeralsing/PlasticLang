using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLangLabb1.Ast
{
    public delegate object PlasticFunction(object[] args);
    public delegate object PlasticMacro(PlasticContext context, IExpression[] args);
}
