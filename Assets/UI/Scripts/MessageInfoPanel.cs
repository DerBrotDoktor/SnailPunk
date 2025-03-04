using UnityEngine;

namespace UI
{
    public class MessageInfoPanel : MonoBehaviour
    {
        [SerializeField] private Message messagePrefab;
        
        public void NewMessage(string message)
        {
            Message newMessage = Instantiate(messagePrefab, transform);
            newMessage.SetMessage(message);
        }
    }
}
