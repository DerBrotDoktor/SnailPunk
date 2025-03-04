using UnityEngine;

namespace Snails
{
    public class DestroyAfterTime : MonoBehaviour
    {
    
        [SerializeField] private float time;
        private float currentTime = 0f;
        
        private void Update()
        {
            currentTime += Time.deltaTime;
            if (currentTime >= time)
            {
                Destroy(gameObject);
            }
        }
    }
}
