using System.Text;
using RDL.Parser.Nodes;

namespace RDL.Parser.Helpers
{
    public static class RdlSyntaxNodeHelper
    {
        /// <summary>
        ///     Gets stringied definition of function.
        /// </summary>
        /// <param name="node">The Function node.</param>
        /// <returns>stringied definition of function.</returns>
        public static string Stringify(this RawFunctionNode node)
        {
            var builder = new StringBuilder();

            builder.Append(node.ReturnType.Name);
            builder.Append(node.Name);

            foreach (var descendant in node.Descendants)
            {
                builder.Append(descendant);
                builder.Append(':');
                builder.Append(descendant.ReturnType.Name);
            }

            return builder.ToString();
        }
    }
}