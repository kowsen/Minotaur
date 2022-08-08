using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class Scene<TGame>
    {
        protected EntityComponentManager _entities;
        protected SystemManager<TGame> _systems;
        protected ErrandManager<TGame> _errands;
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
            _errands.Clear();
            ResetECS();
            Initialize();
        }

        private void ResetECS()
        {
            _entities = new EntityComponentManager();
            _errands = new ErrandManager<TGame>(_entities, _game);
            _systems = new SystemManager<TGame>(_entities, _errands, _game);
        }

        public void Update(TimeSpan time)
        {
            _systems.Update(time);
            _errands.Update(time);
        }

        public void Draw(TimeSpan time)
        {
            _errands.Draw(time, Errand<TGame>.DrawStatus.BEFORE);
            _systems.Draw(time);
            _errands.Draw(time, Errand<TGame>.DrawStatus.AFTER);
        }
    }
}
