using UnityEngine;
using UnityEngine.UI;

namespace System.Loading
{
    public class LoadingScreenUI : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Slider progressSlider;

        /// <summary>
        /// Sets the background image of the loading screen.
        /// </summary>
        /// <param name="sprite">The sprite to display.</param>
        public void SetBackgroundImage(Sprite sprite)
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite;
            }
        }

        /// <summary>
        /// Updates the progress bar value.
        /// </summary>
        /// <param name="progress">Progress value between 0 and 1.</param>
        public void UpdateProgress(float progress)
        {
            if (progressSlider != null)
            {
                progressSlider.value = progress;
            }
        }

        /// <summary>
        /// Shows or hides the loading screen.
        /// </summary>
        /// <param name="isActive">True to show, false to hide.</param>
        public void SetActive(bool isActive)
        {
            gameObject.SetActive(isActive);
        }
    }
}
