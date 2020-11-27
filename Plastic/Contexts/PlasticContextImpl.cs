using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Visitors;

namespace PlasticLang.Contexts
{
    public class PlasticContextImpl : PlasticContext
    {
        private readonly Dictionary<string, object> _cells = new();
        
        public PlasticContextImpl() : base(null)
        {
        }

        public PlasticContextImpl(PlasticContext parentContext) : base(parentContext)
        {
        }

        public override object? this[string name]
        {
            get =>
                //if cell is not populated in this context, fetch from parent
                !_cells.ContainsKey(name) ? Parent?[name] : _cells[name];
            set
            {
                if (!HasProperty(name))
                {
                    _cells[name] = value;
                    return;
                }

                if (!_cells.ContainsKey(name) && Parent is not null)
                    Parent[name] = value;
                else
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

        private static async ValueTask<object> InvokeMacro(PlasticContext context, PlasticMacro macro, Syntax[] args)
        {
            var res = await macro(context, args);
            context.Declare("last", res);
            return res;
        }

        public override async ValueTask<object> Invoke(Syntax head, Syntax[] args)
        {
            var target = await head.Eval(this);
            var expression = target as Syntax;
            var array = target as object[];

            if (target is PlasticMacro macro) return await InvokeMacro(this, macro, args);

            if (expression != null) return await expression.Eval(this);

            if (array == null) throw new NotImplementedException();

            var index = (int) (decimal) await args.First().Eval(this);
            return array[index];
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