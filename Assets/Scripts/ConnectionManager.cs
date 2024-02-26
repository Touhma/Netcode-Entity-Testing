using System.Collections;
using Unity.Entities;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Scenes;
using UnityEngine;
using PlayType = Unity.NetCode.ClientServerBootstrap.PlayType;

public class ConnectionManager : MonoBehaviour
{

    [SerializeField] private string _listenIP = "127.0.0.1";
    [SerializeField] private string _connectIP = "127.0.0.1";
    [SerializeField] private ushort _port = 7979;

    public static World serverWorld = null;
    public static World clientWorld = null;


    private static PlayType _role = PlayType.ClientAndServer;

    private void Start()
    {
        if (Application.isEditor)
        {
            _role = PlayType.ClientAndServer;
        }
        else if (Application.platform is RuntimePlatform.LinuxServer or RuntimePlatform.WindowsServer or RuntimePlatform.OSXServer)
        {
            _role = PlayType.Server;
        }
        else
        {
            _role = PlayType.Client;
        }
        StartCoroutine(Connect());
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

    private IEnumerator Connect()
    {
        if (_role is PlayType.ClientAndServer or PlayType.Server)
        {
            serverWorld = ClientServerBootstrap.CreateServerWorld("ServerWorld");
        }

        if (_role is PlayType.ClientAndServer or PlayType.Client)
        {
            clientWorld = ClientServerBootstrap.CreateClientWorld("ClientWorld");
        }

        DestroyLocalSimulationWorld();
   
        if (serverWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = serverWorld;
        }
        else if (clientWorld != null)
        {
            World.DefaultGameObjectInjectionWorld = clientWorld;
        }

        SubScene[] subScenes = FindObjectsByType<SubScene>(FindObjectsInactive.Include, FindObjectsSortMode.None);

        if (serverWorld != null)
        {
            while (!serverWorld.IsCreated)
            {
                yield return null;
            }
            if (subScenes != null)
            {
                foreach (SubScene t in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new () { Flags = SceneLoadFlags.BlockOnStreamIn };
                    Entity sceneEntity = SceneSystem.LoadSceneAsync(serverWorld.Unmanaged, new Unity.Entities.Hash128(t.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(serverWorld.Unmanaged, sceneEntity))
                    {
                        serverWorld.Update();
                    }
                }
            }
            using EntityQuery query = serverWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Listen(NetworkEndpoint.Parse(_listenIP, _port));
        }

        if (clientWorld != null)
        {
            while (!clientWorld.IsCreated)
            {
                yield return null;
            }
            if (subScenes != null)
            {
                foreach (SubScene t in subScenes)
                {
                    SceneSystem.LoadParameters loadParameters = new () { Flags = SceneLoadFlags.BlockOnStreamIn };
                    Entity sceneEntity = SceneSystem.LoadSceneAsync(clientWorld.Unmanaged, new Unity.Entities.Hash128(t.SceneGUID.Value), loadParameters);
                    while (!SceneSystem.IsSceneLoaded(clientWorld.Unmanaged, sceneEntity))
                    {
                        clientWorld.Update();
                    }
                }
            }
            using EntityQuery query = clientWorld.EntityManager.CreateEntityQuery(ComponentType.ReadWrite<NetworkStreamDriver>());
            query.GetSingletonRW<NetworkStreamDriver>().ValueRW.Connect(clientWorld.EntityManager, NetworkEndpoint.Parse(_connectIP, _port));
        }
    }

}