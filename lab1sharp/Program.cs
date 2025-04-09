using System;
using System.Threading;

namespace ПОП_лаб1
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Введіть кількість потоків: ");
            int threadCount = Convert.ToInt32(Console.ReadLine());

            Thread[] threads = new Thread[threadCount];
            ThreadController[] controllers = new ThreadController[threadCount];

            for (int i = 0; i < threadCount; i++)
            {
                int threadNumber = i + 1;
                int step = threadNumber;
                int duration = 5 + threadNumber * 2;

                var controller = new ThreadController(threadNumber, step, duration);
                controllers[i] = controller;

                threads[i] = new Thread(controller.CalculateSum);
                threads[i].Start();
            }

            foreach (var controller in controllers)
            {
                new Thread(controller.Control).Start();
            }
        }

        class ThreadController
        {
            private int threadNumber;
            private int step;
            private int durationSeconds;
            private volatile bool canRun = true;

            private long sum = 0;
            private long count = 0;

            public ThreadController(int threadNumber, int step, int durationSeconds)
            {
                this.threadNumber = threadNumber;
                this.step = step;
                this.durationSeconds = durationSeconds;
            }

            public void CalculateSum()
            {
                long current = 0;
                while (canRun)
                {
                    sum += current;
                    current += step;
                    count++;
                }

                Console.WriteLine($"Потік {threadNumber}: сума = {sum}, доданків = {count}");
            }

            public void Control()
            {
                Thread.Sleep(durationSeconds * 1000);
                canRun = false;
            }
        }
    }
}