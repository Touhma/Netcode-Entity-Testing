using Authorings;
using Netcode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateBefore(typeof(SpawnOnSplineSystem))]
    public partial struct InitSplineSystem : ISystem
    {
        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabsData>();
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            state.Enabled = false;
            EntityCommandBuffer ecb = new(Allocator.Temp);
           
            float3 startBase= float3.zero;
            float3 endBase= new float3(0,0,100);

            for (int i = 0; i < 1000; i++)
            {
                Entity spline = ecb.CreateEntity();
                float3 offset = new float3(i * 0.1f, 0, 0);
                float3 currentStart = startBase + offset;
                float3 currentEnd = endBase + offset;
                ecb.AddComponent<SplinePositions>(spline, new SplinePositions()
                {
                    Start = currentStart,
                    End = currentEnd
                });
                
                ecb.SetName(spline,"Spline - " + i);
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
    }
}