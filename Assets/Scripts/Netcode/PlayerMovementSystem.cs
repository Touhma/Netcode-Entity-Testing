using Authorings;
using Netcode.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

namespace Netcode
{
    [BurstCompile]
    public partial struct PlayerMovementSystem : ISystem
    {

        [BurstCompile]
        public void OnCreate(ref SystemState state)
        {
            EntityQueryBuilder builder = new(Allocator.Temp);
            builder.WithAll<PlayerData, PlayerInputData, LocalTransform>();
            state.RequireForUpdate(state.GetEntityQuery(builder));
        }

        [BurstCompile]
        public void OnUpdate(ref SystemState state)
        {
            PlayerMovementJob job = new()
            {
                DeltaTime = SystemAPI.Time.DeltaTime
            };
            state.Dependency = job.ScheduleParallel(state.Dependency);
        }

    }

    [BurstCompile]
    public partial struct PlayerMovementJob : IJobEntity
    {
        public float DeltaTime;
        public void Execute(PlayerData player, PlayerInputData input, ref LocalTransform transform)
        {
            float3 movement = new float3(input.Move.x, 0, input.Move.y) * player.Speed * DeltaTime;
            transform.Position = transform.Translate(movement).Position;
        }
    }
}