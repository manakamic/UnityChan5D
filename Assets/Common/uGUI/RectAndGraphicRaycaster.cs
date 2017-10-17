using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace uGUI {

    /// <summary>
    /// 通常の GraphicRaycaster の処理に加えて UITouch での処理も行うクラス.
    /// UITouch は  Canvas の RenderMode が ScreenSpaceOverlay 時のみ動作させる.
    /// </summary>
    public class RectAndGraphicRaycaster : GraphicRaycaster {

        private Canvas _canvasCache;

        private Canvas canvasCache {
            get {
                if (_canvasCache != null)
                    return _canvasCache;

                _canvasCache = GetComponent<Canvas>();

                return _canvasCache;
            }
        }

        [NonSerialized]
        private List<UITouch> _raycastTarget = new List<UITouch>();

        [NonSerialized]
        private List<UITouch> _raycastResults = new List<UITouch>();

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList) {
            // GraphicRaycaster 本来の処理.
            base.Raycast(eventData, resultAppendList);

            // 以降 RectTransform でのタッチ処理.
            var canvas = canvasCache;

            if (canvas == null || canvas.renderMode != RenderMode.ScreenSpaceOverlay) {
                return;
            }

            _raycastTarget.Clear();
            canvas.gameObject.GetComponentsInChildren<UITouch>(_raycastTarget);

            if (_raycastTarget.Count == 0) {
                return;
            }

            var displayIndex = canvas.targetDisplay;
            var currentEventCamera = eventCamera; // Propery can call Camera.main, so cache the reference.
            var eventPosition = Display.RelativeMouseAt(eventData.position);

            if (eventPosition != Vector3.zero) {
                // We support multiple display and display identification based on event position.

                int eventDisplayIndex = (int)eventPosition.z;

                // Discard events that are not part of this display so the user does not interact with multiple displays at once.
                if (eventDisplayIndex != displayIndex)
                    return;
            }
            else {
                // The multiple display system is not supported on all platforms, when it is not supported the returned position
                // will be all zeros so when the returned index is 0 we will default to the event data to be safe.
                eventPosition = eventData.position;

                // We dont really know in which display the event occured. We will process the event assuming it occured in our display.
            }

            // Convert to view space
            Vector2 pos;

            if (currentEventCamera == null) {
                // Multiple display support only when not the main display. For display 0 the reported
                // resolution is always the desktops resolution since its part of the display API,
                // so we use the standard none multiple display method. (case 741751)
                float w = Screen.width;
                float h = Screen.height;
                if (displayIndex > 0 && displayIndex < Display.displays.Length) {
                    w = Display.displays[displayIndex].systemWidth;
                    h = Display.displays[displayIndex].systemHeight;
                }
                pos = new Vector2(eventPosition.x / w, eventPosition.y / h);
            }
            else {
                pos = currentEventCamera.ScreenToViewportPoint(eventPosition);
            }

            // If it's outside the camera's viewport, do nothing
            if (pos.x < 0.0f || pos.x > 1.0f || pos.y < 0.0f || pos.y > 1.0f) {
                return;
            }

            _raycastResults.Clear();
            // Canvasの仕様上 List の最後が手前になる.
            var totalCount = _raycastTarget.Count;
            for (var i = totalCount - 1; i >= 0; --i) {
                var touch = _raycastTarget[i];

                if (!touch.raycastTarget)
                    continue;

                if (!RectTransformUtility.RectangleContainsScreenPoint(touch.rectTransform, eventPosition, eventCamera))
                    continue;

                _raycastResults.Add(touch);
            }

            totalCount = _raycastResults.Count;
            for (var index = 0; index < totalCount; index++) {
                var go = _raycastResults[index].gameObject;
                var appendGraphic = true;

                if (ignoreReversedGraphics) {
                    if (currentEventCamera == null) {
                        // If we dont have a camera we know that we should always be facing forward
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(Vector3.forward, dir) > 0;
                    }
                    else {
                        // If we have a camera compare the direction against the cameras forward.
                        var cameraFoward = currentEventCamera.transform.rotation * Vector3.forward;
                        var dir = go.transform.rotation * Vector3.forward;
                        appendGraphic = Vector3.Dot(cameraFoward, dir) > 0;
                    }
                }

                if (appendGraphic) {
                    var castResult = new RaycastResult {
                        gameObject = go,
                        module = this,
                        distance = 0.0f, // ScreenSpaceOverlay only.
                        screenPosition = eventPosition,
                        index = resultAppendList.Count,
                        depth = -1, // ScreenSpaceOverlay only.
                        sortingLayer = canvas.sortingLayerID,
                        sortingOrder = canvas.sortingOrder
                    };
                    resultAppendList.Add(castResult);
                }
            }
        }
    }
}
