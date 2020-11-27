using System;
using System.Linq;
using System.Threading.Tasks;
using PlasticLang.Ast;

namespace PlasticLang
{
    public class TypeContext : PlasticContext
    {
        private readonly Type _type;

        public TypeContext(Type type, PlasticContext owner) : base(owner)
        {
            _type = type;
        }

        public override object this[string name]
        {
            get
            {
                var prop = typeof(Type).GetProperty(name);
                return prop.GetValue(_type);
            }
            set => throw new NotSupportedException();
        }

        public override bool HasProperty(string name)
        {
            throw new NotSupportedException();
        }

        public override void Declare(string name, object value)
        {
            throw new NotSupportedException();
        }

        public override async ValueTask<object> Invoke(Syntax head, Syntax[] args)
        {
            var memberName = (head as Symbol)?.Value;
            var evaluatedArgs = args.Select(async a => await a.Eval(Parent)).ToArray();
            var members = _type.GetMethods().Where(m => m.Name == memberName);
            foreach (var member in members)
                try
                {
                    var res = member.Invoke(null, evaluatedArgs);
                    return res;
                }
                catch
                {
                }

            throw new Exception("No matching method found.");
        }

        public override ValueTask<object> Number(NumberLiteral numberLiteral)
        {
            throw new NotImplementedException();
        }

        public override ValueTask<object> QuotedString(StringLiteral stringLiteral)
        {
            throw new NotImplementedException();
        }
    }
}