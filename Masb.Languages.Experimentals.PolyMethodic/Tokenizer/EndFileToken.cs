using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("$")]
    public class EndFileToken : Token
    {
        public EndFileToken(string code)
            : base(code, code.Length, 0, TokenSubtype.EndFile)
        {
        }

        public override TokenType Type
        {
            get { return TokenType.EndFile; }
        }
    }
}