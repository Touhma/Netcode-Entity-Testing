using Netcode.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace Netcode
{
    [UpdateInGroup(typeof(GhostInputSystemGroup))]
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    public partial class InputsSystem : SystemBase
    {

        private Controls _controls;

        protected override void OnCreate()
        {
            _controls = new Controls();
            _controls.Enable();
            EntityQueryBuilder builder = new(Allocator.Temp);
            builder.WithAny<PlayerInputData>();
            RequireForUpdate(GetEntityQuery(builder));
        }

        protected override void OnDestroy()
        {
            _controls.Disable();
        }

        protected override void OnUpdate()
        {
            Vector2 playerMove = _controls.Player.Move.ReadValue<Vector2>();
            /*
            foreach (RefRW<PlayerInputData> input in SystemAPI.Query<RefRW<PlayerInputData>>().WithAll<GhostOwnerIsLocal>())
            {
                input.ValueRW.Move = playerMove;
            }*/
            
        }

    }
}