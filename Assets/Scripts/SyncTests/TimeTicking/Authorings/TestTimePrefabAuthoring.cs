using Unity.Entities;
using UnityEngine;

namespace Netcode.Components
{
    public struct TestEntityPrefabTag : IComponentData { }
   

    public class TestTimePrefabAuthoring : MonoBehaviour
    {
        public class TestEntityPrefabBaker : Baker<TestTimePrefabAuthoring>
        {
            public override void Bake(TestTimePrefabAuthoring authoring)
            {
                Entity prefab = GetEntity(TransformUsageFlags.None);
                AddBuffer<TestSyncBuffer>(prefab);
                AddComponent(prefab, new TestEntityPrefabTag {});
            }
        }
    }
}