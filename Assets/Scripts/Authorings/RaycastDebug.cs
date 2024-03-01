using Unity.Entities;
using UnityEngine;

namespace Authorings
{
    public class RaycastDebug : MonoBehaviour
    {
        public class RaycastDebugBaker : Baker<RaycastDebug>
        {
            public override void Bake(RaycastDebug authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new RaycastDebugComponentData());
            }
        }
    }

    public struct RaycastDebugComponentData : IComponentData { }
}