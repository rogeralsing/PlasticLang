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
        private readonly Dictionary<string, object> _cells = new Dictionary<string, object>();
        private readonly PlasticContext _parent;

        public PlasticContext()
        {
            
        }

        public PlasticContext(PlasticContext parentContext)
        {
            _parent = parentContext;
        }

        public void Declare(string name, object value)
        {
            _cells[name] = value;
        }

        public object this[string name]
        {
            get
            {
                //if cell is not populated in this context, fetch from parent
                if (!_cells.ContainsKey(name) && _parent != null)
                    return _parent[name];

                return _cells[name];
            }
            set
            {
                if (!_cells.ContainsKey(name) && _parent != null)
                    _parent[name] = value;
                else
                    _cells[name] = value;
            }
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