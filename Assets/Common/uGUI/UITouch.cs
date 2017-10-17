using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace uGUI {

    /// <summary>
    /// uGUI において RectTransform でのタッチ処理のみを行わせる Component.
    /// 有効にするには Canvas GameObject の GraphicRaycaster を RectAndGraphicRaycaster に変更する.
    /// 現状は Canvas の RenderMode が ScreenSpaceOverlay 時のみ動作します.
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class UITouch : UIBehaviour {

        [NonSerialized]
        private RectTransform _rectTransform;

        public RectTransform rectTransform {
            get {
                return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>());
            }
        }

        [SerializeField]
        private bool _raycastTarget = true;

        public bool raycastTarget {
            get {
                return _raycastTarget;
            }
            set {
                _raycastTarget = value;
            }
        }
    }
}
