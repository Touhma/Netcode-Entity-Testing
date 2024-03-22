using _Commons.Commands;
using _Commons.Components;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Servers.Systems.Clock
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateInGroup(typeof(PostTickGroup))]
    [UpdateBefore(typeof(Network.ServerInitializeClientSystem))]
    public partial struct ServerTickSyncReceiveSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TickClockComponent>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new(Allocator.Temp);

            foreach ((RefRW<TickSyncCommand> heartBeat, RefRW<ReceiveRpcCommandRequest> request, Entity entity) in SystemAPI.Query<RefRW<TickSyncCommand>, RefRW<ReceiveRpcCommandRequest>>().WithEntityAccess())
            {
                Debug.LogWarning("Server HeartBeatCommand");

                TickClockComponent clock = SystemAPI.GetSingleton<TickClockComponent>();

                TickSyncCommand command = new()
                {
                    ServerTs = (uint)(SystemAPI.Time.ElapsedTime * 1000),
                    ServerTick = clock.TickLatest,
                    ServerTickPartial = clock.TickLatestPartial,

                    ClientTs = heartBeat.ValueRO.ClientTs
                };
                
                NetworkHelper.SendCommandToTarget(ref buffer, command, request.ValueRO.SourceConnection);

                buffer.DestroyEntity(entity);
            }

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}