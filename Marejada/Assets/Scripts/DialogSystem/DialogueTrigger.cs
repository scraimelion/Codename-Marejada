using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
    
    if (DialogueManager.Instance != null)
{
    DialogueManager.Instance.StartDialogue(dialogue);
}
else
{
    Debug.LogError("❌ DialogueManager no encontrado.");
}
        [SerializeField] private DialogueRoundSO dialogue;
        [SerializeField] private UnityEvent DialogEvents;

        bool enAreaDialogo = false;


        [Header("Sonido de entrada")]
        [SerializeField] private AudioClip sonidoAlEntrar; // sonido asignable en el inspector
        private AudioSource audioSource;

        private void Start()
        {
            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>(); // lo crea si no hay uno
            }

            audioSource.playOnAwake = false;
        }

        private void Update()
        {
            if (enAreaDialogo && DialogueManager.Instance.IsDialogStartAction) {
                TriggerDialogue();
                enAreaDialogo = false;
            }
        }

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance.IsDialogInProgress) return;

            DialogueManager.Instance.StartDialogue(dialogue);
            DialogEvents.Invoke();
        }

        public void ReplaceDialogue(DialogueRoundSO newDialog) => dialogue = newDialog;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DialogueManager.Instance.PrepareDialog();
                enAreaDialogo = true;

                if (sonidoAlEntrar != null)
                {
                    audioSource.clip = sonidoAlEntrar;
                    audioSource.Play();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                DialogueManager.Instance.UnPrepareDialog();
                enAreaDialogo = false;
                audioSource.Stop();
            }
        }
    }
}
