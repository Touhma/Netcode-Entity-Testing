using Netcode.Commands;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode.Systems
{


    

    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class InputClientSystem : SystemBase
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

            if (Inputs.Player.Space.WasPerformedThisFrame())
            {
                SendMessageRpc("Hello from client ", ConnectionManager.ClientWorld);
            }

            if (Inputs.Player.Spawn.WasPerformedThisFrame())
            {
                Debug.Log("Spawn ! ");
                SpawnUnitRpc(ConnectionManager.ClientWorld);
            }

            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }

        protected override void OnDestroy()
        {
            Inputs.Disable();
        }

        public void SendMessageRpc(string text, World world)
        {
            if (world is null || world.IsCreated is false)
            {
                return;
            }

            Entity messageEntity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ClientMessageRpcCommand));
            world.EntityManager.SetComponentData(messageEntity, new ClientMessageRpcCommand()
            {
                Message = text
            });
        }

        public void SpawnUnitRpc(World world)
        {
            if (world is null || world.IsCreated is false)
            {
                return;
            }

            world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(SpawnUnitRpcCommand) );
        }
    }
}