using System;
using System.Threading.Tasks;
using PlasticLang.Ast;
using PlasticLang.Reflection;

namespace PlasticLang.Contexts
{
    public class ArrayContext : PlasticContext
    {
        private readonly object?[] _array;

        public ArrayContext(object[] array, PlasticContext owner) : base(owner)
        {
            _array = array;
        }

        public override object? this[string name]
        {
            get => _array.GetPropertyValue(name);
            set => throw new NotImplementedException();
        }

        public override ValueTask<object?> Invoke(Syntax head, Syntax[] args)
        {
            if (head is not NumberLiteral numberLiteral) throw new NotImplementedException();
            
            var index = (int) numberLiteral.Value;
            //   var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();

            var res = _array[index];
            return ValueTask.FromResult(res);

        }

        public override Cell GetCell(string name)
        {
            throw new NotImplementedException();
        }

        public override bool HasProperty(string name)
        {
            throw new NotImplementedException();
        }

        public override void Declare(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<object> Number(NumberLiteral numberLiteral)
        {
            var index = (int) numberLiteral.Value;
            var res = _array[index];
            return ValueTask.FromResult(res);
        }

        public override ValueTask<object> QuotedString(StringLiteral stringLiteral)
        {
            var res = this[stringLiteral.Value]!;
            return ValueTask.FromResult(res);
        }
    }
}