using UnityEngine;

namespace DialogueSystem
{
    public class DialogChangeHelper : MonoBehaviour
    {
        [SerializeField] private DialogueRoundSO dialogReplacement;
        
        private DialogueTrigger dialogTrigger;

        private void Awake() => dialogTrigger = GetComponent<DialogueTrigger>();

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                dialogTrigger.ReplaceDialogue(dialogReplacement);
                Destroy(this);
            }
        }
    }
}
