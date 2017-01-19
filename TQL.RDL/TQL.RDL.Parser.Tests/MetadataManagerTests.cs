using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RDL.Parser.Exceptions;

namespace TQL.RDL.Parser.Tests
{
    [TestClass]
    public class MetadataManagerTests
    {
        private RdlMetadata manager;

        private class TestMethodClass
        {
            public void MethodA() { }
            public void MethodA(int param1) { }
            public void MethodA(int param1, int param2) { }
            public void MethodA(int param1, int param2, int param3, string param4 = null) { }
        }

        [TestInitialize]
        public void Initialize()
        {
            manager = new RdlMetadata();
            manager.RegisterMethods<TestMethodClass>(nameof(TestMethodClass.MethodA));
        }

        [TestMethod]
        public void HasMethod_CheckMethodExists_ShouldPass()
        {
            Assert.IsTrue(manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[0]));
            Assert.IsTrue(manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[1] { typeof(int) }));
            Assert.IsTrue(manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[2] { typeof(int), typeof(int) }));
            Assert.IsTrue(manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[4] { typeof(int), typeof(int), typeof(int), typeof(string) }));
            Assert.IsFalse(manager.HasMethod(nameof(TestMethodClass.MethodA), new Type[4] { typeof(int), typeof(int), typeof(int), typeof(object) }));
        }

        [TestMethod]
        public void GetMethod_ShouldPass()
        {
            var method = manager.GetMethod(nameof(TestMethodClass.MethodA), new Type[1] {typeof(int)});
            Assert.IsNotNull(method);
        }

        [TestMethod]
        [ExpectedException(typeof(MethodNotFoundedException))]
        public void GetMethod_MethodNotExist_ShouldThrow()
        {
            var method = manager.GetMethod(nameof(TestMethodClass.MethodA), new Type[1] {typeof(object)});
        }
    }
}
