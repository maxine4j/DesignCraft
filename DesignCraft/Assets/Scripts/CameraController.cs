using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class CursorExtension
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Point
    {
        public int X;
        public int Y;
        public static implicit operator Vector2(Point p)
        {
            return new Vector2(p.X, p.Y);
        }
    }

    [DllImport("user32.dll")]
    public static extern bool SetCursorPos(int X, int Y);
    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out Point pos);
}

public class CameraController : MonoBehaviour
{
    public Transform target;
    public PlayerMovementController playerMovementController;
    public float distance = 15;
    public float xSpeed = 100;
    public float ySpeed = 100;
    public float yMinLimit = -20;
    public float yMaxLimit = 80;
    public float distanceMin = 3;
    public float distanceMax = 15;

    private int mouseDownX;
    private int mouseDownY;
    private float x;
    private float y;

    void Start()
    {
        Vector2 angles = transform.eulerAngles;
        x = angles.x;
        y = angles.y;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    void UpdateMouseInput()
    {
        x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
        y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
        Cursor.visible = false;
    }

    void UpdateCamera()
    {
        y = ClampAngle(y, yMinLimit, yMaxLimit);
        Quaternion rotation = Quaternion.Euler(y, x, 0);
        distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);
        RaycastHit hit;
        if (Physics.Linecast(target.position, transform.position, out hit))
        {
            distance -= hit.distance;
        }
        var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
        transform.rotation = rotation;
        transform.position = position;
    }

    void Update()
    {
        if (target)
        {
            // cursor visbility and positioning
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                CursorExtension.Point p;
                CursorExtension.GetCursorPos(out p);
                mouseDownX = p.X;
                mouseDownY = p.Y;
                Cursor.visible = false;
            }
            if (Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
            {
                CursorExtension.SetCursorPos(mouseDownX, mouseDownY);
                Cursor.visible = true;
            }

            // camera rotation
            if (Input.GetMouseButton(1)) // only right click down
            {
                // update the camera
                UpdateMouseInput();
                UpdateCamera();
                // update player rotation
                playerMovementController.transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
            }
            else if (Input.GetMouseButton(0)) // only left click down
            {
                // update the camera
                UpdateMouseInput();
                UpdateCamera();
            }

            if (Input.GetMouseButton(0) || Input.GetMouseButton(1))
            {
                y = ClampAngle(y, yMinLimit, yMaxLimit);
                Quaternion rotation = Quaternion.Euler(y, x, 0);
                transform.rotation = rotation;
            }

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 10, distanceMin, distanceMax);
            RaycastHit hit;
            if (Physics.Linecast(target.position, transform.position, out hit))
            {
                distance -= hit.distance;
            }
            var position = transform.rotation * new Vector3(0.0f, 0.0f, -distance) + target.position;
            transform.position = position;
        }
    }
}
