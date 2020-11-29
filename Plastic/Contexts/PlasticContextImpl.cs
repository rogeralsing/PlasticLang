using System;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Visitors;

namespace PlasticLang.Contexts
{
    public struct Cell
    {
        public object? Value { get; private set; }
        public bool HasValue { get; private set; }

        public void SetValue(object? value)
        {
            Value = value;
            HasValue = true;
        }
    }

    public sealed class PlasticContextImpl : PlasticContext
    {
        private readonly Cell[] _cells = new Cell[100];

        public PlasticContextImpl(PlasticContextImpl? parentContext = null) : base(parentContext)
        {
        }

        public override object? this[Symbol name]
        {
            get
            {
                if (_cells[name.IdNum].HasValue) return _cells[name.IdNum].Value;

                return Parent?[name];
            }
            set
            {
                if (_cells[name.IdNum].HasValue)
                {
                    _cells[name.IdNum].SetValue(value);
                    return;
                }

                if (Parent is not null && Parent.HasProperty(name))
                {
                    Parent[name] = value!;
                    return;
                }

                //name was not found in self or any parent, declare a new instance right here in this context
                _cells[name.IdNum].SetValue(value);
            }
        }


        public bool HasProperty(Symbol name)
        {
            if (_cells[name.IdNum].HasValue)
                return true;

            if (Parent != null)
                return Parent.HasProperty(name);

            return false;
        }

        public override void Declare(string name, object value)
        {
            Symbol s = new(name);
            _cells[s.IdNum].SetValue(value);
        }

        public void Declare(string name, PlasticMacro value)
        {
            Declare(name, (object) value);
        }

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