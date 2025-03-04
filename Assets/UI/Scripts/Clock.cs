using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Clock : MonoBehaviour
    {
        
        private DayNightCycle dayNightCycle;
        private const float DEGREEE_PER_HOUR = 30f; // 360 /12;
        private const float DEGREE_PER_MINUTE = 6f;

        [SerializeField] private GameObject longArm;
        [SerializeField] private GameObject shortArm;
        
        [SerializeField] private GameObject dayBackground;

        [Header("Buttons")]
        [SerializeField] private Image pauseButton;
        [SerializeField] private Image forwardButton;
        [SerializeField] private Image fastForwardButton;
        
        [Header("Sprites")]
        [SerializeField] private Sprite pauseButtonSprite;
        [SerializeField] private Sprite pauseButtonPressedSprite;
        [SerializeField] private Sprite forwardButtonSprite;
        [SerializeField] private Sprite forwardButtonPressedSprite;
        [SerializeField] private Sprite fastForwardButtonSprite;
        [SerializeField] private Sprite fastForwardButtonPressedSprite;

        private void OnEnable()
        {
            Game.TimeScaleChanged += OnTimeScaleChanged;
            OnTimeScaleChanged();
        }

        private void OnDisable()
        {
            Game.TimeScaleChanged -= OnTimeScaleChanged;
        }

        private void Start()
        {
            dayNightCycle = FindObjectOfType<DayNightCycle>();
            dayBackground.SetActive(dayNightCycle.IsDay);

            UpdateRotation();
        }
        
        private void LateUpdate()
        {
            if (Game.TimeScale <= 0)
            {
                return;
            }
            
            UpdateRotation();
            
            dayBackground.SetActive(dayNightCycle.IsDay);
        }

        private void UpdateRotation()
        {
            int hours = dayNightCycle.hours;
            int minutes = dayNightCycle.minutes;
            
            float hourRotation = -hours * DEGREEE_PER_HOUR;
            float minuteContributionToHour = -minutes * DEGREEE_PER_HOUR / 60f;
            
            shortArm.transform.localRotation = Quaternion.Euler(0, 0, hourRotation + minuteContributionToHour);

            float minuteRotation = -minutes * DEGREE_PER_MINUTE;
            
            longArm.transform.localRotation = Quaternion.Euler(0, 0, minuteRotation);
        }

        public void OnPauseButtonClicked()
        {
            Game.TimeScale = 0;
        }

        public void OnForwardButtonClicked()
        {
            Game.TimeScale = 1;
        }
        
        public void OnFastForwardButtonClicked()
        {
            Game.TimeScale = 2;
        }
        
        private void OnTimeScaleChanged()
        {
            pauseButton.sprite = Game.TimeScale == 0 ? pauseButtonPressedSprite : pauseButtonSprite;
            forwardButton.sprite = Game.TimeScale == 1 ?  forwardButtonPressedSprite : forwardButtonSprite; 
            fastForwardButton.sprite = Game.TimeScale == 2 ? fastForwardButtonPressedSprite : fastForwardButtonSprite;
        }
    }
}
