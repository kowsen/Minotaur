using System.Collections.Generic;
using Xunit;

namespace Minotaur.Test
{
    public class SystemTest
    {
        // class MockEntitySystem : EntitySystem<object>
        // {
        //     public int InitializeCalls;
        //     public int CleanupCalls;

        //     public override void Initialize()
        //     {
        //         InitializeCalls += 1;
        //     }

        //     public override void Cleanup()
        //     {
        //         CleanupCalls += 1;
        //     }

        //     public override void Up()
        //     {
        //         CleanupCalls += 1;
        //     }
        // }

        // [Fact]
        // public void TestEntityGenerationAndReuse() { }
    }
}


// Add system tests that entity systems get called before game systems
// Add system tests that entity systems are called with all expected entities in both ways (with set and per entity)
// Test Init + cleanup logic