using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Buttons
{
    public sealed class PlayButton : ButtonController
    {
        [Header("Info")] [SerializeField] private Global.Global.Scenes sceneToOpen;

        protected override void OnClick()
        {
            SceneManager.LoadScene((int)sceneToOpen);
        }
    }
}