using System.Diagnostics;

namespace PlasticLangLabb1.Ast
{
    public class BinaryExpression : IExpression
    {
        private readonly IExpression _left;
        private readonly BinaryOperator _op;
        private readonly IExpression _right;

        public BinaryExpression(IExpression left, BinaryOperator op, IExpression right)
        {
            _left = left;
            _op = op;
            _right = right;
        }

        public object Eval(PlasticContext context)
        {
            return _op.Eval(context, _left, _right);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", _left, _op, _right);
        }
    }
}