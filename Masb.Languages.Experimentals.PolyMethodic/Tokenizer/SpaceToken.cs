namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class SpaceToken : Token
    {
        public SpaceToken(string code, int start, int length, TokenSubtype subtype)
            : base(code, start, length, subtype)
        {
        }

        public override TokenType Type
        {
            get { return TokenType.Space; }
        }
    }
}