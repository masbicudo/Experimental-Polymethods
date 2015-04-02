using System.Collections.Generic;
using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("block ({Statements.Count} statements)")]
    public class BlockStatementNode : IStatementNode
    {
        public List<IStatementNode> Statements { get; private set; }

        public BlockStatementNode(List<IStatementNode> statements)
        {
            this.Statements = statements;
        }
    }
}