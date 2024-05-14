using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VerySlowCollider : MonoBehaviour
{
    public SkinnedMeshRenderer meshRenderer;
    public MeshCollider collider;

    Mesh colliderMesh;
    private void Start()
    {
        colliderMesh = new Mesh(); 
    }
    public void UpdateCollider()
    {
        colliderMesh.Clear();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = colliderMesh;
    }

    void Update()
    {
        UpdateCollider();
    }
}
