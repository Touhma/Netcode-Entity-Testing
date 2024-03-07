using Unity.Entities;
using UnityEngine;

namespace Authorings
{
    public class PrefabsAuthoring : MonoBehaviour
    {
        public GameObject Unit = null;
        public GameObject Player = null;
        public GameObject Time = null;

        public class PrefabsAuthoringBaker : Baker<PrefabsAuthoring>
        {
            public override void Bake(PrefabsAuthoring authoring)
            {
                Entity unitPrefab = default;
                Entity playerPrefab = default;
                Entity timePrefab = default;
                
                if (authoring.Unit != null)
                {
                    unitPrefab = GetEntity(authoring.Unit, TransformUsageFlags.Dynamic);
                }

                if (authoring.Player != null)
                {
                    playerPrefab = GetEntity(authoring.Player, TransformUsageFlags.Dynamic);
                }
                
                if (authoring.Time != null)
                {
                    timePrefab = GetEntity(authoring.Time, TransformUsageFlags.Dynamic);
                }

                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PrefabsData
                {
                    Unit = unitPrefab,
                    Player = playerPrefab,
                    Time = timePrefab
                });
            }
        }
    }

    public struct PrefabsData : IComponentData
    {
        public Entity Unit;
        public Entity Player;
        public Entity Time;
    }
}