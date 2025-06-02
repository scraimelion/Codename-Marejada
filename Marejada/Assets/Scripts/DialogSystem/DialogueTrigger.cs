using UnityEngine;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueRoundSO dialogue;

        [ContextMenu("Trigger Dialogue")]
        public void TriggerDialogue()
        {
            if (DialogueManager.Instance.IsDialogInProgress)
                return;

            DialogueManager.Instance.StartDialogue(dialogue);
        }

        public void ReplaceDialogue(DialogueRoundSO newDialog)
        {
            dialogue = newDialog;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }
    }
}
