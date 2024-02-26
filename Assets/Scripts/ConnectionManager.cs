using System.Collections;
using System.Net;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;
using Hash128 = Unity.Entities.Hash128;
using PlayType = Unity.NetCode.ClientServerBootstrap.PlayType;

public class ConnectionManager : MonoBehaviour
{
    public string ListenAddress = "127.0.0.1";
    public string ConnectAddress = "127.0.0.1";
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

        StartCoroutine(StartHost());
    }

    public IEnumerator StartHost()
    {
        if (Role is PlayType.ClientAndServer or PlayType.Server)
        {
            ServerWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        }

        if (Role is PlayType.ClientAndServer or PlayType.Client)
        {
            ClientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        }

        DestroyLocalSimulationWorld();

        World.DefaultGameObjectInjectionWorld = ServerWorld ?? ClientWorld;

        SubScene[] subScenes = FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        if (ServerWorld != null)
        {
            while (!ServerWorld.IsCreated)
            {
                yield return null;
            }

            if (subScenes != null)
            {
                foreach (SubScene subScene in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new() { Flags = SceneLoadFlags.BlockOnStreamIn };
                    Entity sceneEntity = SceneSystem.LoadSceneAsync(ServerWorld.Unmanaged, new Hash128(subScene.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(ServerWorld.Unmanaged, sceneEntity))
                    {
                        ServerWorld.Update();
                    }
                }
            }

            using EntityQuery query = ServerWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(ListenAddress, Port));
        }

        if (ClientWorld != null)
        {
            while (!ClientWorld.IsCreated)
            {
                yield return null;
            }
            
            if (subScenes != null)
            {
                foreach (SubScene subScene in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new() { Flags = SceneLoadFlags.BlockOnStreamIn };
                    Entity sceneEntity = SceneSystem.LoadSceneAsync(ClientWorld.Unmanaged, new Hash128(subScene.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(ClientWorld.Unmanaged, sceneEntity))
                    {
                        ClientWorld.Update();
                    }
                }
            }
            
            using EntityQuery query = ClientWorld!.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(ClientWorld.EntityManager, NetworkEndpoint.Parse(ConnectAddress, Port));
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