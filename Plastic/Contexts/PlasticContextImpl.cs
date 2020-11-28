using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
            Cell res;
            if (!_cells.ContainsKey(name))
                res = Parent?.GetCell(name);
            else
                res = _cells[name];
            return res;
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

        private static async ValueTask<object?> InvokeMacro(PlasticContext context, PlasticMacro macro, Syntax[] args)
        {
            var value = await macro(context, args);
            context.Declare("last", value!);
            return value;
        }

        public override async ValueTask<object?> Invoke(Syntax head, Syntax[] args)
        {
            var target = await head.Eval(this);

            switch (target)
            {
                case PlasticMacro macro:
                    return await InvokeMacro(this, macro, args);
                case Syntax expression:
                    return await expression.Eval(this);
                case object[] array:
                {
                    var index = (int) (decimal) await args.First().Eval(this);
                    return array[index];
                }
                default:
                    throw new ArgumentNullException(nameof(target));
            }
        }

        public override ValueTask<object> Number(NumberLiteral numberLiteral)
        {
            var res = numberLiteral.Value;
            return ValueTask.FromResult((object) res);
        }

        public override ValueTask<object> QuotedString(StringLiteral stringLiteral)
        {
            var res = stringLiteral.Value;
            return ValueTask.FromResult((object) res);
        }
    }
}