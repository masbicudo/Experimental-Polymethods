using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public class Lang
    {
        public static Token[] Tokenize(string code)
        {
            var tokens = Tokenizer.Read(code);
            return tokens.ToArray();
        }

        public static FileNode Parse(Token[] tokens)
        {
            return Parser.Parse(tokens);
        }

        public static ModuleScope Link(FileNode[] files, ModuleScope[] otherModules)
        {
            var rootScope = ImmutableDictionary<string, ImmutableArray<INamespaceOrTypeSymbol>>.Empty;
            foreach (var module in otherModules)
            {
                rootScope.AddRange(module.Items);
            }


            var typeSymbols = new Dictionary<string, TypeSymbol>();
            foreach (var file in files)
            {
                foreach (var @class in file.Members.OfType<ClassNode>())
                {
                    var typeName = @class.Name.ToString();
                    var methodSymbols = @class.Members.OfType<MethodNode>()
                        .Select(mn => new MethodSymbol(mn.Name.ToString(), CompileStatements(mn.Statements)))
                        .ToArray();

                    typeSymbols.Add(typeName, new TypeSymbol(typeName, methodSymbols));
                }
            }

            return new ModuleScope(typeSymbols);
        }

        public static Compilation Analyse(Compilation compilation, Analyser[] analysers)
        {
            throw new NotImplementedException();
        }

        public static Compilation Optimize(Compilation compilation, Optimizer[] optimizers)
        {
            throw new NotImplementedException();
        }

        public static Func<IEnumerable<T>> Convert<T>(MethodSymbol method, CompilationConverter<T> executor)
        {
            throw new NotImplementedException();
        }

        public static Compilation Compile(ModuleScope module)
        {
            throw new NotImplementedException();
        }
    }

    public class Instruction
    {
    }

    public enum OpCode
    {
        /// <summary>
        /// RET
        /// <para>
        /// Gets the return address from the memory-frame and sets the execution pointer with that.
        /// After that it discards the memory of the return address, making it invalid.
        /// </para>
        /// </summary>
        Return,

        /// <summary>
        /// SPLT ADDR
        /// <para>
        /// Splits execution in a poly-method, making a copy of the memory-frame,
        /// and continues execution in that copied frame, at the next instruction.
        /// The original memory-frame receives a new value containing the next execution pointer,
        /// that is an argument of the split operation.
        /// </para>
        /// </summary>
        Split,

        /// <summary>
        /// PUSH TYPE VAL<br/>
        /// PUSH BYTES<br/>
        /// PUSH WORDS<br/>
        /// PUSH DWORDS<br/>
        /// <para>
        /// Adds a new value to the memory-frame stack.
        /// </para>
        /// </summary>
        Push,

        /// <summary>
        /// CALLP ADDR
        /// <para>
        /// Calls a poly-method:
        ///  - adds a poly-counter to the parent memory-frame
        /// </para>
        /// </summary>
        CallPoly,
    }

    public class TypeSymbol : MemberSymbol,
        INamespaceOrTypeSymbol
    {
        public MemberSymbol[] Members { get; private set; }

        public TypeSymbol(string name, MemberSymbol[] members)
            : base(name)
        {
            this.Members = members;
        }

        public MemberSymbol FindMember(string name)
        {
            var result = this.Members.SingleOrDefault(x => x.Name == name);
            return result;
        }
    }

    public class Compilation
    {
        public Dictionary<string, TypeSymbol> Types { get; private set; }

        public Compilation(Dictionary<string, TypeSymbol> types)
        {
            this.Types = types;
        }

        public TypeSymbol FindType(string name)
        {
            TypeSymbol value;
            this.Types.TryGetValue(name, out value);
            return value;
        }

        public CodeMap GetCodeMap()
        {
            throw new NotImplementedException();
        }

        public void Save(string fileName)
        {
            throw new NotImplementedException();
        }
    }

    public class ClassSymbol : TypeSymbol
    {
        public ClassSymbol(string name, MemberSymbol[] members)
            : base(name, members)
        {
        }
    }

    public class NamespaceScope :
        INamespaceOrTypeSymbol
    {
        public string Name { get; private set; }

        public ImmutableDictionary<string, INamespaceOrTypeSymbol> Items { get; private set; }

        public NamespaceScope(string name, IEnumerable<INamespaceOrTypeSymbol> items)
        {
            this.Name = name;
            this.Items = items.ToImmutableDictionary(nst => nst.Name);
        }
    }

    public class MethodSymbol : MemberSymbol
    {
        protected MethodSymbol(string name)
            : base(name)
        {
        }
    }

    public class MemberSymbol
    {
        public string Name { get; private set; }

        protected MemberSymbol(string name)
        {
            this.Name = name;
        }
    }

    public class Executor<T> : CompilationConverter<T>
    {
    }

    public class CompilationConverter<T>
    {
    }

    public class ModuleScope
    {
        public string Name { get; private set; }

        public ImmutableDictionary<string, INamespaceOrTypeSymbol> Items { get; private set; }

        public ModuleScope(string name, IEnumerable<INamespaceOrTypeSymbol> items)
        {
            this.Name = name;
            this.Items = items.ToImmutableDictionary(nst => nst.Name);
        }

        public Documentation GetDocumentation()
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Module scope representing the `System` module.
    /// This is a compiled module, meaning that it does not have the source code,
    /// only compiled IL code.
    /// </summary>
    public class SystemModule : ModuleScope
    {
        public SystemModule()
            : base("System", GetScopeItems())
        {
        }

        private static NamespaceScope[] GetScopeItems()
        {
            var ns = new NamespaceScope("System", new[]
                {
                    new TypeSymbol("Int32", new MemberSymbol[0]),
                    new TypeSymbol("Int64", new MemberSymbol[0]),
                    new TypeSymbol("String", new MemberSymbol[0]),
                });

            return new[] { ns };
        }
    }

    public class Analyser
    {
    }

    public class Optimizer
    {
    }
}
