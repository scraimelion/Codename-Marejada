using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI; // necesario para manejar el botón

namespace DialogueSystem
{
    public class DialogueTrigger : MonoBehaviour
    {
        [SerializeField] private DialogueRoundSO dialogue;
        [SerializeField] private UnityEvent DialogEvents;

        // Referencia al botón "Hablar" que está en la UI
        GameObject hablarButton; // activa el botón visual
        private Button hablarBtnComponent;

        private void Start()
        {
            hablarButton = GameObject.Find("Hablar");

            if (hablarButton != null)
            {
                Debug.Log("aaaaaa");
                hablarBtnComponent = hablarButton.GetComponent<Button>();
                //hablarButton.SetActive(false); // desactivado por defecto
            }
        }

        public void TriggerDialogue()
        {
            if (DialogueManager.Instance.IsDialogInProgress) return;

            DialogueManager.Instance.StartDialogue(dialogue);
            DialogEvents.Invoke();

            if (hablarButton != null)
                hablarButton.SetActive(false); // ocultar después de iniciar diálogo
        }

        public void ReplaceDialogue(DialogueRoundSO newDialog) => dialogue = newDialog;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && hablarButton != null)
            {
                hablarButton.SetActive(true);
                hablarBtnComponent.onClick.RemoveAllListeners(); // limpiar previas
                hablarBtnComponent.onClick.AddListener(TriggerDialogue);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && hablarButton != null)
            {
                hablarButton.SetActive(false);
                hablarBtnComponent.onClick.RemoveAllListeners(); // limpiar eventos
            }
        }
    }
}
