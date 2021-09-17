using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public float[,,] coords_vals;

    public float min, max;

    private bool generateTerrain = false;

    public Grid(int x, int y, int z)
    {
    }

    public void createGrid(int x, int y, int z, bool genTer)
    {
        max = 0;
        min = y * y;
        generateTerrain = genTer;
        coords_vals = new float[x + 1, y + 1, z + 1];
        for (int i = 0; i < x + 1; i++)
            for (int j = 0; j < y + 1; j++)
                for (int k = 0; k < z + 1; k++)//x2 + y2 + z2 - x/2
                    coords_vals[i, j, k] = calcDensity(i, j, k, x, y);
        //Debug.Log("Min " + min + " Max " + max);
                    
    }

    float calcDensity(int x, int y, int z, int width, int height)
    {
        float density;
        density = y;
        if (generateTerrain)
        {
            density += height * Mathf.PerlinNoise((float)x / width * 10.05f, (float)z / width * 10.05f) / 4;
            density += height * Mathf.PerlinNoise((float)x / width * 5.30f, (float)z / width * 5.30f) / 2;
            density += height * Mathf.PerlinNoise((float)x / width * 1f, (float)z / width * 1f);
            density += height * Mathf.PerlinNoise((float)z / width * 2f, (float)x / width * 2f);
        }
        if (min > density)
            min = density;
        if (max < density)
            max = density;
        return density;
    }
}
