using System.Threading.Tasks;

namespace PlasticLang.Ast
{
    public interface IExpression
    {
        Task<object> Eval(PlasticContext context);
    }
}