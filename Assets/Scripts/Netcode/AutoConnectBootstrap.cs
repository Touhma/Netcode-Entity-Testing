using Unity.NetCode;
using UnityEngine.Scripting;

[Preserve]
public class AutoConnectBootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 0;
        /*
           if (AutoConnectPort != 0)
            {
                return base.Initialize(defaultWorldName);
            }
            else
            {
                AutoConnectPort = 0;
                CreateLocalWorld(defaultWorldName);
                return true;
            }
         */
        return false;
    }
}