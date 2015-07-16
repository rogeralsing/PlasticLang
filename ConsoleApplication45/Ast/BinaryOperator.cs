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
            var member = right as Identifier;

            var arr = l as object[];
            if (arr != null)
            {
                if (right is Number)
                {
                    var r = right.Eval(context);
                    var index = (int) (decimal) r;
                    return arr[index];
                }
            }

            var pobj = l as PlasticObject;
            if (pobj != null)
            {
                return right.Eval(pobj.Context);
            }


            if (member != null)
            {
                var res = l.GetType().GetProperty(member.Name).GetValue(l);
                return res;
            }
            var str = right as QuotedString;
            if (str != null)
            {
                var res = l.GetType().GetProperty(str.Value).GetValue(l);
                return res;
            }

            var type = l as Type;
            if (type != null)
            {
                var tmpCtx = context.ChildContext();
              //  tmpCtx.Declare();
            }

            throw new NotSupportedException();
        }

        public override string ToString()
        {
            return ".";
        }
    }
}