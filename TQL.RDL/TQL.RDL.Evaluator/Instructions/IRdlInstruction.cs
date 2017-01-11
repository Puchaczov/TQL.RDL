namespace TQL.RDL.Evaluator
{
    public interface IRdlInstruction
    {
        void Run(RDLVirtualMachine machine);
    }
}