using Unity.Entities;

namespace _Commons.Components
{
    public struct LockstepTick : IComponentData
    {
        public uint StartingTs;
        public uint ElapsedTs;
        public uint CurrentTick;
    }

    public struct ClientUpdate : IComponentData
    {
        public float Step;
        public float NetworkStep;

        public bool ShouldUpdate => Step >= NetworkStep;
    }
    
}