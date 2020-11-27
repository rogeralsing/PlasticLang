namespace PlasticLang.Ast
{
    public class Argument
    {
        public Argument(string name, ArgumentType type)
        {
            Name = name;
            Type = type;
        }

        public ArgumentType Type { get;  }
        public string Name { get;  }
    }

    public enum ArgumentType
    {
        Value,
        Expression
    }
}