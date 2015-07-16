namespace PlasticLang.Ast
{
    public class PlasticObject
    {
        public PlasticObject(PlasticContext context)
        {
            Context = context;
        }

        public PlasticContext Context { get; private set; }

        public object this[string property]
        {
            get { return Context[property]; }
            set { Context.Declare(property, value); }
        }
    }
}