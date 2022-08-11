using Xunit;

namespace Minotaur.Test
{
    public class UnitTest1
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
    }
}
