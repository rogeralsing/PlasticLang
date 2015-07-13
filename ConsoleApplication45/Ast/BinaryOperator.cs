namespace PlasticLangLabb1.Ast
{
    public abstract class BinaryOperator
    {
        public abstract object Eval(PlasticContext context, IExpression left, IExpression right);
    }

    public class AddBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) + ((dynamic) right.Eval(context));
        }
    }

    public class SubtractBnary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) - ((dynamic) right.Eval(context));
        }
    }

    public class MultiplyBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context))*((dynamic) right.Eval(context));
        }
    }

    public class DivideBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context))/((dynamic) right.Eval(context));
        }
    }

    public class EqualsBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) == ((dynamic) right.Eval(context));
        }
    }
}