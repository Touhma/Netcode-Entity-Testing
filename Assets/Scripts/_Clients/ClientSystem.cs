using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Netcode
{

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientSystem : SystemBase
    {
        protected override void OnCreate()
        {
            RequireForUpdate<NetworkId>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

    }
}