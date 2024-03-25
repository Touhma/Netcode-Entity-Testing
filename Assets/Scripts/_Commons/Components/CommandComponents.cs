using Unity.Entities;

namespace _Commons.Components
{
    public struct CommandIDComp : IComponentData
    {
        public static uint CurrentCommandID = 0;
        public uint ID;
        
        public static CommandIDComp CreateInstance()
        {
            return new CommandIDComp()
            {
                ID = CurrentCommandID++
            };
        }
    }

    public struct CommandStatusComp : IComponentData
    {
        public bool Validated;
    }
    
    public struct TestPlannedCommandComp : IComponentData
    {
        public uint Tick;
    }
}