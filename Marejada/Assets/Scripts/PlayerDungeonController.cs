using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Unity.FPS.Gameplay {

    [RequireComponent(typeof(CharacterController), typeof(PlayerInputHandler), typeof(AudioSource))]
    public class PlayerDungeonController : MonoBehaviour {

        const float MAG_THRESHOLD = 0.01f;

        [Header("References")] [Tooltip("Reference to the main camera used for the player")]
        public Camera PlayerCamera;

        [Tooltip("Audio source for footsteps, jump, etc...")]
        public AudioSource AudioSource;

        [Header("Movement")] [Tooltip("Max movement speed when grounded (when not sprinting)")]
        public float tileMovement = 10f;

        [Header("Rotation")] [Tooltip("Rotation speed for moving the camera")]
        public float RotationSpeed = 200f;

        [Range(0.1f, 1f)] [Tooltip("Rotation speed multiplier when aiming")]
        public float RotationMultiplier = 0.6f;

        [Tooltip("Sound played for footsteps")]
        public AudioClip FootstepSfx;

        public Vector3 CharacterVelocity { get; set; }

        [SerializeField]
        GameObject m_NodoInicial = null;

        PlayerInputHandler m_InputHandler;
        CharacterController m_Controller;
        Actor m_Actor;
        Vector3 m_MovementDirection;
        float m_CameraVerticalAngle = 0f;
        float m_FootstepDistanceCounter;
        float m_TargetCharacterHeight;
        bool m_OutOfTrigger = false;
        bool m_EnTransito = true;
        GameObject m_CameraFollow;
        GameObject m_NodoActual;
        GameObject m_NodoDestino;
        GameObject m_modeloPersonaje;
        GameObject m_flechasPadre;
        List<GameObject> FlechasNav;

        void Awake()
        {
            ActorsManager actorsManager = FindFirstObjectByType<ActorsManager>();
            if (actorsManager != null)
                actorsManager.SetPlayer(gameObject);
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start() {
            // fetch components on the same gameObject
            m_Controller = GetComponent<CharacterController>();
            DebugUtility.HandleErrorIfNullGetComponent<CharacterController, PlayerCharacterController>(m_Controller,
                this, gameObject);

            m_InputHandler = GetComponent<PlayerInputHandler>();
            DebugUtility.HandleErrorIfNullGetComponent<PlayerInputHandler, PlayerCharacterController>(m_InputHandler,
                this, gameObject);

            if (m_NodoInicial) {
                float x = m_NodoInicial.transform.position.x;
                float y = m_NodoInicial.transform.position.y;
                float z = m_NodoInicial.transform.position.z;

                m_CameraFollow = m_NodoInicial.transform.GetChild(0).gameObject;
                m_CameraFollow.SetActive(true);
                m_NodoActual = m_NodoInicial;
                m_NodoDestino = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[0];
            }
            m_modeloPersonaje = GameObject.Find("PlayerModel");
            
            m_flechasPadre = GameObject.Find("NavArrows");
            FlechasNav = new List<GameObject>();
            foreach (Transform child in m_flechasPadre.transform) {
                child.gameObject.SetActive(false);
                FlechasNav.Add(child.gameObject);
            }

            m_MovementDirection = new Vector3();

        }

        // Update is called once per frame
        void Update() {
            HandleCameraPosition();
            //Cursor.visible = true;

            if (!m_EnTransito)
            {
                //m_EnTransito = selectNewNode();
                return;
            }

            HandleCharacterMovement();
        }

        void HandleCameraPosition()
        {
            // vertical camera rotation
            // add vertical inputs to the camera's vertical angle
            m_CameraVerticalAngle += m_InputHandler.GetLookInputsVertical() * RotationSpeed * RotationMultiplier;

            // limit the camera's vertical angle to min/max
            //m_CameraVerticalAngle = Mathf.Clamp(m_CameraVerticalAngle, -89f, 89f);

            // apply the vertical angle as a local rotation to the camera transform along its right axis (makes it pivot up and down)
            //m_CameraFollow.transform.position = new Vector3(0, m_CameraVerticalAngle, 0);

        }

        void HandleCharacterMovement()
        {
            // calcula la dirección en la que se debe mover
            if (m_MovementDirection.magnitude < MAG_THRESHOLD) { // TODO: Condición para que recalcule la dirección a la que se debe mover.
                Vector3 destino = m_NodoDestino.transform.position;
                m_MovementDirection = destino - m_NodoActual.transform.position;
                m_MovementDirection.Normalize();
                m_modeloPersonaje.transform.LookAt(destino + m_NodoDestino.transform.up);
            }

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            if (worldspaceMoveInput.x > 0 || (worldspaceMoveInput.x < 0 && m_OutOfTrigger)) {
                CharacterVelocity = m_MovementDirection * worldspaceMoveInput.x * tileMovement;
                //Debug.Log("aaaa");
            }
            else {
                CharacterVelocity = new Vector3();
            }

            Vector3 capsuleBottomBeforeMove = GetCapsuleBottomHemisphere();
            Vector3 capsuleTopBeforeMove = GetCapsuleTopHemisphere(m_Controller.height);

            // detect obstructions to adjust velocity accordingly
            if (Physics.CapsuleCast(capsuleBottomBeforeMove, capsuleTopBeforeMove, m_Controller.radius,
                CharacterVelocity.normalized, out RaycastHit hit, CharacterVelocity.magnitude * Time.deltaTime, -1,
                QueryTriggerInteraction.Ignore))
            {
                CharacterVelocity = Vector3.ProjectOnPlane(CharacterVelocity, hit.normal);
            }

            m_Controller.Move(CharacterVelocity * Time.deltaTime);
        }

        void OnTriggerEnter(Collider other) {
            if (other.tag != "Camino") {
                return;
            }
            m_OutOfTrigger = false;
            m_EnTransito = false;
            m_CameraFollow.SetActive(false);
            m_NodoActual = other.gameObject;
            m_CameraFollow = m_NodoActual.transform.GetChild(0).gameObject;
            m_CameraFollow.SetActive(true);
            //List<GameObject> nodos_vecinos = m_NodoActual.GetComponent<NodoCamino>().neighborNodes;

            for (int i = 0; i < m_NodoActual.GetComponent<NodoCamino>().neighborNodes.Length; i++) {
                Vector3 direccion = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[i].transform.position - m_NodoActual.transform.position;
                direccion.Normalize();
                Debug.Log(direccion);
                if (i < FlechasNav.Count)
                {
                    FlechasNav[i].SetActive(true);
                    FlechasNav[i].transform.LookAt(m_NodoActual.GetComponent<NodoCamino>().neighborNodes[i].transform.position);
                }
            }
            Debug.Log("PUEDES SELECCIONAR ESTE NUM DE NODOS: " + m_NodoActual.GetComponent<NodoCamino>().neighborNodes.Length);
        }

        void OnTriggerExit(Collider other) {
            if (other.tag != "Camino") {
                return;
            }

            m_OutOfTrigger = true;
        }

        public void SelectNewNodeArrow(int selected)
        {
            int selected_node = selected;
            if (selected_node >= 0 && selected_node < m_NodoActual.GetComponent<NodoCamino>().neighborNodes.Length)
            {
                m_NodoDestino = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[selected_node];
                m_MovementDirection = new Vector3();

                foreach (GameObject flecha in FlechasNav)
                {
                    flecha.SetActive(false);
                }
                Debug.Log("HAS SELECCIONADO " + selected_node);
                m_EnTransito = true;
                return;
            }
            m_EnTransito = false;
            return;
        }

        bool selectNewNode() {
            int selected_node = m_InputHandler.GetSelectWeaponInput() - 1;
            if (selected_node >= 0 && selected_node < m_NodoActual.GetComponent<NodoCamino>().neighborNodes.Length) { // Si es -1 es que no se ha pulsado
                m_NodoDestino = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[selected_node];
                m_MovementDirection = new Vector3();

                foreach (GameObject flecha in FlechasNav) {
                    flecha.SetActive(false);
                }
                //Debug.Log("HAS SELECCIONADO " + selected_node);
                return true;
            }
            return false;
        }

        // Gets the center point of the bottom hemisphere of the character controller capsule    
        Vector3 GetCapsuleBottomHemisphere()
        {
            return transform.position + (transform.up * m_Controller.radius);
        }

        // Gets the center point of the top hemisphere of the character controller capsule    
        Vector3 GetCapsuleTopHemisphere(float atHeight)
        {
            return transform.position + (transform.up * (atHeight - m_Controller.radius));
        }

    }

}

