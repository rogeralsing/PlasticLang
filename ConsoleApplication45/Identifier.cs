namespace PlasticLangLabb1
{
    internal class Identifier : IExpression
    {
        public Identifier(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}