using Unity.Entities;

namespace _Commons.Components
{
    public struct TickClockComponent : IComponentData
    {
        public uint CurrentTick;
        public uint CurrentPartialTick;
    }
    
    public struct TickTimerComponent : IComponentData
    {
        public uint TickDt;
        public uint AccumulatedTime;
    }

    public struct ConnectionEstablishedTag : IComponentData
    {
        
    }
}