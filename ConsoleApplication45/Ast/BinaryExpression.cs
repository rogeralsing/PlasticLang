namespace PlasticLangLabb1.Ast
{
    public class BinaryExpression : IExpression
    {
        public BinaryExpression(IExpression left, BinaryOperator op, IExpression right)
        {
            Left = left;
            Op = op;
            Right = right;
        }

        public IExpression Left { get; private set; }
        public BinaryOperator Op { get; private set; }
        public IExpression Right { get; private set; }

        public object Eval(PlasticContext context)
        {
            return Op.Eval(context, Left, Right);
        }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Left, Op, Right);
        }
    }
}