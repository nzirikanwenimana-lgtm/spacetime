using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshCollider))]
public class SpaceTimeGravityGrid : MonoBehaviour
{
    [Header("Grid")]
    public int gridResolution = 100;
    public float gridSize = 20f;

    [Header("Gravity Bodies")]
    public Transform mass1;
    public Transform mass2;

    [Header("Mass")]
    public float mass1Value = 300f;
    public float mass2Value = 100f;

    [Header("Gravity")]
    public float gravityStrength = 1f;
    public float influenceRadius = 5f;
    public float maximumDepth = 2f;

    private Mesh mesh;
    private MeshCollider meshCollider;

    private Vector3[] originalVertices;
    private Vector3[] deformedVertices;

    void Start()
    {
        meshCollider = GetComponent<MeshCollider>();

        CreateGrid();
    }

    void LateUpdate()
    {
        if (mass1 == null || mass2 == null)
            return;

        DeformGrid();
    }

    // --------------------------------------------------
    // CREATE GRID
    // --------------------------------------------------

    void CreateGrid()
    {
        mesh = new Mesh();
        mesh.name = "SpaceTimeGridMesh";

        int vertexCount =
            (gridResolution + 1) *
            (gridResolution + 1);

        Vector3[] vertices =
            new Vector3[vertexCount];

        Vector2[] uv =
            new Vector2[vertexCount];

        int[] triangles =
            new int[gridResolution *
                     gridResolution * 6];

        float step =
            gridSize / gridResolution;

        int vertexIndex = 0;

        // Create vertices
        for (int z = 0; z <= gridResolution; z++)
        {
            for (int x = 0; x <= gridResolution; x++)
            {
                float px =
                    -gridSize / 2f +
                    x * step;

                float pz =
                    -gridSize / 2f +
                    z * step;

                vertices[vertexIndex] =
                    new Vector3(px, 0f, pz);

                uv[vertexIndex] =
                    new Vector2(
                        (float)x / gridResolution,
                        (float)z / gridResolution
                    );

                vertexIndex++;
            }
        }

        // Create triangles
        int triangleIndex = 0;

        for (int z = 0; z < gridResolution; z++)
        {
            for (int x = 0; x < gridResolution; x++)
            {
                int bottomLeft =
                    z * (gridResolution + 1) + x;

                int bottomRight =
                    bottomLeft + 1;

                int topLeft =
                    bottomLeft +
                    gridResolution + 1;

                int topRight =
                    topLeft + 1;

                triangles[triangleIndex++] =
                    bottomLeft;

                triangles[triangleIndex++] =
                    topLeft;

                triangles[triangleIndex++] =
                    topRight;

                triangles[triangleIndex++] =
                    bottomLeft;

                triangles[triangleIndex++] =
                    topRight;

                triangles[triangleIndex++] =
                    bottomRight;
            }
        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshFilter>().mesh = mesh;

        originalVertices = mesh.vertices;

        deformedVertices =
            new Vector3[originalVertices.Length];

        // Give MeshCollider the same mesh
        meshCollider.sharedMesh = mesh;
    }

    // --------------------------------------------------
    // DEFORM GRID
    // --------------------------------------------------

    void DeformGrid()
    {
        Vector3 mass1Local =
            transform.InverseTransformPoint(
                mass1.position
            );

        Vector3 mass2Local =
            transform.InverseTransformPoint(
                mass2.position
            );

        for (int i = 0;
             i < originalVertices.Length;
             i++)
        {
            Vector3 vertex =
                originalVertices[i];

            float deformation1 =
                CalculateGravity(
                    vertex,
                    mass1Local,
                    mass1Value
                );

            float deformation2 =
                CalculateGravity(
                    vertex,
                    mass2Local,
                    mass2Value
                );

            float totalDeformation =
                deformation1 +
                deformation2;

            deformedVertices[i] =
                vertex;

            deformedVertices[i].y =
                originalVertices[i].y -
                totalDeformation;
        }

        // Update visual mesh
        mesh.vertices =
            deformedVertices;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        // IMPORTANT:
        // Update the physics collider
        meshCollider.sharedMesh = null;
        meshCollider.sharedMesh = mesh;
    }

    // --------------------------------------------------
    // GRAVITY
    // --------------------------------------------------

    float CalculateGravity(
        Vector3 vertex,
        Vector3 massPosition,
        float mass
    )
    {
        float distance =
            Vector2.Distance(
                new Vector2(
                    vertex.x,
                    vertex.z
                ),
                new Vector2(
                    massPosition.x,
                    massPosition.z
                )
            );

        if (distance >= influenceRadius)
            return 0f;

        float normalizedDistance =
            1f -
            distance / influenceRadius;

        float influence =
            normalizedDistance *
            normalizedDistance *
            normalizedDistance;

        float massMultiplier =
            mass / 100f;

        float deformation =
            influence *
            gravityStrength *
            massMultiplier *
            maximumDepth;

        return deformation;
    }
}