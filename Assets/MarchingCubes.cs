using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class MarchingCubes : MonoBehaviour
{
    public Grid grid;
    private float gridCellSize = 0.2f;
    private float oldGridCellSize = -1f;
    private Tables tables;
    private float isolevelBaseline = 0.01f;
    private float isolevel = 8.5f;
    private float oldIsolevel = -50;
    private Coord[] vertices;
    private List<Triangle> triangles;
    public Material mat;
    private bool generateTerrain = false;
    private int flattenLevel = 0;
    private bool lastMode = false; //if its random terrain or flat terrain last rendered
    private bool updateMesh = false; //if its random terrain or flat terrain last rendered

    private int x = 100, y = 10, z = 100;
    //private int oldX = -1, oldY = -1, oldZ = -1;

    // Start is called before the first frame update
    void Start()
    {
        tables = new Tables();
        grid = new Grid(x, y, z);
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.AddComponent<MeshCollider>();
        gameObject.GetComponent<MeshRenderer>().material = mat;
    }

    // Update is called once per frame
    void Update()
    {
        if(isolevelBaseline != oldIsolevel || gridCellSize != oldGridCellSize || updateMesh)
        {
            if (gridCellSize != oldGridCellSize || updateMesh)
            {
                x = Mathf.RoundToInt(20 / gridCellSize); y = Mathf.RoundToInt(5 / gridCellSize); z = Mathf.RoundToInt(20 / gridCellSize);
                grid.createGrid(x, y, z, lastMode);
                updateMesh = false;
            }
            isolevel = isolevelBaseline / gridCellSize;
            oldIsolevel = isolevelBaseline; oldGridCellSize = gridCellSize;

            marchCubes();
        }
    }

    public void marchCubes()
    {
        triangles = new List<Triangle>();
        for (int i = 0; i < x; i++)
            for (int j = 0; j < y; j++)
                for (int k = 0; k < z; k++)
                    createTriangles(i, j, k);
        renderTriangles();
    }

    void createTriangles(int x1, int y1, int z1)
    {
        int cubeindex;
        cubeindex = 0;
        if (grid.coords_vals[x1, y1, z1] < isolevel) cubeindex |= 1;
        if (grid.coords_vals[x1, y1, z1 + 1] < isolevel) cubeindex |= 2;
        if (grid.coords_vals[x1 + 1, y1, z1 + 1] < isolevel) cubeindex |= 4;
        if (grid.coords_vals[x1 + 1, y1, z1] < isolevel) cubeindex |= 8;
        if (grid.coords_vals[x1, y1 + 1, z1] < isolevel) cubeindex |= 16;
        if (grid.coords_vals[x1, y1 + 1, z1 + 1] < isolevel) cubeindex |= 32;
        if (grid.coords_vals[x1 + 1, y1 + 1, z1 + 1] < isolevel) cubeindex |= 64;
        if (grid.coords_vals[x1 + 1, y1 + 1, z1] < isolevel) cubeindex |= 128;

        int edges = tables.getFromEdgeTable(cubeindex);
        // Cube is entirely in/out of the surface
        if (edges == 0)
            return;
        vertices = new Coord[12];
        // Find the vertices where the surface intersects the cube
        if ((edges & 1) == 1) vertices[0] = VertexInterp(x1, y1, z1, x1, y1, z1 + 1);
        if ((edges & 2) == 2) vertices[1] = VertexInterp(x1, y1, z1 + 1, x1 + 1, y1, z1 + 1);
        if ((edges & 4) == 4) vertices[2] = VertexInterp(x1 + 1, y1, z1 + 1, x1 + 1, y1, z1);
        if ((edges & 8) == 8) vertices[3] = VertexInterp(x1 + 1, y1, z1, x1, y1, z1);
        if ((edges & 16) == 16) vertices[4] = VertexInterp(x1, y1 + 1, z1, x1, y1 + 1, z1 + 1);
        if ((edges & 32) == 32) vertices[5] = VertexInterp(x1, y1 + 1, z1 + 1, x1 + 1, y1 + 1, z1 + 1);
        if ((edges & 64) == 64) vertices[6] = VertexInterp(x1 + 1, y1 + 1, z1 + 1, x1 + 1, y1 + 1, z1);
        if ((edges & 128) == 128) vertices[7] = VertexInterp(x1 + 1, y1 + 1, z1, x1, y1 + 1, z1);
        if ((edges & 256) == 256) vertices[8] = VertexInterp(x1, y1, z1, x1, y1 + 1, z1);
        if ((edges & 512) == 512) vertices[9] = VertexInterp(x1, y1, z1 + 1, x1, y1 + 1, z1 + 1);
        if ((edges & 1024) == 1024) vertices[10] = VertexInterp(x1 + 1, y1, z1 + 1, x1 + 1, y1 + 1, z1 + 1);
        if ((edges & 2048) == 2048) vertices[11] = VertexInterp(x1 + 1, y1, z1, x1 + 1, y1 + 1, z1);

        /* Create the triangle */
        for (int i = 0; tables.getFromTriTable(cubeindex, i) != -1; i += 3)
            triangles.Add(new Triangle(vertices[tables.getFromTriTable(cubeindex, i)], vertices[tables.getFromTriTable(cubeindex, i + 1)], vertices[tables.getFromTriTable(cubeindex, i + 2)]));
    }

    Coord VertexInterp(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        float valp1 = grid.coords_vals[x1, y1, z1];
        float valp2 = grid.coords_vals[x2, y2, z2];

        float x1_g = gridCellSize * x1, y1_g = gridCellSize * y1, z1_g = gridCellSize * z1;
        float x2_g = gridCellSize * x2, y2_g = gridCellSize * y2, z2_g = gridCellSize * z2;

        if (Mathf.Abs((float)(isolevel - valp1)) < 0.00001)
            return new Coord(x1_g, y1_g, z1_g);
        if (Mathf.Abs((float)(isolevel - valp2)) < 0.00001)
            return new Coord(x2_g, y2_g, z2_g);
        if (Mathf.Abs((float)(valp1 - valp2)) < 0.00001)
            return new Coord(x1_g, y1_g, z1_g);

        float mu = (isolevel - valp1) / (valp2 - valp1);
        float x = x1_g + mu * (x2_g - x1_g);
        float y = y1_g + mu * (y2_g - y1_g);
        float z = z1_g + mu * (z2_g - z1_g);
        return new Coord(x, y, z);
    }

    void renderTriangles()
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.Clear();
        mesh.vertices = new Vector3[triangles.Count * 3];
        mesh.uv = new Vector2[triangles.Count * 3];
        mesh.triangles = new int[triangles.Count * 3];

        Vector3[] mesh_vertices = mesh.vertices;
        Vector2[] mesh_uv = mesh.uv;
        int[] mesh_triangles = mesh.triangles;
        for (int i = 0; i < triangles.Count; i++)
        {
            mesh_vertices[i * 3].Set(triangles[i].coords[0].x, triangles[i].coords[0].y, triangles[i].coords[0].z);
            mesh_vertices[i * 3 + 1].Set(triangles[i].coords[1].x, triangles[i].coords[1].y, triangles[i].coords[1].z);
            mesh_vertices[i * 3 + 2].Set(triangles[i].coords[2].x, triangles[i].coords[2].y, triangles[i].coords[2].z);

            mesh_uv[i * 3] = new Vector2(0, 0);
            mesh_uv[i * 3 + 1] = new Vector2(0, 1);
            mesh_uv[i * 3 + 2] = new Vector2(1, 1);

            mesh_triangles[i * 3] = i * 3 + 2;
            mesh_triangles[i * 3 + 1] = i * 3 + 1;
            mesh_triangles[i * 3 + 2] = i * 3;
        }

        mesh.vertices = mesh_vertices;
        mesh.uv = mesh_uv;
        mesh.triangles = mesh_triangles;

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public int getX()
    {
        return x;
    }
    public int getY()
    {
        return y;
    }
    public int getZ()
    {
        return z;
    }

    public float getIsolevel()
    {
        return isolevel;
    }

    public float getGridCellSize()
    {
        return gridCellSize;
    }
    public void setGenerateTerrain()
    {
        lastMode = true;
        updateMesh = true;
        isolevelBaseline = 8.75f;
    }

    public void setFlattenLevel(float val)
    {
        lastMode = false;
        isolevelBaseline = val;
        updateMesh = true;
    }

    public void OnSliderGridCellValueChanged(float value)
    {
        switch (value)
        {
            case 0: 
                gridCellSize = 0.2f;
                break;
            case 1:
                gridCellSize = 0.5f;
                break;
            case 2:
                gridCellSize = 1.0f;
                break;
        }
        GameObject.Find("GridCellSizeText").GetComponent<TextMeshProUGUI>().text = gridCellSize.ToString();
    }
    
}
