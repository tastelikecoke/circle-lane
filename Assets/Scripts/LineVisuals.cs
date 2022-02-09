using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineVisuals : MonoBehaviour
{
    private LineRenderer lineRenderer = null;
    private float lifetime = 5f;
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }
    public void Populate(Vector3 startpoint, Vector3 endpoint, float lifetime = 5f)
    {
        lineRenderer.SetPosition(0, startpoint);
        lineRenderer.SetPosition(1, endpoint);
        this.lifetime = lifetime;
    }

    public void Update()
    {
        lifetime -= Time.deltaTime;
        if(lifetime < 0f)
            Destroy(this.gameObject);
    }
}
