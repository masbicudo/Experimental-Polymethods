namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class StringToken : Token
    {
        public StringToken(string code, int start, int length, TokenSubtype subtype)
            : base(code, start, length, subtype)
        {
        }

        public override TokenType Type
        {
            get { return TokenType.String; }
        }
    }
}