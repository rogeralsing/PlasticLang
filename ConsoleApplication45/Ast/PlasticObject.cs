using System.Collections.Generic;

namespace PlasticLangLabb1.Ast
{
    public class PlasticObject
    {
        private readonly Dictionary<string,object> _properties = new Dictionary<string, object>();

        public bool HasProperty(string name)
        {
            return _properties.ContainsKey(name);
        }

        public object this[string property]
        {
            get
            {
                object res = null;
                _properties.TryGetValue(property, out res);
                return res;
            }
            set { _properties[property] = value; }
        }
    }
}
