namespace TQL.RDL.Evaluator
{
    public interface IRDLInstruction
    {
        void Run(RDLVirtualMachine machine);
    }
}