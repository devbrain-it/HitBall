using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Windows.Speech;

namespace Assets.scripts
{
    public class SpeechControlScript : MonoBehaviour
    {
        private KeywordRecognizer          Recognizer;
        private KeywordRecognizer          KeywordRecognizer;
        private Dictionary<string, Action> cmds;

        [Serializable]
        public struct Command
        {
            public string                    Phrase;
            public EventTrigger.TriggerEvent Operation;
        }

        public Command[] commands;

        private static class AttackerButtons
        {
            public const string TOUCH       = "Touch";
            public const string BALL        = "Ball";
            public const string BASKET_BALL = "Basketball";
        }

        // Use this for initialization
        void Start()
        {
            // TODO TEST
            // PrepareCommandoRecignition();

            // //////////////////////////////////////

            // PrepareStartStopRecognition();
            // KeywordRecognizer.Start();
        }

        private void PrepareCommandoRecignition()
        {
            cmds = new Dictionary<string, Action>();
            foreach (var command in commands)
            {
                // dieser pointer bleibt, solange
                var    cmdVal    = command;
                Action operation = () => cmdVal.Operation.Invoke(new BaseEventData(EventSystem.current));
                cmds.Add(command.Phrase, operation);
            }

            Recognizer                    =  new KeywordRecognizer(cmds.Keys.ToArray());
            Recognizer.OnPhraseRecognized += onRecognizePhrase;
        }

        private void PrepareStartStopRecognition()
        {
            var startListening = new Dictionary<string, Action>
                                 {
                                     {
                                         "do", () =>
                                               {
                                                   Debug.LogWarning("Start Listening");
                                                   Recognizer.Start();
                                               }
                                     },
                                     {
                                         "stop", () =>
                                                 {
                                                     Debug.LogWarning("Stop Listening");
                                                     Recognizer.Stop();
                                                 }
                                     }
                                 };
            KeywordRecognizer                    =  new KeywordRecognizer(startListening.Keys.ToArray());
            KeywordRecognizer.OnPhraseRecognized += args =>
                                                    {
                                                        Action action;
                                                        if (startListening.TryGetValue(args.text, out action))
                                                        {
                                                            action();
                                                        }
                                                    };
        }

        private void onRecognizePhrase(PhraseRecognizedEventArgs args)
        {
            Action action;
            if (cmds.TryGetValue(args.text, out action))
            {
                Debug.LogWarning("Speech: " + args.text);
                action();
            }
        }

        public void upgradeToMaxBasketball()
        {
            UpgradeToMaximum(AttackerButtons.BASKET_BALL);
        }

        public void upgradeToMaxBall()
        {
            UpgradeToMaximum(AttackerButtons.BALL);
        }

        public void upgradeToMaxTouch()
        {
            UpgradeToMaximum(AttackerButtons.TOUCH);
        }

        private static void UpgradeToMaximum(string title)
        {
            var balls = GameObject.FindGameObjectsWithTag(AttackerButtonScript.TAG);
            balls
               .Select(b => b.GetComponent<AttackerButtonScript>()).ToList()
               .Where(b => b.Titel.Equals(title))
               .ToList()
               .ForEach(button => button.UpgradeToMaximum());
        }

        // Update is called once per frame
        void Update()
        {
        }
    }
}