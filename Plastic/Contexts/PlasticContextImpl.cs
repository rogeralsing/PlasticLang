using System;
using System.Collections.Generic;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Visitors;

namespace PlasticLang.Contexts
{
    public class Cell
    {
        public Cell()
        {
        }

        public Cell(object? value)
        {
            Value = value;
        }

        public object? Value { get; set; }
    }

    public class PlasticContextImpl : PlasticContext
    {
        private readonly Dictionary<string, Cell> _cells = new();


        public PlasticContextImpl(PlasticContext? parentContext = null) : base(parentContext)
        {
        }

        public override object? this[string name]
        {
            get
            {
                object? res;
                if (!_cells.ContainsKey(name))
                    res = Parent?[name];
                else
                    res = _cells[name].Value;
                return res;
            }
            set
            {
                if (!HasProperty(name) || Parent is null)
                {
                    _cells[name] = new Cell(value);
                    return;
                }

                Parent[name] = value!;
            }
        }

        public override Cell GetCell(string name)
        {
            if (_cells.ContainsKey(name)) return _cells[name];

            Cell? c = null;
            if (Parent is not null) c = Parent.GetCell(name);

            if (c is not null) return c;
            
            c = new Cell();
            _cells[name] = c;

            return c;

        }

        public override object? GetSymbol(Symbol symbol)
        {
            var c = CellFromSymbol(symbol);
            return c.Value;
        }

        private Cell CellFromSymbol(Symbol symbol)
        {
            var c = symbol.Cell ?? GetCell(symbol.Identity);
            symbol.Cell = c;
            return c;
        }

        public override void SetSymbol(Symbol symbol, object value)
        {
            var c = CellFromSymbol(symbol);
            c.Value = value;
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
            if (!_cells.ContainsKey(name))
                _cells[name] = new Cell(value);
            else
                _cells[name].Value = value;
        }

        public void Declare(string name, PlasticMacro value)
        {
            _cells[name] = new Cell(value);
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