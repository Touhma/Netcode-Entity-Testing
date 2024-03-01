using Netcode.Components;
using Unity.Entities;
using UnityEngine;

namespace Authorings
{
    public class GameSingletonAuthoring : MonoBehaviour
    {
        public GameObject Singleton = null;

        public class GameSingletonAuthoringBaker : Baker<GameSingletonAuthoring>
        {
            public override void Bake(GameSingletonAuthoring authoring)
            {
                Entity entity = GetEntity(TransformUsageFlags.Dynamic);
                AddComponent(entity, new GameSingletonComponentData { Singleton = GetEntity(authoring.Singleton, TransformUsageFlags.Dynamic) });
            }
        }
    }

    public struct GameSingletonComponentData : IComponentData
    {
        public Entity Singleton;
    }
}