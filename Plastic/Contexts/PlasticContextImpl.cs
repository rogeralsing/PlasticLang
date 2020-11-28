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
        

        public PlasticContextImpl(PlasticContext? parentContext = null) : base(parentContext)
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
                    _cells[name] = value!;
                    return;
                }

                if (!_cells.ContainsKey(name) && Parent is not null)
                    Parent[name] = value!;
                else
                    _cells[name] = value!;
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
        
        public void Declare(string name, PlasticMacro value)
        {
            _cells[name] = value;
        }

        private static async ValueTask<dynamic?> InvokeMacro(PlasticContext context, PlasticMacro macro, Syntax[] args)
        {
            var value = await macro(context, args);
            context.Declare("last", value!);
            return value;
        }

        public override async ValueTask<dynamic?> Invoke(Syntax head, Syntax[] args)
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
                    throw new NotImplementedException();
            }
        }

        public override ValueTask<dynamic> Number(NumberLiteral numberLiteral)
        {
            var res = numberLiteral.Value;
            return ValueTask.FromResult((object) res);
        }

        public override ValueTask<dynamic> QuotedString(StringLiteral stringLiteral)
        {
            var res = stringLiteral.Value;
            return ValueTask.FromResult((object) res);
        }
    }
}