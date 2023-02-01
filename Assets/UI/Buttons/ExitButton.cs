#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Buttons
{
    public sealed class ExitButton : ButtonController
    {
        [Header("Info")] [SerializeField] private bool doExitFromApplication;
        [SerializeField] private Global.Global.Scenes sceneToOpen;

        protected override void OnClick()
        {
            if (doExitFromApplication)
            {
#if UNITY_EDITOR
                EditorApplication.ExitPlaymode();
#endif
                Application.Quit();
            }

            SceneManager.LoadScene((int)sceneToOpen);
        }
    }
}