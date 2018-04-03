using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cc : MonoBehaviour
{

    private Vector3 mpLastFrame;
    private bool rmDown = false;
    private float cameraMoveRate;
    private float startSize;

    void Awake()
    {
        startSize = Screen.height / 200;
        cameraMoveRate = 1 / (startSize / Camera.main.orthographicSize * 100);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            Camera.main.orthographicSize += Input.GetAxis("Mouse ScrollWheel") * -8f;
            cameraMoveRate = 1 / (startSize / Camera.main.orthographicSize * 100);
        }

        //移动视角
        if (Input.GetMouseButtonDown(1))
        {
            rmDown = true;
            mpLastFrame = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(1)) rmDown = false;

        if (rmDown)
        {
            Vector3 dir = Input.mousePosition - mpLastFrame;
            Camera.main.transform.Translate(-dir * cameraMoveRate, Space.Self);
            mpLastFrame = Input.mousePosition;
        }
        if (Input.GetMouseButtonDown(1))
        {
            rmDown = true;
            mpLastFrame = Input.mousePosition;
        }

    }
}
