using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace GameInit.LoseAnim
{
    public class LoseSceneAnimation
    {
        private LoseSceneHolder _LoseSceneHolder;
        private Button _reloadButton;

        private float initialIntensity;
        private float currentIntensity;

        public float targetIntensity = 0.62f;
        public float duration = 10.0f;

        private bool _canLose = true;

        private Vignette _vignette;
        public LoseSceneAnimation()
        {
            _LoseSceneHolder = Object.FindObjectOfType<LoseSceneHolder>();
            _vignette = _LoseSceneHolder.GetVignette();
            initialIntensity = _vignette.intensity.value;

            _reloadButton = _LoseSceneHolder.GetReloadButton().GetComponent<Button>();
            _reloadButton.onClick.AddListener(ReloadScene);
        }

        public void Lose()
        {
            if (_canLose)
            {
                _canLose = false;
                _LoseSceneHolder.TurnOnPostProcessing();
                _LoseSceneHolder.GetMono().StartCoroutine(IncreaseIntensity());
            }
        }
        private IEnumerator IncreaseIntensity()
        {
            float elapsedTime = 0.0f;
            _LoseSceneHolder.GetTmp().SetActive(true);
            _LoseSceneHolder.GetAnimator().Play("textFadeLose");

            while (elapsedTime < duration)
            {
                // Изменяем интенсивность пост-обработки постепенно в течение указанного времени
                currentIntensity = Mathf.Lerp(initialIntensity, targetIntensity, elapsedTime / duration);
                _LoseSceneHolder.GetVignette().intensity.value = currentIntensity;

                elapsedTime += Time.deltaTime;
                yield return null;
            }

            // Устанавливаем окончательное значение интенсивности
            _LoseSceneHolder.GetReloadButton().SetActive(true);
            _LoseSceneHolder.GetVignette().intensity.value = targetIntensity;
        }

        public void ReloadScene()
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
    }
}

