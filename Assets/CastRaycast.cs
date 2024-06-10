using System.Collections.Generic;
using UnityEngine;

public class CastRaycast : MonoBehaviour
{
    public MeshFilter meshFilter;

    public int resolution = 20;
    public float epsilon = 0.001f;
    public SphereMove[] sphereMoves;

    bool clicked = false;
    public List<Vertex[]> caputeredPositions;

    public SphereMove projectile;

    public float currentTime = 0;
    public float animTime = 2f;

    private void Start()
    {
        caputeredPositions = new List<Vertex[]>();
    }
    SphereMove Raycast(int x, int y)
    {
        Ray ray = GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);
        Vector3 startingOrigin = ray.origin;
        ray.origin = new Vector3(startingOrigin.x + (x * epsilon), startingOrigin.y + (y * epsilon), startingOrigin.z);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Transform objectHit = hit.transform;
            if (objectHit != null && objectHit.name == "RL_G6_Body")
            {
                SphereMove clone = Instantiate(projectile, hit.point, Quaternion.identity);
                clone.transform.parent = objectHit.transform;
                clone.Init(hit);
                return clone;
            }
        }
        return null;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && !clicked)
        {
            clicked = true;
            sphereMoves = new SphereMove[(resolution * 2) * (resolution * 2)];

            for (int i = -resolution; i < resolution; i++)
            {
                for (int j = -resolution; j < resolution; j++)
                {
                    SphereMove sphere = Raycast(i, j);
                    sphereMoves[(i + resolution) * resolution * 2 + (j + resolution)] = sphere;
                }
            }

        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            CaputeredPositions();
            CreateMesh();
        }


        AnimateFace();
    }

    private void CaputeredPositions()
    {
        if (sphereMoves == null)
            return;

        Vertex[] temp = new Vertex[(resolution * 2) * (resolution * 2)];
        for (int i = 0; i < (resolution * 2) * (resolution * 2); i++)
        {           
            SphereMove ball = sphereMoves[i];

            if(ball == null)
            {
                temp[i] = null;
                continue;
            }

            temp[i] = new Vertex(ball.gameObject.transform.localPosition);
        }
        caputeredPositions.Add(temp);
    }

    void CreateMesh()
    {
        Vertex[] vertices = caputeredPositions[0];

        Vector3[] meshVertices = new Vector3[vertices.Length];
        int size = resolution*2;

        Mesh mesh = new Mesh();
        mesh.Clear();

        for (int i = 0; i < vertices.Length; i++)
        {
            if (vertices[i] == null)
            {
                meshVertices[i] = new Vector3(0, 0, 0);
            }
            else
            {
                meshVertices[i] = vertices[i].position;
            }
        }

        int[] triangles = new int[(size - 1) * (size - 1) * 6];
        int triangleIndex = 0;
        for (int y = 0; y < size - 1; y++)
        {
            for (int x = 0; x < size - 1; x++)
            {
                int vertexIndex = y * size + x;

                if (vertices[vertexIndex] != null &&
                    vertices[vertexIndex + 1] != null &&
                    vertices[vertexIndex + size] != null )
                {
                    triangles[triangleIndex++] = vertexIndex;
                    triangles[triangleIndex++] = vertexIndex + 1;
                    triangles[triangleIndex++] = vertexIndex + size;
                }


                if (vertices[vertexIndex + 1] != null &&
                    vertices[vertexIndex + size + 1] != null &&
                    vertices[vertexIndex + size] != null)
                {
                    triangles[triangleIndex++] = vertexIndex + 1;
                    triangles[triangleIndex++] = vertexIndex + size + 1;
                    triangles[triangleIndex++] = vertexIndex + size;
                }
            }
        }

        mesh.vertices = meshVertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        meshFilter.mesh = mesh;
    }

    
    void AnimateFace()
    {
        if (currentTime > animTime)
            currentTime = 0;

        if (caputeredPositions.Count <= 2)
            return;

        Vertex[] startingPoint = caputeredPositions[caputeredPositions.Count - 2];
        Vertex[] endingPoint = caputeredPositions[caputeredPositions.Count - 1];

        Vector3[] currentPosition = new Vector3[startingPoint.Length];
        float phaseFraction = currentTime / animTime;

        for (int i = 0; i < startingPoint.Length; i++)
        {
            if (startingPoint[i] == null)
            {
                currentPosition[i] = new Vector3(0, 0, 0);
            }
            else
            {
                Vector3 dif = endingPoint[i].position - startingPoint[i].position;
                float C = (1f - Mathf.Cos(phaseFraction * Mathf.PI)) / 2f;
                currentPosition[i] = startingPoint[i].position + dif * C;
            }
        }

        meshFilter.mesh.vertices = currentPosition;
        currentTime += Time.deltaTime;
    }
}

public class Vertex
{
    public Vector3 position;
    public Vertex(Vector3 position)
    {
        this.position = position;
    }
}
