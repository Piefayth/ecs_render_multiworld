using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;

public class Run : MonoBehaviour {
    World serverWorld;
    World clientWorld;

    Client client;
    Server server;

    CustomUpdateSystem customUpdateSystem;

    void Awake() {
        foreach (World world in World.AllWorlds) {
            if (world.Name == "Server World") {
                serverWorld = world;
                server = serverWorld.GetOrCreateManager<Server>();
            }

            if (world.Name == "Client World") {
                clientWorld = world;
                client = clientWorld.GetOrCreateManager<Client>();
            }
        }

        CustomUpdateSystem customUpdateSystem = new CustomUpdateSystem(new ComponentSystem[] { client, server });

        PlayerLoopSystem playerLoopSystem = ScriptBehaviourUpdateOrder.CurrentPlayerLoop;
        PlayerLoopSystem[] subsystems = playerLoopSystem.subSystemList;

        for (int i = 0; i < subsystems.Length; i++) {
            PlayerLoopSystem system = subsystems[i];

            if (system.type == typeof(FixedUpdate)) {
                PlayerLoopSystem fixedUpdateSystem = system;

                List<PlayerLoopSystem> fixedUpdateSystems = new List<PlayerLoopSystem>(system.subSystemList);
                PlayerLoopSystem customPlayerLoop = new PlayerLoopSystem();
                customPlayerLoop.type = typeof(CustomUpdateSystem);
                customPlayerLoop.updateDelegate = customUpdateSystem.FixedUpdate;
                fixedUpdateSystems.Add(customPlayerLoop);

                fixedUpdateSystem.subSystemList = fixedUpdateSystems.ToArray();
                playerLoopSystem.subSystemList[i] = fixedUpdateSystem;
            }
        }

        ScriptBehaviourUpdateOrder.SetPlayerLoop(playerLoopSystem);
    }
}
