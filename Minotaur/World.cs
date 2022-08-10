using System;
using System.Collections.Generic;

namespace Minotaur
{
    public class World<TGame>
    {
        protected Dictionary<string, Scene<TGame>> _scenes = new Dictionary<string, Scene<TGame>>();
        private Scene<TGame> _activeScene;

        private TGame _game;

        public World(TGame game)
        {
            _game = game;
        }

        public void AddScene(string key, Scene<TGame> scene)
        {
            if (_scenes.ContainsKey(key))
            {
                throw new Exception($"Trying to insert duplicate scene with key: {key}");
            }
            scene.Attach(_game);
            _scenes[key] = scene;
        }

        public void LoadContent()
        {
            foreach (var scene in _scenes)
            {
                scene.Value.LoadContent();
            }
        }

        public void Initialize()
        {
            foreach (var scene in _scenes)
            {
                scene.Value.Initialize();
            }
        }

        public void Switch(string key)
        {
            _activeScene?.Deactivate();
            var success = _scenes.TryGetValue(key, out _activeScene);
            if (!success)
            {
                throw new Exception($"Tried to change to nonexistent scene {key}");
            }
            _activeScene.Activate();
        }

        public void Reset(string key)
        {
            var success = _scenes.TryGetValue(key, out var scene);
            if (!success)
            {
                throw new Exception($"Tried to reset nonexistent scene {key}");
            }
            if (scene == _activeScene)
            {
                scene.Deactivate();
                scene.Reset();
                scene.Activate();
            }
            else
            {
                scene.Reset();
            }
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
