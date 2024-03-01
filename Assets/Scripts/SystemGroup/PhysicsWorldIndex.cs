using Unity.Entities;
using Unity.Physics.Systems;

namespace SystemGroup
{
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class ClientPhysicsSystemGroup : CustomPhysicsSystemGroup
    {
        public const int PhysicsWorldIndex = 1;
        public ClientPhysicsSystemGroup() : base(PhysicsWorldIndex, true) {}
    }
    
    [WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
    [UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
    public partial class VisualizationPhysicsSystemGroup : CustomPhysicsSystemGroup
    {
        public VisualizationPhysicsSystemGroup () : base(1, false) { }
    }
}