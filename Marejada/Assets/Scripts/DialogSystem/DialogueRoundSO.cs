using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    // Un ScriptableObject es una clase especial en Unity que permite guardar datos como un archivo independiente del resto del código.
    // Se usa comúnmente para almacenar configuraciones, listas, o contenido reutilizable sin necesidad de crear GameObjects en la escena.
    //
    // En este caso, DialogueRoundSO actúa como un contenedor para una "ronda" de diálogo, es decir,
    // una secuencia de frases (o turnos de diálogo) que serán mostradas por el DialogueManager durante una escena narrativa o conversación.

    [CreateAssetMenu(fileName = "New Dialogue", menuName = "Scriptable Objects/Dialogue Round")]
    public class DialogueRoundSO : ScriptableObject
    {
        // Lista de turnos de diálogo que componen esta ronda de diálogo.
        [SerializeField] private List<DialogueTurn> dialogTurnsList;

        // Propiedad pública de solo lectura que permite acceder a la lista de turnos desde otros scripts.
        public List<DialogueTurn> DialogTurnsList => dialogTurnsList;
    }
}
