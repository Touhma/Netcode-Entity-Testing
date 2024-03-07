using Authorings;
using Netcode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation )]
    [UpdateBefore(typeof(UpdateTimeSystem))]
    [UpdateBefore(typeof(UpdateEntitiesTimeSystem))]
    public partial struct InitTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabsData>();
            state.RequireForUpdate<TestSyncComponent>();
            state.RequireForUpdate<TestEntityPrefabTag>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;

            Entity tse = SystemAPI.GetSingletonEntity<TestSyncComponent>();
            DynamicBuffer<TestSyncBuffer> tsb = SystemAPI.GetBuffer<TestSyncBuffer>(tse);

            EntityCommandBuffer ecb = new(Allocator.Temp);
            for (int i = 0; i < 1000; i++)
            {
                ecb.AppendToBuffer(tse, new TestSyncBuffer
                {
                    Progress = 0
                });
            }

            PrefabsData prefabTag = SystemAPI.GetSingleton<PrefabsData>();

            for (int i = 0; i < 1000; i++)
            {
                Entity inst = ecb.Instantiate(prefabTag.Time);
                ecb.AddComponent<TestInstTag>(inst);
                
                for (int x = 0; x < 1000; x++)
                {
                    ecb.AppendToBuffer(inst, new TestSyncBuffer
                    {
                        Progress = 0
                    });
                }
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}