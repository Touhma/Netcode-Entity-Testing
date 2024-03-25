using Unity.Entities;

namespace _Servers.Systems.Planification
{
    // Will take all confirmed commands per the server & affect them to a tick in the future to be send to the client
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ServerCommandPlanificationListSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}