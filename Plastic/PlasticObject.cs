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
            get => Context[property];
            set => Context.Declare(property, value);
        }
    }
}