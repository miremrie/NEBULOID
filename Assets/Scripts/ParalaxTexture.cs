using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxTexture : MonoBehaviour
{
    // Start is called before the first frame update
    //float scrollSpeed = 0.5f;
    public CameraController camController;
    Renderer rend;
    public float factor;
    public float scale;

    public Transform target;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    private void OnEnable()
    {
        camController.onCameraMoved += OnCameraMoved;
    }
    private void OnDisable()
    {
        camController.onCameraMoved -= OnCameraMoved;
    }

    void OnCameraMoved()
    {
        Parallax();
        transform.localScale = Vector3.one * Camera.main.orthographicSize * scale;
    }

    private void Parallax()
    {
        var p = target.position;
        var pos = new Vector3(p.x, p.y, transform.position.z);
        transform.position = pos;

        float offsetX = pos.x;
        float offsetY = pos.y;
        rend.material.SetTextureOffset("_MainTex", new Vector2(factor * offsetX, factor * offsetY));
    }
}
