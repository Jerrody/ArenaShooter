using UnityEngine;
using UnityEngine.SceneManagement;

namespace Game.UI.Buttons
{
    public sealed class PlayButtonController : ButtonController
    {
        [Header("Info")]
        [SerializeField] private Global.Scenes sceneToOpen;

        protected override void OnClick()
        {
            SceneManager.LoadScene((int)sceneToOpen);
        }
    }
}