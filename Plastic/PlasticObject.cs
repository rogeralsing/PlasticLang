using PlasticLang.Contexts;

namespace PlasticLang
{
    public class PlasticObject
    {
        public PlasticObject(PlasticContext context)
        {
            Context = context;
        }

        public PlasticContext Context { get; }

        public object this[string property]
        {
            set => Context.Declare(property, value);
        }
    }
}