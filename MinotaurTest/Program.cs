using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;
using System.Diagnostics;

namespace MinotaurTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var entityManager = new EntityComponentManager();
            var systemManager = new SystemManager(entityManager);

            systemManager.AddSystem(new TestSystem());
            systemManager.AddSystem(new TestSystem2());
            systemManager.AddSystem(new TestSystem3());

            var entity = entityManager.CreateEntity();
            var entity2 = entityManager.CreateEntity();
            var entity3 = entityManager.CreateEntity();

            entity.AddComponent(new TestComponent(11));

            entity2.AddComponent(new TestComponent(21));
            entity2.AddComponent(new TestComponent2(22));

            entity3.AddComponent(new TestComponent(31));
            entity3.AddComponent(new TestComponent2(32));
            entity3.AddComponent(new TestComponent3(33));

            RunTest(systemManager);
        }

        private static void RunTest(SystemManager systemManager)
        {
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (var i = 0; i < 600000; i++)
            {
                systemManager.Update(new TimeSpan());
            }
            stopWatch.Stop();
            // Get the elapsed time as a TimeSpan value.
            TimeSpan ts = stopWatch.Elapsed;

            // Format and display the TimeSpan value.
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds / 10);
            Console.WriteLine("RunTime " + elapsedTime);
        }
    }
}
