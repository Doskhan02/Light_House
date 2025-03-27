using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    private float sessionTime;
    private float gameActiveTime;
    private int sessionTimeInSeconds;
    private int sessionTimeInMinutes;

    public int SessionTimeInMinutes => sessionTimeInMinutes;
    public int SessionTimeInSeconds => sessionTimeInSeconds;
    public float GameActiveTime => gameActiveTime;
    public void ResetTime()
    {
        gameActiveTime = 0;
        sessionTimeInSeconds = 0;
        sessionTimeInMinutes = 0;
    }
    public void UpdateSessionTime()
    {
        sessionTime += Time.deltaTime;
        if (sessionTime > 1)
        {
            sessionTimeInSeconds++;
            sessionTime = 0;
            if (sessionTimeInSeconds % 60 == 0)
            {
                sessionTimeInMinutes++;
                sessionTimeInSeconds = 0;
            }
        }
    }
}
