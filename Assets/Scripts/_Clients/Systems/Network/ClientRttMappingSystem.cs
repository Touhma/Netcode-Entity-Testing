using _Commons.Commands;
using _Commons.Helpers;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _Clients.Systems.Network
{
    public partial struct ClientRttMappingSystem : ISystem
    {
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new(Allocator.Temp);
            foreach ((RefRW<HeartBeatCommand> heartBeat, RefRW<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRW<HeartBeatCommand>, RefRW<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
               NetworkHelper.BroadcastCommand(ref state, heartBeat.ValueRW);
               buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}