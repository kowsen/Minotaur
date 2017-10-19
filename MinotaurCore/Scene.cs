using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<T>
    {
        protected EntityComponentManager _ecm;
        protected SystemManager<T> _sm;

        protected T _game;

        public virtual void Attach(T game)
        {
            _game = game;
            ResetECS();
        }

        public virtual void Initialize()
        {

        }

        public virtual void LoadContent()
        {

        }

        public void Activate()
        {
            _sm.ActivateSystems();
        }

        public void Deactivate()
        {
            _sm.DeactivateSystems();
        }

        public void Reset()
        {
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            _ecm = new EntityComponentManager();
            _sm = new SystemManager<T>(_ecm, _game);
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
