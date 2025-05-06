using DG.Tweening;
using Jadsa.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DialogueSystem
{
    public class DialogueManager : SingletonUnity<DialogueManager>
    {
        [Header("Elementos UI del Diálogo")]
        [SerializeField] private RectTransform dialogBox;            // Caja del diálogo en pantalla
        [SerializeField] private Image characterPhoto;               // Imagen del personaje
        [SerializeField] private TextMeshProUGUI characterName;      // Nombre del personaje
        [SerializeField] private TextMeshProUGUI dialogArea;         // Área donde se muestra el texto del diálogo

        [Header("Velocidad de escritura")]
        [SerializeField] private float typingSpeed = 0.1f;           // Tiempo entre cada letra escrita

        [Header("Sonidos del diálogo")]
        [SerializeField] private AudioSource typingAudioSource;      // Sonido de escritura letra por letra
        [SerializeField] private AudioSource skipAudioSource;        // Sonido al omitir diálogo

        // Posiciones de anclaje para animaciones de entrada/salida
        private readonly Vector2 dialogBoxScreenAnchorPosition = new(0, 0);      // Posición visible
        private readonly Vector2 dialogBoxOutAnchorPosition = new(0, -220f);     // Posición oculta

        private Queue<DialogueTurn> dialogTurnsQueue;       // Cola de turnos de diálogo
        private DialogueTurn currentTurn;                   // Turno actual de diálogo
        private bool isTypingDialogTurn = false;            // ¿Se está escribiendo una línea actualmente?
        private bool isEndingDialogue = false;              // ¿El diálogo está terminando (para evitar múltiples animaciones)?

        private void Update()
        {
            IsDialogStartAction = Input.GetMouseButtonDown(0);
            // Si el diálogo está activo y se hace clic, avanzar o mostrar texto completo
            if (IsDialogInProgress && !isEndingDialogue && IsDialogStartAction)
            {
                if (isTypingDialogTurn)
                {
                    // Si está escribiendo, mostrar inmediatamente la línea completa
                    StopAllCoroutines();
                    dialogArea.text = currentTurn.DialogLine;
                    isTypingDialogTurn = false;
                }
                else
                {
                    // Si ya terminó de escribir, pasar al siguiente turno
                    DisplayNextDialogTurn();
                }
            }
        }

        // Propiedad pública que indica si el diálogo está activo
        public bool IsDialogInProgress { get; private set; } = false;
        public bool IsDialogStartAction { get; private set; } = false;

        public void StartDialogue(DialogueRoundSO dialog)
        {
            IsDialogInProgress = true;

            // Convertir la lista de turnos de diálogo en una cola
            dialogTurnsQueue = new Queue<DialogueTurn>(dialog.DialogTurnsList);

            // Preparar el primer turno y mostrar caja de diálogo
            PreparareFirstDialogTurn();
            ShowAndAnimateDialogbox();

            // Detener controles del jugador (si se usan)
            // inputReader.EnableInputUI();
        }

        private void Awake()
        {
            dialogTurnsQueue = new Queue<DialogueTurn>();
            dialogBox.anchoredPosition = dialogBoxOutAnchorPosition;
            HideDialogBox();  // Asegurar que la caja esté oculta al iniciar
        }

        // Método para omitir el texto actual
        private void SkipDialog()
        {
            if (isTypingDialogTurn)
            {
                StopAllCoroutines();
                isTypingDialogTurn = false;
                dialogArea.text = currentTurn.DialogLine;
                skipAudioSource.Play();
            }
            else
            {
                dialogArea.text = string.Empty;
                DisplayNextDialogTurn();
            }
        }

        // Preparar el primer turno sin eliminarlo de la cola
        private void PreparareFirstDialogTurn()
        {
            if (dialogTurnsQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            var nextDialogTurn = dialogTurnsQueue.Peek();
            characterPhoto.sprite = nextDialogTurn.Character.ProfilePhoto;
            characterName.text = nextDialogTurn.Character.Name;
        }

        // Mostrar la caja de diálogo con animación
        private void ShowAndAnimateDialogbox()
        {
            float scaleDuration = 1f;

            dialogBox.gameObject.SetActive(true);
            dialogBox.DOAnchorPos(dialogBoxScreenAnchorPosition, scaleDuration).SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    DisplayNextDialogTurn(); // Comenzar a mostrar los diálogos
                });
        }

        // Mostrar el siguiente turno de diálogo
        private void DisplayNextDialogTurn()
        {
            if (dialogTurnsQueue.Count == 0)
            {
                EndDialogue();
                return;
            }

            currentTurn = dialogTurnsQueue.Dequeue();

            // Cargar datos del personaje
            characterPhoto.sprite = currentTurn.Character.ProfilePhoto;
            characterName.text = currentTurn.Character.Name;

            StopAllCoroutines();
            StartCoroutine(TypeSentence(currentTurn));
        }

        // Escribir letra por letra la oración
        private IEnumerator TypeSentence(DialogueTurn dialogTurn)
        {
            isTypingDialogTurn = true;
            dialogArea.text = string.Empty;
            var typingWaitSeconds = new WaitForSeconds(typingSpeed);

            foreach (char letter in dialogTurn.DialogLine.ToCharArray())
            {
                dialogArea.text += letter;

                if (!char.IsWhiteSpace(letter))
                    typingAudioSource.Play();

                yield return typingWaitSeconds;
            }

            isTypingDialogTurn = false;
            currentTurn = null;
        }

        // Ocultar la caja de diálogo
        private void HideDialogBox()
        {
            dialogArea.text = string.Empty;
            characterName.text = string.Empty;
            characterPhoto.sprite = null;
            dialogBox.gameObject.SetActive(false);
        }

        // Terminar el diálogo con animación
        private void EndDialogue()
        {
            if (isEndingDialogue) return;

            float scaleDuration = 0.6f;
            isEndingDialogue = true;
            skipAudioSource.Play();

            dialogBox.gameObject.SetActive(true);
            dialogBox.DOAnchorPos(dialogBoxOutAnchorPosition, scaleDuration).SetEase(Ease.InBack)
                .OnComplete(() =>
                {
                    HideDialogBox();
                    IsDialogInProgress = false;
                    isEndingDialogue = false;
                    // inputReader.DisableInputUI();
                });

            Debug.Log($"<color=yellow>Dialogue ended</color>");
        }
    }
}
