using _Commons.Commands;
using _Commons.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Clients.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation| WorldSystemFilterFlags.ThinClientSimulation)]
    public partial struct ClientTickSyncReceiveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new(Allocator.Temp);
            foreach ((RefRW<TickSyncCommand> heartBeat, RefRW<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRW<TickSyncCommand>, RefRW<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                uint ClientTs = heartBeat.ValueRO.ClientTs;
                uint ClientCurrentTS = (uint)(SystemAPI.Time.ElapsedTime * 1000);
                uint ClientRTT = ClientCurrentTS - ClientTs;
                Debug.Log("ClientTs :  " + ClientTs);
                Debug.Log("ClientCurrentTS : " + ClientCurrentTS);
                Debug.Log("ClientRTT : " + ClientRTT);

                Entity tag = buffer.CreateEntity();
                buffer.AddComponent<ConnectionEstablishedTag>(tag);

                RefRW<TickClockComponent> clock = SystemAPI.GetSingletonRW<TickClockComponent>();
                RefRW<TickTimerComponent> clockTimer = SystemAPI.GetSingletonRW<TickTimerComponent>();
                clockTimer.ValueRW.AccumulatedTime = heartBeat.ValueRO.ServerTickPartial + (uint)(ClientRTT / 2) ;
                clock.ValueRW.CurrentTick = heartBeat.ValueRO.ServerTick;
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}