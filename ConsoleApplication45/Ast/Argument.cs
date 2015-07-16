using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlasticLangLabb1.Ast
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
        Expression,

    }
}
