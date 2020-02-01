using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform target;
    public float smooth;

    // Update is called once per frame
    void Update()
    {
        var pos = target.position;
        var tar = new Vector3(pos.x, pos.y, transform.position.z);

        transform.position =  Vector3.Lerp(transform.position, tar, smooth * Time.deltaTime);
    }
}
