namespace TQL.RDL.Evaluator
{
    public interface IRdlInstruction
    {
        void Run(RdlVirtualMachine machine);
    }
}