using System;
using System.Linq;
using Xunit;

namespace Minotaur.Test
{
    public class SystemTest
    {
        class ColorComponent : Component
        {
            public int UpdateCounter;
            public int DrawCounter;

            public override void Reset()
            {
                UpdateCounter = 0;
                DrawCounter = 0;
            }
        }

        class RedComponent : ColorComponent { }

        class GreenComponent : ColorComponent { }

        class BlueComponent : ColorComponent { }

        class InitialPurpleFlagComponent : Component
        {
            public override void Reset() { }
        }

        class PurpleFactoryComponent : Component
        {
            public int Counter;

            public override void Reset()
            {
                Counter = 0;
            }
        }

        class MockGame
        {
            public int ActiveSystems = 0;
            public int ToAddToCounter = 1;
        }

        class PurpleFactorySystem : System<MockGame>
        {
            public override void Initialize()
            {
                Game.ActiveSystems += 1;
                Signature.AddRequirement<PurpleFactoryComponent>();
            }

            public override void Cleanup()
            {
                Game.ActiveSystems -= 1;
            }

            public override void Update(TimeSpan time, EntitySet entities)
            {
                foreach (var entity in entities)
                {
                    var factory = entity.GetComponent<PurpleFactoryComponent>();
                    if (factory.Counter == 0)
                    {
                        entity.RemoveComponent<PurpleFactoryComponent>();
                    }
                    else
                    {
                        var newPurple = Entities.Create();
                        newPurple.AddComponent<RedComponent>();
                        newPurple.AddComponent<BlueComponent>();
                        factory.Counter -= 1;
                    }
                }
            }
        }

        class PurpleDrawSystem : System<MockGame>
        {
            public override void Initialize()
            {
                Game.ActiveSystems += 1;
                Signature.AddRequirement<RedComponent>();
                Signature.AddRequirement<BlueComponent>();
                Signature.AddRestriction<GreenComponent>();
            }

            public override void Cleanup()
            {
                Game.ActiveSystems -= 1;
            }

            public override void Update(TimeSpan time, EntitySet entities)
            {
                foreach (var entity in entities)
                {
                    entity.GetComponent<RedComponent>().UpdateCounter += Game.ToAddToCounter;
                    entity.GetComponent<BlueComponent>().UpdateCounter += Game.ToAddToCounter;
                }
            }

            public override void Draw(TimeSpan time, EntitySet entities)
            {
                foreach (var entity in entities)
                {
                    entity.GetComponent<RedComponent>().DrawCounter += Game.ToAddToCounter;
                    entity.GetComponent<BlueComponent>().DrawCounter += Game.ToAddToCounter;
                }
            }
        }

        [Fact]
        public void TestWorld()
        {
            var game = new MockGame();

            var world = new World<MockGame>(game);

            world.AddSystem<PurpleFactorySystem>();
            world.AddSystem<PurpleDrawSystem>();

            Assert.Equal(2, game.ActiveSystems);

            var whiteEntity = world.Entities.Create();
            whiteEntity.AddComponent<RedComponent>();
            whiteEntity.AddComponent<BlueComponent>();
            whiteEntity.AddComponent<GreenComponent>();

            var purpleEntity = world.Entities.Create();
            purpleEntity.AddComponent<RedComponent>();
            purpleEntity.AddComponent<BlueComponent>();
            purpleEntity.AddComponent<InitialPurpleFlagComponent>();

            var generatedPurpleSignature = new Signature();
            generatedPurpleSignature.AddRequirement<RedComponent>();
            generatedPurpleSignature.AddRequirement<BlueComponent>();
            generatedPurpleSignature.AddRestriction<GreenComponent>();
            generatedPurpleSignature.AddRestriction<InitialPurpleFlagComponent>();

            var factoryEntity = world.Entities.Create();
            var factoryComponent = factoryEntity.AddComponent<PurpleFactoryComponent>();
            factoryComponent.Counter = 5;

            for (var i = 0; i < 4; i++)
            {
                world.Update(new TimeSpan());
                world.Draw(new TimeSpan());
            }

            Assert.True(factoryEntity.HasComponent<PurpleFactoryComponent>());
            Assert.Equal(1, factoryEntity.GetComponent<PurpleFactoryComponent>().Counter);

            for (var i = 0; i < 6; i++)
            {
                world.Update(new TimeSpan());
                world.Draw(new TimeSpan());
            }

            Assert.False(factoryEntity.HasComponent<PurpleFactoryComponent>());

            Assert.Equal(0, whiteEntity.GetComponent<RedComponent>().UpdateCounter);
            Assert.Equal(0, whiteEntity.GetComponent<BlueComponent>().UpdateCounter);
            Assert.Equal(0, whiteEntity.GetComponent<GreenComponent>().UpdateCounter);

            Assert.Equal(10, purpleEntity.GetComponent<RedComponent>().UpdateCounter);
            Assert.Equal(10, purpleEntity.GetComponent<BlueComponent>().UpdateCounter);

            Assert.Equal(10, purpleEntity.GetComponent<RedComponent>().DrawCounter);
            Assert.Equal(10, purpleEntity.GetComponent<BlueComponent>().DrawCounter);

            var generatedPurpleEntities = world.Entities.Get(generatedPurpleSignature);
            Assert.Equal(5, generatedPurpleEntities.Count);

            var list = generatedPurpleEntities.ToList();
            list.Sort(
                (Entity a, Entity b) =>
                {
                    return a.GetComponent<RedComponent>()
                        .UpdateCounter.CompareTo(b.GetComponent<RedComponent>().DrawCounter);
                }
            );

            for (var i = 0; i < list.Count; i++)
            {
                var item = list[i];
                var updateCount = i + 5;

                Assert.Equal(updateCount, item.GetComponent<RedComponent>().UpdateCounter);
                Assert.Equal(updateCount, item.GetComponent<BlueComponent>().UpdateCounter);

                Assert.Equal(updateCount + 1, item.GetComponent<RedComponent>().DrawCounter);
                Assert.Equal(updateCount + 1, item.GetComponent<BlueComponent>().DrawCounter);
            }

            world.Cleanup();
            Assert.Equal(0, game.ActiveSystems);
        }
    }
}
