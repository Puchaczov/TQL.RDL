namespace TQL.RDL.Evaluator.Instructions
{
    public class JumpNotEqual : IRdlInstruction
    {
        private int shift;

        public JumpNotEqual(int shift)
        {
            this.shift = shift;
        }

        public void Run(RDLVirtualMachine machine)
        {
            if(machine.Values.Pop() != machine.Values.Pop())
            {
                machine.InstructionPointer += shift;
                return;
            }
            machine.InstructionPointer += 1;
        }
    }
}