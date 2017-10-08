using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace MinotaurTest
{
    public class TestSystem : GameSystem
    {
        private const bool IS_SILENT = true;
        private int val;

        public TestSystem()
        {
            AddComponentConstraint<TestComponent>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var num = entity.GetComponent<TestComponent>().Val;
            val = num * num;
            if (!IS_SILENT)
            {
                Console.WriteLine($"System: Entity {entity.Id} has TestComponent with val {num}");
            }
        }
    }

    public class TestSystem2 : GameSystem
    {
        private const bool IS_SILENT = true;
        private int val;

        public TestSystem2()
        {
            AddComponentConstraint<TestComponent>();
            AddComponentConstraint<TestComponent2>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var num = entity.GetComponent<TestComponent>().Val;
            var num2 = entity.GetComponent<TestComponent2>().Val;
            val = num * num2;
            if (!IS_SILENT)
            {
                Console.WriteLine($"System2: Entity {entity.Id} has TestComponent with val {num}, TestComponent2 with val {num2}");
            }
        }
    }

    public class TestSystem3 : GameSystem
    {
        private const bool IS_SILENT = true;
        private int val;

        public TestSystem3()
        {
            AddComponentConstraint<TestComponent>();
            AddComponentConstraint<TestComponent2>();
            AddComponentConstraint<TestComponent3>();
        }

        public override void Update(TimeSpan time, Entity entity)
        {
            var num = entity.GetComponent<TestComponent>().Val;
            var num2 = entity.GetComponent<TestComponent2>().Val;
            var num3 = entity.GetComponent<TestComponent3>().Val;
            val = num * num2 * num3;
            if (!IS_SILENT)
            {
                Console.WriteLine($"System3: Entity {entity.Id} has TestComponent with val {num}, TestComponent2 with val {num2}, TestComponent3 with val {num3}");
            }
        }
    }
}
