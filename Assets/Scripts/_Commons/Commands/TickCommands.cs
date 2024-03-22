using Unity.NetCode;

namespace _Commons.Commands
{
    public struct TickSyncCommand : IRpcCommand
    {
        public uint ServerTs;
        public uint ServerTick;
        public uint ServerTickPartial;
        public uint ClientTs;
    }

    public struct HeartBeatCommand : IRpcCommand
    {
        public uint SentTick;
        public uint ServerTs;
    }
}