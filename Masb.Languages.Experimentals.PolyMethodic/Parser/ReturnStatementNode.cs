using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("return {Expression}")]
    public class ReturnStatementNode : IStatementNode
    {
        public IExpressionNode Expression { get; private set; }

        public ReturnStatementNode(IExpressionNode expression)
        {
            this.Expression = expression;
        }
    }
}