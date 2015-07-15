using System.Collections.Generic;

namespace PlasticLangLabb1.Ast
{
    public class PlasticObject
    {
        public PlasticContext Context { get;private set; }

        public PlasticObject(PlasticContext context)
        {
            Context = context;
        }

        public object this[string property]
        {
            get
            {
                return Context[property];
                
            }
            set
            {
                Context.Declare(property, value);
                
            }
        }
    }
}
