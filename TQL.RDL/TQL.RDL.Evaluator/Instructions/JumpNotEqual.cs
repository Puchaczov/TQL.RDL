namespace TQL.RDL.Evaluator.Instructions
{
    public class JumpNotEqual : IRdlInstruction
    {
        private readonly int _shift;

        public JumpNotEqual(int shift)
        {
            _shift = shift;
        }

        public void Run(RdlVirtualMachine machine)
        {
            if(machine.Values.Pop() != machine.Values.Pop())
            {
                machine.InstructionPointer += _shift;
                return;
            }
            machine.InstructionPointer += 1;
        }
    }
}