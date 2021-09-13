using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    private Grid grid;
    private Tables tables;
    public double isolevel = 6.5;
    private Coord[] vertices = new Coord[12];
    private Triangle[] triangles = new Triangle[5];
    // Start is called before the first frame update
    void Start()
    {
        tables = new Tables();
        grid = new Grid(tables.cubeCoords, tables.cubeVals);
        int ret = createTriangles();
        Debug.Log("Vratio je " + ret + " trouglova!");
        for (int i = 0; i < ret; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                Debug.Log("X: " + triangles[i].coords[j].x + " Y: " + triangles[i].coords[j].y + " Z: " + triangles[i].coords[j].z);
            }
        }

        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[] {
            new Vector3((float)triangles[0].coords[0].x, (float)triangles[0].coords[0].y, (float)triangles[0].coords[0].z),
            new Vector3((float)triangles[0].coords[1].x, (float)triangles[0].coords[1].y, (float)triangles[0].coords[1].z),
            new Vector3((float)triangles[0].coords[2].x, (float)triangles[0].coords[2].y, (float)triangles[0].coords[2].z)
        };
        mesh.uv = new Vector2[] { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1) };
        mesh.triangles = new int[] { 0, 1, 2 };
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int createTriangles()
    {
        int cubeindex;
        cubeindex = 0;
        if (grid.vals[0] < isolevel) cubeindex |= 1;
        if (grid.vals[1] < isolevel) cubeindex |= 2;
        if (grid.vals[2] < isolevel) cubeindex |= 4;
        if (grid.vals[3] < isolevel) cubeindex |= 8;
        if (grid.vals[4] < isolevel) cubeindex |= 16;
        if (grid.vals[5] < isolevel) cubeindex |= 32;
        if (grid.vals[6] < isolevel) cubeindex |= 64;
        if (grid.vals[7] < isolevel) cubeindex |= 128;
        //Debug.Log("Cubeindex: " + cubeindex);

        int edges = tables.getFromEdgeTable(cubeindex);
        //Debug.Log("Edges: " + edges);
        //Debug.Log("Edges&: " + (edges & 64));
        // Cube is entirely in/out of the surface
        if (edges == 0)
            return 0;
        // Find the vertices where the surface intersects the cube
        if ((edges & 1) == 1) vertices[0] = VertexInterp(0, 1);
        if ((edges & 2) == 2) vertices[1] = VertexInterp(1, 2);
        if ((edges & 4) == 4) vertices[2] = VertexInterp(2, 3);
        if ((edges & 8) == 8) vertices[3] = VertexInterp(3, 0);
        if ((edges & 16) == 16) vertices[4] = VertexInterp(4, 5);
        if ((edges & 32) == 32) vertices[5] = VertexInterp(5, 6);
        if ((edges & 64) == 64) vertices[6] = VertexInterp(6, 7);
        if ((edges & 128) == 128) vertices[7] = VertexInterp(7, 4);
        if ((edges & 256) == 256) vertices[8] = VertexInterp(0, 4);
        if ((edges & 512) == 512) vertices[9] = VertexInterp(1, 5);
        if ((edges & 1024) == 1024) vertices[10] = VertexInterp(2, 6);
        if ((edges & 2048) == 2048) vertices[11] = VertexInterp(3, 7);

        /* Create the triangle */
        int ntriang = 0;
        for (int i = 0; tables.getFromTriTable(cubeindex, i) != -1; i += 3)
            triangles[ntriang++] = new Triangle(vertices[tables.getFromTriTable(cubeindex, i)], vertices[tables.getFromTriTable(cubeindex, i + 1)], vertices[tables.getFromTriTable(cubeindex, i + 2)]);
        return (ntriang);

    }

    Coord VertexInterp(int v1, int v2)
    {
        Coord p1 = new Coord(grid.coords[v1, 0], grid.coords[v1, 1], grid.coords[v1, 2]);
        Coord p2 = new Coord(grid.coords[v2, 0], grid.coords[v2, 1], grid.coords[v2, 2]);
        double valp1 = grid.vals[v1];
        double valp2 = grid.vals[v2];

        if (Mathf.Abs((float)(isolevel - valp1)) < 0.00001)
            return (p1);
        if (Mathf.Abs((float)(isolevel - valp2)) < 0.00001)
            return (p2);
        if (Mathf.Abs((float)(valp1 - valp2)) < 0.00001)
            return (p1);

        double mu = (isolevel - valp1) / (valp2 - valp1);
        double x = p1.x + mu * (p2.x - p1.x);
        double y = p1.y + mu * (p2.y - p1.y);
        double z = p1.z + mu * (p2.z - p1.z);
        return new Coord(x, y, z);
    }
}
