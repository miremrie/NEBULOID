using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolTestAI : MonoBehaviour
{
    public float moveSpeed = 2;
    public PatrolPath path;

    public bool circular;
    public bool forward;
    public int startIndex;
    public float nearDistance = 0.1f;

    PatrolWalk walk;

    // Start is called before the first frame update
    void Start()
    {
        UpdatePath();
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate((walk.Target() - transform.position).normalized
                            * moveSpeed * Time.deltaTime);
    }

    [ContextMenu("Update Path")]
    public void UpdatePath()
    {
        var cong = new PatrolWalkConfig
        {
            path = path,
            trans = transform,
            circular = circular,
            forward = forward,
            nearDistance = nearDistance,
            startIndex = startIndex
        };
        walk = new PatrolWalk(cong);
    }
}
