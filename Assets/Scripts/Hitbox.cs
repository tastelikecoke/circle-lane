using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Hitbox : MonoBehaviour
{
    public List<Collider2D> collidingObjects = new List<Collider2D>();
    public delegate void OnHitEnterDelegate(Collider2D collider);
    public OnHitEnterDelegate onHitEnter;

    void OnTriggerEnter2D(Collider2D collider)
    {
        if(onHitEnter != null)
            onHitEnter(collider);
        collidingObjects.Add(collider);
    }

    public void Refresh()
    {
        for(int i = 0; i < collidingObjects.Count; i++)
        {
            if(collidingObjects[i] == null)
            {
                collidingObjects.RemoveAt(i);
                i--;
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D collider)
    {
        if(collidingObjects.Contains(collider))
            collidingObjects.Remove(collider);
        Debug.Log(collidingObjects.Count);
    }
}
