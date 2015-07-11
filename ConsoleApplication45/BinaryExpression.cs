namespace PlasticLangLabb1
{
    internal class BinaryExpression : IExpression
    {
        private IExpression left;
        private BinaryOperator op;
        private IExpression right;

        public BinaryExpression(IExpression left, BinaryOperator op, IExpression right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
    }
}