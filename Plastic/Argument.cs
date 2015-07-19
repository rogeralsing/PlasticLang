namespace PlasticLang.Ast
{
    public class Argument
    {
        public Argument(string name, ArgumentType type)
        {
            Name = name;
            Type = type;
        }

        public ArgumentType Type { get; set; }
        public string Name { get; set; }
    }

    public enum ArgumentType
    {
        Value,
        Expression
    }
}