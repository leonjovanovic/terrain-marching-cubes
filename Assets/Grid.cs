using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public float[,,] coords_vals;

    public float min, max;
    public Grid(int x, int y, int z)
    {
        createGrid(x, y, z);
        max = 0;
        min = y * y;
    }

    void createGrid(int x, int y, int z)
    {
        coords_vals = new float[x + 1, y + 1, z + 1];
        for (int i = 0; i < x + 1; i++)
            for (int j = 0; j < y + 1; j++)
                for (int k = 0; k < z + 1; k++)//x2 + y2 + z2 - x/2
                    coords_vals[i, j, k] = calcDensity(i, j, k, x, y);
        Debug.Log("Min " + min + " Max " + max);
                    
    }

    float calcDensity(int x, int y, int z, int width, int height)
    {
        float density;
        density = y;
        density += height * Mathf.PerlinNoise((float) x / width * 8.03f, (float) z / width * 8.03f);
        //density += height * Mathf.PerlinNoise((float)x / width * 1.96f, (float)z / width * 1.96f) / 2;
        //density += height * Mathf.PerlinNoise((float)x / width * 1.01f, (float)z / width * 1.01f);
        if (min > density)
            min = density;
        if (max < density)
            max = density;
        return density;
    }
}
