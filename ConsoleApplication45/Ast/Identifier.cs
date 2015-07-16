namespace PlasticLang.Ast
{
    public class Identifier : IExpression
    {
        public Identifier(string name)
        {
            Name = name;
        }

        public string Name { get; set; }

        public object Eval(PlasticContext context)
        {
            return context[Name];
        }

        public override string ToString()
        {
            return Name;
        }
    }
}