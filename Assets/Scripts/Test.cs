using System.Globalization;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    public void Shuffle(List<int> numbers)
    {
        for (int i=0;i<numbers.Count;i++)
        {
            int index =UnityEngine.Random.Range(i,numbers.Count);
            int temp = numbers[index];
            numbers[index] = numbers[i];
            numbers[i] = temp;
        }
    }

    public bool ValidShuffle(Action<List<int>> shuffleFunc)
    {
        int[,] testRes = new int[10,10];
        List<int> list = new List<int>(){0,1,2,3,4,5,6,7,8,9};
        for (int i=0;i<10000;i++)
        {
            shuffleFunc(list);
            for (int j=0;j<10;j++)
            {
                testRes[list[j],j]++;
            }
        }
        for (int i=0;i<10;i++)
        {
            for (int j=0;j<10;j++)
            {
                if (testRes[i,j]>13000 || testRes[i,j]<9700)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public List<int> Generate(int min,int max,int count)
    {
        bool[] flag  = new bool[max-min];
        List<int> res = new List<int>();
        for (int i=0;i<(max-min);i++)
        {
            flag[i] = false;
        }
        while (count!=0)
        {
            int temp = UnityEngine.Random.Range(min,max);
            if (!flag[temp-min])
            {
                res.Add(temp);
                flag[temp-min] = true;
                count--;
            }
        }
        return res;
    }

}
