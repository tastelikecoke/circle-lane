using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickEffect : MonoBehaviour
{
    private float lifetime = 0f;
    void Update()
    {
        lifetime += Time.deltaTime;
        if(lifetime > 5f)
        {
            Destroy(this.gameObject);
        }
    }
}
