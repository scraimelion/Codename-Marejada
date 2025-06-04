using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

namespace Marejada
{
    public class EventClick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        private Material m_Material;
        private Color m_Color;

        public UnityEvent ClickEvent;

        private void Awake()
        {
            m_Material = GetComponent<Renderer>().material;
            m_Color = m_Material.color;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            //Debug.Log("ABAJOOOO");
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            //Debug.Log("ARRIBAAAAAA");
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            m_Material.color = m_Color;
            ClickEvent.Invoke();
            //Debug.Log("CLIIIIIICK");
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            m_Material.color = new Color(0.3f, 0.4f, 0.6f, 0.3f);
            //Debug.Log("AAAAAAAAAAAA");
        }
        public void OnPointerExit(PointerEventData eventData)
        {
            m_Material.color = m_Color;
            //Debug.Log("BBBBBBBBBBBB");
        }

    }
}
