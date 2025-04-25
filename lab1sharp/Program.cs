using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

class Program
{
    static int threadCount;
    static int[] durations;  
    static int[] steps;      
    static bool[] canStop;

    static void Main(string[] args)
    {
        threadCount = 4;
        durations = new int[] { 3000, 2000, 5000, 4000 }; 
        steps = new int[] { 1, 2, 3, 4 };

        canStop = new bool[threadCount];
        Thread[] threads = new Thread[threadCount];

        for (int i = 0; i < threadCount; i++)
        {
            int index = i;
            int time = durations[i];
            int step = steps[i];
            threads[i] = new Thread(() => CalculateSum(index, step, time));
            threads[i].Start();
        }

        Thread controller = new Thread(TimeController);
        controller.Start();
    }

    static void CalculateSum(int threadIndex, int step, int time)
    {
        long sum = 0;
        int current = 0;
        int count = 0;

        while (!canStop[threadIndex])
        {
            sum += current;
            current += step;
            count++;
        }

        Console.WriteLine($"Потiк {threadIndex + 1}: сума = {sum}, крок = {step}, доданкiв = {count}, час = {time / 1000} сек");
    }

    static void TimeController()
    {
        int[] sortedIndexes = new int[threadCount];
        int[] sortedDurations = new int[threadCount];
        durations.CopyTo(sortedDurations, 0);

        for (int i = 0; i < threadCount; i++)
            sortedIndexes[i] = i;

        Array.Sort(sortedDurations, sortedIndexes);

        int prevTime = 0;
        for (int i = 0; i < threadCount; i++)
        {
            int index = sortedIndexes[i];
            int sleepTime = sortedDurations[i] - prevTime;
            Thread.Sleep(sleepTime);
            canStop[index] = true;
            prevTime = sortedDurations[i];
        }
    }
}