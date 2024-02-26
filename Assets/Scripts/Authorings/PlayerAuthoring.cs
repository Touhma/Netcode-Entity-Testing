using Netcode.Components;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Serialization;

namespace Authorings
{
    public class PlayerAuthoring : MonoBehaviour
    {
        [FormerlySerializedAs("speed")] public float Speed = 5f;

        public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerData
                {
                    Speed = authoring.Speed
                });
                AddComponent<PlayerInputData>(entity);
            }
        }
    }

    public struct PlayerData : IComponentData
    {
        public float Speed;
    }
}