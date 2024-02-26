using Unity.Collections;
using Unity.NetCode;

namespace Netcode.CommandsRpc
{
    public struct SpawnUnitRpcCommand : IRpcCommand { }

    public struct ClientMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }

    public struct ServerMessageRpcCommand : IRpcCommand
    {
        public FixedString64Bytes Message;
    }

    public struct GoInGameCommand : IRpcCommand { }
}