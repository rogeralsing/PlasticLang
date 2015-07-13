namespace PlasticLangLabb1.Ast
{
    public class BinaryExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly BinaryOperator _op;
        private readonly IExpression _right;

        public BinaryExpression(IExpression left, BinaryOperator op, IExpression right)
        {
            this._left = left;
            this._op = op;
            this._right = right;
        }

        public object Eval(PlasticContext context)
        {
            return _op.Eval(context, _left, _right);
        }
    }
}