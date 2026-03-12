// ============================================================
//  XRCanvasController.cs
//  Android XR UI Builder — Runtime
//
//  Attached to the AndroidXRCanvas root GameObject.
//  Holds the ThemeData reference and exposes it to all
//  child XR components via static lookup.
//
//  Android XR Spatial guidelines implemented here:
//  • World Space canvas at 1.75m spawn depth (from ThemeData)
//  • Canvas sized to 1024dp × 720dp (0.868 dp-to-dmm ratio)
//  • Panel corner radius: 32dp default (from ThemeData)
// ============================================================

using UnityEngine;
using UnityEngine.UI;

namespace AndroidXRUIBuilder
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Canvas))]
    [RequireComponent(typeof(CanvasScaler))]
    [RequireComponent(typeof(GraphicRaycaster))]
    public class XRCanvasController : MonoBehaviour
    {
        // ── Inspector ─────────────────────────────────────
        [Header("Theme")]
        [Tooltip("Assign your ThemeData ScriptableObject here.\nCreate one via: Right-click → Create → Android XR UI Builder → Theme Data")]
        public ThemeData theme;

        [Header("XR Canvas Settings")]
        [Tooltip("Physical width of the canvas in meters when viewed at spawn depth.")]
        public float canvasWidthMeters = 1.556f; // 1024dp at 1.75m (0.868 dp-to-dmm)

        [Tooltip("Physical height of the canvas in meters when viewed at spawn depth.")]
        public float canvasHeightMeters = 1.094f; // 720dp at 1.75m

        // ── Internal refs ─────────────────────────────────
        Canvas _canvas;
        CanvasScaler _scaler;

        // ── Static accessor (child components use this) ───
        /// <summary>
        /// Nearest XRCanvasController in parent hierarchy.
        /// Child XR components call this to get ThemeData.
        /// </summary>
        public static XRCanvasController FindForComponent(Component child)
        {
            return child.GetComponentInParent<XRCanvasController>();
        }

        // ── Unity lifecycle ───────────────────────────────
        void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _scaler = GetComponent<CanvasScaler>();

            ApplySpatialSettings();
        }

#if UNITY_EDITOR
        void OnValidate()
        {
            if (_canvas == null) _canvas = GetComponent<Canvas>();
            if (_scaler == null) _scaler = GetComponent<CanvasScaler>();
            ApplySpatialSettings();
        }
#endif

        void ApplySpatialSettings()
        {
            // World Space canvas — required for Android XR spatial placement
            _canvas.renderMode = RenderMode.WorldSpace;

            // Canvas Scaler: reference resolution follows Android XR 0.868 dp-to-dmm ratio
            // Max panel: 2560×1800dp per Android XR docs
            _scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            _scaler.referenceResolution = new Vector2(1024, 720);
            _scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            _scaler.matchWidthOrHeight = 0.5f;

            // Physical canvas size (meters) at spawn depth
            var rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.sizeDelta = new Vector2(canvasWidthMeters * 1000f, canvasHeightMeters * 1000f);
                rt.localScale = Vector3.one * 0.001f; // 1 Unity unit = 1mm
            }

            // Spawn position: in front of origin at theme-defined depth
            if (theme != null)
            {
                float depth  = theme.panelSpawnDepth;
                float vertDeg = theme.panelVerticalOffsetDegrees;
                float vertOff = Mathf.Tan(vertDeg * Mathf.Deg2Rad) * depth;
                transform.localPosition = new Vector3(0f, vertOff, depth);
                transform.localRotation = Quaternion.identity;
            }
        }

        // ── Public API ────────────────────────────────────

        /// <summary>
        /// Repositions the canvas at the current theme spawn depth.
        /// Call after changing ThemeData.panelSpawnDepth at runtime.
        /// </summary>
        public void RefreshSpatialPosition()
        {
            ApplySpatialSettings();
        }
    }
}
