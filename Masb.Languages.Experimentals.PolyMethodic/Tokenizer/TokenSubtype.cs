using System.Collections.Generic;
using System.Linq;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public enum TokenSubtype
    {
        Error,

        /// <summary>
        /// Represents a floating point number, either by using a dot, or exponential notation.
        /// (e.g. 2.0, 2.0m, 2e5, .2 3e-2, 3.2e+5f)
        /// </summary>
        FloatNumber,

        /// <summary>
        /// Represents a hex number.
        /// Only integers accept this representation.
        /// </summary>
        HexNumber,

        /// <summary>
        /// Represents a number that is an integer in the source representation,
        /// but could be a floating point in the type system. (e.g. 1f, 22m, 0d, 13, 01)
        /// </summary>
        IntegerNumber,

        NullConditionMemberSymbol,
        NullConditionIndexerSymbol,
        NullCoalescingSymbol,
        IncrementSymbol,
        DecrementSymbol,
        PointsToSymbol,
        LessThanSymbol,
        LessThanOrEqualSymbol,
        GreaterThanSymbol,
        GreaterThanOrEqualSymbol,
        EqualSymbol,
        DifferentSymbol,
        LogicOrSymbol,
        LogicAndSymbol,
        GoesToSymbol,

        PlusSymbol,
        SubtractSymbol,
        MultiplySymbol,
        DivideSymbol,
        ModuloSymbol,
        OrSymbol,
        AndSymbol,
        //LeftShiftSymbol,
        //RightShiftSymbol,

        DoubleDotSeparatorSymbol,
        StatementSeparatorSymbol,
        ListSeparatorSymbol,

        ConditionSymbol,
        AssignSymbol,
        AddAssignSymbol,
        SubtractAssignSymbol,
        MultiplyAssignSymbol,
        DivideAssignSymbol,
        ModuloAssignSymbol,
        OrAssignSymbol,
        AndAssignSymbol,
        LeftShiftAssignSymbol,
        RightShiftAssignSymbol,

        OpenBracesSymbol,
        CloseBracesSymbol,
        OpenBracketsSymbol,
        CloseBracketsSymbol,
        OpenParenthesysSymbol,
        CloseParenthesysSymbol,

        StartFile,
        EndFile,

        SingleLineCommentSpace,
        MultiLineCommentSpace,
        SpaceChars,

        IdentifierOrContextualKeyword,

        ClassKeyword,
        PolyKeyword,
        IntKeyword,
        SplitKeyword,
        BranchKeyword,
        ReturnKeyword,

        PublicKeyword,
        PrivateKeyword,
        ProtectedKeyword,
        InternalKeyword,
        StaticKeyword,
    }

    public static class TokenSubtypeHelper
    {
        private static readonly Dictionary<TokenSubtype, string> dicSymbolTypesToText
            = new Dictionary<TokenSubtype, string>
                {
                    { TokenSubtype.NullConditionMemberSymbol, "?." },
                    { TokenSubtype.NullConditionIndexerSymbol, "?[" },
                    { TokenSubtype.NullCoalescingSymbol, "??" },
                    { TokenSubtype.IncrementSymbol, "++" },
                    { TokenSubtype.DecrementSymbol, "--" },
                    { TokenSubtype.PointsToSymbol, "->" },
                    { TokenSubtype.LessThanSymbol, "<" },
                    { TokenSubtype.LessThanOrEqualSymbol, "<=" },
                    { TokenSubtype.GreaterThanSymbol, ">" },
                    { TokenSubtype.GreaterThanOrEqualSymbol, ">=" },
                    { TokenSubtype.EqualSymbol, "==" },
                    { TokenSubtype.DifferentSymbol, "!=" },
                    { TokenSubtype.LogicOrSymbol, "||" },
                    { TokenSubtype.LogicAndSymbol, "&&" },
                    { TokenSubtype.GoesToSymbol, "=>" },
                    { TokenSubtype.PlusSymbol, "+" },
                    { TokenSubtype.SubtractSymbol, "-" },
                    { TokenSubtype.MultiplySymbol, "*" },
                    { TokenSubtype.DivideSymbol, "/" },
                    { TokenSubtype.ModuloSymbol, "%" },
                    { TokenSubtype.OrSymbol, "|" },
                    { TokenSubtype.AndSymbol, "&" },
                    //{ TokenSubtype.LeftShiftSymbol, "<<" },
                    //{ TokenSubtype.RightShiftSymbol, ">>" },
                    { TokenSubtype.DoubleDotSeparatorSymbol, ":" },
                    { TokenSubtype.StatementSeparatorSymbol, ";" },
                    { TokenSubtype.ListSeparatorSymbol, "," },
                    { TokenSubtype.ConditionSymbol, "?" },
                    { TokenSubtype.AssignSymbol, "=" },
                    { TokenSubtype.AddAssignSymbol, "+=" },
                    { TokenSubtype.SubtractAssignSymbol, "-=" },
                    { TokenSubtype.MultiplyAssignSymbol, "*=" },
                    { TokenSubtype.DivideAssignSymbol, "/=" },
                    { TokenSubtype.ModuloAssignSymbol, "%=" },
                    { TokenSubtype.OrAssignSymbol, "|=" },
                    { TokenSubtype.AndAssignSymbol, "&=" },
                    { TokenSubtype.LeftShiftAssignSymbol, "<<=" },
                    { TokenSubtype.RightShiftAssignSymbol, ">>=" },
                    { TokenSubtype.OpenBracesSymbol, "{" },
                    { TokenSubtype.CloseBracesSymbol, "}" },
                    { TokenSubtype.OpenBracketsSymbol, "[" },
                    { TokenSubtype.CloseBracketsSymbol, "]" },
                    { TokenSubtype.OpenParenthesysSymbol, "(" },
                    { TokenSubtype.CloseParenthesysSymbol, ")" },
                    { TokenSubtype.ClassKeyword, "class" },
                    { TokenSubtype.PolyKeyword, "poly" },
                    { TokenSubtype.IntKeyword, "int" },
                    { TokenSubtype.SplitKeyword, "split" },
                    { TokenSubtype.BranchKeyword, "branch" },
                    { TokenSubtype.ReturnKeyword, "return" },
                    { TokenSubtype.PublicKeyword, "public" },
                    { TokenSubtype.PrivateKeyword, "private" },
                    { TokenSubtype.ProtectedKeyword, "protected" },
                    { TokenSubtype.InternalKeyword, "internal" },
                    { TokenSubtype.StaticKeyword, "static" },
                };

        private static readonly Dictionary<string, TokenSubtype> dicTextToSymbolTypes
            = dicSymbolTypesToText.ToDictionary(x => x.Value, x => x.Key);

        public static string GetTokenText(this TokenSubtype subtype)
        {
            string text;
            dicSymbolTypesToText.TryGetValue(subtype, out text);
            return text;
        }

        public static TokenSubtype? FromTokenText(string tokenText)
        {
            TokenSubtype subtype;
            if (dicTextToSymbolTypes.TryGetValue(tokenText, out subtype))
                return subtype;

            return null;
        }
    }
}