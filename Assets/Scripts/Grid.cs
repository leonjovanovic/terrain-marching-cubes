using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid
{
    public float[,,] coords_vals;

    public float min, max;

    private bool generateTerrain = false;

    private float c1, c2, c3, c4;

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
            density += height * Mathf.PerlinNoise((float)x / width * c1, (float)z / width * c1) / 4; //10.05
            density += height * Mathf.PerlinNoise((float)x / width * c2, (float)z / width * c2) / 2; //5.30
            density += height * Mathf.PerlinNoise((float)x / width * c3, (float)z / width * c3) /2.5f; //1
            density += height * Mathf.PerlinNoise((float)z / width * c4, (float)x / width * c4);//2
        }
        if (min > density)
            min = density;
        if (max < density)
            max = density;
        return density;
    }

    public void createCoefficients()
    {
        c1 = Random.Range(8, 12);
        c2 = Random.Range(3.3f, 7.3f);
        c3 = Random.Range(1f, 2f);
        c4 = Random.Range(1f, 3f);
    }
}
