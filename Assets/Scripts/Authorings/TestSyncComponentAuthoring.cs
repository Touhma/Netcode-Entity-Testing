using Netcode.Components;
using Unity.Entities;
using UnityEngine;

namespace Authorings
{
    public class TestSyncComponentAuthoring : MonoBehaviour
    {
        public class TestSyncComponentBaker : Baker<TestSyncComponentAuthoring>
        {
            public override void Bake(TestSyncComponentAuthoring authoring)
            {
                var entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new TestSyncComponent());
            }
        }
    }
}