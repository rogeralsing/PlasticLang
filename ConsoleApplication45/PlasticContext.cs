using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Policy;
using PlasticLang.Ast;

namespace PlasticLang
{

    public abstract class PlasticContext
    {

        public abstract object Invoke(IExpression head, IExpression[] args);

        public abstract object this[string name] { get; set; }


        public abstract bool HasProperty(string name);

        public abstract void Declare(string name, object value);
    }

    public class PlasticContextImpl : PlasticContext
    {
        private readonly Dictionary<string, object> _cells = new Dictionary<string, object>();
        private readonly PlasticContext _parent;

        public PlasticContextImpl()
        {
        }

        public PlasticContextImpl(PlasticContext parentContext)
        {
            _parent = parentContext;
        }

        public override object this[string name]
        {
            get
            {
                //if cell is not populated in this context, fetch from parent
                if (!_cells.ContainsKey(name) && _parent != null)
                    return _parent[name];

                return _cells[name];
            }
            set
            {
                if (!HasProperty(name))
                {
                    _cells[name] = value;
                    return;
                }

                if (!_cells.ContainsKey(name) && _parent != null)
                    _parent[name] = value;
                else
                    _cells[name] = value;
            }
        }

        public override bool HasProperty(string name)
        {
            if (_cells.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.HasProperty(name);

            return false;
        }

        public override void Declare(string name, object value)
        {
            _cells[name] = value;
        }

        private object InvokeMacro(PlasticContext context, PlasticMacro macro, IExpression[] Args)
        {
            var ctx = new PlasticContextImpl(context);
            var args = Args;
            var res = macro(ctx, args);
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
    }

    public class TypeContext : PlasticContext
    {
        private Type _type;
        private readonly System.Reflection.MemberInfo[] _members;
        private readonly Dictionary<string, _MemberInfo> _lookup;
        private PlasticContext _owner;

        public TypeContext(Type type,PlasticContext owner)
        {
            _type = type;
            _owner = owner;           
        }

        public override object this[string name]
        {
            get { throw new NotSupportedException(); }
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
            var memberName = (head as Identifier).Name;
            var evaluatedArgs = args.Select(a => a.Eval(_owner)).ToArray();
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
    }
}