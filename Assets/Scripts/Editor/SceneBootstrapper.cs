using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using System;

namespace Pantheon.Editor {
  [InitializeOnLoad]
  public class SceneBootstrapper {
    private const string _bootstrapSceneKey = "Pantheon/BootstrapScene";
    private const string _previousSceneKey = "Pantheon/PreviousScene";
    private const string _loadBootstrapSceneKey = "Pantheon/LoadBootstrapScene";

    private const string _loadBootstrapSceneOnPlay = "Pantheon/Load bootstrap scene on play";
    private const string _doNotLoadBootstrapSceneOnPlay =
        "Pantheon/Don't load bootstrap scene on play";

    private static bool _stoppingAndStarting;

    private static string _bootstrapScene {
      get {
        if (!EditorPrefs.HasKey(_bootstrapSceneKey)) {
          EditorPrefs.SetString(_bootstrapSceneKey, EditorBuildSettings.scenes[0].path);
        }
        return EditorPrefs.GetString(_bootstrapSceneKey, EditorBuildSettings.scenes[0].path);
      }
      set => EditorPrefs.SetString(_bootstrapSceneKey, value);
    }

    private static string _previousScene {
      get => EditorPrefs.GetString(_previousSceneKey);
      set => EditorPrefs.SetString(_previousSceneKey, value);
    }

    private static bool _loadBootstrapScene {
      get {
        if (!EditorPrefs.HasKey(_loadBootstrapSceneKey)) {
          EditorPrefs.SetBool(_loadBootstrapSceneKey, true);
        }
        return EditorPrefs.GetBool(_loadBootstrapSceneKey, true);
      }
      set => EditorPrefs.SetBool(_loadBootstrapSceneKey, value);
    }

    static SceneBootstrapper() {
      EditorApplication.playModeStateChanged += EditorApplicationOnplayModeStateChanged;
    }

    [MenuItem(_loadBootstrapSceneOnPlay, true)]
    private static bool ShowLoadBootstrapSceneOnPlay() {
      return !_loadBootstrapScene;
    }

    [MenuItem(_loadBootstrapSceneOnPlay)]
    private static void EnableLoadBootstrapSceneOnPlay() {
      _loadBootstrapScene = true;
    }

    [MenuItem(_doNotLoadBootstrapSceneOnPlay, true)]
    private static bool ShowDoNotLoadBootstrapSceneOnPlay() {
      return _loadBootstrapScene;
    }

    [MenuItem(_doNotLoadBootstrapSceneOnPlay)]
    private static void DisableDoNotLoadBootstrapSceneOnPlay() {
      _loadBootstrapScene = false;
    }

    private static void EditorApplicationOnplayModeStateChanged(PlayModeStateChange change) {
      if (!_loadBootstrapScene) {
        return;
      }

      if (_stoppingAndStarting) {
        if (change == PlayModeStateChange.EnteredPlayMode) {
          _stoppingAndStarting = false;
        }
        return;
      }

      if (change == PlayModeStateChange.ExitingEditMode) {
        _previousScene = EditorSceneManager.GetActiveScene().path;

        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) {
          if (!string.IsNullOrEmpty(_bootstrapScene) &&
              Array.Exists(EditorBuildSettings.scenes, scene => scene.path == _bootstrapScene)) {
            var activeScene = EditorSceneManager.GetActiveScene();

            _stoppingAndStarting =
                activeScene.path == string.Empty || !_bootstrapScene.Contains(activeScene.path);

            if (_stoppingAndStarting) {
              EditorApplication.ExitPlaymode();

              EditorSceneManager.OpenScene(_bootstrapScene);

              EditorApplication.EnterPlaymode();
            }
          }
        } else {
          EditorApplication.isPlaying = false;
        }
      } else if (change == PlayModeStateChange.EnteredEditMode) {
        if (!string.IsNullOrEmpty(_previousScene)) {
          EditorSceneManager.OpenScene(_previousScene);
        }
      }
    }
  }
}
