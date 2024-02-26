using Netcode.Commands;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode.Systems
{
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class GetServerMessageSystem : SystemBase
    {
        public NetcodeEntityInputs Inputs;

        protected override void OnCreate()
        {
            Inputs = new NetcodeEntityInputs();
            Inputs.Enable();
            // this mean we have a connection to the server
            RequireForUpdate<NetworkId>();
        }

        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<ServerMessageRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ServerMessageRpcCommand>>().WithEntityAccess())
            {
                Debug.Log(command.ValueRO.Message);
                commandBuffer.DestroyEntity(entity);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        protected override void OnDestroy()
        {
            Inputs.Disable();
        }
    }
}