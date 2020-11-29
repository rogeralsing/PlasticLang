using PlasticLang.Contexts;

namespace PlasticLang
{
    public class PlasticObject
    {
        public PlasticObject(PlasticContextImpl context)
        {
            Context = context;
        }

        public PlasticContextImpl Context { get; }

        public object this[string property]
        {
            set => Context.Declare(property, value);
        }
    }
}