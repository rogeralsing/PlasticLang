using System;
using System.Collections.Generic;
using System.Linq;
using PlasticLang.Ast;

namespace PlasticLang
{

    public abstract class PlasticContext
    {
        public PlasticContext Parent { get; private set; }

        protected PlasticContext(PlasticContext parent)
        {
            Parent = parent;
        }
        public abstract object Number(NumberLiteral numberLiteral);

        public abstract object QuotedString(StringLiteral stringLiteral);


        public abstract object Invoke(IExpression head, IExpression[] args);

        public abstract object this[string name] { get; set; }


        public abstract bool HasProperty(string name);

        public abstract void Declare(string name, object value);
    }

    public class PlasticContextImpl : PlasticContext
    {
        private readonly Dictionary<string, object> _cells = new Dictionary<string, object>();
        

        public PlasticContextImpl() : base(null)
        {
        }

        public PlasticContextImpl(PlasticContext parentContext) : base(parentContext)
        {
        }

        public override object this[string name]
        {
            get
            {
                //if cell is not populated in this context, fetch from parent
                if (!_cells.ContainsKey(name))
                {
                    if (Parent != null)
                        return Parent[name];
                    return null;
                }

                return _cells[name];
            }
            set
            {
                if (!HasProperty(name))
                {
                    _cells[name] = value;
                    return;
                }

                if (!_cells.ContainsKey(name) && Parent != null)
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

        private object InvokeMacro(PlasticContext context, PlasticMacro macro, IExpression[] Args)
        {
            var args = Args;
            var res = macro(context, args);
            context.Declare("last", res);
            return res;
        }

        public override object Invoke(IExpression head, IExpression[] args)
        {
            var target = head.Eval(this);
            var macro = target as PlasticMacro;
            var expression = target as IExpression;
            var array = target as object[];

            if (macro != null)
            {
                return InvokeMacro(this, macro, args);
            }

            if (expression != null)
            {
                return expression.Eval(this);
            }

            if (array != null)
            {
                var index = (int)(decimal)args.First().Eval(this);
                return array[index];
            }
            throw new NotImplementedException();
        }

        public override object Number(NumberLiteral numberLiteral)
        {
            return numberLiteral.Value;
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            return stringLiteral.Value;
        }
    }

    public class TypeContext : PlasticContext
    {
        private readonly Type _type;

        public TypeContext(Type type,PlasticContext owner) : base(owner)
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
            set
            {
                throw new NotSupportedException();
            }
        }

        public override bool HasProperty(string name)
        {
            throw new NotSupportedException();
        }

        public override void Declare(string name, object value)
        {
            throw new NotSupportedException();
        }

        public override object Invoke(IExpression head, IExpression[] args)
        {

            var memberName = (head as Symbol).Value;
            var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();
            var members = _type.GetMethods().Where(m => m.Name == memberName);
            foreach (var member in members)
            {
                try
                {
                    var res = member.Invoke(null, evaluatedArgs);
                    return res;
                }
                catch
                {
                    
                }
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

    public class ArrayContext : PlasticContext
    {
        private readonly object[] _array;

        public ArrayContext(object[] array, PlasticContext owner) : base(owner)
        {
            _array = array;
        }

        public override object Invoke(IExpression head, IExpression[] args)
        {
            var index = (int)(head as NumberLiteral).Value;
            var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();

            return _array[index];
        }

        public override object this[string name]
        {
            get
            {
                var prop = _array.GetType().GetProperty(name);
                var res = prop.GetValue(_array);
                return res;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasProperty(string name)
        {
            throw new NotImplementedException();
        }

        public override void Declare(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override object Number(NumberLiteral numberLiteral)
        {
            var index = (int) numberLiteral.Value;
            return _array[index];
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            return this[stringLiteral.Value];
        }
    }

    public class InstanceContext : PlasticContext
    {
        private readonly object _obj;

        public InstanceContext(object obj, PlasticContext owner):base(owner)
        {
            _obj = obj;
        }

        public override object Invoke(IExpression head, IExpression[] args)
        {
            var memberName = "";
            if (head is IStringLiteral)
            {
                memberName = (head as IStringLiteral).Value;
            }



            var methods = _obj.GetType().GetMethods().Where(m => m.Name == memberName);
            if (methods.Any())
            {
                 var evaluatedArgs = args.Select(a => a.Eval(Parent)).ToArray();
                foreach (var method in methods)
                {
                    try
                    {
                        var res = method.Invoke(_obj, evaluatedArgs);
                        return res;
                    }
                    catch
                    {

                    }
                }
            }

            var properties = _obj.GetType().GetProperties().Where(m => m.Name == memberName);
            foreach (var property in properties)
            {
                try
                {
                    var m = property.GetGetMethod().Invoke(_obj, null) as PlasticMacro;
                    var res = m(this, args);
                    return res;
                }
                catch
                {

                }
            }
            throw new Exception("No matching method found.");
        }

        public override object this[string name]
        {
            get
            {
                var prop = _obj.GetType().GetProperty(name);
                var res = prop.GetValue(_obj);
                return res;
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool HasProperty(string name)
        {
            throw new NotImplementedException();
        }

        public override void Declare(string name, object value)
        {
            throw new NotImplementedException();
        }

        public override object Number(NumberLiteral numberLiteral)
        {
            throw new NotImplementedException();
        }

        public override object QuotedString(StringLiteral stringLiteral)
        {
            return this[stringLiteral.Value];
        }
    }
}