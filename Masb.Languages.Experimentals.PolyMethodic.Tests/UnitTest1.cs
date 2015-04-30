using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Masb.Languages.Experimentals.PolyMethodic.Tests
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void ParseTest()
        {
            // Create a list of tokens from the code.
            var tokens = Lang.Tokenize(@"
public class Main
{
    /* comments */
    public static poly double MultipleInts()
    {
        // returns 1 and 2
        split branch
            return 1.2;
        branch
            return 2.1e+5;
    }
}
");

            // Main.MultipleInts:
            //      split _splt1
            //      push System.Double "1.2"
            //      # same as PUSH 0x3FF3333333333333
            //      ret
            //  _splt1:
            //      push System.Double "2.1e+5"
            //      # same as PUSH 0x4109A28000000000
            //      ret

            // Create an AST representing the code,
            //  but not linked yet, that is, names
            //  and scopes are still unresolved.
            var fileAst = Lang.Parse(tokens);

            // Create a model with definitions,
            //  and a map between them and their originating
            //  sintatic elements.
            var module = Lang.Define(
                "Test",
                new[] { fileAst },
                new[] { new ModuleImportSymbol("System", new SystemModule()) });

            // Creates links between
            //  definers, referrers and referred definitions,
            //  by resolving scope elements, and then
            //  linking identifiers to declarations.
            var links = Lang.Link(module);

            // Create the documentation.
            var docs = module.GetDocumentation();
            docs.Save("doc");

            // Create a compiled module from the
            //  source module.
            var compilation = Lang.Compile(module);
            compilation.Save("lib");

            // Create a map between IL and source code.
            var map = compilation.GetCodeMap();
            map.Save("map");

            // Analyses a compilation, and associates
            //  information with the model elements, given
            //  a list of code analysers.
            var analysedCompilation = Lang.Analyse(compilation, new[] { new Analyser() });

            // Optimizes a compilation, replacing model
            //  elements with more optimized ones, given
            //  a list of optimizers.
            var optimizedCompilation = Lang.Optimize(compilation, new[] { new Optimizer() });

            // Create an executor delegate that can run
            //  code represented by the compilation method,
            //  by using the Executor converter.
            var executor = Lang.Convert(
                (MethodDefinition)optimizedCompilation.FindType("Main").Members["MultipleInts"],
                new Executor<Func<IEnumerable<int>>>());

            // Calls the executor and gets the list of
            //  resulting alternatives.
            var result = executor().ToList();

            Assert.Equals(result, new[] { 1, 2 });
        }
    }
}
