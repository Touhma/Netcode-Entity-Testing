using _Commons.Components;
using Unity.Entities;

namespace _Commons.SystemGroups
{
    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class TickFlowGroup : ComponentSystemGroup { } // Main Factory Stuff group

    [UpdateInGroup(typeof(TickFlowGroup))]
    [UpdateBefore(typeof(UpdateTickGroup))]
    public partial class InitTickGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(TickFlowGroup))]
    [UpdateBefore(typeof(PostTickGroup))]
    public partial class UpdateTickGroup : ComponentSystemGroup { }
    
    [UpdateInGroup(typeof(TickFlowGroup))]
    public partial class PostTickGroup : ComponentSystemGroup { }

    [UpdateInGroup(typeof(SimulationSystemGroup), OrderFirst = true)]
    public partial class UpdateOnTickGroup : ComponentSystemGroup
    {
        protected override void OnCreate()
        {
            CheckedStateRef.RequireForUpdate<TickUpdateTag>();
            base.OnCreate();
        }
    }
}