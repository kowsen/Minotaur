using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace MinotaurTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var manager = new EntityComponentManager();

            var entity = manager.CreateEntity();
            var entity2 = manager.CreateEntity();

            entity.AddComponent(new TestComponent(1));

            var entities = manager.GetEntities(manager.GetSignature(new List<Type>() { typeof(TestComponent) }));

            entity2.AddComponent(new TestComponent(2));

            entities = manager.GetEntities(manager.GetSignature(new List<Type>() { typeof(TestComponent) }));

            var component = entity.GetComponent<TestComponent>();
            var component2 = entity2.GetComponent<TestComponent>();

            entity.RemoveComponent(typeof(TestComponent));

            entities = manager.GetEntities(manager.GetSignature(new List<Type>() { typeof(TestComponent) }));

            component2 = entity2.GetComponent<TestComponent>();
            component = entity.GetComponent<TestComponent>();
        }
    }
}
