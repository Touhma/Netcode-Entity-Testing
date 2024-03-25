using Unity.Entities;

namespace _Servers.Systems.Planification
{
    // Will take all commands incoming that have been validated by gameplay system 
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ServerCommandsValidationSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}