using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Jadsa.Managers
{
    public class SingletonUnity<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T instance;
        private static readonly object lockObj = new();
        private static bool isApplicationQuitting = false;

        // Dictionary to store singleton instances based on their type
        private static readonly Dictionary<Type, T> singletonInstances = new Dictionary<Type, T>();

        // Property to access the singleton instance
        public static T Instance
        {
            get
            {
                if (isApplicationQuitting)
                {
                    Debug.LogWarning("[Singleton] Instance '" + typeof(T) +
                                     "' already destroyed. Returning null.");
                    return null;
                }

                lock (lockObj)
                {

                    Type type = typeof(T);
                    if (singletonInstances.ContainsKey(type) == false)
                    {
                        // Create the instance if it doesn't exist
                        CreateInstance(type);
                    }

                    // Return the singleton instance
                    return singletonInstances[type];
                }
            }
        }

        // Method to create a new instance of the singleton
        private static void CreateInstance(Type type)
        {
            // Check scene for existing instance first
            instance = FindAnyObjectByType<T>();

            // Ensure there's only one instance of the singleton
            if (FindObjectsByType<T>(FindObjectsSortMode.None).Length > 1)
            {
                Debug.LogError("[Singleton] Something went really wrong " +
                               " - there should never be more than 1 singleton!" +
                               " Reopening the scene might fix it.");
                return;
            }

            // If instance doesn't exist, create a new one
            if (instance == null)
            {
                GameObject singleton = new GameObject();
                instance = singleton.AddComponent<T>();
                singleton.name = $"Singleton - {type.Name}";

                // Make the singleton persist across scene loads
                DontDestroyOnLoad(singleton);

                Debug.Log($"[Singleton] An instance of {type.Name} " +
                          $"is needed in the scene, so '{singleton}' " +
                          $"was created with DontDestroyOnLoad.");
            }
            else
            {
                Debug.Log($"[Singleton] Using instance already created: {instance.gameObject.name}");
            }

            // Add the instance to the dictionary
            singletonInstances.Add(type, instance);
        }

        // Method called when the singleton GameObject is destroyed
        private void OnDestroy()
        {
            // Set flag to indicate application is quitting
            isApplicationQuitting = true;

            // Remove the destroyed instance from the dictionary
            if (singletonInstances.ContainsKey(typeof(T)))
            {
                singletonInstances.Remove(typeof(T));
            }
        }

        // Additional Features

        /// <summary>
        /// Allows lazy initialization of the singleton instance.
        /// </summary>
        public static void Initialize()
        {
            // Call Instance property to ensure lazy initialization
            _ = Instance;
        }

        /// <summary>
        /// Registers the singleton instance in a specific scene.
        /// </summary>
        /// <param name="scene">The scene to register the singleton in.</param>
        public static void RegisterInScene(Scene scene)
        {
            if (instance == null)
            {
                // Create the instance if it doesn't exist
                CreateInstance(typeof(T));
            }

            // Move the singleton GameObject to the specified scene
            SceneManager.MoveGameObjectToScene(instance.gameObject, scene);
        }

        /// <summary>
        /// Provides a method to destroy the singleton instance.
        /// </summary>
        public static void DestroyInstance()
        {
            if (instance != null)
            {
                // Destroy the GameObject associated with the instance
                Destroy(instance.gameObject);
                instance = null;

                // Remove the instance from the dictionary
                if (singletonInstances.ContainsKey(typeof(T)))
                {
                    singletonInstances.Remove(typeof(T));
                }

                // Set flag to indicate application is quitting
                isApplicationQuitting = true;
            }
        }

        // Add more features as needed
    }
}
