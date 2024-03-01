using Authorings;
using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct CreateTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<GameSingletonComponentData>();
        }

        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            Debug.Log("CreateTimeSystem");

            EntityCommandBuffer commandBuffer = new(Allocator.Temp);

            if (!SystemAPI.TryGetSingleton(out GameSingletonComponentData prefabs) || prefabs.Singleton == Entity.Null)
            {
                Debug.Log("Error");
            }

            Entity singleton = commandBuffer.Instantiate(prefabs.Singleton);
            commandBuffer.SetName(singleton, "Test");
            commandBuffer.AddComponent<TestSyncComponent>(singleton);

            commandBuffer.Playback(state.EntityManager);
            commandBuffer.Dispose();
        }
    }
}