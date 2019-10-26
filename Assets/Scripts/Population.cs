using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Population
{
    public static int GENMAX = 100;  // 世代数
    public static float MUTATEPROB = 0.2f; // 突然変異率
    public Rocket[] curIndividuals;
    private Rocket[] nextIndividuals;
    private float[] trFit;
    private int elite = 0; 
    private int superElite = 0; // elite which reaches the target 100%

    public Population(GameObject prefab)
    {
        curIndividuals  = new Rocket[Simulation.ROCKET_NUM];
        nextIndividuals = new Rocket[Simulation.ROCKET_NUM];
        trFit = new float[Simulation.ROCKET_NUM];

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

        // 前世代にTargetに到達した数を現世代のエリート数とする
        elite = 2;
        superElite = 0;
        for (int i = 0; i < Simulation.ROCKET_NUM; i++) 
        {
            if (curIndividuals[i].isReachedTarget) superElite++;
        }
        elite = Mathf.Max(elite+superElite, 5); // elite上限5
        
        // エリートは無条件に保存する
        for (int i = 0; i < elite; i++)
        {
            for (int j = 0; j < Rocket.LIFESPAN; j++)
            {
                nextIndividuals[i].chrom[j] = curIndividuals[i].chrom[j];
            }
        }

        float totalFitness = 0;
        float worstFitness = curIndividuals[Simulation.ROCKET_NUM-1].fitness; // ソート後、配列最後尾に最悪適応度が格納されている
        for (int i = 0; i < Simulation.ROCKET_NUM; i++) 
        {
            trFit[i] = (worstFitness - curIndividuals[i].fitness + 0.001f) / worstFitness;
            trFit[i] = Mathf.Pow(trFit[i], 4.0f);
            totalFitness += trFit[i];
        }

        // 交叉
        for (int i = elite; i < Simulation.ROCKET_NUM; i++) 
        {
            int parent = rouletteSelection(totalFitness);
            int r = Random.Range(0, 3);
            if (r == 0) 
            {
                nextIndividuals[i].prevFitness = trFit[i] * trFit[parent];
                nextIndividuals[i].Crossover(curIndividuals[i], curIndividuals[parent]);
            }
            else if (r == 1)
            {
                nextIndividuals[i].prevFitness = trFit[i] * trFit[parent];
                nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[i]);
            }
            else 
            {
                int anotherParent = rouletteSelection(totalFitness);
                nextIndividuals[i].prevFitness = trFit[anotherParent] * trFit[parent];
                nextIndividuals[i].Crossover(curIndividuals[parent], curIndividuals[anotherParent]);
            }
        }

        // 突然変異
        for (int i = superElite; i < Simulation.ROCKET_NUM; i++) 
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

    // ルーレット選択
    private int rouletteSelection(float totalFitness) 
    {
        float rand = Random.Range(0.0f, 1.0f);
        int rank = 1;
        for (; rank <= Simulation.ROCKET_NUM; rank++) 
        {
            float prob = trFit[rank-1] / totalFitness;
            if (rand <= prob) break;
            rand -= prob;
        }
        return rank-1;
    }

    // 確率に基づくランキング選択
    private int rankingSelection() 
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
            float pivot = curIndividuals[(int)((lb + ub)/2)].fitness;
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