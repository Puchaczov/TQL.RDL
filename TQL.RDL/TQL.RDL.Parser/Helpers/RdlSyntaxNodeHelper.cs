using System.Text;
using TQL.RDL.Parser.Nodes;

namespace TQL.RDL.Parser.Helpers
{
    public static class RdlSyntaxNodeHelper
    {
        /// <summary>
        ///     Gets stringied definition of function.
        /// </summary>
        /// <param name="node">The Function node.</param>
        /// <returns>stringied definition of function.</returns>
        public static string DetailedFunctionIdentifier(this RawFunctionNode node)
        {
            var builder = new StringBuilder();

            builder.Append(node.ReturnType.Name);
            builder.Append(node.Name);

            if (node.Descendants.Length > 0)
            {
                foreach (var descendant in node.Descendants)
                {
                    builder.Append(descendant);
                    builder.Append(':');
                    builder.Append(descendant.ReturnType.Name);
                }
            }
            else
            {
                builder.Append("none");
                builder.Append(':');
                builder.Append("void");
            }

            return builder.ToString();
        }

        /// <summary>
        /// Gets general identifier of function (without using parameters)
        /// </summary>
        /// <param name="node">The raw function node.</param>
        /// <returns>Identifier of function</returns>
        public static string GeneralFunctionIdentifier(this RawFunctionNode node)
        {
            var builder = new StringBuilder();

            builder.Append(node.ReturnType.Name);
            builder.Append(node.Name);

            return builder.ToString();
        }

        public static string SpecificFunctionIdentifier(this RawFunctionNode node)
        {
            var builder = new StringBuilder();

            builder.Append(node.ReturnType.Name);
            builder.Append(node.Name);
            builder.Append(node.FullSpan.Start);
            builder.Append(node.FullSpan.Length);

            return builder.ToString();
        }
    }
}