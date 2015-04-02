using System.Collections.Generic;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class FileNode
    {
        public List<IMemberNode> Members { get; private set; }

        public FileNode(List<IMemberNode> members)
        {
            this.Members = members;
        }
    }
}