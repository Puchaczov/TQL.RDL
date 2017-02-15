using System;
using System.Reflection;
using RDL.Parser;

namespace TQL.RDL.Parser.Tests.Resolvers
{
    internal class DummyDeclarationResolver : IMethodDeclarationResolver
    {
        public bool TryResolveMethod(string name, Type[] callArgs, out MethodInfo result)
        {
            result = typeof(DummyDeclarationResolver).GetMethod(name);
            return true;
        }

        public bool A(bool param) => true;

        public bool B(bool param) => true;

        public bool MyFunction4(bool a, bool b, bool c) => true;

        public int GetDay() => 1;

        public int Abc(int a, int b, int c) => 1;

        public int GetMonth() => 1;

        public int GetYear() => 1;

        public bool CanBeCached(MethodInfo method) => true;
    }
}