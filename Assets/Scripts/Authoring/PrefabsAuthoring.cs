using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

public class PrefabsAuthoring : MonoBehaviour
{
     public GameObject Unit = null;
     public GameObject Player = null;

     public class PrefabsBaker : Baker<PrefabsAuthoring>
     {
          public override void Bake(PrefabsAuthoring authoring)
          {
               Entity entity = GetEntity(TransformUsageFlags.Dynamic);
               AddComponent(entity, new PrefabsComponentData { Unit = GetEntity(authoring.Unit, TransformUsageFlags.Dynamic) });
               AddComponent(entity, new PrefabsComponentData { Player = GetEntity(authoring.Player, TransformUsageFlags.Dynamic) });
          }
     }
}

public struct PrefabsComponentData : IComponentData
{
     public Entity Unit;
     public Entity Player;
} 
