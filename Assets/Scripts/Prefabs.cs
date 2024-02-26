using Unity.Entities;
using UnityEngine;

public class Prefabs : MonoBehaviour
{
     public GameObject Prefab = null;

     public class PrefabsBaker : Baker<Prefabs>
     {
          public override void Bake(Prefabs authoring)
          {
               Entity entity = GetEntity(TransformUsageFlags.Dynamic);
               AddComponent(entity, new PrefabsComponentData { Prefab = GetEntity(authoring.Prefab, TransformUsageFlags.Dynamic) });
          }
     }
}

public struct PrefabsComponentData : IComponentData
{
     public Entity Prefab;
} 
