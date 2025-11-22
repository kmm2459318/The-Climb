using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Loading;

namespace System.Loading
{
    public class SceneLoader : MonoBehaviour
    {
        public static SceneLoader Instance { get; private set; }

        [Header("UI Settings")]
        [SerializeField] private LoadingScreenUI loadingScreenUI;
        [SerializeField] private List<Sprite> backgroundImages;

        [Header("Configuration")]
        [SerializeField] private float minLoadingTime = 1.0f; // Minimum time to show the loading screen

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            if (loadingScreenUI != null)
            {
                loadingScreenUI.SetActive(false);
            }
        }

        /// <summary>
        /// Loads a scene by name with a loading screen.
        /// </summary>
        /// <param name="sceneName">The name of the scene to load.</param>
        public void LoadScene(string sceneName)
        {
            StartCoroutine(LoadSceneAsync(sceneName));
        }

        private IEnumerator LoadSceneAsync(string sceneName)
        {
            if (loadingScreenUI != null)
            {
                loadingScreenUI.SetActive(true);
                loadingScreenUI.UpdateProgress(0);

                // Set a random background image
                if (backgroundImages != null && backgroundImages.Count > 0)
                {
                    int randomIndex = UnityEngine.Random.Range(0, backgroundImages.Count);
                    loadingScreenUI.SetBackgroundImage(backgroundImages[randomIndex]);
                }
            }

            AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);
            operation.allowSceneActivation = false;

            float timer = 0f;

            while (!operation.isDone)
            {
                timer += Time.deltaTime;
                
                // Fake progress calculation to ensure the slider moves smoothly
                // operation.progress goes from 0 to 0.9 while loading
                float progress = Mathf.Clamp01(operation.progress / 0.9f);
                
                // If we have a minimum loading time, we can artificially slow down the progress bar
                // or just wait until the timer is up.
                // Here we just update the slider with the actual loading progress.
                if (loadingScreenUI != null)
                {
                    loadingScreenUI.UpdateProgress(progress);
                }

                // Check if loading is finished (at 0.9) and minimum time has passed
                if (operation.progress >= 0.9f && timer >= minLoadingTime)
                {
                    if (loadingScreenUI != null)
                    {
                        loadingScreenUI.UpdateProgress(1f);
                    }
                    operation.allowSceneActivation = true;
                }

                yield return null;
            }

            if (loadingScreenUI != null)
            {
                loadingScreenUI.SetActive(false);
            }
        }
    }
}
