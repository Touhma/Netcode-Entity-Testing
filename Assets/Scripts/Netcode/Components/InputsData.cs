using Unity.Mathematics;
using Unity.NetCode;

namespace Netcode.Components
{
    [GhostComponent(PrefabType = GhostPrefabType.AllPredicted)]
    public struct PlayerInputData : IInputComponentData
    {
        public float2 Move;
        public InputEvent Jump;
    }
}