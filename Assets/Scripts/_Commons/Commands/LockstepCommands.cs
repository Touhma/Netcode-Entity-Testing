using Unity.NetCode;

namespace _Commons.Commands
{
    public struct LockstepCommands : IRpcCommand
    {
        public uint ServerTick;
        public uint ServerElapsedTs;
        public uint ServerStartingTs;
        public uint ServerTs;
    }
}