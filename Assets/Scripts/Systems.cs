using Unity.Entities;
using Unity.Transforms;
using Unity.Rendering;
using UnityEngine;

[DisableAutoCreation]
class Client : ComponentSystem {
    EntityManager entityManager;
    SimulationSystemGroup simGroup;
    PresentationSystemGroup presGroup;

    protected override void OnUpdate() {
        Debug.Log("CLIENT UPDATE");
        simGroup.Update();
        presGroup.Update();
        return;
    }

    protected override void OnCreateManager() {
        entityManager = World.GetOrCreateManager<EntityManager>();
        simGroup = World.GetOrCreateManager<SimulationSystemGroup>();
        presGroup = World.GetOrCreateManager<PresentationSystemGroup>();
        Entity player = entityManager.CreateEntity(typeof(RenderMesh), typeof(LocalToWorld), typeof(Translation), typeof(Rotation), typeof(NonUniformScale));

        GameObject meshPrefab = (GameObject)Resources.Load("Meshes/player");
        entityManager.SetSharedComponentData(player, new RenderMesh {
            mesh = meshPrefab.GetComponent<MeshFilter>().sharedMesh,
            material = meshPrefab.GetComponent<Renderer>().sharedMaterial,
        });

        NonUniformScale scale = new NonUniformScale {
            Value = meshPrefab.transform.localScale,
        };
        entityManager.SetComponentData(player, scale);

        Rotation newRotation = new Rotation {
            Value = meshPrefab.transform.rotation,
        };
        entityManager.SetComponentData(player, newRotation);
    }
}

[DisableAutoCreation]
class Server : ComponentSystem {
    EntityManager entityManager;

    protected override void OnUpdate() {
        Debug.Log("SERVER UPDATE");
        return;
    }

    protected override void OnCreateManager() {
        entityManager = World.GetOrCreateManager<EntityManager>();
    }
}