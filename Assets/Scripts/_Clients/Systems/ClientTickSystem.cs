using _Commons.Commands;
using _Commons.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace _Clients.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ClientTickSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkTime>();
            state.RequireForUpdate<ClientServerTickRate>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<LockstepCommands> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<LockstepCommands>>().WithEntityAccess())
            {
                uint timeDiff = NetworkTimeSystem.TimestampMS - command.ValueRO.ServerTs;
                Entity lockstep = buffer.CreateEntity();
                ClientServerTickRate clientServerTickRate = SystemAPI.GetSingleton<ClientServerTickRate>();
              
                LockstepTick tick = new LockstepTick()
                {
                    CurrentTick = 0,
                    StartingTs = 0,
                    ElapsedTs = 0
                };

               
                tick.StartingTs = command.ValueRO.ServerTs;
                tick.ElapsedTs = timeDiff;

                float tickMsValue = 1000f / clientServerTickRate.SimulationTickRate;  
                
                float deltaTicks = timeDiff / tickMsValue  ;
                Debug.LogWarning("-Client- NetworkTimeSystem.TimestampMS : " + NetworkTimeSystem.TimestampMS);
                Debug.LogWarning("-Client- command.ValueRO.ServerTs : " + command.ValueRO.ServerTs);
                Debug.LogWarning("-Client- tickMsValue: " + tickMsValue);
                Debug.LogWarning("-Client- timeDiff : " + timeDiff);
                Debug.LogWarning("-Client- deltaTicks : " + deltaTicks);
                tick.CurrentTick = command.ValueRO.ServerTick + (uint)math.trunc(deltaTicks)  ;
                ClientUpdate update = new ClientUpdate()
                {
                    Step = 0,
                    NetworkStep = 1f / SystemAPI.GetSingletonRW<ClientServerTickRate>().ValueRO.NetworkTickRate
                };
               
                    
                buffer.AddComponent(lockstep,tick);
                buffer.AddComponent(lockstep,update);
                
                buffer.DestroyEntity(entity);
            }
            
            buffer.Playback(state.EntityManager);
            buffer.Dispose();
            
            
        }
    }
}