using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode.Systems
{
    public struct InitializedClient : IComponentData
    {
        
    }
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial class ServerSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            EntityCommandBuffer commandBuffer = new (Allocator.Temp);
            
            foreach ((RefRO<NetworkId> networkId, Entity entity) in SystemAPI.Query<RefRO<NetworkId>>().WithNone<InitializedClient>().WithEntityAccess())
            {
                commandBuffer.AddComponent<InitializedClient>(entity);
                Debug.Log("Client connected with id = " + networkId.ValueRO.Value);
                
            }
            
            foreach ((RefRO<ReceiveRpcCommandRequest> request, RefRO<ClientMessageRpcCommand> command, Entity entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<ClientMessageRpcCommand>>().WithEntityAccess())
            {
                Debug.Log(command.ValueRO.Message + " From client index " + request.ValueRO.SourceConnection.Index + "version " + request.ValueRO.SourceConnection.Version);
                commandBuffer.DestroyEntity(entity);
            }
            commandBuffer.Playback(EntityManager);
            commandBuffer.Dispose();
        }
    }
}