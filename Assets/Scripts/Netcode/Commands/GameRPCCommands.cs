using Unity.Collections;
using Unity.NetCode;

namespace Netcode.Commands
{
    public struct ClientMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }
    
    public struct ServerMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }
    
    public struct GoInGameCommand : IRpcCommand { }
    
    public struct SpawnUnitRpcCommand : IRpcCommand { }
}