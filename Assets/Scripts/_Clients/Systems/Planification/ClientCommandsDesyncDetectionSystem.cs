using Unity.Entities;

namespace _Clients.Systems.Planification
{
    // Check if any command planned are on a tick that is in the past, if found any that mean there is a desync on that client
    public partial struct ClientCommandsDesyncDetectionSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}