using Netcode.Components;
using Unity.Burst;
using Unity.Entities;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    public partial struct UpdateTimeSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<TestSyncComponent>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            float time = SystemAPI.Time.DeltaTime;
            RefRW<TestSyncComponent> tsc = SystemAPI.GetSingletonRW<TestSyncComponent>();
            tsc.ValueRW.Time += time;

            //*
            new UpdateTimeJob()
            {
                Delta = time
            }.ScheduleParallel();
            //*/
        }
    }

    [WithAll(typeof(TestSyncComponent))]
    [BurstCompile]
    public partial struct UpdateTimeJob : IJobEntity
    {
        public float Delta;

        private void Execute([ChunkIndexInQuery] int chunkIndex, ref DynamicBuffer<TestSyncBuffer> Tsb)
        {
            for (int index = 0; index < Tsb.Length; index++)
            {
                ref TestSyncBuffer testSyncBuffer = ref Tsb.ElementAt(index);
                testSyncBuffer.Progress += Delta * index;
            }
        }
    }
}