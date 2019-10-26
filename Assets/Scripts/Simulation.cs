using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO:
// 軌跡を表現する
public class Simulation : MonoBehaviour
{
    public static int curGeneration = 0;
    public static int ROCKET_NUM = 50;
    public static Vector2 startPos;
    public static GameObject[] rockets;

    public static GameObject target;
    public static int Y_BORDER;
    public GameObject prefab;
    private Population population;
    private Camera cam;
    private bool isDragging;
    private int rotDirection = 1;

    private void Start() 
    {
        // set default info
        rockets   = new GameObject[ROCKET_NUM];
        target    = GameObject.Find("target");
        startPos  = new Vector2(0f, -5f);
        Y_BORDER = 6;
        cam = Camera.main;
        isDragging = false;
        for (int i = 0; i < ROCKET_NUM; i++) 
        {
            rockets[i] = Instantiate(prefab, startPos,Quaternion.Euler(-90f, 0f, 0f))as GameObject;
        }
        Evolution(); // start initial evolution
    }

    private void Update() 
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit2D = Physics2D.Raycast(ray.origin, ray.direction, 100.0f);
            
            if (hit2D.collider) 
            {
                if (hit2D.collider.gameObject.tag == "Target") 
                {
                    isDragging = true;
                }
            }
        } 
        else if (Input.GetMouseButtonUp(0)) 
        {
            isDragging = false;
            target.transform.localScale = new Vector3(0.4f, 0.4f, 0.4f);
            target.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, 1f);
            target.transform.rotation = Quaternion.Euler(Vector3.zero);
        }

        if (isDragging)
        {
            var screen = Input.mousePosition;
            screen.z = 0f;
            var p = cam.ScreenToWorldPoint(screen);
            var p2D = new Vector2(p.x, p.y);
            target.transform.position = p2D;
            target.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            target.GetComponent<Renderer>().material.color = new Color(1.0f, 0.7f, 0.7f, 1.0f);
            target.transform.Rotate(0f, 0f, 20f * rotDirection);
            rotDirection *= -1;
        }

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
        if (curGeneration <= Population.GENMAX) 
        {
            if (curGeneration == 1) // if this is the first time to execute, then create new population
            {
                population = new Population(prefab);
            }
            else // from the second generation, let's alternate generation
            {
                population.alternate();
            }
        }
        else 
        {
            Debug.Log($"最良適応度：{this.population.curIndividuals[0].fitness}");
        }
    }
}