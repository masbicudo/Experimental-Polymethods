using System.Collections.Generic;
using System.Globalization;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class Tokenizer
    {
        private static readonly string CrLf = "\r\n";
        private static readonly string Separators = "\f\n\r\t\v\x85";
        private static readonly string[] TripleCharSymbols = { "<<=", ">>=" };
        private static readonly string[] TwinCharSymbols = { ".?", "++", "--", "->", "||", "&&", ">=", "<=", "!=", "==", "??", "=>", "<<", ">>", "+=", "-=", "*=", "/=", "|=", "&=", "%=", "^=" };
        private static readonly string SingleCharSymbols = ":+-*/|&{}()[]<>,;?=!~%^.";
        private static readonly string NumberPostfixesWithDot = "mfdMFD";
        private static readonly string NumberPostfixesWithoutDot = "ulmfdULMFD";
        private static readonly string NumberPostfixesHex = "ulUL";
        private static readonly string HexDigits = "0123456789abcdefABCDEF";

        public static IEnumerable<Token> Read(string code)
        {
            var pos = 0;

            if (code != null)
            {
                yield return new StartFileToken(code);

                while (true)
                {
                    var oldPos = pos;
                    var token = ReadSpace(code, ref pos) as Token
                                ?? ReadIdentifierOrKeyword(code, ref pos) as Token
                                ?? ReadSymbol(code, ref pos) as Token
                                ?? ReadNumber(code, ref pos) as Token
                                ?? ReadEndFile(code, ref pos) as Token;

                    if (token == null)
                        break;

                    yield return token;

                    if (pos == oldPos)
                        break;
                }
            }
        }

        private static EndFileToken ReadEndFile(string code, ref int pos)
        {
            return pos == code.Length ? new EndFileToken(code) : null;
        }

        private static SymbolToken ReadSymbol(string code, ref int pos)
        {
            if (pos + 1 < code.Length)
                for (int it = 0; it < TwinCharSymbols.Length; it++)
                    if (TwinCharSymbols[it][0] == code[pos] && TwinCharSymbols[it][1] == code[pos + 1])
                    {
                        var subtype = TokenSubtypeHelper.FromTokenText(code.Substring(pos, 2));
                        pos += 2;
                        return new SymbolToken(code, pos - 2, 2, subtype ?? TokenSubtype.Error);
                    }

            if (pos < code.Length)
                if (SingleCharSymbols.IndexOf(code[pos]) >= 0)
                {
                    var subtype = TokenSubtypeHelper.FromTokenText(code.Substring(pos, 1));
                    return new SymbolToken(code, pos++, 1, subtype ?? TokenSubtype.Error);
                }

            return null;
        }

        private static IdentifierOrKeywordToken ReadIdentifierOrKeyword(string code, ref int pos)
        {
            var oldPos = pos;
            if (pos < code.Length && (char.IsLetter(code[pos]) || code[pos] == '_'))
            {
                pos++;

                while (pos < code.Length && (char.IsLetterOrDigit(code[pos]) || code[pos] == '_'))
                    pos++;
            }

            var subtype = TokenSubtypeHelper.FromTokenText(code.Substring(oldPos, pos - oldPos))
                          ?? TokenSubtype.IdentifierOrContextualKeyword;

            return pos == oldPos ? null : new IdentifierOrKeywordToken(code, oldPos, pos - oldPos, subtype);
        }

        private static NumberToken ReadNumber(string code, ref int pos)
        {
            var oldPos = pos;

            bool isHex = false;
            if (pos + 2 < code.Length && (code[pos + 1] == 'x' || code[pos + 1] == 'X') && code[pos] == '0')
                if (HexDigits.IndexOf(code[pos + 2]) >= 0)
                {
                    isHex = true;
                    pos += 3;
                }

            while (pos < code.Length && char.IsDigit(code[pos]))
                pos++;

            bool hasDot = false;
            if (!isHex && pos + 1 < code.Length && code[pos] == '.')
            {
                if (char.IsDigit(code[pos + 1]))
                {
                    hasDot = true;
                    pos += 2;
                    while (pos < code.Length && char.IsDigit(code[pos]))
                        pos++;
                }
            }

            bool isExpNotation = false;
            if (pos + 1 < code.Length && (code[pos] == 'e' || code[pos] == 'E'))
            {
                if (pos + 2 < code.Length && (code[pos + 1] == '+' || code[pos + 1] == '-'))
                {
                    if (char.IsDigit(code[pos + 2]))
                    {
                        isExpNotation = true;
                        pos += 3;
                    }
                }
                else if (char.IsDigit(code[pos + 1]))
                {
                    isExpNotation = true;
                    pos += 2;
                }

                while (pos < code.Length && char.IsDigit(code[pos]))
                    pos++;
            }

            if (hasDot || isExpNotation)
            {
                if (pos < code.Length && NumberPostfixesWithDot.IndexOf(code[pos]) >= 0)
                    pos++;

                return new NumberToken(code, oldPos, pos - oldPos, TokenSubtype.FloatNumber);
            }

            if (isHex)
            {
                if (pos < code.Length && NumberPostfixesHex.IndexOf(code[pos]) >= 0)
                    pos++;

                return new NumberToken(code, oldPos, pos - oldPos, TokenSubtype.HexNumber);
            }

            // ends with "ul", "Ul", "uL", "UL", "lu", "Lu", "lU", "LU"
            if (pos + 1 < code.Length && (
                ((code[pos] == 'U' || code[pos] == 'u') && (code[pos + 1] == 'L' || code[pos + 1] == 'l')) ||
                ((code[pos] == 'L' || code[pos] == 'l') && (code[pos + 1] == 'U' || code[pos + 1] == 'u'))))
            {
                pos += 2;
            }
            else if (pos < code.Length && NumberPostfixesWithoutDot.IndexOf(code[pos]) >= 0)
            {
                pos++;
            }

            if (pos > oldPos)
                return new NumberToken(code, oldPos, pos - oldPos, TokenSubtype.IntegerNumber);

            return null;
        }

        private static SpaceToken ReadSpace(string code, ref int pos)
        {
            var oldPos = pos;
            if (pos + 1 < code.Length && code[pos] == '/')
            {
                if (code[pos + 1] == '/')
                {
                    pos += 2;
                    while (pos < code.Length && CrLf.IndexOf(code[pos]) < 0)
                        pos++;

                    return new SpaceToken(code, oldPos, pos - oldPos, TokenSubtype.SingleLineCommentSpace);
                }

                if (code[pos + 1] == '*')
                {
                    pos += 2;
                    while (pos + 1 < code.Length && code[pos] != '*' && code[pos + 1] != '/')
                        pos++;

                    pos += 2;

                    return pos == oldPos ? null : new SpaceToken(code, oldPos, pos - oldPos, TokenSubtype.MultiLineCommentSpace);
                }
            }
            else
            {
                while (pos < code.Length &&
                       (Separators.IndexOf(code[pos]) >= 0 || char.GetUnicodeCategory(code[pos]) == UnicodeCategory.SpaceSeparator))
                    pos++;

                return pos == oldPos ? null : new SpaceToken(code, oldPos, pos - oldPos, TokenSubtype.SpaceChars);
            }

            return null;
        }
    }
}