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
                MappingLatestTick = new NativeHashMap<int, uint>(16, Allocator.Persistent),
                MappingRTT = new NativeHashMap<int, uint>(16, Allocator.Persistent)
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<NetworkMappingSingleton> singleton = SystemAPI.GetSingletonRW<NetworkMappingSingleton>();
            uint currentTime = (uint)(SystemAPI.Time.ElapsedTime * 1000);

            EntityCommandBuffer buffer = new(Allocator.Temp);
            foreach ((RefRW<HeartBeatCommand> heartBeat, RefRW<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRW<HeartBeatCommand>, RefRW<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                int currentNetworkID = SystemAPI.GetComponent<NetworkId>(request.ValueRO.SourceConnection).Value;

                if (!singleton.ValueRW.MappingLatestTick.ContainsKey(currentNetworkID))
                {
                    singleton.ValueRW.MappingLatestTick.Add(currentNetworkID, 0);
                    singleton.ValueRW.MappingRTT.Add(currentNetworkID, 0);
                }

                uint currentLatestTick = singleton.ValueRW.MappingLatestTick[currentNetworkID];
                
                if (currentLatestTick < heartBeat.ValueRO.SentTick)
                {
                    singleton.ValueRW.MappingRTT[currentNetworkID] = currentTime - heartBeat.ValueRW.ServerTs;
                    singleton.ValueRW.MappingLatestTick[currentNetworkID] = heartBeat.ValueRO.SentTick;
                }

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