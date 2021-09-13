using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Triangle
{
    public Coord[] coords = new Coord[3];
    public Triangle(Coord c1, Coord c2, Coord c3)
    {
        coords[0] = c1;
        coords[1] = c2;
        coords[2] = c3;
    }
}
