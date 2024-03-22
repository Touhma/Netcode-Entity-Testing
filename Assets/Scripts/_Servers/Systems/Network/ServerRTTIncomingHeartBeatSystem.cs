using _Commons.Commands;
using _Servers.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers.Systems.Network
{
    [UpdateInGroup(typeof(SimulationSystemGroup))]
    public partial struct ServerRTTIncomingHeartBeatSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.EntityManager.CreateSingleton(new NetworkMappingSingleton()
            {
                MappingLatestTick = new NativeHashMap<uint, uint>(16, Allocator.Persistent),
                MappingRTT = new NativeParallelMultiHashMap<Entity, RttElement>(16, Allocator.Persistent)
            });
        }
        
        public void OnUpdate(ref SystemState state)
        {
            RefRW<NetworkMappingSingleton> singleton = SystemAPI.GetSingletonRW<NetworkMappingSingleton>();
            
            EntityCommandBuffer buffer = new(Allocator.Temp);
            foreach ((RefRW<HeartBeatCommand> heartBeat, RefRW<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRW<HeartBeatCommand>, RefRW<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }

        public void OnDestroy(ref SystemState state)
        {
            RefRW<NetworkMappingSingleton> singleton = SystemAPI.GetSingletonRW<NetworkMappingSingleton>();
            singleton.ValueRW.MappingLatestTick.Dispose();
            singleton.ValueRW.MappingRTT.Dispose();
        }
    }
}