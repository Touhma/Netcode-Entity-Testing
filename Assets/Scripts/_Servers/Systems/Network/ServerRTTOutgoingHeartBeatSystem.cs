using _Commons.Commands;
using _Commons.Components;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Entities;
using Unity.NetCode;

namespace _Servers.Systems.Network
{
    [UpdateInGroup(typeof(UpdateOnTickGroup))]
    public partial struct ServerRTTOutgoingHeartBeatSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        public void OnUpdate(ref SystemState state)
        {
            RefRW<TickClockComponent> currentTick = SystemAPI.GetSingletonRW<TickClockComponent>();

            HeartBeatCommand command = new()
            {
                SentTick = currentTick.ValueRO.TickLatest,
                ServerTs = (uint)(SystemAPI.Time.ElapsedTime * 1000)
            };
            
            NetworkHelper.BroadcastCommand(ref state, command);
        }
    }
}