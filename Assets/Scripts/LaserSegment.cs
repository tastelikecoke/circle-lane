using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserSegment : MonoBehaviour
{
    public Vector3 target;
    public float maxLifetime = 5f;
    public float segmentLength = 0.43f;
    public string flag = "";
    
    private float lifetime = 5f;
    private float nextSegmentRemainingTime = 0.1f;
    private float nextSegmentTime = 0.1f;
    private bool finished = false;

    public void Populate(Vector3 target, float maxLifetime, float nextSegmentTime, string flag)
    {
        this.target = target;
        this.nextSegmentTime = nextSegmentTime;
        this.maxLifetime = maxLifetime;
        this.flag = flag;
        finished = false;

        if((target - this.transform.position).magnitude < 1f)
        {
            finished = true;
        }
        
        Vector3 directionVector = this.transform.position - target;
        this.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(directionVector.y, directionVector.x) * 180f / Mathf.PI);
        lifetime = maxLifetime;
        nextSegmentRemainingTime = nextSegmentTime;
    }

    void Update()
    {
        lifetime -= Time.deltaTime;
        nextSegmentRemainingTime -= Time.deltaTime;
        if(lifetime < 0f)
            Destroy(this.gameObject);
        if(nextSegmentRemainingTime < 0f && !finished)
        {
            LaserSegment newSegment = Instantiate(this);
            newSegment.gameObject.SetActive(true);
            newSegment.transform.position = this.transform.position;
            Vector3 segmentDirection = (target - this.transform.position).normalized;
            newSegment.transform.position += (segmentDirection * segmentLength);
            newSegment.Populate(target, maxLifetime, nextSegmentTime, flag);
            finished = true;
        }
    }
    
    public void Explode()
    {
        Destroy(this.gameObject);
    }
}
