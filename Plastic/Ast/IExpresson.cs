namespace PlasticLang.Ast
{
    public interface IExpression
    {
        object Eval(PlasticContext context);
    }
}