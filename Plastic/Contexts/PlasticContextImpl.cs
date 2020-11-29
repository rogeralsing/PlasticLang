using System;
using System.Collections.Generic;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Visitors;

namespace PlasticLang.Contexts
{
    public class PlasticContextImpl : PlasticContext
    {
        private readonly Dictionary<string, object> _cells = new();


        public PlasticContextImpl(PlasticContext? parentContext = null) : base(parentContext)
        {
        }

        public override object? this[string name]
        {
            get
            {
                if (_cells.TryGetValue(name, out var existing)) return existing;
                
                return Parent?[name];
            }
            set
            {
                if (_cells.ContainsKey(name))
                {
                    _cells[name] = value;
                    return;
                }

                if (Parent is not null && Parent.HasProperty(name))
                {
                    Parent[name] = value!;
                    return;
                }

                //name was not found in self or any parent, declare a new instance right here in this context
                _cells[name] = value;
            }
        }


        public override bool HasProperty(string name)
        {
            if (_cells.ContainsKey(name))
                return true;

            if (Parent != null)
                return Parent.HasProperty(name);

            return false;
        }

        public override void Declare(string name, object value)
        {
            _cells[name] = value;
        }

        public void Declare(string name, PlasticMacro value) => Declare(name, (object) value);

        private static object? InvokeMacro(PlasticContext context, PlasticMacro macro, Syntax[] args)
        {
            var value = macro(context, args);
            context.Declare("last", value!);
            return value;
        }

        public override object? Invoke(Syntax head, Syntax[] args)
        {
            var target = head.Eval(this);

            switch (target)
            {
                case PlasticMacro macro:
                    return InvokeMacro(this, macro, args);
                case Syntax expression:
                    return expression.Eval(this);
                case object[] array:
                {
                    var index = (int) (decimal) args.First().Eval(this);
                    return array[index];
                }
                default:
                    throw new ArgumentNullException(nameof(target));
            }
        }

        public override object Number(NumberLiteral numberLiteral)
        {
            var res = numberLiteral.Value;
            return res;
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            var res = stringLiteral.Value;
            return res;
        }
    }
}