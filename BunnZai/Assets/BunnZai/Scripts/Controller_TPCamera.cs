using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// TODO:
/// 
/// ---Fix weird player(?) lagging when in cinematic camera mode(possible lerp problems?)
/// 
/// ---Fix camera movement distancing away from player too much (find normal vector from hardpoint to crane base, limit possible movement of camera along that vector?)
/// 
/// </summary>
public class Controller_TPCamera : MonoBehaviour
{
    public Transform Player;
    public Transform camCrane;
    public bool enableCinematicCamera;
    Transform camHardPoint;
    Transform tppCam;
    Vector3 camOffsetVector;
    public float rotationSpeed = 2f;
    float camAngleH;
    float camAngleV;
    // Start is called before the first frame update
    void Start()
    {
        camHardPoint = new GameObject().transform;
        camOffsetVector = new Vector3(0, 0, -2);
        tppCam = transform;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
       
    }
    void LateUpdate()
    {
        CameraMovement();    
    }

    void CameraMovement() {
        //camAngleH += Mathf.Clamp(Input.GetAxis("Mouse X"), -1, 1) * rotationSpeed;                //mouse input with CLAMPED magnitude
        //camAngleV -= Mathf.Clamp(Input.GetAxis("Mouse Y"), -1, 1) * rotationSpeed/2; 
        camAngleH += Input.GetAxis("Mouse X") * rotationSpeed;                                      //mouse input with UNCLAMPED magnitude
        camAngleV = Mathf.Clamp(camAngleV - Input.GetAxis("Mouse Y") * rotationSpeed/2, -15, 75);   //vertical camera angle input clamped for dead angles

        camCrane.rotation = Quaternion.Euler(Mathf.Clamp(camAngleV, -15, 75), camAngleH, 0);        //Crane rotation also clamped vertically, just in case
        //Player.rotation = Quaternion.Euler(0, camAngleH, 0);
        UpdateHardPoint();
        float hardpointDist = Vector3.Distance(transform.position, camHardPoint.position);

        //Camera Lag Toggle
        if (enableCinematicCamera)
        {
            this.transform.position = Vector3.Lerp(transform.position, camHardPoint.position, hardpointDist * 0.15f); //camera movement WITH lag behind hardpoint
        }
        else {
            this.transform.position = camHardPoint.position;                                                          //camera movement WITHOUT lag
        }
        
        //Debug.Log("Distance to the Hardpoint: " + hardpointDist);

        this.transform.rotation = Quaternion.Euler(Mathf.Clamp(camAngleV, -15, 75), camAngleH, 0);
    }

    void UpdateHardPoint() {
        camHardPoint.position = camCrane.position + camOffsetVector;
        Vector3 craneDir = camHardPoint.position - camCrane.position;
        craneDir = camCrane.rotation * craneDir;
        //craneDir = Vector3.Lerp(craneDir, hardCraneDir, 0.1f);
        camHardPoint.position = craneDir + camCrane.position;
    }
}
