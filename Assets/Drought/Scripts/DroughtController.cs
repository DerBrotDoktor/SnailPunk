using System;
using UnityEngine;

namespace Drought
{
    public class DroughtController : MonoBehaviour
    {
        public static bool IsDrought = false;
        
        private int droughtLevel = 0;
        [SerializeField] private AnimationCurve droughtCurve;
        private float currentDroughtTime = 0f;
        private float targetDroughtTime = 0f;
        
        private float timeSinceLastDrought = 0f;
        private float timeBetweenDroughts = 0f;
        [SerializeField] private float minTimeBetweenDroughts = 5f;
        [SerializeField] private float maxTimeBetweenDroughts = 10f;

        private void Awake()
        {
            droughtCurve.postWrapMode = WrapMode.Clamp;
        }

        private void Start()
        {
            timeBetweenDroughts = UnityEngine.Random.Range(minTimeBetweenDroughts, maxTimeBetweenDroughts);
        }
        
        private void Update()
        {
            if (IsDrought)
            {
                currentDroughtTime += Time.deltaTime;
                if (currentDroughtTime >= targetDroughtTime)
                {
                    FinishDrought();
                }
            }
            else
            {
                timeSinceLastDrought += Time.deltaTime;
                if (timeSinceLastDrought >= timeBetweenDroughts)
                {
                    StartDrought();
                }
            }
        }
        
        private void StartDrought()
        {
            timeSinceLastDrought = 0f;
            currentDroughtTime = 0f;

            targetDroughtTime = droughtCurve.Evaluate(droughtLevel);
            
            print("Start Drought: " + targetDroughtTime);
            
            IsDrought = true;
        }
        
        private void FinishDrought()
        {
            droughtLevel++;
            timeBetweenDroughts = UnityEngine.Random.Range(minTimeBetweenDroughts, maxTimeBetweenDroughts);
            
            print("Finish Drought: next in: " + timeBetweenDroughts);
            
            IsDrought = false;
        }
        
        #if UNITY_EDITOR
        public void EditorStartDrought()
        {
            if (Application.isPlaying)
            {
                StartDrought();
            }
        }

        public void EditorFinishDrought()
        {
            if(Application.isPlaying && IsDrought)
            {
                FinishDrought();
            }
        }
        #endif
    }
}
