using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaitForSEcondsRealTime : CustomYieldInstruction
{
    private float waitTime;

    public override bool keepWaiting
    {
        get
        { 
        return Time.realtimeSinceStartup < waitTime;
        }
    
    }

    public WaitForSEcondsRealTime(float time)
    {
        waitTime = Time.realtimeSinceStartup + time;
    }
}
