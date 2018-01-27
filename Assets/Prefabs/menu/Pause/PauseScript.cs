using UnityEngine;
using AnimationClip = Assets.scripts.clip.AnimationClip;

namespace Assets.Prefabs.menu.Pause
{
    public class PauseScript : MonoBehaviour
    {
        public static bool IsPaused;

        public Canvas   PauseMenuCanvas;
        public Animator FadeOutAnimator;
        public string   FadeOutAnimationTriggerName;

        void Start()
        {
            ResumeGame();
            FadeOutAnimator.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (IsPaused)
                {
                    ResumeGame();
                }
                else
                {
                    PauseGame();
                }
            }

            if (!IsPaused)
            {
                if (!new AnimationClip(FadeOutAnimator, FadeOutAnimationTriggerName).IsPlaying)
                {
                    PauseMenuCanvas.gameObject.SetActive(false);
                    FadeOutAnimator.enabled = false;
                }
            }
        }

        public void PauseGame()
        {
            IsPaused                = true;
            Time.timeScale          = 0;
            FadeOutAnimator.enabled = true;
            PauseMenuCanvas.gameObject.SetActive(true);
        }

        public void ResumeGame()
        {
            IsPaused       = false;
            Time.timeScale = 1;
            if (FadeOutAnimator == null || FadeOutAnimationTriggerName == null)
            {
                PauseMenuCanvas.gameObject.SetActive(false);
            }
            else
            {
                FadeOutAnimator.SetTrigger(FadeOutAnimationTriggerName);
            }
        }

        public void QuitGame()
        {
            Debug.Log("Quit game ...");
            Application.Quit();
        }
    }
}