using Netcode.Components;
using Unity.Burst;
using Unity.Entities;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UpdateEntitiesTimeSystem : ISystem
    {
        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float time = SystemAPI.Time.DeltaTime;
            
            new UpdateTimeEntityJob()
            {
                Delta = time
            }.ScheduleParallel();
        }
        
    }
    
    [WithAll(typeof(TestInstTag))]
    [BurstCompile]
    public partial struct UpdateTimeEntityJob : IJobEntity
    {
        public float Delta;

        private void Execute(ref DynamicBuffer<TestSyncBuffer> Tsb)
        {
            for (int index = 0; index < Tsb.Length; index++)
            {
                ref TestSyncBuffer testSyncBuffer = ref Tsb.ElementAt(index);
                testSyncBuffer.Progress += Delta * index;
            }
        }
    }
}