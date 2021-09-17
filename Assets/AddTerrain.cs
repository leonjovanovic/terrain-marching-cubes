using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class AddTerrain : MonoBehaviour
{
    public float radius = 2.0f;
    private float oldRadius = -1f;
    private RaycastHit hit;
    public GameObject terrainBrush;
    private CameraMovement cam;
    private float[,,] coords_vals;
    private float isosurface;
    private float x, y, z;
    // Start is called before the first frame update
    void Start()
    {
        terrainBrush = GameObject.Find("TerrainBrush");
        cam = Camera.main.GetComponent<CameraMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (cam.drag) return;
        Ray look = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (IsPointerOverUIObject()) return;
        if (Physics.Raycast(look, out hit))
        {
            terrainBrush.transform.position = hit.point;
            if (Input.GetMouseButton(0))
            {
                findAllVertices(hit.point, true);
            }
            else if (Input.GetMouseButton(1))
            {
                findAllVertices(hit.point, false);
            }
        }
        else
        {
            terrainBrush.transform.position = new Vector3(0,20,0);
        }
        if (radius != oldRadius)
        {
            oldRadius = radius;
            updateBrushSize();
        }
    }
    
    // Kada kliknemo, pomerimo sferu na tu poziciju
    // Sve tacke koje se nalaze unutar sfere ALI sa gornje/donje strane menjamo vrednosti tako da se vide/brisu

    private void findAllVertices(Vector3 coord, bool add)
    {
        float valX, valY, valZ;
        coords_vals = GetComponent<MarchingCubes>().grid.coords_vals;
        isosurface = GetComponent<MarchingCubes>().getIsolevel();
        x = GetComponent<MarchingCubes>().getX();
        y = GetComponent<MarchingCubes>().getY();
        z = GetComponent<MarchingCubes>().getZ();
        
        coord = coord / GetComponent<MarchingCubes>().getGridCellSize();
        float radiusScaled = radius / GetComponent<MarchingCubes>().getGridCellSize();

        float r = Mathf.Pow(radiusScaled, 2);
        for (int i = (int)(Mathf.Max(coord.x - radiusScaled, 0)); i < Mathf.Min(coord.x + radiusScaled + 1, x + 1); i++)//x osa
        {
            valX = Mathf.Pow(coord.x - i, 2);
            for (int j = (int)(Mathf.Max(coord.y - radiusScaled, 0)); j < Mathf.Min(coord.y + radiusScaled + 1, y + 1); j++)//y osa
            {
                valY = Mathf.Pow(coord.y - j, 2);
                for (int k = (int)(Mathf.Max(coord.z - radiusScaled, 0)); k < Mathf.Min(coord.z + radiusScaled + 1, z + 1); k++)
                {
                    valZ = Mathf.Pow(coord.z - k, 2);
                    if ((valX + valY + valZ) <= r)
                    {
                        if (add && j!= y)
                        {
                            coords_vals[i, j, k] -= 0.5f;//isosurface - 0.01f;
                        }
                        if (!add && j!= 0)
                        {
                            coords_vals[i, j, k] += 0.5f;// = isosurface + 0.01f;
                        }
                    }
                }
            }
        }
        GetComponent<MarchingCubes>().marchCubes();
    }

    void updateBrushSize()
    {
        terrainBrush.transform.localScale = new Vector3(radius * radius + 1, radius * radius + 1, radius * radius + 1);
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }

    public void OnSliderBrushSizeValueChanged(float value)
    {
        radius = value;
        GameObject.Find("BrushSizeText").GetComponent<TextMeshProUGUI>().text = (Mathf.Round(radius * 100f) / 100f).ToString();
    }
}
