using _Commons.Commands;
using _Commons.Helpers;
using _Commons.SystemGroups;
using Unity.Entities;
using Unity.NetCode;

namespace _Clients.Systems.Clock
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(PostTickGroup))]
    public partial struct ClientTickSyncSendSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<NetworkId>();
        }

        public void OnUpdate(ref SystemState state)
        {
            TickSyncCommand command = new ()
            {
                ClientTs = (uint)(SystemAPI.Time.ElapsedTime * 1000)
            };
            NetworkHelper.BroadcastCommand(ref state, command);

            state.Enabled = false;
        }
    }
}