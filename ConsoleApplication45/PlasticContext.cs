using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLang
{
    public class PlasticContext
    {
        private readonly Dictionary<string, object> _cells = new Dictionary<string, object>();
        private readonly PlasticContext _parent;

        public PlasticContext()
        {
        }

        private PlasticContext(PlasticContext parentContext)
        {
            _parent = parentContext;
        }

        public object this[string name]
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

        public bool HasProperty(string name)
        {
            if (_cells.ContainsKey(name))
                return true;

            if (_parent != null)
                return _parent.HasProperty(name);

            return false;
        }

        public PlasticContext ChildContext()
        {
            var child = new PlasticContext(this);
            return child;
        }

        public void Declare(string name, object value)
        {
            _cells[name] = value;
        }
    }
}
