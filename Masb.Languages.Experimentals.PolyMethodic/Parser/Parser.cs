using System.Collections.Generic;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public static class Parser
    {
        public static FileNode Parse(Token[] tokens)
        {
            var pos = 0;

            // skipping a single start-file token
            if (tokens[pos++].Type != TokenType.StartFile)
                return null;

            SkipSpaces(tokens, ref pos);

            // reading file members
            IMemberNode member;
            var members = new List<IMemberNode>();
            while ((member = ReadMember(tokens, ref pos)) != null)
                members.Add(member);

            return new FileNode(members);
        }

        private static ClassNode ReadClassNode(Token[] tokens, ref int pos)
        {
            int newPos = pos;
            MethodFlags flags = 0;

            // method visibility
            switch (tokens[newPos].Subtype)
            {
                case TokenSubtype.PublicKeyword:
                    flags |= MethodFlags.IsPublic;
                    newPos++;
                    break;

                case TokenSubtype.PrivateKeyword:
                    flags |= MethodFlags.IsPrivate;
                    newPos++;
                    break;

                default:
                    if (tokens[newPos].Subtype == TokenSubtype.ProtectedKeyword)
                    {
                        flags |= MethodFlags.IsProtected;
                        newPos++;
                    }

                    SkipSpaces(tokens, ref newPos);
                    if (tokens[newPos].Subtype == TokenSubtype.InternalKeyword)
                    {
                        flags |= MethodFlags.IsInternal;
                        newPos++;
                    }

                    break;
            }

            SkipSpaces(tokens, ref newPos);
            if (tokens[newPos].Subtype == TokenSubtype.ClassKeyword)
            {
                newPos++;
                if (tokens[newPos].Type == TokenType.Space)
                {
                    newPos++;
                    SkipSpaces(tokens, ref newPos);
                    if (tokens[newPos].Subtype == TokenSubtype.IdentifierOrContextualKeyword)
                    {
                        var className = tokens[newPos++];

                        SkipSpaces(tokens, ref newPos);
                        if (tokens[newPos].Subtype == TokenSubtype.OpenBracesSymbol)
                        {
                            newPos++;
                            IMemberNode member;
                            var members = new List<IMemberNode>();
                            while ((member = ReadMember(tokens, ref newPos)) != null)
                                members.Add(member);

                            SkipSpaces(tokens, ref newPos);
                            if (tokens[newPos].Subtype == TokenSubtype.CloseBracesSymbol)
                            {
                                newPos++;
                                pos = newPos;
                                return new ClassNode(className, members, flags);
                            }
                        }
                        else
                        {
                            // error branch
                            return null;
                        }
                    }
                }
            }

            return null;
        }

        private static IMemberNode ReadMember(Token[] tokens, ref int pos)
        {
            SkipSpaces(tokens, ref pos);
            return ReadClassNode(tokens, ref pos)
                   ?? ReadMethodNode(tokens, ref pos) as IMemberNode;
        }

        private static MethodNode ReadMethodNode(Token[] tokens, ref int pos)
        {
            var newPos = pos;
            MethodFlags flags = 0;

            // method visibility
            switch (tokens[newPos].Subtype)
            {
                case TokenSubtype.PublicKeyword:
                    flags |= MethodFlags.IsPublic;
                    newPos++;
                    break;

                case TokenSubtype.PrivateKeyword:
                    flags |= MethodFlags.IsPrivate;
                    newPos++;
                    break;

                default:
                    if (tokens[newPos].Subtype == TokenSubtype.ProtectedKeyword)
                    {
                        flags |= MethodFlags.IsProtected;
                        newPos++;
                    }

                    SkipSpaces(tokens, ref newPos);
                    if (tokens[newPos].Subtype == TokenSubtype.InternalKeyword)
                    {
                        flags |= MethodFlags.IsInternal;
                        newPos++;
                    }

                    break;
            }

            // method instance/static
            SkipSpaces(tokens, ref newPos);
            if (tokens[newPos].Subtype == TokenSubtype.StaticKeyword)
            {
                flags |= MethodFlags.IsStatic;
                newPos++;
            }

            IdentifierOrKeywordToken type;
            {
                SkipSpaces(tokens, ref newPos);
                if (tokens[newPos].Subtype == TokenSubtype.PolyKeyword)
                {
                    flags |= MethodFlags.IsPolyMethod;
                    newPos++;
                }

                SkipSpaces(tokens, ref newPos);

                // reading type (an identifier or keyword)
                if (tokens[newPos].Type == TokenType.IdentifierOrKeyword)
                    type = (IdentifierOrKeywordToken)tokens[newPos++];
                else
                    return null;
            }

            SkipSpaces(tokens, ref newPos);

            // reading name (an identifier or keyword)
            IdentifierOrKeywordToken name;
            if (tokens[newPos].Type == TokenType.IdentifierOrKeyword)
                name = (IdentifierOrKeywordToken)tokens[newPos++];
            else
                return null;

            SkipSpaces(tokens, ref newPos);

            // reading args list
            if (tokens[newPos].Subtype == TokenSubtype.OpenParenthesysSymbol)
            {
                newPos++;
                if (tokens[newPos].Subtype == TokenSubtype.CloseParenthesysSymbol)
                {
                    newPos++;
                    SkipSpaces(tokens, ref newPos);
                    if (tokens[newPos].Subtype == TokenSubtype.OpenBracesSymbol)
                    {
                        newPos++;

                        // reading statements in the method body
                        IStatementNode statement;
                        var statements = new List<IStatementNode>();
                        while ((statement = ReadStatement(tokens, ref newPos)) != null)
                            statements.Add(statement);

                        SkipSpaces(tokens, ref newPos);
                        if (tokens[newPos].Subtype == TokenSubtype.CloseBracesSymbol)
                        {
                            newPos++;
                            pos = newPos;
                            return new MethodNode(name, type, statements, flags);
                        }
                    }
                    else
                    {
                        // error branch: missing braces
                    }
                }
            }
            else
            {
                // error branch: missing parenthesys
            }

            return null;
        }

        private static void SkipSpaces(Token[] tokens, ref int pos)
        {
            // skipping spaces
            while (tokens[pos].Type == TokenType.Space)
                pos++;
        }

        private static IStatementNode ReadStatement(Token[] tokens, ref int pos)
        {
            SkipSpaces(tokens, ref pos);
            return ReadReturn(tokens, ref pos)
                   ?? ReadSplit(tokens, ref pos)
                   ?? ReadBlock(tokens, ref pos) as IStatementNode;
        }

        private static SplitStatementNode ReadSplit(Token[] tokens, ref int pos)
        {
            if (tokens[pos].Subtype == TokenSubtype.SplitKeyword)
            {
                pos++;
                var alternatives = new List<IStatementNode>();
                while (true)
                {
                    SkipSpaces(tokens, ref pos);

                    if (tokens[pos].Subtype == TokenSubtype.BranchKeyword)
                        pos++;
                    else
                        break;

                    var statement = ReadStatement(tokens, ref pos);

                    if (statement == null)
                    {
                        // error
                        return null;
                    }

                    alternatives.Add(statement);
                }

                if (alternatives.Count == 0)
                {
                    // error
                    return null;
                }

                return new SplitStatementNode(alternatives);
            }

            return null;
        }

        private static ReturnStatementNode ReadReturn(Token[] tokens, ref int pos)
        {
            if (tokens[pos].Subtype == TokenSubtype.ReturnKeyword)
            {
                pos++;
                SkipSpaces(tokens, ref pos);
                var expression = ReadExpression(tokens, ref pos);

                if (expression == null)
                {
                    // error
                    return null;
                }

                if (tokens[pos].Subtype == TokenSubtype.StatementSeparatorSymbol)
                    pos++;
                else
                {
                    // error
                    return null;
                }

                return new ReturnStatementNode(expression);
            }

            return null;
        }

        private static BlockStatementNode ReadBlock(Token[] tokens, ref int pos)
        {
            if (tokens[pos].Subtype == TokenSubtype.OpenBracesSymbol)
            {
                pos++;

                // reading statements in the method body
                IStatementNode statement;
                var statements = new List<IStatementNode>();
                while ((statement = ReadStatement(tokens, ref pos)) != null)
                    statements.Add(statement);

                if (tokens[pos++].Subtype == TokenSubtype.CloseBracesSymbol)
                    return new BlockStatementNode(statements);
            }
            else
            {
                // error branch: missing braces
            }

            return null;
        }

        private static IExpressionNode ReadExpression(Token[] tokens, ref int pos)
        {
            SkipSpaces(tokens, ref pos);
            return ReadLiteral(tokens, ref pos) as IExpressionNode;
        }

        private static LiteralExpressionNode ReadLiteral(Token[] tokens, ref int pos)
        {
            if (tokens[pos].Type == TokenType.String || tokens[pos].Type == TokenType.Number)
                return new LiteralExpressionNode(tokens[pos++]);

            return null;
        }
    }
}