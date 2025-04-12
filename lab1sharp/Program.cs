using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading;

class Program
{
    static void Main(string[] args)
    {
        int[] threadIds = { 1, 2, 3, 4 };
        int[] delays = { 8000, 2000, 5000, 1000 };
        int[] steps = { 1, 2, 5, 7 };

        BreakThread breakThread = new BreakThread(threadIds, delays);

        for (int i = 0; i < threadIds.Length; i++)
        {
            new MainThread(threadIds[i], steps[i], breakThread).Start();
        }

        new Thread(breakThread.Run).Start();
    }
}

class MainThread
{
    private readonly int id;
    private readonly int step;
    private readonly BreakThread breakThread;
    private Thread thread;

    public MainThread(int id, int step, BreakThread breakThread)
    {
        this.id = id;
        this.step = step;
        this.breakThread = breakThread;
        this.thread = new Thread(Run);
    }

    public void Start()
    {
        thread.Start();
    }

    private void Run()
    {
        while (!breakThread.CanBreak(id))
        {
            Thread.Sleep(1);
        }

        Stopwatch stopwatch = Stopwatch.StartNew();
        long sum = 0;
        int count = 0;
        int value = 0;

        int workDuration = breakThread.GetDelay(id);

        while (stopwatch.ElapsedMilliseconds < workDuration)
        {
            sum += value;
            value += step;
            count++;
            Thread.Sleep(1);
        }

        stopwatch.Stop();
        int timeInSeconds = (int)Math.Round(stopwatch.ElapsedMilliseconds / 1000.0);
        Console.WriteLine($"Потік {id}: Сума = {sum}, Доданків = {count}, Час = {timeInSeconds} сек");
    }
}

class BreakThread
{
    private readonly ConcurrentDictionary<int, bool> canBreakMap = new ConcurrentDictionary<int, bool>();
    private readonly (int id, int delay)[] threadDelays;

    public BreakThread(int[] threadIds, int[] delays)
    {
        threadDelays = threadIds.Zip(delays, (id, delay) => (id, delay))
                                .OrderBy(t => t.delay)
                                .ToArray();
    }

    public void Run()
    {
        foreach (var (id, delay) in threadDelays)
        {
            Thread.Sleep(delay);
            canBreakMap[id] = true;
        }
    }

    public bool CanBreak(int id)
    {
        return canBreakMap.TryGetValue(id, out bool canBreak) && canBreak;
    }

    public int GetDelay(int id)
    {
        return threadDelays.FirstOrDefault(t => t.id == id).delay;
    }
}
