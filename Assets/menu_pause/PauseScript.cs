using System.Collections;
using UnityEngine;

namespace Assets.menu_pause
{
    public class PauseScript : MonoBehaviour
    {
        private static bool GameIsPaused;

        public GameObject PauseMenu;
        public Animator   PauseAnimator;
        public string     FadeOutTriggerName = "FadeOut";

        void Start()
        {
            PauseMenu.SetActive(false);
            GameIsPaused = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                TogglePause();
            }
        }

        void TogglePause()
        {
            if (GameIsPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        public void PauseGame()
        {
            if (!GameIsPaused)
            {
                const string pauseMethod = nameof(ShowPaused);
                StopCoroutine(pauseMethod);
                StartCoroutine(pauseMethod);
            }
        }

        IEnumerator ShowPaused()
        {
            PauseMenu.SetActive(true);
            PauseAnimator.SetBool(FadeOutTriggerName, false);

            yield return new WaitForSeconds(0.15f);

            GameIsPaused   = true;
            Time.timeScale = 0;
        }

        private bool inAnimation(int layer)
        {
            return PauseAnimator.IsInTransition(layer) || PauseAnimator.GetCurrentAnimatorStateInfo(layer).normalizedTime < 1;
        }

        public void ResumeGame()
        {
            if (GameIsPaused)
            {
                const string hideMethod = nameof(HidePause);
                StopCoroutine(hideMethod);
                StartCoroutine(hideMethod);
            }
        }

        IEnumerator HidePause()
        {
            PauseAnimator.SetBool(FadeOutTriggerName, true);

            while (inAnimation(0))
            {
                yield return null;
            }
            
            PauseAnimator.SetBool(FadeOutTriggerName, false);
            
            PauseMenu.SetActive(false);
            Time.timeScale = 1;
            GameIsPaused   = false;
            yield return null;
        }

        public void QuitGame()
        {
            Debug.Log("Quit game ...");
            Application.Quit();
        }
    }
}