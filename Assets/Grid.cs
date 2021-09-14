using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public float[,,] coords_vals;
    public Grid(int x, int y, int z)
    {
        createGrid(x, y, z);
    }

    void createGrid(int x, int y, int z)
    {
        coords_vals = new float[x + 1, y + 1, z + 1];
        for (int i = 0; i < x + 1; i++)
            for (int j = 0; j < y + 1; j++)
                for (int k = 0; k < z + 1; k++)//x2 + y2 + z2 - x/2
                    coords_vals[i, j, k] = Mathf.Abs((i - (float)x / 2) * (i - (float)x / 2) + (j - (float)y / 2) * (j - (float)y / 2) + (k - (float)z / 2) * (k - (float)z / 2));
    }
}
