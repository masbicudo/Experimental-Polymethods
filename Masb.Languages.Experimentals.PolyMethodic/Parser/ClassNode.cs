using System.Collections.Generic;
using System.Diagnostics;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [DebuggerDisplay("class {Name} ({Flags}, {Members.Count} members)")]
    public class ClassNode :
        IMemberNode
    {
        public Token Name { get; private set; }

        public List<IMemberNode> Members { get; private set; }

        public MethodFlags Flags { get; private set; }

        public ClassNode(Token name, List<IMemberNode> members, MethodFlags flags)
        {
            this.Name = name;
            this.Members = members;
            this.Flags = flags;
        }
    }
}