using Unity.Entities;
using System.Collections.Generic;
using System;
using UnityEngine.Experimental.LowLevel;
using UnityEngine.Experimental.PlayerLoop;

public class Bootstrap : ComponentSystem, ICustomBootstrap {
    Client client;
    Server server;

    CustomUpdateSystem customUpdateSystem;
    bool init = false;

    protected override void OnUpdate() {
    }

    public List<Type> Initialize(List<Type> types) {
        if (World.Active.Name == "Default World") {
            DoBootstrap();
            return new List<Type>();
        }

        if (World.Active.Name == "Server World") {
            for (int ti = 0; ti < types.Count; ti++) {
                var attrs = types[ti].GetCustomAttributes(false);
                for (int i = 0; i < attrs.Length; i++) {
                    if (attrs[i].GetType() == typeof(UpdateInGroupAttribute)) {
                        UpdateInGroupAttribute group = (UpdateInGroupAttribute) attrs[i];
                        if (group.GroupType == typeof(PresentationSystemGroup) || group.GroupType == typeof(SimulationSystemGroup)) {
                            types.RemoveAt(ti);
                            ti--;
                        }
                    }
                }
            }
        }

        return types;
    }

    void DoBootstrap() {
        DefaultWorldInitialization.Initialize("Server World", false);
        DefaultWorldInitialization.Initialize("Client World", false);
    }
    
}

class CustomUpdateSystem {
    List<ScriptBehaviourManager> systems;

    public CustomUpdateSystem(params ScriptBehaviourManager[] systems) {
        this.systems = new List<ScriptBehaviourManager>(systems);
    }

    public void FixedUpdate() {
        foreach (ScriptBehaviourManager system in systems) {
            system.Update();
        }
    }
}
