using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Minotaur;

namespace MinotaurTest
{
    public class TestComponent : IComponent
    {
        public int Val;

        public TestComponent(int val)
        {
            Val = val;
        }
    }

    public class TestComponent2 : IComponent
    {
        public int Val;

        public TestComponent2(int val)
        {
            Val = val;
        }
    }

    public class TestComponent3 : IComponent
    {
        public int Val;
        
        public TestComponent3(int val)
        {
            Val = val;
        }
    }
}
