using System;
using UnityEngine;
using UnityEngine.Splines;
using Unity.Mathematics;
using System.Collections.Generic;

#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Spline_Sampler : MonoBehaviour
{
    [SerializeField] private SplineContainer m_splineContainer;
    [SerializeField] private int m_splineIndex;

    [SerializeField] private float m_width = 4f;
    [SerializeField] private int resolution = 32;

    private List<float3> p1List = new();
    private List<float3> p2List = new();

    private Mesh mesh;

    private void OnEnable()
    {
        mesh = new Mesh();
        mesh.name = "Road Mesh";
        GetComponent<MeshFilter>().sharedMesh = mesh;
    }

    private void Start()
    {
        UpdateSpline();

        GetComponent<MeshCollider>().sharedMesh = GetComponent<MeshFilter>().mesh;
    }

    [ContextMenu("Update spline")]
    private void UpdateSpline()
    {
        if (!m_splineContainer) return;

        SampleSpline();
        BuildMesh();
    }

    // -------------------------
    // SPLINE SAMPLING
    // -------------------------
    void SampleSpline()
    {
        p1List.Clear();
        p2List.Clear();

        for (int i = 0; i <= resolution; i++)
        {
            float t = i / (float)resolution;

            m_splineContainer.Evaluate(
                m_splineIndex,
                t,
                out float3 position,
                out float3 tangent,
                out float3 up
            );

            float3 right = math.normalize(math.cross(up, tangent));

            float3 p1 = position - right * (m_width * 0.5f);
            float3 p2 = position + right * (m_width * 0.5f);

            p1List.Add(p1);
            p2List.Add(p2);
        }
    }

    // -------------------------
    // MESH GENERATION
    // -------------------------
    void BuildMesh()
    {
        mesh.Clear();

        int vertCount = (resolution + 1) * 2;

        Vector3[] vertices = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];
        int[] triangles = new int[resolution * 6];

        for (int i = 0; i <= resolution; i++)
        {
            int v = i * 2;

            vertices[v] = p1List[i];
            vertices[v + 1] = p2List[i];

            float uvV = i / (float)resolution;
            uvs[v] = new Vector2(0, uvV);
            uvs[v + 1] = new Vector2(1, uvV);
        }

        int t = 0;
        for (int i = 0; i < resolution; i++)
        {
            int v = i * 2;

            triangles[t++] = v;
            triangles[t++] = v + 2;
            triangles[t++] = v + 1;

            triangles[t++] = v + 1;
            triangles[t++] = v + 2;
            triangles[t++] = v + 3;
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

#if UNITY_EDITOR
    // -------------------------
    // DEBUG DRAW
    // -------------------------
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        for (int i = 0; i < p1List.Count; i++)
        {
            Gizmos.DrawSphere(p1List[i], 0.1f);
            Gizmos.DrawSphere(p2List[i], 0.1f);
        }
    }
#endif
}

