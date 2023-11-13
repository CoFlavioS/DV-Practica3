using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Camera-Control/Mouse Look")]

public class CameraMovement : MonoBehaviour
{
    public Rigidbody rb;
    public int speed;
    float actualSpeed;
    Vector3 velocity;

    Ray lightSwitch;
    RaycastHit other;
    public int reach;
    Light otherLight;
    Renderer otherRender;

    public enum RotationAxes { MouseXAndY = 0, MouseX = 1, MouseY = 2 }
    public RotationAxes axes = RotationAxes.MouseXAndY;
    public float sensitivityX = 10F;
    public float sensitivityY = 10F;

    public float minimumX = -360F;
    public float maximumX = 360F;

    public float minimumY = -50F;
    public float maximumY = 50F;

    float rotationX = 0F;
    float rotationY = 0F;

    Quaternion originalRotation;

    void Start()
    {
        actualSpeed = 0;
        velocity = Vector3.zero;

        // Make the rigid body not change rotation
        rb.freezeRotation = true;
        originalRotation = transform.localRotation;
    }
    void Update()
    {
        // LightSwitch raycastaa

        lightSwitch = new Ray(this.transform.position, this.transform.forward);
        if (Physics.Raycast(lightSwitch, out other) && other.distance <= reach)
        {
            otherLight = other.transform.gameObject.GetComponent<Light>();
            otherRender = other.transform.gameObject.GetComponent<Renderer>();
            if (otherLight != null)
            {
                Debug.DrawRay(lightSwitch.origin, lightSwitch.direction * reach);
                if (Input.GetButtonDown("Jump"))
                {
                    if (otherLight.enabled)
                    {
                        otherRender.material.shader = Shader.Find("Shader Graphs/BulbShader");
                        // Emision:______Color(4, 1.77254903, 0.580392182, 0)
                        // MainColor:____#1C1C4D00
                        otherLight.enabled = false;
                    }
                    else
                    {
                        otherRender.material.shader = Shader.Find("Universal Render Pipeline/Lit");
                        // Emision:______Color(6.11540747,3.9323225,0.657694399,1)
                        otherLight.enabled = true;
                    }
                }
            }
        }

        // Movement

        if (Input.GetButton("Horizontal") || Input.GetButton("Vertical"))
        {
            actualSpeed = Mathf.MoveTowards(actualSpeed, speed, 0.1f);
            velocity = (Vector3.right * actualSpeed * Input.GetAxis("Horizontal") + Vector3.forward * actualSpeed * Input.GetAxis("Vertical"));
        }
        else
        {
            actualSpeed = Mathf.MoveTowards(actualSpeed, 0, 0.1f);
            velocity = (Vector3.right * actualSpeed * Input.GetAxis("Horizontal") + Vector3.forward * actualSpeed * Input.GetAxis("Vertical"));
        }

        rb.velocity = transform.TransformDirection(velocity);

        // Looking rotation

        if (axes == RotationAxes.MouseXAndY)
        {
            // Read the mouse input axis
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;

            rotationX = ClampAngle(rotationX, minimumX, maximumX);
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            Quaternion yQuaternion = Quaternion.AngleAxis(rotationY, -Vector3.right);

            transform.localRotation = originalRotation * xQuaternion * yQuaternion;
        }
        else if (axes == RotationAxes.MouseX)
        {
            rotationX += Input.GetAxis("Mouse X") * sensitivityX;
            rotationX = ClampAngle(rotationX, minimumX, maximumX);

            Quaternion xQuaternion = Quaternion.AngleAxis(rotationX, Vector3.up);
            transform.localRotation = originalRotation * xQuaternion;
        }
        else
        {
            rotationY += Input.GetAxis("Mouse Y") * sensitivityY;
            rotationY = ClampAngle(rotationY, minimumY, maximumY);

            Quaternion yQuaternion = Quaternion.AngleAxis(-rotationY, Vector3.right);
            transform.localRotation = originalRotation * yQuaternion;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
