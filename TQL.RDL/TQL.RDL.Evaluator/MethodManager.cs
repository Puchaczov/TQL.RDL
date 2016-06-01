using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace TQL.RDL.Evaluator
{
    public class MethodManager
    {
        private Dictionary<string, List<Tuple<MethodInfo, object>>> methods;

        public MethodManager()
        {
            methods = new Dictionary<string, List<Tuple<MethodInfo, object>>>();
        }

        public Tuple<MethodInfo, object> GetMethod(string name, Type[] methodArgs)
        {
            if (!this.methods.ContainsKey(name))
            {
                throw new Exception(name);
            }

            var methods = this.methods[name];

            for (int i = 0, j = methods.Count; i < j; ++i)
            {
                var methodInfo = methods[i].Item1;
                var parameters = methodInfo.GetParameters();

                if (methodArgs.Length != parameters.Length)
                    continue;

                bool hasMatchedArgTypes = true;
                for (int f = 0; f < parameters.Length; ++f)
                {
                    if (parameters[f].ParameterType != methodArgs[f])
                    {
                        hasMatchedArgTypes = false;
                        break;
                    }
                }

                if (!hasMatchedArgTypes)
                    continue;

                return methods[i];
            }

            throw new Exception("Not matched");
        }

        public void RegisterMethod(string name, object instance, MethodInfo methodInfo)
        {
            if (this.methods.ContainsKey(name))
                this.methods[name].Add(new Tuple<MethodInfo, object>(methodInfo, instance));
            else
                this.methods.Add(name, new List<Tuple<MethodInfo, object>>() { new Tuple<MethodInfo, object>(methodInfo, instance) });
        }
    }
}
