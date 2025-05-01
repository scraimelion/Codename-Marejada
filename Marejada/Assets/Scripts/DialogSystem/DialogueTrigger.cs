using UnityEngine;

namespace DialogueSystem
{
    // Este script sirve como disparador (trigger) para iniciar un diálogo cuando el jugador entra en una zona determinada.
    // Debe estar adjunto a un GameObject con un Collider marcado como "Is Trigger".
    // Al detectar al jugador, le indica al DialogueManager que inicie el diálogo correspondiente.

    public class DialogueTrigger : MonoBehaviour
    {
        // Referencia al ScriptableObject que contiene la ronda de diálogo que se debe reproducir.
        [SerializeField] private DialogueRoundSO dialogue;

        // Permite ejecutar manualmente el diálogo desde el menú contextual del editor.
        [ContextMenu("Trigger Dialogue")]
        public void TriggerDialogue()
        {
            // Si ya hay un diálogo en curso, no hace nada para evitar interrupciones.
            if (DialogueManager.Instance.IsDialogInProgress) 
                return;

            // Llama al DialogueManager para comenzar el diálogo definido.
            DialogueManager.Instance.StartDialogue(dialogue);
        }

        // Método que permite reemplazar el diálogo actual con uno nuevo (útil si se quiere cambiar el diálogo desde otro script).
        public void ReplaceDialogue(DialogueRoundSO newDialog) => dialogue = newDialog;

        // Este método se ejecuta automáticamente cuando otro Collider entra en el Collider de este GameObject.
        // Si el objeto que entra tiene el tag "Player", se dispara el diálogo.
        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                TriggerDialogue();
            }
        }
    }
}
