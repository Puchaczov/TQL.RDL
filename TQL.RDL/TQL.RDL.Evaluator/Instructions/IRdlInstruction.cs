namespace TQL.RDL.Evaluator.Instructions
{
    public interface IRdlInstruction
    {
        /// <summary>
        ///     Runs set of operations that are defined for operation.
        /// </summary>
        /// <param name="machine">Virtual machine on that code will be executed.</param>
        void Run(RdlVirtualMachine machine);
    }
}