using _Commons.Commands;
using _Commons.Components;
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Servers.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ServerTickSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<ClientServerTickRate>();
            state.RequireForUpdate<LockstepTick>();
            
            Debug.LogWarning("LockstepTick");

            state.EntityManager.CreateSingleton(new LockstepTick()
            {
                ElapsedTs = 0,
                StartingTs = NetworkTimeSystem.TimestampMS,
                CurrentTick = 0
            });
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
            foreach ((RefRO<NetworkId> id, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                RefRW<LockstepTick> tick = SystemAPI.GetSingletonRW<LockstepTick>();
                tick.ValueRW.ElapsedTs = NetworkTimeSystem.TimestampMS - tick.ValueRO.StartingTs;
                
                Debug.Log("Send ServerTick");
                Debug.LogWarning("NetworkTimeSystem.TimestampMS  - Server " + NetworkTimeSystem.TimestampMS);
                ClientServerTickRate clientServerTickRate = SystemAPI.GetSingleton<ClientServerTickRate>();
                NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();
                

                tick.ValueRW.CurrentTick = networkTime.ServerTick.SerializedData;
                float deltaTime = SystemAPI.Time.DeltaTime;
                float deltaTicks = deltaTime * clientServerTickRate.SimulationTickRate;

                Entity request = state.EntityManager.CreateEntity();
                
                Debug.LogWarning("LockstepCommands");
                buffer.AddComponent(request, new SendRpcCommandRequest());
                buffer.AddComponent(request, new LockstepCommands()
                {
                    ServerTick = tick.ValueRW.CurrentTick,
                    ServerTs = NetworkTimeSystem.TimestampMS,
                    ServerStartingTs = tick.ValueRW.StartingTs,
                    ServerElapsedTs = tick.ValueRW.ElapsedTs
                });
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}