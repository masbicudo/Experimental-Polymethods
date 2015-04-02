using System.Collections.Generic;
using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("split ({Alternatives.Count} alternatives)")]
    public class SplitStatementNode : IStatementNode
    {
        public List<IStatementNode> Alternatives { get; private set; }

        public SplitStatementNode(List<IStatementNode> alternatives)
        {
            this.Alternatives = alternatives;
        }
    }
}