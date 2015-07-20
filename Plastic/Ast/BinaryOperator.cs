using System;

namespace PlasticLang.Ast
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

        public override string ToString()
        {
            return "+";
        }
    }

    public class SubtractBnary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) - ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "-";
        }
    }

    public class MultiplyBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context))*((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "*";
        }
    }

    public class DivideBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context))/((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "/";
        }
    }

    public class EqualsBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) == ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "==";
        }
    }

    public class NotEqualsBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) != ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "!=";
        }
    }

    public class GreaterThanBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) > ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return ">";
        }
    }

    public class GreaterOrEqualBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) >= ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return ">=";
        }
    }

    public class LessThanBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) < ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "<";
        }
    }

    public class LessOrEqualBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic) left.Eval(context)) <= ((dynamic) right.Eval(context));
        }

        public override string ToString()
        {
            return "<=";
        }
    }


    public class DotBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            var l = left.Eval(context);            

            var arr = l as object[];
            if (arr != null)
            {
                var arrayContext = new ArrayContext(arr, context);
                return right.Eval(arrayContext);
            }

            var pobj = l as PlasticObject;
            if (pobj != null)
            {
                return right.Eval(pobj.Context);
            }

            var type = l as Type;
            if (type != null)
            {
                var typeContext = new TypeContext(type, context);
                return right.Eval(typeContext);
            }


            var objContext = new InstanceContext(l, context);
            return right.Eval(objContext);
        }

        public override string ToString()
        {
            return ".";
        }
    }
}