using Unity.Entities;

namespace _Servers.Systems.Planification
{
    // Will take all refused commands per the server & send back the command to the client
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ServerCommandCancellationListSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}