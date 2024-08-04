using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Generator : MonoBehaviour
{
    /*--------INITIALISE---------*/
    
    [SerializeField] private float 
        size = 100, maxTerrainHeight = 10, xSampleSpeed = 0.1f, ySampleSpeed = 0.1f, xOffset = 0, yOffset = 0;
    [SerializeField] private int numberOfPoints = 20; //vertices points
    [Range(0.01f, 2f)][SerializeField] float sampleOffset = 0.1f;
    
    private Mesh mesh;
    private Vector3[,] points;
    private Vector3[] vertices;
    private Vector2[] uvs;
    private int[] tris;
    
    
    void Start()
    {
        points = new Vector3[numberOfPoints,numberOfPoints];

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        
        InitPoints();
        GenerateMesh();
    }


    void Update()
    {
        AnimateMesh();
    }

    void InitPoints()
    {
        Vector3 startPos = new Vector3(-size/2, 0, -size/2);
        float interval = size / numberOfPoints;

        float radiusPoint = 0;
        
        
        for (int y = 0; y < numberOfPoints; y++)
        {
            startPos += Vector3.forward * interval;
            for (int x = 0; x < numberOfPoints; x++)
            {
                    float yPos = Mathf.PerlinNoise(x * sampleOffset, y * sampleOffset) * maxTerrainHeight;
                    points[x, y] = startPos 
                                   + Vector3.right * x * interval * 1
                                   + Vector3.up * yPos * 1 * y;
            }
        }
    }

    void AnimateMesh()
    {
        xOffset += xSampleSpeed * Time.deltaTime;
        yOffset += ySampleSpeed * Time.deltaTime;
        
        for (int y = 0; y < numberOfPoints; y++)
        {
            for (int x = 0; x < numberOfPoints; x++)
            {
                float yPos = Mathf.PerlinNoise(x * sampleOffset + xOffset, y * sampleOffset + this.yOffset) * maxTerrainHeight;
                int index = x + numberOfPoints * y;

                vertices[index].y = yPos;
            }
        }
        mesh.vertices = vertices;
        mesh.RecalculateNormals();
    }


    private void GenerateMesh()
    {
        vertices = new Vector3[numberOfPoints * numberOfPoints];
        uvs = new Vector2[numberOfPoints * numberOfPoints];
        tris = new int[(numberOfPoints - 1) * 6 * (numberOfPoints - 1)];
        int triIndex = 0;
        
        for (int y = 0; y < numberOfPoints; y++)
        {
            for (int x = 0; x < numberOfPoints; x++)
            {
                
                //adding the vertices
                int vertIndex = x + numberOfPoints * y;
                vertices[vertIndex] = points[x, y];
                
                //adding the uvs
                uvs[vertIndex] = new Vector2(x / (float)numberOfPoints, y/ (float)numberOfPoints);

                if (x >= numberOfPoints -1 || y >= numberOfPoints - 1) continue;

                
                //1st triangle
                tris[triIndex] = x + numberOfPoints * y;
                tris[triIndex + 1] = (x + numberOfPoints * y) + numberOfPoints;
                tris[triIndex + 2] = x + numberOfPoints * y + 1 +  numberOfPoints;

                //2nd triangle

                tris[triIndex + 3] = vertIndex;
                tris[triIndex + 4] = vertIndex + 1 + numberOfPoints;
                tris[triIndex + 5] = vertIndex + 1;

                triIndex += 6;
            }
        }
        
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = tris;
        mesh.RecalculateNormals();

    }
    

    private void OnDrawGizmos()
    {
        if (points == null) return;
        
        for (int y = 0; y < numberOfPoints; y++)
        {
            for (int x = 0; x < numberOfPoints; x++)
            {
                Gizmos.DrawSphere(points[x,y], 1f);
                Gizmos.color = new Color(points[x, y].x / size * 120, points[x,y].y / maxTerrainHeight , points[x,y].z / size * 120);
            }
        }
    }

}
