using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SphereMove : MonoBehaviour
{
    MeshCollider meshCollider;
    int triangleIndex;
    Vector3 barycentric;
    int[] triangles;
    Vector3[] verticies;
    RaycastHit info;
    public void Init(RaycastHit info)
    {
        this.info = info;
        barycentric = info.barycentricCoordinate;
        triangleIndex = info.triangleIndex;
    }

    Vector3 GetPosition()
    {
        meshCollider = info.collider as MeshCollider;
        triangles = meshCollider.sharedMesh.triangles;
        verticies = meshCollider.sharedMesh.vertices;

        Vector3 vertexA, vertexB, vertexC;
        vertexA = verticies[triangles[triangleIndex * 3 + 0]];
        vertexB = verticies[triangles[triangleIndex * 3 + 1]];
        vertexC = verticies[triangles[triangleIndex * 3 + 2]];

        return barycentric.x * vertexA + barycentric.y * vertexB + barycentric.z * vertexC;
    }

    void Update()
    {
        transform.localPosition = GetPosition();
    }
}
