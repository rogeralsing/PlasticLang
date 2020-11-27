using System;
using System.Linq;
using System.Threading.Tasks;
using PlasticLang.Ast;

namespace PlasticLang
{
    public class InstanceContext : PlasticContext
    {
        private readonly object _obj;

        public InstanceContext(object obj, PlasticContext owner) : base(owner)
        {
            _obj = obj;
        }

        public override object this[string name]
        {
            get
            {
                var prop = _obj.GetType().GetProperty(name);
                var res = prop.GetValue(_obj);
                return res;
            }
            set => throw new NotImplementedException();
        }

        public override ValueTask<object?> Invoke(Syntax head, Syntax[] args)
        {
            var memberName = "";
            if (head is StringLiteral sl) memberName = sl.Value;


            var methods = _obj.GetType().GetMethods().Where(m => m.Name == memberName).ToArray();
            if (methods.Any())
            {
                ValueTask<object>[] evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();
                
         
                foreach (var method in methods)
                    try
                    {
                        var res = method.Invoke(_obj, evaluatedArgs);
                        return ValueTask.FromResult(res);
                    }
                    catch
                    {
                    }
            }

            var properties = _obj.GetType().GetProperties().Where(m => m.Name == memberName).ToArray();
            foreach (var property in properties)
                try
                {
                    var m = property.GetGetMethod()?.Invoke(_obj, null) as PlasticMacro;
                    var res = m(this, args);
                    return res;
                }
                catch
                {
                }

            throw new Exception("No matching method found.");
        }

        public override bool HasProperty(string name) => throw new NotImplementedException();

        public override void Declare(string name, object value) => throw new NotImplementedException();

        public override ValueTask<object> Number(NumberLiteral numberLiteral) => throw new NotImplementedException();

        public override ValueTask<object> QuotedString(StringLiteral stringLiteral)
        {
            var res = this[stringLiteral.Value];
            return ValueTask.FromResult(res);
        }
    }
}