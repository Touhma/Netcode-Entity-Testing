using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode.Systems
{
    public struct ClientMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class ClientSystem : SystemBase
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
            // To send command need 1 entity per command 
            if (Inputs.Player.Space.WasPerformedThisFrame())
            {
                SendMessageRpc("Hello from client ", ConnectionManager.ClientWorld);
            }
        }

        protected override void OnDestroy()
        {
            Inputs.Disable();
        } 
        
        public void SendMessageRpc(string text, World world)
        {
            if (world is null || world.IsCreated is false) { return; }

            Entity messageEntity = world.EntityManager.CreateEntity(typeof(SendRpcCommandRequest), typeof(ClientMessageRpcCommand));
            world.EntityManager.SetComponentData(messageEntity, new ClientMessageRpcCommand()
            {
                Message = text
            });
        }
    }
}