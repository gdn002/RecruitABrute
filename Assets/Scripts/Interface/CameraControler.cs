using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControler : MonoBehaviour
{
    // Start is called before the first frame update
    private GameObject cam;
    private float xAxis;
    private float zAxis;
    private float camSpeed;
    public float zoomLevel;
    private float zoomScale;
    GameObject grid;
    void Start()
    {
        cam = gameObject;
        camSpeed = 0.05f;
        grid = GameObject.Find("Grid");
        zoomLevel = 40.0f;
        zoomScale = 5.0f;
        ChangeZoom(zoomLevel, -5.0f);
    }

    // Update is called once per frame
    void Update()
    {
        CameraMovement();
        CameraRotation();
        CameraAngle();

    }

    void CameraMovement()
    {
        xAxis = Input.GetAxis("Horizontal") * camSpeed;
        zAxis = Input.GetAxis("Vertical") * camSpeed;
        cam.transform.position += new Vector3(xAxis, 0.0f, zAxis);
    }
    void CameraRotation()
    {
        if (Input.GetKeyDown("e"))
        { 
            grid.transform.RotateAround(new Vector3(grid.GetComponent<Grid>().gridSize.x / 2.0f -0.5f, 0.0f, grid.GetComponent<Grid>().gridSize.y / 2.0f - 0.5f), new Vector3(0.0f, 1.0f, 0.0f), 90.0f);
            foreach(GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                unit.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), -90.0f);
            }
        }
        if (Input.GetKeyDown("q"))
        {
            grid.transform.RotateAround(new Vector3(grid.GetComponent<Grid>().gridSize.x / 2.0f - 0.5f, 0.0f, grid.GetComponent<Grid>().gridSize.y / 2.0f - 0.5f), new Vector3(0.0f, 1.0f, 0.0f), -90.0f);
            foreach (GameObject unit in GameObject.FindGameObjectsWithTag("Unit"))
            {
                unit.transform.Rotate(new Vector3(0.0f, 1.0f, 0.0f), 90.0f);
            }
        }
    }
    void CameraAngle()
    {
        float scrollDelta = Input.mouseScrollDelta.y;
        zoomLevel += scrollDelta * zoomScale;
        if (scrollDelta != 0.0f) {
            if (zoomLevel > 100) zoomLevel = 100;
            else if (zoomLevel < 0) zoomLevel = 0;
            else { ChangeZoom(zoomLevel, 7.0f * scrollDelta * zoomScale / 100.0f); }
        }
        
    }
    void ChangeZoom(float level, float zOffset)
    {
        /*if(level == 100)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, 2.0f, cam.transform.position.z);
            cam.GetComponent<Camera>().fieldOfView = 20;
            cam.transform.localEulerAngles = new Vector3(10.0f, 0.0f, 0.0f);
        }
        if(level >= 40 && level <= 50)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, 5.5f, cam.transform.position.z);
            cam.GetComponent<Camera>().fieldOfView = 40;
            cam.transform.localEulerAngles = new Vector3(30.0f, 0.0f, 0.0f);
        }
        if (level == 0)
        {
            cam.transform.position = new Vector3(cam.transform.position.x, 7.0f, cam.transform.position.z);
            cam.GetComponent<Camera>().fieldOfView = 70;
            cam.transform.localEulerAngles = new Vector3(80.0f, 0.0f, 0.0f);
        }
        */
        // the curve will have to be tweaked a bit
        float camHeight = 7.0f - 5.0f * level / 100.0f;
        float FoV = 70.0f - 50.0f * level / 100.0f;
        float angle = 80.0f - 70.0f * level / 100.0f;

        cam.transform.position = new Vector3(cam.transform.position.x, camHeight, cam.transform.position.z - zOffset);
        cam.GetComponent<Camera>().fieldOfView = FoV;
        cam.transform.localEulerAngles = new Vector3(angle, 0.0f, 0.0f);

    }
}
