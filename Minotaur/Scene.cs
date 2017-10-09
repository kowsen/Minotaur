using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene
    {
        protected EntityComponentManager _ecm;
        protected SystemManager _sm;

        protected EventBus _bus;

        public Scene()
        {
            ResetECS();
        }

        public virtual void Initialize(EventBus bus)
        {
            _bus = bus;
        }

        public void Reset()
        {
            ResetECS();
            Initialize(_bus);
        }

        private void ResetECS()
        {
            _ecm = new EntityComponentManager();
            _sm = new SystemManager(_ecm);
        }

        public void Update(TimeSpan time)
        {
            _sm.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            _sm.Draw(time);
        }
    }
}
