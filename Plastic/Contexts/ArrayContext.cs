using System;
using PlasticLang.Ast;
using PlasticLang.Reflection;

namespace PlasticLang.Contexts
{
    public class ArrayContext : PlasticContext
    {
        private readonly object?[] _array;

        public ArrayContext(object[] array, PlasticContextImpl owner) : base(owner)
        {
            _array = array;
        }

        public override object? this[Symbol name]
        {
            get => _array.GetPropertyValue(name.Identity);
            set => throw new NotImplementedException();
        }

        public override object? Invoke(Syntax head, Syntax[] args)
        {
            if (head is not NumberLiteral numberLiteral) throw new NotImplementedException();

            var index = (int) numberLiteral.Value;
            //   var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();

            var res = _array[index];
            return res;
        }

        public override void Declare(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override object Number(NumberLiteral numberLiteral)
        {
            var index = (int) numberLiteral.Value;
            var res = _array[index];
            return res;
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            var res = this[new Symbol(stringLiteral.Value)]!;
            return res;
        }
    }
}