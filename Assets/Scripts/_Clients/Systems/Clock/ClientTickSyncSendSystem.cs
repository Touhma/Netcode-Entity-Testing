using _Commons.Commands;
using _Commons.SystemGroups;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace _Clients.Systems
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
            EntityCommandBuffer buffer = new(Allocator.Temp);

            Entity heartBeat = buffer.CreateEntity();

            buffer.AddComponent(heartBeat, new SendRpcCommandRequest());
            buffer.AddComponent(heartBeat, new TickSyncCommand()
            {
                ClientTs = (uint)(SystemAPI.Time.ElapsedTime * 1000)
            });

            buffer.Playback(state.EntityManager);
            buffer.Dispose();
            state.Enabled = false;
        }
    }
}