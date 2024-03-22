using Netcode.Components;
using Unity.Entities;
using Unity.NetCode;

namespace _Commons.Helpers
{
    public static class NetworkHelper
    {
        public static void BroadcastCommand<T>(ref SystemState state, T command) where T : unmanaged, IRpcCommand, IComponentData
        {
            Entity entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, command);
            state.EntityManager.AddComponent<SendRpcCommandRequest>(entity);
        }
        
        public static void SendCommandToTarget<T>(ref SystemState state, T command, Entity target) where T : unmanaged, IRpcCommand, IComponentData
        {
            Entity entity = state.EntityManager.CreateEntity();
            state.EntityManager.AddComponentData(entity, command);
            state.EntityManager.AddComponentData(entity, new SendRpcCommandRequest()
            {
                TargetConnection = target
            });
        }
        
        public static void SendCommandToTarget<T>(ref EntityCommandBuffer state, T command, Entity target) where T : unmanaged, IRpcCommand, IComponentData
        {
            Entity entity = state.CreateEntity();
            state.AddComponent(entity, command);
            state.AddComponent(entity, new SendRpcCommandRequest()
            {
                TargetConnection = target
            });
        }

        public static void InitializeClient(ref EntityCommandBuffer state, Entity target)
        {
            state.AddComponent<InitializedClientTag>(target);
        }
    }
}