using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;

namespace Masb.Languages.Experimentals.PolyMethodic
{
    public struct SintaticSemantic
    {
        public readonly IMemberNode SintaticNode;
        public readonly MemberDefinition SemanticNode;

        public SintaticSemantic(IMemberNode sintaticNode, MemberDefinition semanticNode)
        {
            this.SintaticNode = sintaticNode;
            this.SemanticNode = semanticNode;
        }
    }

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


        public static DefinitionResult<ModuleDefinition> Define(
            string moduleName,
            FileNode[] files,
            ModuleImportSymbol[] imports)
        {
            var allDefinitions = new Dictionary<string, INamespaceItemDefinition>();

            var rootNamesMap = ImmutableDictionary<string, MemberDefinition>.Empty;
            var sintaticSemanticPairs = ImmutableArray<Tuple<object, object>>.Empty;
            foreach (var file in files)
            {
                var def = ModuleDefinition(file);

                // inserting into the namespace map
                foreach (var definition in def.Definition.Definitions)
                {
                    var memberDefinition = definition as INamespaceItemDefinition;
                    if (namespaceItem == null)
                    {
                        INamespaceItemDefinition prevDef;
                        InvalidNamespaceItemDefinition invalidDef = null;
                        if (allDefinitions.TryGetValue(definition.Name, out prevDef))
                            invalidDef = prevDef as InvalidNamespaceItemDefinition;

                        if (invalidDef != null)
                        {
                            var errorDefs = invalidDef.Definitions;
                            errorDefs = errorDefs.Add(definition);
                            invalidDef = new InvalidNamespaceItemDefinition(definition.Name, errorDefs);
                        }
                        else
                        {
                            var errorDefs = ImmutableArray<MemberDefinition>.Empty;
                            if (prevDef != null)
                                errorDefs = errorDefs.Add(new DuplicateNamespaceMemberDefinition(prevDef.Name, prevDef));
                            errorDefs = errorDefs.Add(definition);
                            invalidDef = new InvalidNamespaceItemDefinition(definition.Name, errorDefs);
                        }

                        allDefinitions[invalidDef.Name] = invalidDef;
                        namespaceItem = invalidDef;
                    }
                }

                // inserting into the sintatic-semantic map
                var link = Tuple.Create(file as object, def.Definition as object);
                sintaticSemanticPairs = sintaticSemanticPairs.Add(link);
                sintaticSemanticPairs = sintaticSemanticPairs.AddRange(def.SintaticSemanticMap);
            }

            var result = new ModuleDefinition(
                moduleName,
                allDefinitions.Values.ToImmutableArray(),
                imports.ToImmutableArray());

            return DefinitionResult.Create(result, sintaticSemanticPairs);
        }

        public static DefinitionResult<MemberDefinition> NamespaceItemDefinition(ClassNode memberNode)
        {

        }


        public static DefinitionResult<PartialModuleDefinition> ModuleDefinition(FileNode fileNode)
        {
            var definitions = new Dictionary<string, INamespaceItemDefinition>();
            var sintaticSemanticMap = ImmutableArray<Tuple<object, object>>.Empty;
            foreach (var memberNode in fileNode.Members)
            {
                // linking
                var def = MemberDefinition(memberNode);

                var itemDef = def.Definition as INamespaceItemDefinition;

                INamespaceItemDefinition prevItemDef;
                InvalidNamespaceItemDefinition invalidDef = null;
                if (definitions.TryGetValue(def.Definition.Name, out prevItemDef))
                    invalidDef = prevItemDef as InvalidNamespaceItemDefinition;

                // alternativees
                //  1.  valid/invalid inside a namespace
                //  2.  can/cannot merge with previous

                //  1.  the definition is valid inside a namespace
                //      a) and there is nothing with the same name - SET VALUE
                //      b) and there is already something with the same name
                //          b0 - that is an Error - TRY ADD OR MERGE WITHIN OTHERWISE ADD ERRORS
                //          b1 - that can merge with that - MERGE
                //          b2 - that cannot merge with that - SET ERROR
                //  2.  the definition is not valid inside a namespace
                //      a) and there is nothing with the same name - SET ERROR
                //      b) and there is already something with the same name
                //          b0 - that is an Error - TRY ADD OR MERGE WITHIN OTHERWISE ADD ERRORS
                //          b1 - that can merge with that - MERGE
                //          b2 - that cannot merge with that - SET ERROR

                if (namespaceItem == null)
                {

                    if (invalidDef != null)
                    {
                        var errorDefs = invalidDef.Definitions;
                        errorDefs = errorDefs.Add(def.Definition);
                        invalidDef = new InvalidNamespaceItemDefinition(def.Definition.Name, errorDefs);
                    }
                    else
                    {
                        var errorDefs = ImmutableArray<MemberDefinition>.Empty;
                        if (prevDef != null)
                            errorDefs = errorDefs.Add(new DuplicateNamespaceMemberDefinition(prevDef.Name, prevDef));
                        errorDefs = errorDefs.Add(def.Definition);
                        invalidDef = new InvalidNamespaceItemDefinition(def.Definition.Name, errorDefs);
                    }

                    definitions[invalidDef.Name] = invalidDef;
                    namespaceItem = invalidDef;
                }
                else
                {
                    if (invalidDef != null)
                    {
                        var errorDefs = invalidDef.Definitions;
                        errorDefs = errorDefs.Add(def.Definition);
                        invalidDef = new InvalidNamespaceItemDefinition(def.Definition.Name, errorDefs);
                    }
                    else
                    {
                        var errorDefs = ImmutableArray<MemberDefinition>.Empty;
                        if (prevDef != null)
                            errorDefs = errorDefs.Add(new DuplicateNamespaceMemberDefinition(prevDef.Name, prevDef));
                        errorDefs = errorDefs.Add(def.Definition);
                        invalidDef = new InvalidNamespaceItemDefinition(def.Definition.Name, errorDefs);
                    }
                }

                // inserting into the namespace map
                definitions.Add(def.Definition.Name, namespaceItem);

                // inserting into the sintatic-semantic map
                var link = Tuple.Create(memberNode as object, namespaceItem as object);
                sintaticSemanticMap = sintaticSemanticMap.Add(link);
                sintaticSemanticMap = sintaticSemanticMap.AddRange(def.SintaticSemanticMap);
            }

            var result = new PartialModuleDefinition(definitions.Values.ToImmutableArray());
            return DefinitionResult.Create(result, sintaticSemanticMap);
        }

        public static DefinitionResult<MemberDefinition> MemberDefinition(IMemberNode memberNode)
        {
            var classNode = memberNode as ClassNode;

            var sintaticSemanticMap = ImmutableDictionary<IMemberNode, ImmutableArray<MemberDefinition>>.Empty;
            var semanticSintaticMap = ImmutableDictionary<MemberDefinition, ImmutableArray<IMemberNode>>.Empty;

            var namesMap = ImmutableDictionary<string, MemberDefinition>.Empty;

            if (classNode != null)
            {
                foreach (var childMemberNode in classNode.Members)
                {
                    // linking
                    var def = MemberDefinition(childMemberNode);

                    // inserting into the namespace map
                    namesMap = namesMap.Add(def.Definition.Name, def.Definition);

                    // inserting into the sintatic-semantic map
                    ImmutableArray<MemberDefinition> array0;
                    sintaticSemanticMap.TryGetValue(childMemberNode, out array0);
                    array0 = array0.Add(def.Definition);
                    sintaticSemanticMap = sintaticSemanticMap.SetItem(childMemberNode, array0);

                    // inserting into the semantic-sintatic map
                    ImmutableArray<IMemberNode> array;
                    semanticSintaticMap.TryGetValue(def.Definition, out array);
                    array = array.Add(childMemberNode);
                    semanticSintaticMap = semanticSintaticMap.SetItem(def.Definition, array);
                }

                var result = new TypeDefinition(
                    classNode.Name.ToString(),
                    namesMap);

                return DefinitionResult.Create(result, sintaticSemanticMap);
            }

            var methodNode = memberNode as MethodNode;

            if (methodNode != null)
            {
                foreach (var statement in methodNode.Statements)
                {
                    var varNode = statement as VariableDeclarationStatementNode;
                    if (varNode != null)
                    {

                    }
                }

                var result = new MethodDefinition(
                    methodNode.Name.ToString());

                return DefinitionResult.Create(result, sintaticSemanticMap);
            }

            return DefinitionResult.Empty<MemberDefinition>();
        }

        public static Compilation Analyse(Compilation compilation, Analyser[] analysers)
        {
            throw new NotImplementedException();
        }

        public static Compilation Optimize(Compilation compilation, Optimizer[] optimizers)
        {
            throw new NotImplementedException();
        }

        public static Func<IEnumerable<T>> Convert<T>(MethodDefinition method, CompilationConverter<T> executor)
        {
            throw new NotImplementedException();
        }

        public static Compilation Compile(ModuleDefinition module)
        {
            throw new NotImplementedException();
        }
    }

    public struct Tuple<T1, T2>
    {
        public readonly T1 Value1;

        public readonly T2 Value2;

        public Tuple(T1 value1, T2 value2)
        {
            this.Value1 = value1;
            this.Value2 = value2;
        }
    }

    public static class Tuple
    {
        public static Tuple<T1, T2> Create<T1, T2>(T1 value1, T2 value2)
        {
            return new Tuple<T1, T2>(value1, value2);
        }
    }

    public struct DefinitionResult<TDefinition>
    {
        public readonly TDefinition Definition;

        public readonly ImmutableArray<Tuple<object, object>> SintaticSemanticMap;

        public DefinitionResult(
            TDefinition definition,
            ImmutableArray<Tuple<object, object>> sintaticSemanticMap)
        {
            this.Definition = definition;
            this.SintaticSemanticMap = sintaticSemanticMap;
        }
    }

    public static class DefinitionResult
    {
        public static DefinitionResult<TDefinition> Create<TDefinition>(
            TDefinition definition,
            ImmutableArray<Tuple<object, object>> sintaticSemanticMap)
        {
            return new DefinitionResult<TDefinition>(definition, sintaticSemanticMap);
        }

        public static DefinitionResult<T> Empty<T>()
        {
            return default(DefinitionResult<T>);
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

    public class TypeDefinition : MemberDefinition,
        INamespaceItemDefinition
    {
        public ImmutableDictionary<string, MemberDefinition> Members { get; private set; }

        public TypeDefinition(string name, ImmutableDictionary<string, MemberDefinition> members)
            : base(name)
        {
            this.Members = members;
        }
    }

    public class Compilation
    {
        public Dictionary<string, TypeDefinition> Types { get; private set; }

        public Compilation(Dictionary<string, TypeDefinition> types)
        {
            this.Types = types;
        }

        public TypeDefinition FindType(string name)
        {
            TypeDefinition value;
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

    public class ClassDefinition : TypeDefinition
    {
        public ClassDefinition(string name, ImmutableDictionary<string, MemberDefinition> members)
            : base(name, members)
        {
        }
    }

    public class NamespaceDefinition :
        INamespaceItemDefinition
    {
        public string Name { get; private set; }

        public ImmutableDictionary<string, INamespaceItemDefinition> Items { get; private set; }

        public NamespaceDefinition(string name, IEnumerable<INamespaceItemDefinition> items)
        {
            if (name == null)
                throw new ArgumentNullException("name");

            this.Name = name;
            this.Items = items.ToImmutableDictionary(nst => nst.Name);
        }
    }

    public class MethodDefinition : MemberDefinition
    {
        public MethodDefinition(string name)
            : base(name)
        {
        }
    }

    public class DuplicateNamespaceMemberDefinition : MemberDefinition,
        INamespaceItemDefinition
    {
        public INamespaceItemDefinition Definition { get; private set; }

        public DuplicateNamespaceMemberDefinition(string name, INamespaceItemDefinition definition)
            : base(name)
        {
            this.Definition = definition;
        }
    }

    public class MemberDefinition
    {
        public string Name { get; private set; }

        protected MemberDefinition(string name)
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

    public class ModuleImportSymbol
    {
        public ModuleImportSymbol(string ns, ModuleDefinition module)
        {
            this.Namespace = ns.Split('.').ToImmutableArray();
            this.Module = module;
        }

        /// <summary>
        /// Gets a list of hierarchical namespace names.
        /// </summary>
        public ImmutableArray<string> Namespace { get; private set; }

        /// <summary>
        /// Gets the module containing the exported definitions.
        /// </summary>
        public ModuleDefinition Module { get; private set; }
    }

    public class ModuleDefinition
    {
        public string Name { get; private set; }

        /// <summary>
        /// Gets a list of exported symbols, excluding the symbols from imported modules.
        /// </summary>
        public ImmutableArray<INamespaceItemDefinition> Exports { get; private set; }

        /// <summary>
        /// Gets a list of imported modules.
        /// </summary>
        public ImmutableArray<ModuleImportSymbol> Imports { get; private set; }

        public ModuleDefinition(string name, ImmutableArray<INamespaceItemDefinition> exports, ImmutableArray<ModuleImportSymbol> imports)
        {
            this.Name = name;
            this.Exports = exports;
            this.Imports = imports;
        }

        public Documentation GetDocumentation()
        {
            throw new NotImplementedException();
        }
    }

    public class PartialModuleDefinition
    {
        public ImmutableArray<INamespaceItemDefinition> Definitions { get; private set; }

        public PartialModuleDefinition(ImmutableArray<INamespaceItemDefinition> definitions)
        {
            this.Definitions = definitions;
        }
    }

    public class InvalidNamespaceItemDefinition :
        INamespaceItemDefinition
    {
        public string Name { get; private set; }

        public ImmutableArray<MemberDefinition> Definitions { get; private set; }

        public InvalidNamespaceItemDefinition(string name, ImmutableArray<MemberDefinition> definitions)
        {
            this.Name = name;
            this.Definitions = definitions;
        }
    }

    /// <summary>
    /// Module scope representing the `System` module.
    /// This is a compiled module, meaning that it does not have the source code,
    /// only compiled IL code.
    /// </summary>
    public class SystemModule : ModuleDefinition
    {
        public SystemModule()
            : base("System", GetScopeItems(), Enumerable.Empty<ModuleImportSymbol>())
        {
        }

        private static NamespaceDefinition[] GetScopeItems()
        {
            var ns = new NamespaceDefinition("System", new[]
                {
                    new TypeDefinition("Int32", ImmutableDictionary<string, MemberDefinition>.Empty),
                    new TypeDefinition("Int64", ImmutableDictionary<string, MemberDefinition>.Empty),
                    new TypeDefinition("String", ImmutableDictionary<string, MemberDefinition>.Empty),
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
