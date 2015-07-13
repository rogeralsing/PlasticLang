﻿using System;

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

    public class NotEqualsBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic)left.Eval(context)) != ((dynamic)right.Eval(context));
        }
    }

    public class GreaterThanBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic)left.Eval(context)) > ((dynamic)right.Eval(context));
        }
    }

    public class GreaterOrEqualBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic)left.Eval(context)) >= ((dynamic)right.Eval(context));
        }
    }

    public class LessThanBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic)left.Eval(context)) < ((dynamic)right.Eval(context));
        }
    }

    public class LessOrEqualBinary : BinaryOperator
    {
        public override object Eval(PlasticContext context, IExpression left, IExpression right)
        {
            return ((dynamic)left.Eval(context)) <= ((dynamic)right.Eval(context));
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
                
                if (right is Number)
                {
                    var r = right.Eval(context);
                    var index = (int) (decimal) r;
                    return arr[index];
                }
            }

            var id = right as Identifier;
            if (id != null)
            {
                var res = l.GetType().GetProperty(id.Name).GetValue(l);
                return res;
            }
            var str = right as QuotedString;
            if (str != null)
            {
                var res = l.GetType().GetProperty(str.Value).GetValue(l);
                return res;
            }

            throw new NotSupportedException();
        }
    }
}