using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AddTerrain : MonoBehaviour
{
    public float radius = 5.0f;
    private RaycastHit hit;
    public GameObject terrainBrush;
    // Start is called before the first frame update
    void Start()
    {
        terrainBrush = GameObject.Find("TerrainBrush");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            Ray look = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(look, out hit))
            {
                Debug.Log(hit.distance);
                Debug.Log("Hit " + hit.transform.name + " on " + hit.point);
                terrainBrush.transform.position = hit.point;

                findAllVertices(hit.point);
            }
        }

    }

    private void OnMouseDown()
    {
        /*
        Ray look = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(look, out hit))
        {
            Debug.Log(hit.distance);
            Debug.Log("Hit " + hit.transform.name + " on " + hit.point);
            terrainBrush.transform.position = hit.point;
        }*/
        //Naci koordinate mesta gde smo kliknuli - hit.point

        //Napraviti radijus

        //Naci koje sve tacke su obuhvacene radijusom

        //Povecati/smanjiti vrednosti tih tacaka u gridu
    }

    // Kada kliknemo, pomerimo sferu na tu poziciju
    // Sve tacke koje se nalaze unutar sfere ALI sa gornje/donje strane menjamo vrednosti tako da se vide/brisu

    private void findAllVertices(Vector3 coord)
    {

    }
}
