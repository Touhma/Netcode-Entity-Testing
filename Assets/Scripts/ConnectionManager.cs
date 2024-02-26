using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using UnityEngine;
using PlayType = Unity.NetCode.ClientServerBootstrap.PlayType;

public class ConnectionManager : MonoBehaviour
{
    public string Address = "127.0.0.1";
    public const ushort Port = 7979;

    public static World ClientWorld = null;
    public static World ServerWorld = null;

    public static PlayType Role = PlayType.ClientAndServer;

    private void Start()
    {
        if (Application.isEditor)
        {
            Role = PlayType.ClientAndServer;
        }
        else if (Application.platform is RuntimePlatform.LinuxServer or RuntimePlatform.WindowsServer or RuntimePlatform.OSXServer)
        {
            Role = PlayType.Server;
        }
        else
        {
            Role = PlayType.Client;
        }

        StartHost();
    }

    public void StartHost()
    {
        if (ClientServerBootstrap.RequestedPlayType != PlayType.ClientAndServer)
        {
            Debug.LogError($"Creating client/server worlds is not allowed if playmode is set to {ClientServerBootstrap.RequestedPlayType}");
            return;
        }
        
        if (Role is PlayType.ClientAndServer or PlayType.Server)
        {
            ServerWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        }

        if (Role is PlayType.ClientAndServer or PlayType.Client)
        {
            ClientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        }
        
        DestroyLocalSimulationWorld();
        
        World.DefaultGameObjectInjectionWorld ??= ServerWorld;
        
        if (ServerWorld != null)
        {
            using EntityQuery query = ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(ClientServerBootstrap.DefaultListenAddress.WithPort(Port));
        }

        if (ClientWorld != null)
        {
            IPAddress serverAddress = IPAddress.Parse(Address);
            NativeArray<byte> nativeArrayAddress = new NativeArray<byte>(serverAddress.GetAddressBytes().Length, Allocator.Temp);
            nativeArrayAddress.CopyFrom(serverAddress.GetAddressBytes());
            NetworkEndpoint endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.SetRawAddressBytes(nativeArrayAddress);
            endpoint.Port = Port;
            using EntityQuery query = ClientWorld!.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager, endpoint);
        }
        
    }


    private static void DestroyLocalSimulationWorld()
    {
        foreach (World world in World.All)
        {
            if (world.Flags != WorldFlags.Game) continue;
            world.Dispose();
            break;
        }
    }
    
}