using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    public static int LIFESPAN = 180; // 寿命 (frame)
    public Vector2[] chrom { get; private set; }
    public float fitness { get; private set; }
    public bool isStopRunning;
    private Rigidbody2D rb;
    private Vector2 acceleration;
    private Vector2 velocity;
    private int nowLife;
    private float speed;
    private int penalty = 1;

    private void Awake() 
    {
        chrom = new Vector2[LIFESPAN];
        for (int i = 0; i < LIFESPAN; i++) 
        {
            // assign random 2d vector
            this.chrom[i] = new Vector2(Mathf.Cos(Random.Range(-Mathf.PI/4, Mathf.PI+Mathf.PI/4)), 
                                        Mathf.Sin(Random.Range(-Mathf.PI/4, Mathf.PI+Mathf.PI/4)));
        }
        rb = this.GetComponent<Rigidbody2D>();
        fitness = 0.0f;
        isStopRunning = false;
        nowLife = 0;
        speed = 0.2f;
        // assign random 2d vector ranging from 0 to pi
        acceleration = chrom[0];
        velocity = acceleration;
    }

    private void FixedUpdate() 
    {
        if (isStopRunning) return;

        acceleration = chrom[nowLife];
        velocity += acceleration;
        rb.MovePosition(rb.position + velocity * speed * Time.fixedDeltaTime);

        // 寿命が来る、もしくはスクリーン上部から消えたら終了
        if (++nowLife == LIFESPAN || rb.position.y >= Simulation.Y_BORDER.y) 
        {
            if (rb.position.x < Simulation.X_BORDER.x ||
                rb.position.x > Simulation.X_BORDER.y ||
                rb.position.y < Simulation.Y_BORDER.x)
            {
                penalty *= 50;
            }
            StopRunning();
        }
    }

    private void OnCollisionEnter2D(Collision2D other) 
    {
        if (other.gameObject.CompareTag("Obstacle"))
        {
            // impose penalty if the rocket hits obstacle
            penalty *= 100;
        }
        StopRunning();
    }

    private void Update()
    {
        if (isStopRunning) 
            return;

        // heading toward moving direction
        transform.rotation = Quaternion.LookRotation(velocity, Vector3.forward);
    }

    private void calcFitness() 
    {
        // The closer the rocket to the target is, the lower the fitness is
        // In this case, the lower the fitness is, the better dna the rocket has
        fitness = Vector2.Distance(transform.position, Simulation.targetPos) * penalty;
    }

    // 交叉
    public void Crossover(Rocket p1, Rocket p2)  
    {
        UniformCrossover(p1, p2);
    }

    // 1点交叉
    private void OnePointCrossover(Rocket p1, Rocket p2) 
    {
        int point = Random.Range(0, Simulation.ROCKET_NUM-1);
        for (int i = 0; i <= point; i++)
        {
            this.chrom[i] = p1.chrom[i];
        }
        for (int i = point+1; i < Simulation.ROCKET_NUM; i++) 
        {
            this.chrom[i] = p2.chrom[i];
        }
    }

    // 2点交叉
    private void TwoPointCrossover(Rocket p1, Rocket p2) 
    {
        int point1 = Random.Range(0, Simulation.ROCKET_NUM-1);
        int point2 = (point1 + 1 + Random.Range(0, Simulation.ROCKET_NUM-2)) % (Simulation.ROCKET_NUM-1);
        // point1 < point2 にする
        if (point1 > point2) 
        {
            int tmp = point1;
            point1 = point2;
            point2 = tmp;
        }
        for (int i = 0; i <= point1; i++) 
        {
            this.chrom[i] = p1.chrom[i];
        }
        for (int i = point1+1; i <= point2; i++) 
        {
            this.chrom[i] = p2.chrom[i];
        }
        for (int i = point2+1; i < Simulation.ROCKET_NUM; i++) 
        {
            this.chrom[i] = p1.chrom[i];
        }
    }

    // 一様交叉
    private void UniformCrossover(Rocket p1, Rocket p2) 
    {
        for (int i = 0; i < Simulation.ROCKET_NUM; i++) 
        {
            if (Random.Range(0, 2) == 0) 
            {
                this.chrom[i] = p1.chrom[i];
            }
            else
            {
                this.chrom[i] = p2.chrom[i];
            }
        }
    }

    // 突然変異
    public void Mutate() 
    {
        for (int i = 0; i < LIFESPAN; i++) 
        {
            if (Random.Range(0.0f, 1.0f) < Population.MUTATEPROB) 
            {
                chrom[i] = new Vector2(Mathf.Cos(Random.Range(0, Mathf.PI)), 
                                       Mathf.Sin(Random.Range(0, Mathf.PI)));
            }
        }
    }

    private void StopRunning() 
    {
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        calcFitness();
        isStopRunning = true;
    }

    public void Reset() 
    {
        transform.position = Simulation.startPos;
        rb.constraints = RigidbodyConstraints2D.None;
        nowLife = 0;
        penalty = 1;
        acceleration = chrom[0];
        velocity = acceleration;
        isStopRunning = false;
    }
}