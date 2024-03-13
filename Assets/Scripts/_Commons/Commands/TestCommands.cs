using Unity.NetCode;

namespace _Commons.Commands
{
    public struct HeartBeatCommand : IRpcCommand
    {
        public uint ServerTick;
        public uint ServerTs;
        public uint ClientTick;
        public uint ClientTs;
    }
}