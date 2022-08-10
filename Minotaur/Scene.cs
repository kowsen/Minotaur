using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<TGame>
    {
        protected EntityComponentManager _entities;
        protected SystemManager<TGame> _systems;
        protected TGame _game;

        public virtual void Attach(TGame game)
        {
            _game = game;
            ResetECS();
        }

        public virtual void Initialize() { }

        public virtual void LoadContent() { }

        public void Activate()
        {
            _systems.ActivateSystems();
        }

        public void Deactivate()
        {
            _systems.DeactivateSystems();
        }

        public void Reset()
        {
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            _entities = new EntityComponentManager();
            _systems = new SystemManager<TGame>(_entities, _game);
        }

        public void Update(TimeSpan time)
        {
            _systems.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            _systems.Draw(time);
        }
    }
}
