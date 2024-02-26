using Unity.NetCode;
using UnityEngine.Scripting;

namespace Netcode
{
    [Preserve]
    public class AutoConnectBootstrap : ClientServerBootstrap
    {
        public override bool Initialize(string defaultWorldName)
        {
            AutoConnectPort = 0;
            return false;
        }
    }
}