using Unity.Entities;

namespace _Commons.Components
{
    public struct TickClockComponent : IComponentData
    {
        public uint TickDelta; // Config value
        public uint TickLatest; // the uint of the current tick ID
        public uint TickLatestPartial; // What was left on the last tick increase
        public uint TickIncreaseDelta; // How much tick has been popped this frame
        public uint TickCurrentPartial; // What is currently left, this is 0 when the tick pop right on the perfect timing
        public uint TickCurrentDelta; // System.deltaTime on this frame converted to ms

        // Recover the ms value since last tick
        public float DeltaTime() => TickCurrentDelta / 1000f;
        public uint FullSimulationTime() => TickLatest * TickDelta + TickCurrentPartial;
    }

    public struct ConnectionEstablishedTag : IComponentData { }
}