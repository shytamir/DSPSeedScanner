using System.Threading;

namespace DSPSeedScanner.Runtime
{
    public sealed class RuntimeOperationGate
    {
        private int active;

        public bool TryEnter()
        {
            return Interlocked.CompareExchange(ref active, 1, 0) == 0;
        }

        public void Exit()
        {
            Volatile.Write(ref active, 0);
        }
    }
}
