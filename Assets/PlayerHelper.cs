using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Bolt;
using Ludiq;

public class PlayerHelper : MonoBehaviour
{
    /* This didn't work out.  But mostly because I forgot to refresh my Unit Options. */
    /* This would work to augment bolt with C# code but I already got a bolt equivalent */
    public bool IsThereRemainingTimeLeft()
    {
        float remainingTime = Variables.Object(this).Get<float>("remainingTime");
        Variables.Object(this).Set("remainingTime", remainingTime - Time.deltaTime);
        return remainingTime - Time.deltaTime > 0f;
    }

}
