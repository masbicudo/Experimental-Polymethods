using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("{ToString()}")]
    public abstract class Token
    {
        private readonly string code;
        private readonly int start;
        private readonly int length;
        private readonly TokenSubtype subtype;

        protected Token(string code, int start, int length, TokenSubtype subtype)
        {
            this.code = code;
            this.start = start;
            this.length = length;
            this.subtype = subtype;
        }

        public abstract TokenType Type { get; }

        public TokenSubtype Subtype
        {
            get { return subtype; }
        }

        public override string ToString()
        {
            return this.code.Substring(this.start, this.length);
        }
    }
}