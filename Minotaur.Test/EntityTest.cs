using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Minotaur.Test
{
    public class EntityTest
    {
        [Fact]
        public void TestEntityGenerationAndReuse()
        {
            var manager = new EntityComponentManager();

            var firstEntity = manager.Create();
            var secondEntity = manager.Create();
            Assert.NotEqual(firstEntity, secondEntity);

            firstEntity.Delete();

            var thirdEntity = manager.Create();

            Assert.NotEqual(firstEntity, thirdEntity);
            Assert.NotEqual(secondEntity, thirdEntity);

            manager.CommitChanges();

            var reusedFirstEntity = manager.Create();
            Assert.Equal(firstEntity, reusedFirstEntity);

            var fourthEntity = manager.Create();
            Assert.NotEqual(firstEntity, fourthEntity);
            Assert.NotEqual(secondEntity, fourthEntity);
            Assert.NotEqual(thirdEntity, fourthEntity);
        }

        [Fact]
        public void TestGetEntityById()
        {
            var manager = new EntityComponentManager();

            var firstEntity = manager.Create();
            var secondEntity = manager.Create();
            var entityToDelete = manager.Create();

            entityToDelete.Delete();

            Assert.Equal(firstEntity, manager.GetById(firstEntity.Id));
            Assert.Equal(secondEntity, manager.GetById(secondEntity.Id));
            Assert.Equal(entityToDelete, manager.GetById(entityToDelete.Id));

            manager.CommitChanges();

            Assert.Equal(firstEntity, manager.GetById(firstEntity.Id));
            Assert.Equal(secondEntity, manager.GetById(secondEntity.Id));
            Assert.NotNull(Record.Exception(() => manager.GetById(entityToDelete.Id)));
        }

        class RedComponent : Component
        {
            public int Counter;

            public override void Reset()
            {
                Counter = 0;
            }
        }

        class GreenComponent : Component
        {
            public int Counter;

            public override void Reset()
            {
                Counter = 0;
            }
        }

        class BlueComponent : Component
        {
            public int Counter;

            public override void Reset()
            {
                Counter = 0;
            }
        }

        [Fact]
        public void TestSimpleQuery()
        {
            var manager = new EntityComponentManager();

            var redSignature = new Signature();
            redSignature.SetRequirements(typeof(RedComponent));

            var greenSignature = new Signature();
            greenSignature.SetRequirements(typeof(GreenComponent));

            var blueSignature = new Signature();
            blueSignature.SetRequirements(typeof(BlueComponent));

            var redEntity = manager.Create();
            redEntity.AddComponent<RedComponent>();

            var greenEntity = manager.Create();
            greenEntity.AddComponent<GreenComponent>();

            var blueEntity = manager.Create();
            blueEntity.AddComponent<BlueComponent>();

            Assert.Empty(manager.Get(redSignature));
            Assert.Empty(manager.Get(greenSignature));
            Assert.Empty(manager.Get(blueSignature));

            manager.CommitChanges();

            Assert.Equal(new List<Entity>() { redEntity }, manager.Get(redSignature));
            Assert.Equal(new List<Entity>() { greenEntity }, manager.Get(greenSignature));
            Assert.Equal(new List<Entity>() { blueEntity }, manager.Get(blueSignature));
        }

        [Fact]
        public void TestComplexQuery()
        {
            var manager = new EntityComponentManager();

            var redSignature = new Signature();
            redSignature.SetRequirements(typeof(RedComponent));

            var greenSignature = new Signature();
            greenSignature.SetRequirements(typeof(GreenComponent));

            var blueSignature = new Signature();
            blueSignature.SetRequirements(typeof(BlueComponent));

            var purpleSignature = new Signature();
            purpleSignature.SetRequirements(typeof(RedComponent), typeof(BlueComponent));
            purpleSignature.SetRestrictions(typeof(GreenComponent));

            var yellowSignature = new Signature();
            yellowSignature.SetRequirements(typeof(RedComponent), typeof(GreenComponent));
            yellowSignature.SetRestrictions(typeof(BlueComponent));

            var cyanSignature = new Signature();
            cyanSignature.SetRequirements(typeof(GreenComponent), typeof(BlueComponent));
            cyanSignature.SetRestrictions(typeof(RedComponent));

            var whiteSignature = new Signature();
            whiteSignature.SetRequirements(
                typeof(RedComponent),
                typeof(GreenComponent),
                typeof(BlueComponent)
            );

            var purpleEntity = manager.Create();
            purpleEntity.AddComponent<RedComponent>();
            purpleEntity.AddComponent<BlueComponent>();

            var yellowEntity = manager.Create();
            yellowEntity.AddComponent<RedComponent>();
            yellowEntity.AddComponent<GreenComponent>();

            var cyanEntity = manager.Create();
            cyanEntity.AddComponent<BlueComponent>();
            cyanEntity.AddComponent<GreenComponent>();

            manager.CommitChanges();

            Assert.Equal(
                new List<Entity>() { purpleEntity, yellowEntity },
                manager.Get(redSignature)
            );
            Assert.Equal(
                new List<Entity>() { yellowEntity, cyanEntity },
                manager.Get(greenSignature)
            );
            Assert.Equal(
                new List<Entity>() { purpleEntity, cyanEntity },
                manager.Get(blueSignature)
            );

            Assert.Equal(new List<Entity>() { purpleEntity }, manager.Get(purpleSignature));
            Assert.Equal(new List<Entity>() { yellowEntity }, manager.Get(yellowSignature));
            Assert.Equal(new List<Entity>() { cyanEntity }, manager.Get(cyanSignature));

            Assert.Empty(manager.Get(whiteSignature));

            var whiteEntity = purpleEntity;
            whiteEntity.AddComponent<GreenComponent>();

            manager.CommitChanges();

            Assert.Equal(
                new List<Entity>() { whiteEntity, yellowEntity },
                manager.Get(redSignature)
            );
            Assert.Equal(
                new List<Entity>() { yellowEntity, cyanEntity, whiteEntity },
                manager.Get(greenSignature)
            );
            Assert.Equal(
                new List<Entity>() { whiteEntity, cyanEntity },
                manager.Get(blueSignature)
            );

            Assert.Equal(new List<Entity>() { whiteEntity }, manager.Get(whiteSignature));
            Assert.Equal(new List<Entity>() { yellowEntity }, manager.Get(yellowSignature));
            Assert.Equal(new List<Entity>() { cyanEntity }, manager.Get(cyanSignature));

            Assert.Empty(manager.Get(purpleSignature));
        }

        [Fact]
        public void TestEntityComponents()
        {
            var manager = new EntityComponentManager();

            var redSignature = new Signature();
            redSignature.SetRequirements(typeof(RedComponent));

            var blueSignature = new Signature();
            blueSignature.SetRequirements(typeof(BlueComponent));

            var purpleEntity = manager.Create();
            purpleEntity.AddComponent<RedComponent>();
            purpleEntity.AddComponent<BlueComponent>();

            manager.CommitChanges();

            var purpleEntityFromRed = manager.Get(redSignature).First();
            var purpleEntityFromBlue = manager.Get(blueSignature).First();

            Assert.Equal(purpleEntityFromRed, purpleEntityFromBlue);

            Assert.Equal(0, purpleEntityFromRed.GetComponent<RedComponent>().Counter);
            Assert.Equal(0, purpleEntityFromRed.GetComponent<BlueComponent>().Counter);

            Assert.Equal(0, purpleEntityFromBlue.GetComponent<RedComponent>().Counter);
            Assert.Equal(0, purpleEntityFromBlue.GetComponent<BlueComponent>().Counter);

            purpleEntityFromRed.GetComponent<RedComponent>().Counter = 1;
            purpleEntityFromBlue.GetComponent<BlueComponent>().Counter = 2;

            Assert.Equal(1, purpleEntityFromRed.GetComponent<RedComponent>().Counter);
            Assert.Equal(2, purpleEntityFromRed.GetComponent<BlueComponent>().Counter);

            Assert.Equal(1, purpleEntityFromBlue.GetComponent<RedComponent>().Counter);
            Assert.Equal(2, purpleEntityFromBlue.GetComponent<BlueComponent>().Counter);
        }
    }
}
