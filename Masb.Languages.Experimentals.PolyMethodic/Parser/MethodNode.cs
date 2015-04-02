using System.Collections.Generic;
using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("method {Name} as {Type} ({Flags})")]
    public class MethodNode :
        IMemberNode
    {
        public IdentifierOrKeywordToken Name { get; private set; }

        public IdentifierOrKeywordToken Type { get; private set; }

        public List<IStatementNode> Statements { get; private set; }

        public MethodFlags Flags { get; private set; }

        public MethodNode(
            IdentifierOrKeywordToken name,
            IdentifierOrKeywordToken type,
            List<IStatementNode> statements,
            MethodFlags flags)
        {
            this.Name = name;
            this.Type = type;
            this.Statements = statements;
            this.Flags = flags;
        }
    }
}