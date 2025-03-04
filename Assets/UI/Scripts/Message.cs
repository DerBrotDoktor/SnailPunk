using System;
using System.Collections;
using TMPro;
using UnityEngine;

namespace UI
{
    public class Message : MonoBehaviour
    {
        [SerializeField] private float lifetime = 7f;
        [SerializeField] private TMP_Text text;

        public void SetMessage(string message)
        {
            text.text = message;

            StartCoroutine(DestroyAfterDelay());
        }
        
        private IEnumerator DestroyAfterDelay()
        {
            yield return new WaitForSeconds(lifetime);
            Destroy(gameObject);
        }
    }
}
