using Authorings;
using SystemGroup;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using UnityEngine;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

namespace Netcode
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation |WorldSystemFilterFlags.LocalSimulation)]
    [UpdateInGroup(typeof(VisualizationPhysicsSystemGroup))]
    [UpdateAfter(typeof(PhysicsSystemGroup))]
    public partial class DebugRaycastSystem : SystemBase
    {
        protected override void OnCreate()
        {
            Debug.Log("OnCreate");
        }

        protected override void OnStartRunning()
        {
            Debug.Log("OnStartRunning");
            base.OnStartRunning();
        }

        protected override void OnUpdate()
        {
            Debug.Log("OnUpdate");
            Entity cube = SystemAPI.GetSingletonEntity<RaycastDebugComponentData>();

            CollisionWorld collisionWorld = SystemAPI.GetSingleton<PhysicsWorldSingleton>().CollisionWorld;
            Camera main;
            Entity hitEntity = DoAdjustedRaycast(
                (main = Camera.main).ScreenPointToRay((Vector2)Input.mousePosition),
                main.transform.position,
                float3.zero, 
                new CollisionFilter()
                {
                    BelongsTo = ~0u,
                    CollidesWith = ~0u, // all 1s, so all layers, collide with everything
                    GroupIndex = 0
                },
                ref collisionWorld ,
                out RaycastHit raycastHit);
            
            
            Debug.Log("test" + raycastHit.Entity);
            Debug.Log("test" + raycastHit.Position);
        }
        
        public static Entity DoAdjustedRaycast(in Ray ray, float3 cameraPosition, float3 planetPosition, CollisionFilter filter, ref CollisionWorld collisionWorld, out RaycastHit raycastHit)
        {
            float3 rayHitPoint = ray.GetPoint(math.distance(cameraPosition, planetPosition) * 10);
            return Raycast(ray.origin, rayHitPoint, filter, ref collisionWorld, out raycastHit);
        }
    
        public static Entity Raycast(float3 rayFrom, float3 rayTo, CollisionFilter filter, ref CollisionWorld collisionWorld, out RaycastHit hit)
        {
            RaycastInput input = new RaycastInput()
            {
                Start = rayFrom,
                End = rayTo,
                Filter = filter
            };

            hit = new RaycastHit();
            bool haveHit = collisionWorld.CastRay(input, out hit);
            return haveHit ? hit.Entity : Entity.Null;
        }
    }
    
    
}