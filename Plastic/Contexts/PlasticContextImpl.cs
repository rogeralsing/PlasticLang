using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
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
        private Cell[] Cells { get; } = new Cell[100];

        public PlasticContextImpl(PlasticContextImpl? parentContext = null) : base(parentContext)
        {
        }

        public override object? this[Symbol name]
        {
            get
            {
                var current = this;
                while (current != null)
                {
                    if (current.Cells[name.IdNum].HasValue) return current.Cells[name.IdNum].Value;
                    current = current.Parent;
                }

                return null;
            }
            set
            {
                var current = this;
                while (current != null)
                {
                    if (current.Cells[name.IdNum].HasValue)
                    {
                        current.Cells[name.IdNum].SetValue(value);
                        return;
                    }

                    current = current.Parent;
                }

                Cells[name.IdNum].SetValue(value);
            }
        }

        public void Declare(string name, object value)
        {
            Declare(new Symbol(name),value);
        }
        
        public void Declare(Symbol s, object value)
        {
            Cells[s.IdNum].SetValue(value);
        }

        public void Declare(string name, PlasticMacro value)
        {
            Declare(name, (object) value);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static object? InvokeMacro(PlasticContext context, PlasticMacro macro, Syntax[] args)
        {
            var value = macro(context, args);
            ((PlasticContextImpl)context).Declare(Symbol.Last, value!);
            return value;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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