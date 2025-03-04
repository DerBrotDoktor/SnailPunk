using System.Collections;
using UnityEngine;
using TMPro;
using FMODUnity;

public class FMODMessageCheck : MonoBehaviour
{
    [SerializeField] private EventReference fmodEventIfWordPresent; // FMOD event to play if the word is present
    [SerializeField] private EventReference fmodEventIfWordAbsent;  // FMOD event to play if the word is absent
    [SerializeField] private string specificWord = "wordToCheck";   // The word to check in the message
    [SerializeField] private string childTextMeshProName = "Text TMP"; // Name of the child object containing the TMP component

    [SerializeField] private float checkInterval = 0.5f; // Time between text checks
    [SerializeField] private float lifeTime = 7f;      // Delay after playing the event

    private TMP_Text textComponent;

    private void Start()
    {
        // Find the child object with the TMP component
        Transform child = transform.Find(childTextMeshProName);
        if (child != null)
        {
            textComponent = child.GetComponent<TMP_Text>();
        }

        if (textComponent == null)
        {
            Debug.LogError("TextMeshPro component not found in child object.");
            return;
        }

        // Start checking the text periodically
        StartCoroutine(CheckTextPeriodically());
    }

    private IEnumerator CheckTextPeriodically()
    {
        while (true)
        {
            // Check if the text contains the specific word
            if (textComponent.text != null)
            {
                if (textComponent.text.Contains(specificWord))
                {
                    // Play the FMOD event if the word is present
                    RuntimeManager.PlayOneShot(fmodEventIfWordPresent);
                }
                else
                {
                    // Play the FMOD event if the word is absent
                    RuntimeManager.PlayOneShot(fmodEventIfWordAbsent);
                }

                // Wait for the specified event delay before continuing
                yield return new WaitForSeconds(lifeTime);
            }

            // Wait for the specified time before checking again
            yield return new WaitForSeconds(checkInterval);
        }
    }
}
