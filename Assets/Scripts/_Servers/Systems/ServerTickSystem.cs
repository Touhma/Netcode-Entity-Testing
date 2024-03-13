using _Commons.Commands;
using _Commons.Components;
using Netcode;
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Servers.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    [UpdateBefore(typeof(ServerSystem))]
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

                Entity request = buffer.CreateEntity();
                buffer.AddComponent(request, new SendRpcCommandRequest());
                buffer.AddComponent(request, new LockstepCommands()
                {
                    ServerTick = tick.ValueRW.CurrentTick,
                    ServerTs = NetworkTimeSystem.TimestampMS,
                    ServerStartingTs = tick.ValueRW.StartingTs,
                    ServerElapsedTs = tick.ValueRW.ElapsedTs
                });
            }

            foreach ((RefRW<HeartBeatCommand> heartBeat, Entity entity) in SystemAPI.Query<RefRW<HeartBeatCommand>>().WithEntityAccess())
            {
                NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();

                Debug.LogWarning("Client heartBeat - ServerTick : " + heartBeat.ValueRO.ServerTick + " --> current ServerTick SerializedData = " + networkTime.ServerTick.SerializedData);
                Debug.LogWarning("Client heartBeat - ClientTick :" + heartBeat.ValueRO.ClientTick);

                uint delta = networkTime.ServerTick.SerializedData - heartBeat.ValueRO.ClientTick;
                Debug.LogWarning("Client heartBeat - delta :" +  delta);
                if (delta <= 0)
                {
                    Debug.LogError("Prediction error : " + delta);
                }

                /*
                Entity heartSBeat = buffer.CreateEntity();

                buffer.AddComponent(heartSBeat, new SendRpcCommandRequest());
                buffer.AddComponent(heartSBeat, new HeartBeatCommand()
                {
                    ServerTick = networkTime.ServerTick.SerializedData,
                    ServerTs = NetworkTimeSystem.TimestampMS,
                    ClientTick = tick.ValueRW.InterpolationTick.SerializedData,
                    ClientTs = NetworkTimeSystem.TimestampMS
                });
                //*/

                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}