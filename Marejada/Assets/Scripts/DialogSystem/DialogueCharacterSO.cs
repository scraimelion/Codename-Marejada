using System;
using System.Collections.Generic;
using UnityEngine;

namespace DialogueSystem
{
    // Este ScriptableObject representa a un personaje dentro del sistema de diálogos.
    // Almacena su nombre, una imagen de perfil por defecto y una lista de emociones opcionales con sus respectivos sprites.
    // Es útil para reutilizar personajes en múltiples conversaciones y mantener organizada su información.

    [CreateAssetMenu(fileName = "New Dialogue Character", menuName = "Scriptable Objects/Dialogue Character")]
    public class DialogueCharacterSO : ScriptableObject
    {
        [Header("Character Info")]
        // Nombre del personaje (visible en la interfaz del diálogo)
        [SerializeField] private string characterName;

        // Imagen de perfil por defecto del personaje
        [SerializeField] private Sprite profilePhoto;

        [Space(7), Header("Emotions besides the default one (normal)")]
        // Lista de emociones opcionales del personaje (además de la imagen por defecto)
        [SerializeField] private List<CharacterEmotion> emotions;

        // Getters públicos para acceder a los datos desde otros scripts
        public string Name => characterName;
        public Sprite ProfilePhoto => profilePhoto;
        public List<CharacterEmotion> Emotion => emotions;

        // Devuelve el sprite correspondiente a la emoción indicada
        // Si la emoción es "normal" o no se encuentra, se devuelve el sprite por defecto
        public Sprite GetEmotionSprite(string emotion)
        {
            if (string.IsNullOrEmpty(emotion) || emotion.ToLower() == "normal")
            {
                return profilePhoto;
            }

            // Busca la emoción por nombre en la lista
            foreach (CharacterEmotion e in emotions)
            {
                if (e.EmotionName.Equals(emotion, StringComparison.OrdinalIgnoreCase))
                {
                    return e.EmotionSprite;
                }
            }

            // Si no se encuentra ninguna emoción, se devuelve el sprite por defecto
            return profilePhoto;
        }
    }

    // Clase auxiliar que representa una emoción del personaje con un nombre y un sprite asociado
    [Serializable]
    public class CharacterEmotion
    {
        // Nombre de la emoción (ej: "enfadado", "feliz", etc.)
        [SerializeField] private string emotionName;

        // Sprite que representa la emoción
        [SerializeField] private Sprite emotionSprite;

        // Getters públicos para acceder a los datos desde otros scripts
        public string EmotionName => emotionName;
        public Sprite EmotionSprite => emotionSprite;
    }    
}
