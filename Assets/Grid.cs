using System.Collections;
using System.Collections.Generic;

public class Grid
{
    public double[,] coords = new double[8, 3];
    public double[] vals = new double[8];
    public Grid(double[,] grid_coords, double[] grid_vals)
    {
        coords = grid_coords;
        vals = grid_vals;
    }
}
