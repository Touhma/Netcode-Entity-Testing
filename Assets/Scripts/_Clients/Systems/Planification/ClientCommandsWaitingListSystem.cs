using Unity.Entities;

namespace _Clients.Systems.Planification
{
    // Responsible to save the command about to be sent in a waiting list until received server confirmation
    public partial struct ClientCommandsWaitingListSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}