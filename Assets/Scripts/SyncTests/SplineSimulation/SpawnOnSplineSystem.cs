using Authorings;
using Helper;
using Netcode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

namespace Netcode
{
    [WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
    [UpdateBefore(typeof(UpdateOnSplineSystem))]
    public partial struct SpawnOnSplineSystem : ISystem
    {
        private float _delta;

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<PrefabsData>();
            _delta = 0;
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer ecb = new(Allocator.Temp);

            PrefabsData prefabTag = SystemAPI.GetSingleton<PrefabsData>();

            _delta += SystemAPI.Time.DeltaTime;

            if (_delta >= 2.0f)
            {
                foreach ((SplinePositions spline, Entity entity) in SystemAPI.Query<SplinePositions>().WithEntityAccess())
                {
                    Entity newUnit = ecb.Instantiate(prefabTag.Unit);
                    ecb.AddComponent(newUnit, new SplineProgress()
                    {
                        Position = 0,
                        Spline = entity
                    });
                    ecb.SetComponent(newUnit, UT.GetLTFrom(spline.Start, quaternion.identity, 1f));
                }

                _delta -= 2.0f;
            }
            
            ecb.Playback(state.EntityManager);
            ecb.Dispose();
        }
        
       
    }
}