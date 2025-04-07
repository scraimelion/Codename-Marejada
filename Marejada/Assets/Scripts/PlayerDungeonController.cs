using Unity.FPS.Game;
using UnityEngine;
using UnityEngine.Events;

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
        bool m_EnTransito = true;
        GameObject m_NodoActual;
        GameObject m_NodoDestino;

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
                //m_Controller.transform.position = new Vector3(x,y,z);
                m_NodoActual = m_NodoInicial;
                m_NodoDestino = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[0];
            }

            m_MovementDirection = new Vector3();
        }

        // Update is called once per frame
        void Update() {
            HandleCharacterMovement();
        }

        void HandleCharacterMovement()
        {
            // calcula la dirección en la que se debe mover
            if (m_MovementDirection.magnitude < MAG_THRESHOLD) { // TODO: Condición para que recalcule la dirección a la que se debe mover.
                Vector3 destino = m_NodoDestino.transform.position;
                m_MovementDirection = destino - m_NodoActual.transform.position;
                m_MovementDirection.Normalize();
            }

            // converts move input to a worldspace vector based on our character's transform orientation
            Vector3 worldspaceMoveInput = transform.TransformVector(m_InputHandler.GetMoveInput());

            if (worldspaceMoveInput.x != 0) {
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
            m_EnTransito = false;

            if (true) {
                m_NodoActual = m_NodoDestino;
                m_NodoDestino = m_NodoDestino.GetComponent<NodoCamino>().neighborNodes[0];

                m_MovementDirection = new Vector3();
            }
        }

        bool selectNewNode() {
            int selected_node = m_InputHandler.GetSelectWeaponInput() - 1;
            if (selected_node >= 0) { // Si es -1 es que no se ha pulsado
                m_NodoDestino = m_NodoActual.GetComponent<NodoCamino>().neighborNodes[selected_node];
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

