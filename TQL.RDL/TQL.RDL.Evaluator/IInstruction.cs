namespace TQL.RDL.Evaluator
{
    public interface IRDLInstruction
    {
        void Run(RDLVirtualMachine machine);
    }

    public interface IFunction
    {
        bool Call(params object[] args);
    }
}