using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("^")]
    public class StartFileToken : Token
    {
        public StartFileToken(string code)
            : base(code, 0, 0, TokenSubtype.StartFile)
        {
        }

        public override TokenType Type
        {
            get { return TokenType.StartFile; }
        }
    }
}