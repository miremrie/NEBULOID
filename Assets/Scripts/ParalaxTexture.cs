using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParalaxTexture : MonoBehaviour
{
    // Start is called before the first frame update
    //float scrollSpeed = 0.5f;
    Renderer rend;
    public float factor;

    public Transform target;

    void Start()
    {
        rend = GetComponent<Renderer>();
    }

    void Update()
    {
        var p = target.position;
        var pos = new Vector3(p.x, p.y, transform.position.z);
        transform.position = pos;

        float offsetX = pos.x;
        float offsetY = pos.y;
        rend.material.SetTextureOffset("_MainTex", new Vector2(factor * offsetX, factor * offsetY));
    }

}
