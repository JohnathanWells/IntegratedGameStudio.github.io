using UnityEngine;
using System.Collections;

public class CameraHovering : MonoBehaviour {

    public bool hovering;
    public Vector2 minimunOffset;
    public Vector2 maximumOffset;
    public float speedOfHovering = 1f;

    Vector3 initialPosition;
    int dir = 1;

    void Start()
    {
        initialPosition = transform.position;
    }

    void Update()
    {
        if (hovering)
        {
            if (transform.position.y >=  initialPosition.y + maximumOffset.y)
                dir = -1;
            else if (transform.position.y <= initialPosition.y + minimunOffset.y)
                dir = 1;

            transform.Translate(0, dir * speedOfHovering * Time.deltaTime, 0);
            transform.Rotate(dir * speedOfHovering * Time.deltaTime * 4, 0, 0);
        }
    }
}
