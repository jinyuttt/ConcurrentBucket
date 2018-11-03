using ConcurrentList;
using System;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static int num = 10;
        static void Main(string[] args)
        {
            for (int i = 0; i < num; i++)
            {
                Test();
            }
            Console.Read();

        }
        private static void Test()
        {
           
            //ConcurrentBucket<int> stack = new ConcurrentBucket<int>();
            IBucketStack<int> stack = new ConcurrentBucket<int>();
            
            int num = 100;
            Task[] tasks = new Task[num];
            Task[] resut = new Task[10];
            DateTime start = DateTime.Now;
            for (int i = 0; i < num; i++)
            {
                Task task = Task.Factory.StartNew(() =>
                  {
                      for (int j = i * 1000; j < 1000 + i * 1000; j++)
                      {
                          stack.Push(j);
                      }
                  });
                tasks[i] = task;
            }
           for (int i = 0; i < resut.Length; i++)
            {

            Task task = Task.Factory.StartNew(() =>
            {
                int r = 0;
                while (!stack.IsEmpty)
                {
                    stack.TryPop(out r);
                }
            });
            resut[i] = task;
        }
        Task.WaitAll(tasks);
        Task.WaitAll(resut);

            Console.WriteLine((DateTime.Now - start).TotalSeconds);
            Console.WriteLine("");
          

        }


    }
}
