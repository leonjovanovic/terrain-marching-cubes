using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraMovement : MonoBehaviour
{

    private float speed = 100f;
    private Vector3 dragPos;
    public bool drag = false;
    private Vector3 center;

    float minFov = 15f;
    float maxFov = 90f;
    float sensitivity = 10f;
    // Start is called before the first frame update
    void Start()
    {
        center = new Vector3(10, 0, 10);
        transform.LookAt(center);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
            Application.Quit(0);

        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            transform.RotateAround(center, Vector3.up, speed * Time.deltaTime);
        }
        else if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            transform.RotateAround(center, Vector3.up, -speed* Time.deltaTime);
        }

        float fov = Camera.main.fieldOfView;
        fov -= Input.GetAxis("Mouse ScrollWheel") * sensitivity;
        fov = Mathf.Clamp(fov, minFov, maxFov);
        Camera.main.fieldOfView = fov;

        if (IsPointerOverUIObject() || Input.GetMouseButtonUp(0))
        {
            drag = false; 
            return;
        }
        Ray look = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(look))
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragPos = Input.mousePosition;
                drag = true;
            }
        }
        if (drag)
        {
            Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - dragPos);
            transform.RotateAround(center, Vector3.up, speed * pos.x * Time.deltaTime * 4);
        }

    }

    bool getDrag()
    {
        return drag;
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
