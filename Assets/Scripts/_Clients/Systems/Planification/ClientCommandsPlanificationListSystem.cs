using Unity.Entities;

namespace _Clients.Systems.Planification
{
    // Responsible to process the confirmed commands from the server & put them in a list for processing later by the gameplay code
    public partial struct ClientCommandsPlanificationListSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}