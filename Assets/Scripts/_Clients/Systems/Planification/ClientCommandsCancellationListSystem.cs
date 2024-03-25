using Unity.Entities;

namespace _Clients.Systems.Planification
{
    // Responsible to process the commands from the server that got refused , basically put them in a list for later system to remove previews / etc ...
    public partial struct ClientCommandsCancellationListSystem : ISystem
    {
        public void OnUpdate(ref SystemState state) { }
    }
}