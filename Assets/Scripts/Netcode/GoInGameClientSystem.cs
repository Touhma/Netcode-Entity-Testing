using Netcode.CommandsRpc;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Netcode
{


    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct GoInGameClientSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new(Allocator.Temp);
            builder.WithAny<NetworkId>();
            builder.WithNone<NetworkStreamInGame>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<NetworkStreamInGame>().WithEntityAccess())
            {
                commandBuffer.AddComponent<NetworkStreamInGame>(entity);
                Entity request = commandBuffer.CreateEntity();
                commandBuffer.AddComponent<GoInGameCommand>(request);
                commandBuffer.AddComponent<SendRpcCommandRequest>(request);
            }
            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}