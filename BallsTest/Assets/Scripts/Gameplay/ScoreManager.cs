using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Test.Gameplay
{
    public class ScoreManager : MonoBehaviour
    {
        public Text text;
        public RectTransform scoreTransform;
        public RectTransform scoreMaxTransform;

        float score = 0;

        Vector3 startScoreScale;
        LTDescr lt;

        public float Score
        {
            get
            {
                return score;
            }
            set
            {
                score = value;
            }
        }
        public float lerpingScore
        {
            get;
            set;
        }

        public void UpdateText()
        {
            lerpingScore =
            Mathf.Lerp(
                lerpingScore
                , Score
                , ((Score - lerpingScore) / 10) * Time.deltaTime
            );

            lerpingScore =
            Mathf.CeilToInt(lerpingScore);

            text.text =
            "SCORE: " + lerpingScore.ToString();
        }

        public void AddScore(float score)
        {
            Score += score;

            if (scoreTransform)
            {
                StopAnimation();

                lt = scoreTransform.LeanScale(scoreMaxTransform.localScale, .25f)
                .setOnComplete(() =>
                {
                    scoreTransform.LeanScale(startScoreScale, 1f)
                    .setEaseInBack();
                });
            }
        }

        public void StopAnimation()
        {
            try
            {
                lt.cancel(scoreTransform.gameObject);
            }
            catch
            {

            }

            scoreTransform.localScale = startScoreScale;
        }

        void Start()
        {
            startScoreScale = scoreTransform.localScale;
        }

        void Update()
        {
            UpdateText();
        }
    }
}