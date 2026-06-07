using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SBabchuk
{
    public class GoToScene : MonoBehaviour
    {
        public Scene target;

        public void SwitchScene()
        {
            Debug.Log("SceneName to load: " + target);

            SceneManager.LoadScene(target.ToString());
        }
    }
}
