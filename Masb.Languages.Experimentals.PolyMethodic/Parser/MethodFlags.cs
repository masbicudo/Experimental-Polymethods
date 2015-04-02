using System;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    [Flags]
    public enum MethodFlags
    {
        IsPolyMethod = 1,
        IsAbstract = 2,
        IsVirtual = 4,
        IsImplementer = 8,
        IsAsync = 0x10,
        IsStatic = 0x20,
        IsPrivate = 0x40,
        IsPublic = 0x80,
        IsProtected = 0x100,
        IsInternal = 0x200,
    }
}