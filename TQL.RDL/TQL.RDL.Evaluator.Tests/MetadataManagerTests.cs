using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TQL.RDL.Parser.Exceptions;

namespace TQL.RDL.Evaluator.Tests
{
    [TestClass]
    public class MetadataManagerTests
    {
        private RdlMetadata _manager;

        [TestInitialize]
        public void Initialize()
        {
            _manager = new RdlMetadata();
            _manager.RegisterMethods<TestMethodClass>(nameof(TestMethodClass.MethodA));
        }

        [TestMethod]
        public void HasMethod_CheckMethodExists_ShouldPass()
        {
            Assert.IsTrue(_manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[0]));
            Assert.IsTrue(_manager.HasMethod(nameof(TestMethodClass.MethodA), new[] {typeof(int)}));
            Assert.IsTrue(_manager.HasMethod(nameof(TestMethodClass.MethodA), new[] {typeof(int), typeof(int)}));
            Assert.IsTrue(_manager.HasMethod(nameof(TestMethodClass.MethodA),
                new Type[4] {typeof(int), typeof(int), typeof(int), typeof(string)}));
            Assert.IsFalse(_manager.HasMethod(nameof(TestMethodClass.MethodA),
                new Type[4] {typeof(int), typeof(int), typeof(int), typeof(object)}));
        }

        [TestMethod]
        public void GetMethod_ShouldPass()
        {
            var method = _manager.GetMethod(nameof(TestMethodClass.MethodA), new Type[1] {typeof(int)});
            Assert.IsNotNull(method);
        }

        [TestMethod]
        [ExpectedException(typeof(MethodNotFoundedException))]
        public void GetMethod_MethodNotExist_ShouldThrow()
        {
            var method = _manager.GetMethod(nameof(TestMethodClass.MethodA), new Type[1] {typeof(object)});
        }

        private class TestMethodClass
        {
            public void MethodA()
            {
            }

            public void MethodA(int param1)
            {
            }

            public void MethodA(int param1, int param2)
            {
            }

            public void MethodA(int param1, int param2, int param3, string param4 = null)
            {
            }
        }
    }
}