namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class SymbolToken : Token
    {
        public SymbolToken(string code, int start, int length, TokenSubtype subtype)
            : base(code, start, length, subtype)
        {
        }

        public override TokenType Type
        {
            get { return TokenType.Symbol; }
        }
    }
}