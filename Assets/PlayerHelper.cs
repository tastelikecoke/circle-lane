using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Ludiq;

public class PlayerHelper : MonoBehaviour
{
    public bool IsThereRemainingTimeLeft()
    {
        float remainingTime = Variables.Object(this).Get<float>("remainingTime");
        Variables.Object(this).Set("remainingTime", remainingTime - Time.deltaTime);
        return remainingTime - Time.deltaTime > 0f;
    }
}
