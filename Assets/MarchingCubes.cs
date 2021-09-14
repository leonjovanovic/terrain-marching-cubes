using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarchingCubes : MonoBehaviour
{
    private Grid grid;
    private Tables tables;
    public double isolevel = 6.5;
    private double old_isolevel = -50;
    private Coord[] vertices;
    private Triangle[] triangles;
    public Material mat;

    int ntriang = 0;
    int x = 10, y = 10, z = 10;

    // Start is called before the first frame update
    void Start()
    {
        tables = new Tables();
        grid = new Grid(x, y, z);
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        gameObject.GetComponent<MeshRenderer>().material = mat;

    }

    // Update is called once per frame
    void Update()
    {
        if(isolevel != old_isolevel)
        {
            triangles = new Triangle[5000];
            ntriang = 0;
            old_isolevel = isolevel;
            for (int i = 0; i < x; i++)
                for (int j = 0; j < y; j++)
                    for (int k = 0; k < z; k++)//For every cube
                    {
                        createTriangles(i, j, k);
                    }

            Debug.Log("Vratio je ukupno " + ntriang + " trouglova!");
            renderTriangles(ntriang);
        }
    }

    int createTriangles(int x1, int y1, int z1)
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

        //Debug.Log("Cubeindex: " + cubeindex);
        int edges = tables.getFromEdgeTable(cubeindex);
        // Cube is entirely in/out of the surface
        if (edges == 0)
            return 0;
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
        {
            triangles[ntriang++] = new Triangle(vertices[tables.getFromTriTable(cubeindex, i)], vertices[tables.getFromTriTable(cubeindex, i + 1)], vertices[tables.getFromTriTable(cubeindex, i + 2)]);
        }

        
        Debug.Log("ntriangs " + ntriang);

        return (ntriang);

    }

    Coord VertexInterp(int x1, int y1, int z1, int x2, int y2, int z2)
    {
        double valp1 = grid.coords_vals[x1, y1, z1];
        double valp2 = grid.coords_vals[x2, y2, z2];

        if (Mathf.Abs((float)(isolevel - valp1)) < 0.00001)
            return new Coord(x1, y1, z1);
        if (Mathf.Abs((float)(isolevel - valp2)) < 0.00001)
            return new Coord(x2, y2, z2);
        if (Mathf.Abs((float)(valp1 - valp2)) < 0.00001)
            return new Coord(x1, y1, z1);

        double mu = (isolevel - valp1) / (valp2 - valp1);
        double x = x1 + mu * (x2 - x1);
        double y = y1 + mu * (y2 - y1);
        double z = z1 + mu * (z2 - z1);
        return new Coord(x, y, z);
    }

    void renderTriangles(int ntriangles)
    {
        Mesh mesh = gameObject.GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = new Vector3[ntriangles * 3];
        mesh.uv = new Vector2[ntriangles * 3];
        mesh.triangles = new int[ntriangles * 3];

        Vector3[] mesh_vertices = mesh.vertices;
        Vector2[] mesh_uv = mesh.uv;
        int[] mesh_triangles = mesh.triangles;

        for (int i = 0; i < ntriangles; i++)
        {
            //Debug.Log("i " + i * 3 + " triangle " + (float)triangles[i].coords[0].x);
            mesh_vertices[i * 3].Set((float)triangles[i].coords[0].x, (float)triangles[i].coords[0].y, (float)triangles[i].coords[0].z);
            mesh_vertices[i * 3 + 1].Set((float)triangles[i].coords[1].x, (float)triangles[i].coords[1].y, (float)triangles[i].coords[1].z);
            mesh_vertices[i * 3 + 2].Set((float)triangles[i].coords[2].x, (float)triangles[i].coords[2].y, (float)triangles[i].coords[2].z);

            mesh_uv[i * 3] = new Vector2(0, 0);
            mesh_uv[i * 3 + 1] = new Vector2(0, 1);
            mesh_uv[i * 3 + 2] = new Vector2(1, 1);

            mesh_triangles[i * 3] = i * 3;
            mesh_triangles[i * 3 + 1] = i * 3 + 1;
            mesh_triangles[i * 3 + 2] = i * 3 + 2;
        }
        mesh.vertices = mesh_vertices;
        mesh.uv = mesh_uv;
        mesh.triangles = mesh_triangles;
    }
}
