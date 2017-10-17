using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace uGUI {

    public class UIClick : UIBehaviour, IPointerClickHandler {

        public UnityAction<PointerEventData> OnClickEvent {
            set;
            private get;
        }

        public void OnPointerClick(PointerEventData eventData) {
            if (OnClickEvent != null) {
                OnClickEvent(eventData);
            }
        }
    }
}