using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueRoundSO dialogue;
        [SerializeField] private UnityEvent DialogEvents;

        // Referencia al botón "Hablar" que está en la UI
        GameObject hablarButton;
        private Button hablarBtnComponent;

        [Header("Sonido de entrada")]
        [SerializeField] private AudioClip sonidoAlEntrar; // sonido asignable en el inspector
        private AudioSource audioSource;

        private void Start()
        {
            hablarButton = GameObject.Find("Hablar");

            if (hablarButton != null)
            {
                Debug.Log("aaaaaa");
                hablarBtnComponent = hablarButton.GetComponent<Button>();
            }

            audioSource = GetComponent<AudioSource>();
            if (audioSource == null)
            {
                audioSource = gameObject.AddComponent<AudioSource>(); // lo crea si no hay uno
            }

            audioSource.playOnAwake = false;
        }

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance.IsDialogInProgress) return;

            DialogueManager.Instance.StartDialogue(dialogue);
            DialogEvents.Invoke();

            if (hablarButton != null)
                hablarButton.SetActive(false);
        }

        public void ReplaceDialogue(DialogueRoundSO newDialog) => dialogue = newDialog;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && hablarButton != null)
            {
                hablarButton.SetActive(true);
                hablarBtnComponent.onClick.RemoveAllListeners();
                hablarBtnComponent.onClick.AddListener(TriggerDialogue);

                if (sonidoAlEntrar != null)
                {
                    audioSource.clip = sonidoAlEntrar;
                    audioSource.Play();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && hablarButton != null)
            {
                hablarButton.SetActive(false);
                hablarBtnComponent.onClick.RemoveAllListeners();
            }
        }
    }
}
