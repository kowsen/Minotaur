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

        // Check that queries work
        // Check you can get an entity by id
        // Check that delete updates queries only after we CommitChanges
        // Check that queries are cached
        // Check that all entity functions only update after CommitChanges


        // Add system tests that entity systems get called before game systems
        // Add system tests that they are otherwise called in registration order
        // Add system tests that entity systems are called with all expected entities in both ways (with set and per entity)
        // Test Init + cleanup logic
    }
}
