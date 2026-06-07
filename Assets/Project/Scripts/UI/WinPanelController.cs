using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace SBabchuk
{
    public class WinPanelController : MonoBehaviour
    {
        public Panels type;

        public GameObject panel;

        public void OnEnable()
        {
            BarricadeController.OnGameOver += Show;
            LevelController.OnGameOver += Show;
        }

        public void OnDisable()
        {
            BarricadeController.OnGameOver -= Show;
            LevelController.OnGameOver -= Show;
        }

        public void Show(Panels _panel)
        {
            Debug.Log("Show" + _panel);
            if (type == _panel)
            {
                panel.SetActive(true);

                Time.timeScale = 0;
            }
        }

        public void Hide()
        {
            panel.SetActive(true);
        }

        public void SwitchScene()
        {
            Time.timeScale = 1;

            SceneManager.LoadScene(Scene.MainScene.ToString());

            Hide();
        }
    }
}
