using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public static int GENMAX = 100;  // 世代数
    public static float MUTATEPROB = 0.1f; // 突然変異率
    public static int ELITE = 5; // デフォルトのエリート数
    public Rocket[] curIndividuals;
    private Rocket[] nextIndividuals;
    public Population(GameObject prefab)
    {
        curIndividuals  = new Rocket[Simulation.ROCKET_NUM];
        nextIndividuals = new Rocket[Simulation.ROCKET_NUM];

        for (int i = 0; i < Simulation.ROCKET_NUM; i++) 
        {
            curIndividuals[i] = Simulation.rockets[i].GetComponent<Rocket>();
            curIndividuals.CopyTo(nextIndividuals, 0);
        }
    }

    public void alternate() 
    {
        // 適応度を昇順に並び替える
        quickSort(0, Simulation.ROCKET_NUM-1); 

        Debug.Log($"第{Simulation.curGeneration-1}世代 最良適応度: {this.curIndividuals[0].fitness}");
        
        // エリートは無条件に保存する
        for (int i = 0; i < Population.ELITE; i++)
        {
            for (int j = 0; j < Rocket.LIFESPAN; j++)
            {
                nextIndividuals[i].chrom[j] = curIndividuals[i].chrom[j];
            }
        }

        // 親を選択して交叉する
        for (int i = ELITE; i < Simulation.ROCKET_NUM; i++) 
        {
            int p1 = selection();
            int p2 = selection();
            nextIndividuals[i].Crossover(curIndividuals[p1], curIndividuals[p2]);
        }

        // 突然変異
        for (int i = ELITE; i < Simulation.ROCKET_NUM; i++) 
        {
            nextIndividuals[i].Mutate();
        }

        // 次世代に受け継ぐ
        Rocket[] tmp = new Rocket[Simulation.ROCKET_NUM];
        curIndividuals.CopyTo(tmp, 0);
        nextIndividuals.CopyTo(curIndividuals, 0);
        tmp.CopyTo(nextIndividuals, 0);

        // reset all rockets 
        for (int i = 0; i < Simulation.ROCKET_NUM; i++) 
        {
            curIndividuals[i].Reset();
        }
    }

    // select one number based on the individual's fitness
    private int selection() 
    {
        int rn = Simulation.ROCKET_NUM;
        int denom = rn * (rn + 1) / 2;
        float r = Random.Range(0.0f, 1.0f);

        int rank = 1;
        for (; rank <= rn; rank++) 
        {
            // 個体群は適応度に基づいて昇順にソートされているから、ランキングが高い（小さい）方が選ばれやすい
            float prob = (float)(rn - (rank-1)) / denom;
            if (r <= prob) break;
            r -= prob;
        }
        return rank-1;
    }

    // クイックソート
    private void quickSort(int lb, int ub) 
    {
        if (lb < ub) 
        {
            float pivot = this.curIndividuals[(int)((lb + ub)/2)].fitness;
            int i = lb;
            int j = ub;
            while (i <= j) 
            {
                while (this.curIndividuals[i].fitness < pivot) 
                {
                    i++;
                }
                while (this.curIndividuals[j].fitness > pivot) 
                {
                    j--;
                }
                // swap
                if (i <= j) 
                {
                    var tmp = curIndividuals[i];
                    curIndividuals[i] = curIndividuals[j];
                    curIndividuals[j] = tmp;
                    i++;
                    j--;
                }
            }
            quickSort(lb, j);
            quickSort(i, ub);
        }
    }
}