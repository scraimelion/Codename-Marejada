using System;
using UnityEngine;

namespace DialogueSystem
{
    // Esta clase representa un "turno" dentro de un diálogo.
    // Cada turno contiene el personaje que habla y la línea de texto que se debe mostrar.
    // Es una clase serializable para poder usarse dentro de listas en un ScriptableObject (como DialogueRoundSO).

    [Serializable]
    public class DialogueTurn
    {
        // Referencia al personaje que habla en este turno.
        // Se utiliza [field: SerializeField] para permitir la serialización automática y mantener la encapsulación con un getter privado.
        [field: SerializeField]
        public DialogueCharacterSO Character { get; private set; }

        // Línea de diálogo que se mostrará en este turno.
        // TextArea permite editar varias líneas desde el inspector de Unity.
        [SerializeField, TextArea(2, 4)]
        private string dialogLine = string.Empty;

        // Getter público para acceder a la línea de diálogo.
        public string DialogLine => dialogLine;
    }
}
