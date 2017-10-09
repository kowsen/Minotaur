using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class World
    {
        protected Dictionary<string, Scene> _scenes;
        private Scene _activeScene;

        public EventBus Bus { get; private set; }

        public World()
        {
            _scenes = new Dictionary<string, Scene>();
            Bus = new EventBus();
        }

        public void AddScene(string key, Scene scene)
        {
            if (_scenes.ContainsKey(key))
            {
                throw new Exception($"Trying to insert duplicate scene with key: {key}");
            }
            scene.Initialize(Bus);
            _scenes[key] = scene;
        }

        public void Switch(string key)
        {
            var success = _scenes.TryGetValue(key, out _activeScene);
            if (!success)
            {
                throw new Exception($"Tried to change to nonexistent scene {key}");
            }
        }

        public void Reset(string key)
        {
            var success = _scenes.TryGetValue(key, out Scene scene);
            if (!success)
            {
                throw new Exception($"Tried to reset nonexistent scene {key}");
            }
            scene.Reset();
        }

        public void Update(TimeSpan time)
        {
            if (_activeScene != null)
            {
                _activeScene.Update(time);
            }
        }

        public void Draw(TimeSpan time)
        {
            if (_activeScene != null)
            {
                _activeScene.Draw(time);
            }
        }
    }
}
