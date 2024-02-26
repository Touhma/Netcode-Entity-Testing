using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

namespace Authoring
{
    public class PlayerAuthoring : MonoBehaviour
    {
        public float Speed = 5f;

        public class PlayerAuthoringBaker : Baker<PlayerAuthoring>
        {
            public override void Bake(PlayerAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new PlayerComponentData { Speed = authoring.Speed });
                AddComponent(entity, new PlayerInputData());
            }
        }
    }

    public struct PlayerComponentData : IComponentData
    {
        public float Speed;
    }
    
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInputData : IInputComponentData
    {
        public float2 Move;
        public InputEvent Jump;
    }
}