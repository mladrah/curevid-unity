using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WPMF;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private static CameraController _instance;
    public static CameraController Instance { get => _instance; }

    #region Components
    private Camera cam;
    #endregion

    #region Camera Movement
    [Header("Camera Movement")]
    [SerializeField] private float movementSpeed = 0.75f;
    [SerializeField] private float movementTime = 7;
    public bool canMove = true;
    private Vector3 boundary;
    private Vector3 newPos;
    private bool isFlying;
    #endregion

    #region Camera Zoom
    [Header("Camera Zoom")]
    [SerializeField] private float zoomValue = 3;
    [SerializeField] private float zoomTime = 5;
    [SerializeField] private int maxZoomIn = 17;
    [SerializeField] private int maxZoomOut = 56;
    private float newOrthoSize;
    #endregion

    private void Awake() {
        _instance = this;
        cam = GetComponent<Camera>();
        newPos = transform.position;
        newOrthoSize = cam.orthographicSize;
        boundary.z = cam.transform.position.z;
    }

    void Update() {
        if (!canMove)
            return;

        if (!isFlying)
            Movement();
        Zoom();
    }

    #region Camera Movement Methods
    public void Movement() {
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) {
            newPos += (transform.up * movementSpeed);
        }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) {
            newPos += Vector3.down * movementSpeed;
        }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) {
            newPos += Vector3.left * movementSpeed;
        }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) {
            newPos += Vector3.right * movementSpeed;
        }

        newPos.x = Mathf.Clamp(newPos.x, -boundary.x, boundary.x);
        newPos.y = Mathf.Clamp(newPos.y, -boundary.y, boundary.y);
        newPos.z = Mathf.Clamp(newPos.z, boundary.z, boundary.z);

        transform.position = Vector3.Lerp(transform.position, newPos, movementTime * Time.deltaTime);
    }
    #endregion

    #region Camera Zoom Methods
    public void Zoom() {
        if (!UIManager.IsMouseOverUI()) {
            if (Input.GetAxis("Mouse ScrollWheel") > 0 && newOrthoSize > maxZoomIn) {
                newOrthoSize -= zoomValue;
                boundary.x += zoomValue * 2;
                boundary.y += zoomValue;
            }
            if (Input.GetAxis("Mouse ScrollWheel") < 0 && newOrthoSize < maxZoomOut) {
                newOrthoSize += zoomValue;
                boundary.x -= zoomValue * 2;
                boundary.y -= zoomValue;
            }
        }

        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, newOrthoSize, zoomTime * Time.deltaTime);
    }
    #endregion

    public void FlyToCountry(string name) {
        //if (cam.orthographicSize >= 30)
        //    return;

        isFlying = true;
        DOTween.Sequence().
        AppendCallback(() => WorldMap2D.instance.FlyToCountry(name)).
        AppendInterval(WorldMap2D.instance.navigationTime).
        AppendCallback(() => newPos = transform.position).
        AppendCallback(() => isFlying = false);
    }
}
