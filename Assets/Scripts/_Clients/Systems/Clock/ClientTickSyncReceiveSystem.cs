using _Commons.Commands;
using _Commons.Components;
using _Commons.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Clients.Systems
{
   
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation| WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PostTickGroup))]
    [UpdateBefore(typeof(ClientTickSyncSendSystem))]
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
                clock.ValueRW.TickCurrentPartial = heartBeat.ValueRO.ServerTickPartial + ClientRTT / 2 ;
                clock.ValueRW.TickLatest = heartBeat.ValueRO.ServerTick;
                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}