using System;
using System.Linq;
using PlasticLang.Ast;
using PlasticLang.Visitors;

namespace PlasticLang.Contexts
{
    public class ClrTypeContext : PlasticContext
    {
        private readonly Type _type;

        public ClrTypeContext(Type type, PlasticContextImpl owner) : base(owner)
        {
            _type = type;
        }

        public override object? this[Symbol name]
        {
            get
            {
                var prop = typeof(Type).GetProperty(name.Identity);
                return prop.GetValue(_type);
            }
            set => throw new NotSupportedException();
        }

        public override object? Invoke(Syntax head, Syntax[] args)
        {
            var memberName = (head as Symbol)?.Identity;
            var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();
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

        public override object Number(NumberLiteral numberLiteral)
        {
            throw new NotImplementedException();
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            throw new NotImplementedException();
        }
    }
}