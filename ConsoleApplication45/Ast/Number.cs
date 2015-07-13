using System.Collections.Generic;
using System.Globalization;

namespace PlasticLangLabb1.Ast
{
    public interface IExpression
    {
        object Eval(PlasticContext context);
    }

    public class PlasticContext
    {
        private readonly Dictionary<string,object> _cells = new Dictionary<string, object>(); 

        public object this[string name]
        {
            get { return _cells[name]; }
            set { _cells[name] = value; }
        }
    }



    public class Number : IExpression
    {
        public Number(string numb)
        {
            Value = decimal.Parse(numb, NumberFormatInfo.InvariantInfo);
        }

        public decimal Value { get; private set; }

        public object Eval(PlasticContext context)
        {
            return Value;
        }
    }
}