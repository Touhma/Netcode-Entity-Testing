using System;
using _Commons.Commands;
using _Commons.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

namespace _Clients.Systems
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation | WorldSystemFilterFlags.ThinClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial struct ClientTickUpdateSystem : ISystem
    {

        public void OnCreate(ref SystemState state)
        {
            state.RequireForUpdate<LockstepTick>();
        }

        public void OnUpdate(ref SystemState state)
        {
            EntityCommandBuffer buffer = new EntityCommandBuffer(Allocator.Temp);
            
            foreach ((RefRW<NetworkTime> tick, Entity entity) in SystemAPI.Query<RefRW<NetworkTime>>().WithEntityAccess())
            {
                Debug.Log(NetworkTimeSystem.TimestampMS + " --> current Client InterpolationTick = " + tick.ValueRW.InterpolationTick.SerializedData);
                Debug.Log(NetworkTimeSystem.TimestampMS + " --> current ServerTick SerializedData = " + tick.ValueRW.ServerTick.SerializedData);
                
                Entity heartBeat = buffer.CreateEntity();
                
                buffer.AddComponent(heartBeat, new SendRpcCommandRequest());
                buffer.AddComponent(heartBeat, new HeartBeatCommand()
                {
                    ServerTick = tick.ValueRO.ServerTick.SerializedData,
                    ServerTs = 0,
                    ClientTick = tick.ValueRW.InterpolationTick.SerializedData,
                    ClientTs = NetworkTimeSystem.TimestampMS
                });
                
                //Debug.Log(NetworkTimeSystem.TimestampMS + " --> current Client ServerTick = " + tick.ValueRW.ServerTick.SerializedData);
            }
           //*
            foreach ((RefRW<LockstepTick> tick, Entity entity) in SystemAPI.Query<RefRW<LockstepTick>>().WithEntityAccess())
            {
                
                tick.ValueRW.CurrentTick++; 
              //  Debug.Log(NetworkTimeSystem.TimestampMS + " --> current Client Tick = " + tick.ValueRW.CurrentTick);
            }
            //*/
            
            buffer.Playback(state.EntityManager);
            buffer.Dispose();
        }
    }
}