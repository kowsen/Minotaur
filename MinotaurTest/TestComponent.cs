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
        public int Sup;

        public TestComponent(int sup)
        {
            Sup = sup;
        }
    }
}
