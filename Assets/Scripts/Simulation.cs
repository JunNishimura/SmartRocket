using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// add obstacles
public class Simulation : MonoBehaviour
{
    public static int curGeneration = 0;
    public static int ROCKET_NUM = 50;
    public static Vector2 spawnPos;
    public static Vector2 startPos;
    public static Vector2 targetPos;
    public static GameObject[] rockets;
    public static Vector2 X_BORDER;
    public static Vector2 Y_BORDER;
    public GameObject prefab;
    private Population population;

    private void Start() 
    {
        // set default info
        rockets   = new GameObject[ROCKET_NUM];
        startPos  = new Vector2(0f, -5f);
        targetPos = new Vector2(0f, 4.5f);
        X_BORDER = new Vector2(-4.3f, 4.3f);
        Y_BORDER = new Vector2(-6f, 6f);
        for (int i = 0; i < ROCKET_NUM; i++) 
        {
            rockets[i] = Instantiate(prefab, startPos,Quaternion.Euler(-90f, 0f, 0f))as GameObject;
        }
        Evolution(); // start initial evolution
    }

    private void Update() 
    {
        int stopRocketCnt = 0;
        for (int i = 0; i < ROCKET_NUM; i++) 
        {
            if(rockets[i].GetComponent<Rocket>().isStopRunning) stopRocketCnt++;
        }

        if (stopRocketCnt == ROCKET_NUM) 
        {
            Evolution();
        }
    }

    private void Evolution()
    {
        ++curGeneration;
        if (curGeneration == Population.GENMAX) return; // finish evolution
       
        if (curGeneration == 1) // if this is the first time to execute, then create new population
        {
            population = new Population(prefab);
        }
        else // from the second generation, let's alternate generation
        {
            population.alternate();
        }
    }
}