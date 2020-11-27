using System.Threading.Tasks;
using PlasticLang.Ast;

namespace PlasticLang
{
    public abstract class PlasticContext
    {
        protected PlasticContext(PlasticContext parent)
        {
            Parent = parent;
        }

        public PlasticContext Parent { get; }

        public abstract object this[string name] { get; set; }
        public abstract ValueTask<object> Number(NumberLiteral numberLiteral);

        public abstract ValueTask<object> QuotedString(StringLiteral stringLiteral);


        public abstract ValueTask<object> Invoke(Syntax head, Syntax[] args);


        public abstract bool HasProperty(string name);

        public abstract void Declare(string name, object value);
    }
}