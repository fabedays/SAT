using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class collisionSAT : MonoBehaviour
{
    public GameObject cubo1;
    public GameObject cubo2;
    // Start is called before the first frame update
    void Start()
    {
        MeshRenderer meshRenderer = cubo1.GetComponent<MeshRenderer>();
        Material material = new Material(meshRenderer.sharedMaterial);
        material.color = Color.green;
        meshRenderer.material = material;
    }

    public bool increase_vert2d = false, increase_vert3d = false;
    public int vert3d;
    public int vert2d;
    public AudioSource collisionsound;

    // há 5 funções que práticamente fazem quase o mesmo
    //project3dquick() chama o project3 várias vezes e evita que o vertice que ta a ver passe dos 22 (escolhido para o cubo)
    //project3d faz uma única projeção 3d
    //project2d faz uma única projeção 2d, chamada pela project3d
    //d3 check faz a verificacao em todas os vértices (devia ser arestas mas fiz vertices) e devolve bool para colisão
    //d2 check faz a verificacao em 2d e devolve bool para colisão
    
    

    void Update()
    {
        project3dquick();
        // MeshRenderer meshRenderer = cubo1.GetComponent<MeshRenderer>();
        // Material material = new Material(meshRenderer.sharedMaterial);
        // if (d3check())
        //     material.color = Color.red;
        // else
        //     material.color = Color.green;

        // meshRenderer.material = material;

    }
    [ContextMenu("d3check")]
    bool d3check()
    {
        CleanUp();
        Mesh mesh = cubo1.GetComponent<MeshFilter>().mesh;
        Mesh mesh2 = cubo2.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] vertices2 = mesh2.vertices;

        for (int d3vert = 0; d3vert < vertices.Length - 1; d3vert++)
        {
            Vector3 directionVector = vertices[d3vert] - vertices[d3vert + 1];

            // Normalize direction vector
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
            Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
            Plane projectionPlane = new Plane(directionVector.normalized, vertices[d3vert]);

            // Project object vertices perpendicularly to perpendicular vector
            Vector3[] projectedVertices = new Vector3[vertices.Length];
            Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
            }
            for (int i = 0; i < vertices2.Length; i++)
            {
                projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
            }

            // Create GameObjects to hold the projected meshes
            GameObject plano1 = new GameObject("ProjectedMesh1");
            GameObject plano2 = new GameObject("ProjectedMesh2");
            plano1.tag = "garbage";
            plano2.tag = "garbage";

            // Add MeshFilters to hold the meshes
            MeshFilter meshFilter1 = plano1.AddComponent<MeshFilter>();
            MeshFilter meshFilter2 = plano2.AddComponent<MeshFilter>();

            // Create Meshes for the projected shapes
            Mesh projectedMesh1 = new Mesh();
            Mesh projectedMesh2 = new Mesh();

            // Assign vertices to the meshes
            projectedMesh1.vertices = projectedVertices;
            projectedMesh2.vertices = projectedVertices2;

            // Define triangles for the projected shapes
            int[] triangles1 = new int[(projectedVertices.Length - 2) * 3];
            int[] triangles2 = new int[(projectedVertices2.Length - 2) * 3];

            for (int i = 0; i < projectedVertices.Length - 2; i++)
            {
                triangles1[i * 3] = 0;
                triangles1[i * 3 + 1] = i + 1;
                triangles1[i * 3 + 2] = i + 2;
            }

            for (int i = 0; i < projectedVertices2.Length - 2; i++)
            {
                triangles2[i * 3] = 0;
                triangles2[i * 3 + 1] = i + 1;
                triangles2[i * 3 + 2] = i + 2;
            }

            // Assign triangles to the meshes
            projectedMesh1.triangles = triangles1;
            projectedMesh2.triangles = triangles2;

            // Create normals for the meshes (assuming they face upwards)
            Vector3[] normals1 = new Vector3[projectedVertices.Length];
            Vector3[] normals2 = new Vector3[projectedVertices2.Length];
            Vector3 normal = Vector3.up;

            for (int i = 0; i < projectedVertices.Length; i++)
            {
                normals1[i] = normal;
            }

            for (int i = 0; i < projectedVertices2.Length; i++)
            {
                normals2[i] = normal;
            }

            // Assign normals to the meshes
            projectedMesh1.normals = normals1;
            projectedMesh2.normals = normals2;

            // Set the meshes to the MeshFilters
            meshFilter1.mesh = projectedMesh1;
            meshFilter2.mesh = projectedMesh2;

            // Position the projected meshes
            plano1.transform.position = cubo1.transform.position + directionVector.normalized * 2;
            plano2.transform.position = cubo2.transform.position + directionVector.normalized * 2;

            // Equalize distances
            Vector3 displacement = cubo2.transform.position - cubo1.transform.position;
            float dist = Vector3.Dot(displacement, directionVector.normalized);

            plano1.transform.position = plano1.transform.position + directionVector.normalized * dist;

            if (!d2check(plano1, plano2, d3vert))
            {
                return false;
            }
            else
            {
                //  Debug.Log("other stuff");
            }
        }

        for (int d3vert = 0; d3vert < vertices2.Length - 1; d3vert++)
        {
            Vector3 directionVector = vertices2[d3vert] - vertices2[d3vert + 1];

            // Normalize direction vector
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
            Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
            Plane projectionPlane = new Plane(directionVector.normalized, vertices2[d3vert]);

            // Project object vertices perpendicularly to perpendicular vector
            Vector3[] projectedVertices = new Vector3[vertices.Length];
            Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
            }
            for (int i = 0; i < vertices2.Length; i++)
            {
                projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
            }

            // Create GameObjects to hold the projected meshes
            GameObject plano1 = new GameObject("ProjectedMesh1");
            GameObject plano2 = new GameObject("ProjectedMesh2");
            plano1.tag = "garbage";
            plano2.tag = "garbage";

            // Add MeshFilters to hold the meshes
            MeshFilter meshFilter1 = plano1.AddComponent<MeshFilter>();
            MeshFilter meshFilter2 = plano2.AddComponent<MeshFilter>();

            // Create Meshes for the projected shapes
            Mesh projectedMesh1 = new Mesh();
            Mesh projectedMesh2 = new Mesh();

            // Assign vertices to the meshes
            projectedMesh1.vertices = projectedVertices;
            projectedMesh2.vertices = projectedVertices2;

            // Define triangles for the projected shapes
            int[] triangles1 = new int[(projectedVertices.Length - 2) * 3];
            int[] triangles2 = new int[(projectedVertices2.Length - 2) * 3];

            for (int i = 0; i < projectedVertices.Length - 2; i++)
            {
                triangles1[i * 3] = 0;
                triangles1[i * 3 + 1] = i + 1;
                triangles1[i * 3 + 2] = i + 2;
            }

            for (int i = 0; i < projectedVertices2.Length - 2; i++)
            {
                triangles2[i * 3] = 0;
                triangles2[i * 3 + 1] = i + 1;
                triangles2[i * 3 + 2] = i + 2;
            }

            // Assign triangles to the meshes
            projectedMesh1.triangles = triangles1;
            projectedMesh2.triangles = triangles2;

            // Create normals for the meshes (assuming they face upwards)
            Vector3[] normals1 = new Vector3[projectedVertices.Length];
            Vector3[] normals2 = new Vector3[projectedVertices2.Length];
            Vector3 normal = Vector3.up;

            for (int i = 0; i < projectedVertices.Length; i++)
            {
                normals1[i] = normal;
            }

            for (int i = 0; i < projectedVertices2.Length; i++)
            {
                normals2[i] = normal;
            }

            // Assign normals to the meshes
            projectedMesh1.normals = normals1;
            projectedMesh2.normals = normals2;

            // Set the meshes to the MeshFilters
            meshFilter1.mesh = projectedMesh1;
            meshFilter2.mesh = projectedMesh2;

            // Position the projected meshes
            plano1.transform.position = cubo1.transform.position + directionVector.normalized * 2;
            plano2.transform.position = cubo2.transform.position + directionVector.normalized * 2;

            // Equalize distances
            Vector3 displacement = cubo2.transform.position - cubo1.transform.position;
            float dist = Vector3.Dot(displacement, directionVector.normalized);

            plano1.transform.position = plano1.transform.position + directionVector.normalized * dist;

            if (!d2check(plano1, plano2, d3vert))
            {
                return false;
            }
            else
            {
                // Debug.Log("other stuff");
            }
        }

        Debug.Log("cheguei aqui");
        return true;

    }
    bool d2check(GameObject proj1, GameObject proj2, int d3vert)
    {
        Mesh mesh = proj1.GetComponent<MeshFilter>().mesh;
        Mesh mesh2 = proj2.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] vertices2 = mesh2.vertices;
        vertices = RemoveDuplicates(vertices);
        vertices2 = RemoveDuplicates(vertices2);
        

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = vertices[i] + proj1.transform.position;
        for (int i = 0; i < vertices2.Length; i++)
            vertices2[i] = vertices2[i] + proj2.transform.position;


        for (int d2vert = 0; d2vert < vertices.Length; d2vert++)
        {
            CleanUp();
            int anti_repeat_offset = 1;
            try
            {
                while (vertices[d2vert] == vertices[d2vert + anti_repeat_offset])
                {
                    anti_repeat_offset++;
                }
            }
            catch (System.Exception)
            {
                continue;
            }
            Vector3 directionVector = vertices[d2vert] - vertices[d2vert + anti_repeat_offset];


            // Normalize direction vector
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
            Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
            Plane projectionPlane = new Plane(directionVector.normalized, vertices[d2vert]);

            // Project object vertices perpendicularly to perpendicular vector
            Vector3[] projectedVertices = new Vector3[vertices.Length];
            Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
            }
            for (int i = 0; i < vertices2.Length; i++)
            {
                projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
            }

            GameObject ponto11 = new GameObject("Projectedline11");
            GameObject ponto12 = new GameObject("Projectedline12");
            ponto11.tag = "garbage";
            ponto12.tag = "garbage";

            int min1 = 0, min2 = 0, max1 = 0, max2 = 0;
            for (int i = 0; i < projectedVertices.Length; i++)
            {
                if (IsPointSmaller(projectedVertices[i], projectedVertices[min1]))
                {
                    min1 = i;
                }
                if (!IsPointSmaller(projectedVertices[i], projectedVertices[max1]))
                {
                    max1 = i;
                }
            }
            for (int i = 0; i < projectedVertices2.Length; i++)
            {
                if (IsPointSmaller(projectedVertices2[i], projectedVertices2[min2]))
                {
                    min2 = i;
                }
                if (!IsPointSmaller(projectedVertices2[i], projectedVertices2[max2]))
                {
                    max2 = i;
                }
            }

            ponto11.transform.position = projectedVertices[min1] + directionVector.normalized * 2;
            ponto12.transform.position = projectedVertices[max1] + directionVector.normalized * 2;

            GameObject ponto21 = new GameObject("Projectedline21");
            GameObject ponto22 = new GameObject("Projectedline22");
            ponto21.tag = "garbage";
            ponto22.tag = "garbage";
            ponto21.transform.position = projectedVertices2[min2] + directionVector.normalized * 2;
            ponto22.transform.position = projectedVertices2[max2] + directionVector.normalized * 2;

            if (!LinesIntersect(ponto11.transform.position, ponto12.transform.position, ponto21.transform.position, ponto22.transform.position))
            {
                Debug.Log("return false1");
                return false;
            }
        }

        for (int d2vert = 0; d2vert < vertices2.Length; d2vert++)
        {
            CleanUp();

            int anti_repeat_offset = 1;

            try
            {
                while (vertices2[d2vert] == vertices2[d2vert + anti_repeat_offset])
                {
                    anti_repeat_offset++;
                }

            }
            catch (System.Exception)
            {
                continue;
            }
            Vector3 directionVector = vertices2[d2vert] - vertices2[d2vert + anti_repeat_offset];


            // Normalize direction vector
            Vector3 normalizedDirection = directionVector.normalized;

            Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
            Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
            Plane projectionPlane = new Plane(directionVector.normalized, vertices2[d2vert]);

            // Project object vertices perpendicularly to perpendicular vector
            Vector3[] projectedVertices = new Vector3[vertices.Length];
            Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
            for (int i = 0; i < vertices.Length; i++)
            {
                projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
            }
            for (int i = 0; i < vertices2.Length; i++)
            {
                projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
            }

            GameObject ponto11 = new GameObject("Projectedline11");
            GameObject ponto12 = new GameObject("Projectedline12");
            ponto11.tag = "garbage";
            ponto12.tag = "garbage";

            int min1 = 0, min2 = 0, max1 = 0, max2 = 0;
            for (int i = 0; i < projectedVertices.Length; i++)
            {
                if (IsPointSmaller(projectedVertices[i], projectedVertices[min1]))
                {
                    min1 = i;
                }
                if (!IsPointSmaller(projectedVertices[i], projectedVertices[max1]))
                {
                    max1 = i;
                }
            }
            for (int i = 0; i < projectedVertices2.Length; i++)
            {
                if (IsPointSmaller(projectedVertices2[i], projectedVertices2[min2]))
                {
                    min2 = i;
                }
                if (!IsPointSmaller(projectedVertices2[i], projectedVertices2[max2]))
                {
                    max2 = i;
                }
            }

            ponto11.transform.position = projectedVertices[min1] + directionVector.normalized * 2;
            ponto12.transform.position = projectedVertices[max1] + directionVector.normalized * 2;

            GameObject ponto21 = new GameObject("Projectedline21");
            GameObject ponto22 = new GameObject("Projectedline22");
            ponto21.tag = "garbage";
            ponto22.tag = "garbage";
            ponto21.transform.position = projectedVertices2[min2] + directionVector.normalized * 2;
            ponto22.transform.position = projectedVertices2[max2] + directionVector.normalized * 2;

            if (!LinesIntersect(ponto11.transform.position, ponto12.transform.position, ponto21.transform.position, ponto22.transform.position))
            {
                Debug.LogWarning("pontos :" + ponto11.transform.position + ", " + ponto12.transform.position + ", " + ponto21.transform.position + ", " + ponto22.transform.position + ", ");
                Debug.LogWarning(LinesIntersect(ponto11.transform.position, ponto12.transform.position, ponto21.transform.position, ponto22.transform.position));
                Debug.LogWarning("vertices: " + d2vert + "," + d3vert);
                return false;
            }
        }
        // Debug.Log("return true");
        return true;
    }

    [ContextMenu("collision")]
    void collision()
    {
        collisionsound.Play(0);

    }


    [ContextMenu("quick")]
    void project3dquick()
    {
        project3d();
        if (vert2d >= 22)
        {
            vert2d = 0;
        }
        if (vert3d >= 22)
        {
            vert2d = 0;
        }
        
    }


    [ContextMenu("project3d")]
    void project3d()
    {
        CleanUp();
        Mesh mesh = cubo1.GetComponent<MeshFilter>().mesh;
        Mesh mesh2 = cubo2.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] vertices2 = mesh2.vertices;


        Vector3 directionVector = vertices[vert3d] - vertices[vert3d + 1];
        HighlightVertex(cubo1, vert3d, Color.yellow);
        HighlightVertex(cubo1, vert3d + 1, Color.yellow);
        HighlightVertex(cubo2, vert3d, Color.yellow);
        HighlightVertex(cubo2, vert3d + 1, Color.yellow);



        // Normalize direction vector
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
        Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
        Plane projectionPlane = new Plane(directionVector.normalized, vertices[vert3d]);

        // Project object vertices perpendicularly to perpendicular vector
        Vector3[] projectedVertices = new Vector3[vertices.Length];
        Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
        }
        for (int i = 0; i < vertices2.Length; i++)
        {
            projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
        }

        // Create GameObjects to hold the projected meshes
        GameObject plano1 = new GameObject("ProjectedMesh1");
        GameObject plano2 = new GameObject("ProjectedMesh2");
        plano1.tag = "garbage";
        plano2.tag = "garbage";

        // Add MeshFilters to hold the meshes
        MeshFilter meshFilter1 = plano1.AddComponent<MeshFilter>();
        MeshFilter meshFilter2 = plano2.AddComponent<MeshFilter>();

        // Create Meshes for the projected shapes
        Mesh projectedMesh1 = new Mesh();
        Mesh projectedMesh2 = new Mesh();

        // Assign vertices to the meshes
        projectedMesh1.vertices = projectedVertices;
        projectedMesh2.vertices = projectedVertices2;

        // Define triangles for the projected shapes
        int[] triangles1 = new int[(projectedVertices.Length - 2) * 3];
        int[] triangles2 = new int[(projectedVertices2.Length - 2) * 3];

        for (int i = 0; i < projectedVertices.Length - 2; i++)
        {
            triangles1[i * 3] = 0;
            triangles1[i * 3 + 1] = i + 1;
            triangles1[i * 3 + 2] = i + 2;
        }

        for (int i = 0; i < projectedVertices2.Length - 2; i++)
        {
            triangles2[i * 3] = 0;
            triangles2[i * 3 + 1] = i + 1;
            triangles2[i * 3 + 2] = i + 2;
        }

        // Assign triangles to the meshes
        projectedMesh1.triangles = triangles1;
        projectedMesh2.triangles = triangles2;

        // Create normals for the meshes (assuming they face upwards)
        Vector3[] normals1 = new Vector3[projectedVertices.Length];
        Vector3[] normals2 = new Vector3[projectedVertices2.Length];
        Vector3 normal = Vector3.up;

        for (int i = 0; i < projectedVertices.Length; i++)
        {
            normals1[i] = normal;
        }

        for (int i = 0; i < projectedVertices2.Length; i++)
        {
            normals2[i] = normal;
        }

        // Assign normals to the meshes
        projectedMesh1.normals = normals1;
        projectedMesh2.normals = normals2;

        // Set the meshes to the MeshFilters
        meshFilter1.mesh = projectedMesh1;
        meshFilter2.mesh = projectedMesh2;

        // Position the projected meshes
        plano1.transform.position = cubo1.transform.position + directionVector.normalized * 2;
        plano2.transform.position = cubo2.transform.position + directionVector.normalized * 2;

        // Equalize distances
        Vector3 displacement = cubo2.transform.position - cubo1.transform.position;
        float dist = Vector3.Dot(displacement, directionVector.normalized);

        plano1.transform.position = plano1.transform.position + directionVector.normalized * dist;


        // Add MeshRenderers to render the meshes
        MeshRenderer meshRenderer1 = plano1.AddComponent<MeshRenderer>();
        MeshRenderer meshRenderer2 = plano2.AddComponent<MeshRenderer>();

        // Optionally, assign materials to the MeshRenderers
        meshRenderer1.material = new Material(Shader.Find("Standard"));
        meshRenderer2.material = new Material(Shader.Find("Standard"));

        if (increase_vert3d)
        {
            vert3d++;
            if (vert3d >= 22)
                vert3d = 0;
        }
        project2d(plano1, plano2);


    }

    void project2d(GameObject proj1, GameObject proj2)
    {
        Mesh mesh = proj1.GetComponent<MeshFilter>().mesh;
        Mesh mesh2 = proj2.GetComponent<MeshFilter>().mesh;

        Vector3[] vertices = mesh.vertices;
        Vector3[] vertices2 = mesh2.vertices;

        for (int i = 0; i < vertices.Length; i++)
            vertices[i] = vertices[i] + proj1.transform.position;
        for (int i = 0; i < vertices2.Length; i++)
            vertices2[i] = vertices2[i] + proj2.transform.position;


        int anti_repeat_offset = 1;
        try
        {

            while (vertices[vert2d] == vertices[vert2d + anti_repeat_offset])
            {
                anti_repeat_offset++;
            }
        }
        catch (System.Exception)
        {
            return;
            throw;
        }
        Vector3 directionVector = vertices[vert2d] - vertices[vert2d + anti_repeat_offset];
        HighlightVertex(proj1, vert2d, Color.blue);
        HighlightVertex(proj1, vert2d + anti_repeat_offset, Color.blue);
        HighlightVertex(proj2, vert2d, Color.blue);
        HighlightVertex(proj2, vert2d + anti_repeat_offset, Color.blue);


        // Normalize direction vector
        Vector3 normalizedDirection = directionVector.normalized;

        Vector3 perpendicularVector1 = Vector3.Cross(directionVector, Vector3.up).normalized; // First perpendicular vector
        Vector3 perpendicularVector2 = Vector3.Cross(directionVector, perpendicularVector1).normalized; // Second perpendicular vector
        Plane projectionPlane = new Plane(directionVector.normalized, vertices[vert2d]);

        // Project object vertices perpendicularly to perpendicular vector
        Vector3[] projectedVertices = new Vector3[vertices.Length];
        Vector3[] projectedVertices2 = new Vector3[vertices2.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            projectedVertices[i] = vertices[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices[i]);
        }
        for (int i = 0; i < vertices2.Length; i++)
        {
            projectedVertices2[i] = vertices2[i] - projectionPlane.normal * projectionPlane.GetDistanceToPoint(vertices2[i]);
        }

        GameObject ponto11 = new GameObject("Projectedline11");
        GameObject ponto12 = new GameObject("Projectedline12");
        ponto11.tag = "garbage";
        ponto12.tag = "garbage";

        int min1 = 0, min2 = 0, max1 = 0, max2 = 0;
        for (int i = 0; i < projectedVertices.Length; i++)
        {
            if (IsPointSmaller(projectedVertices[i], projectedVertices[min1]))
            {
                min1 = i;
            }
            if (!IsPointSmaller(projectedVertices[i], projectedVertices[max1]))
            {
                max1 = i;
            }
        }
        for (int i = 0; i < projectedVertices2.Length; i++)
        {
            if (IsPointSmaller(projectedVertices2[i], projectedVertices2[min2]))
            {
                min2 = i;
            }
            if (!IsPointSmaller(projectedVertices2[i], projectedVertices2[max2]))
            {
                max2 = i;
            }
        }

        ponto11.transform.position = projectedVertices[min1] + directionVector.normalized * 2;
        ponto12.transform.position = projectedVertices[max1] + directionVector.normalized * 2;

        GameObject ponto21 = new GameObject("Projectedline21");
        GameObject ponto22 = new GameObject("Projectedline22");
        ponto21.tag = "garbage";
        ponto22.tag = "garbage";
        ponto21.transform.position = projectedVertices2[min2] + directionVector.normalized * 2;
        ponto22.transform.position = projectedVertices2[max2] + directionVector.normalized * 2;

        // Equalize distances
        // Vector3 displacement = proj2.transform.position - proj1.transform.position;
        //   float dist = Vector3.Dot(displacement, directionVector.normalized);

        //ponto11.transform.position = ponto11.transform.position + directionVector.normalized * dist;
        //ponto12.transform.position = ponto12.transform.position + directionVector.normalized * dist;
        HighlightVertex(ponto11, 0, Color.red);
        HighlightVertex(ponto12, 1, Color.red);
        HighlightVertex(ponto21, 0, Color.green);
        HighlightVertex(ponto22, 1, Color.green);

        if (increase_vert2d)
        {
            vert2d++;
            if (vert2d >= 22)
            {
                vert2d = 0;
            }
        }

        // Debug.Log(ponto11.transform.position);
        // Debug.Log(ponto12.transform.position);
        // Debug.Log(ponto21.transform.position);
        // Debug.Log(ponto22.transform.position);

        // Debug.Log(IsPointSmaller(ponto21.transform.position, ponto12.transform.position));
        // Debug.Log(IsPointSmaller(ponto11.transform.position, ponto21.transform.position));
        // Debug.Log(IsPointSmaller(ponto11.transform.position, ponto22.transform.position));
        // Debug.Log(IsPointSmaller(ponto21.transform.position, ponto11.transform.position));


        MeshRenderer meshRenderer = cubo1.GetComponent<MeshRenderer>();
        Material material = new Material(meshRenderer.sharedMaterial);
        if (LinesIntersect(ponto11.transform.position, ponto12.transform.position, ponto21.transform.position, ponto22.transform.position)){
            material.color = Color.red;
            if (!collisionsound.isPlaying)
            {
                collisionsound.Play(0);
            }
            
        }
        else
        {
            material.color = Color.green;
            collisionsound.Stop();
            Debug.Log("yes");
        }

        meshRenderer.material = material;
    }


    public Vector3[] RemoveDuplicates(Vector3[] vectors)
    {
        HashSet<Vector3> uniqueVectors = new HashSet<Vector3>();

        // Add vectors to a HashSet to remove duplicates
        foreach (Vector3 vector in vectors)
        {
            uniqueVectors.Add(vector);
        }

        // Convert HashSet back to an array
        Vector3[] uniqueArray = new Vector3[uniqueVectors.Count];
        uniqueVectors.CopyTo(uniqueArray);

        return uniqueArray;
    }
    int[] RemoveDuplicates(int[] arr)
    {
        int[] uniqueArray = new int[arr.Length];
        int index = 0;

        foreach (int num in arr)
        {
            if (Array.IndexOf(uniqueArray, num) == -1)
            {
                uniqueArray[index++] = num;
            }
        }

        Array.Resize(ref uniqueArray, index);
        return uniqueArray;
    }
    void CleanUp()
    {
        // Find and destroy any GameObjects with names containing "ProjectedMesh"
        GameObject[] garbage = GameObject.FindGameObjectsWithTag("garbage");
        foreach (GameObject obj in garbage)
            Destroy(obj);
    }
    void HighlightVertex(GameObject cubeObject, int index, Color cor)
    {
        MeshFilter meshFilter = cubeObject.GetComponent<MeshFilter>();
        if (meshFilter != null)
        {
            Mesh mesh = meshFilter.mesh;
            if (index >= 0 && index < mesh.vertices.Length)
            {
                // Get the position of the vertex in world space
                Vector3 vertexPosition = cubeObject.transform.TransformPoint(mesh.vertices[index]);

                // Create a marker object (e.g., sphere) at the vertex position
                GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                marker.name = ColorUtility.ToHtmlStringRGB(cor);
                marker.tag = "garbage";
                marker.transform.position = vertexPosition;
                marker.transform.localScale = Vector3.one * 0.1f; // Adjust the size of the marker
                marker.GetComponent<Renderer>().material.color = cor; // Adjust the color of the marker
            }
            else
            {
                Debug.Log("Vertex index out of range.");
            }
        }
        else
        {
            GameObject marker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            marker.name = ColorUtility.ToHtmlStringRGB(cor);
            marker.tag = "garbage";
            marker.transform.position = cubeObject.transform.position;
            marker.transform.localScale = Vector3.one * 0.1f;
            marker.GetComponent<Renderer>().material.color = cor;
        }
    }
    bool IsPointSmaller(Vector3 point1, Vector3 point2)
    {
        return point1.x < point2.x || (point1.x == point2.x && (point1.y < point2.y || (point1.y == point2.y && point1.z < point2.z)));
    }
    bool LinesIntersect(Vector3 point11, Vector3 point12, Vector3 point21, Vector3 point22)
    {
        // Check if any endpoint of the first line segment lies within the second line segment
        if (PointOnLineSegment(point11, point21, point22) || PointOnLineSegment(point12, point21, point22))
            return true;

        // Check if any endpoint of the second line segment lies within the first line segment
        if (PointOnLineSegment(point21, point11, point12) || PointOnLineSegment(point22, point11, point12))
            return true;

        if (point11 == point21 || point11 == point22 || point11 == point21 || point12 == point22 || point11 == point12 || point21 == point22)
        {
            return true;
        }
        return false;
    }

    bool PointOnLineSegment(Vector3 point, Vector3 lineStart, Vector3 lineEnd)
    {
        float distanceFromStart = (point - lineStart).magnitude;
        float distanceFromEnd = (point - lineEnd).magnitude;
        float lineLength = (lineEnd - lineStart).magnitude;

        // Check if the point is within the line segment
        return Mathf.Approximately(distanceFromStart + distanceFromEnd, lineLength);
    }

}
