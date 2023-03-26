using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test.Game
{
    public class StartMenu : MonoBehaviour
    {
        float startTimeScale;
        public GameObject menuBackground;
        public GameObject inGameStats;

        public static bool isPaused = false;

        public void Pause()
        {
            if (menuBackground)
            {
                menuBackground.SetActive(true);
            }
            if (inGameStats)
            {
                inGameStats.SetActive(false);
            }

            Time.timeScale = 0;
            isPaused = true;
        }
        public void OpenShop()
        {

        }
        public void Play()
        {
            if (menuBackground)
            {
                menuBackground.SetActive(false);
            }
            if (inGameStats)
            {
                inGameStats.SetActive(true);
            }

            Time.timeScale = 1;

            isPaused = false;
        }
        public void Exit()
        {
            Application.Quit();
        }

        void Start()
        {
            startTimeScale = Time.timeScale;

            Pause();
        }
    }
}