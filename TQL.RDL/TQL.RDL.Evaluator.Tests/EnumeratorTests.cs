﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TQL.Core.Tokens;
using TQL.RDL.Evaluator.Enumerators;
using TQL.RDL.Parser.Nodes;
using TQL.RDL.Parser.Tokens;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class EnumeratorTests
    {

        [TestMethod]
        public void Enumerator_EnumerateKeywords_ShouldPass()
        {
            //"case when 1 then 2 when 3 then 4 else 0"
            var rootComponent = new RootScriptNode(new RdlSyntaxNode[] {
                new CaseNode(
                    new Token("case", Parser.Tokens.StatementType.Case, new Core.Tokens.TextSpan(0, 0)),
                    new WhenThenNode[] {
                        new WhenThenNode(
                            new WhenNode(new Token("when", StatementType.Numeric, new TextSpan()), new NumericNode(new Token("1", StatementType.Numeric, new TextSpan()))),
                            new ThenNode(new Token("then", StatementType.Numeric, new TextSpan()), new NumericNode(new Token("2", StatementType.Numeric, new TextSpan())))
                        ),
                        new WhenThenNode(
                            new WhenNode(new Token("when", StatementType.Numeric, new TextSpan()), new NumericNode(new Token("3", StatementType.Numeric, new TextSpan()))),
                            new ThenNode(new Token("then", StatementType.Numeric, new TextSpan()), new NumericNode(new Token("4", StatementType.Numeric, new TextSpan())))
                        )
                    }, 
                    new ElseNode(new Token("else", StatementType.Else, new TextSpan()), new NumericNode(new Token("0", StatementType.Numeric, new TextSpan()))))
            });

            var enumerator = new KeywordEnumerator(rootComponent);

            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is RootScriptNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is CaseNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is WhenThenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is WhenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is ThenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is WhenThenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is WhenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is ThenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is ElseNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode);
            Assert.IsFalse(enumerator.MoveNext());
        }

        [TestMethod]
        public void Enumerator_EnumerateArythmetics_ShouldPass()
        {
            //"7 * ( 8 + 1 ) - 4"
            var rootComponent = new RootScriptNode(
                new RdlSyntaxNode[] {
                    new HyphenNode(
                        new StarNode(
                            new NumericNode(
                                new Token("7", StatementType.Numeric, new TextSpan())),
                            new AddNode(
                                new NumericNode(new Token("8", StatementType.Numeric, new TextSpan())),
                                new NumericNode(new Token("1", StatementType.Numeric, new TextSpan()))
                            )),
                        new NumericNode(new Token("4", StatementType.Numeric, new TextSpan())))
                });

            var enumerator = new ArythmeticEnumerator(rootComponent);

            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode && (enumerator.Current as NumericNode).Value == 7);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode && (enumerator.Current as NumericNode).Value == 8);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode && (enumerator.Current as NumericNode).Value == 1);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is AddNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is StarNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is NumericNode && (enumerator.Current as NumericNode).Value == 4);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is HyphenNode);
            enumerator.MoveNext();
            Assert.IsTrue(enumerator.Current is RootScriptNode);
            Assert.IsFalse(enumerator.MoveNext());
        }
    }
}