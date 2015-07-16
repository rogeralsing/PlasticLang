using System.Collections.Generic;

namespace PlasticLang
{

    public abstract class PlasticContext
    {
       

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
    }
}