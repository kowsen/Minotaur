using System;
using System.Collections.Generic;
using System.Text;

namespace Minotaur
{
    public class World<T>
    {
        protected Dictionary<string, Scene<T>> _scenes;
        private Scene<T> _activeScene;

        protected T Game;

        // Game represents a global game object we want available to all our scenes and systems.
        public World(T game)
        {
            _scenes = new Dictionary<string, Scene<T>>();
            Game = game;
        }

        public void AddScene(string key, Scene<T> scene)
        {
            if (_scenes.ContainsKey(key))
            {
                throw new Exception($"Trying to insert duplicate scene with key: {key}");
            }
            scene.Attach(Game);
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
            InitializeListeners();
        }

        public virtual void InitializeListeners()
        {

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
            Errand<T>.Clear();
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
