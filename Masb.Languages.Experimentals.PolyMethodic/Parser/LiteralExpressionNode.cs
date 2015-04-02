using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("literal {Token}")]
    public class LiteralExpressionNode : IExpressionNode
    {
        public Token Token { get; private set; }

        public LiteralExpressionNode(Token token)
        {
            this.Token = token;
        }

        public override string ToString()
        {
            return this.Token.ToString();
        }
    }
}