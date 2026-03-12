// ============================================================
//  ThemeData.cs
//  Android XR UI Builder — Runtime
//
//  Brand-driven color + spatial token ScriptableObject.
//  Based on Android XR Design Guidelines:
//  https://developer.android.com/design/ui/xr/guides/get-started
//
//  Create your theme:
//    Right-click in Project Window →
//    Create → Android XR UI Builder → Theme Data
//
//  Assign it to the XRCanvasController on your
//  AndroidXRCanvas GameObject. All components read
//  their colors from this asset via OnValidate (editor)
//  and Awake (runtime).
// ============================================================

using UnityEngine;

namespace AndroidXRUIBuilder
{
    [CreateAssetMenu(
        fileName  = "XRThemeData",
        menuName  = "Android XR UI Builder/Theme Data",
        order     = 0)]
    public class ThemeData : ScriptableObject
    {
        // ────────────────────────────────────────────────────
        // META
        // ────────────────────────────────────────────────────

        [Header("Theme Info")]
        [Tooltip("Human-readable name displayed in the Builder window.")]
        public string themeName = "My Brand";

        // ────────────────────────────────────────────────────
        // BRAND COLORS  (user-defined)
        // Per Android XR docs: Unity apps are free to use any
        // design language. Set your brand's primary and
        // secondary here — all M3 roles are derived from these.
        // ────────────────────────────────────────────────────

        [Header("Brand Colors — Set Your Brand Here")]

        [Tooltip("Your primary brand color.\nUsed for: Filled button background, Outlined border/text, icon tint.\nXR Tip: Avoid very dark colors — on wired XR glasses, dark = transparent.")]
        public Color primary = new Color(0.396f, 0.333f, 0.561f); // #65558F default

        [Tooltip("Color that appears ON your primary color (text/icons on filled buttons).\nMust have high contrast against Primary.\nXR Tip: Use bright/white — improves legibility on transparent displays.")]
        public Color onPrimary = Color.white;

        [Tooltip("Secondary brand color.\nUsed for: Tonal button background, chip backgrounds, secondary surfaces.\nMust contrast with OnSecondaryContainer.")]
        public Color secondaryContainer = new Color(0.910f, 0.871f, 0.973f); // #E8DEF8 default

        [Tooltip("Color that appears ON your secondary container (Tonal button text/icons).\nMust have high contrast against SecondaryContainer.")]
        public Color onSecondaryContainer = new Color(0.290f, 0.267f, 0.349f); // #4A4459 default

        [Tooltip("Accent or tertiary color for additional brand expression.\nUsed for: Elevated button background, surface containers.\nXR Tip: Brighter surfaces have more visual weight on wired glasses.")]
        public Color surfaceContainerLow = new Color(0.969f, 0.949f, 0.980f); // #F7F2FA default

        // ────────────────────────────────────────────────────
        // NEUTRAL COLORS
        // These are shared across all components for text,
        // borders, disabled states, and backgrounds.
        // ────────────────────────────────────────────────────

        [Header("Neutral Colors")]

        [Tooltip("Default text/icon color on light surfaces.\nAlso used at 38% opacity for disabled text.")]
        public Color onSurface = new Color(0.114f, 0.106f, 0.125f); // #1D1B20

        [Tooltip("Border color for Outlined components (button border, text field outline).\nShould be softer than OnSurface.")]
        public Color outline = new Color(0.475f, 0.455f, 0.494f); // #79747E

        [Tooltip("General surface/background color for panels and cards.\nFor wired XR glasses: avoid pure black — it renders as transparent.")]
        public Color surface = new Color(0.996f, 0.969f, 1.000f); // #FEF7FF

        // ────────────────────────────────────────────────────
        // STATE LAYER OPACITIES
        // M3 interaction model: color overlays for hover/focus/press.
        // Docs: https://m3.material.io/foundations/interaction/states
        // ────────────────────────────────────────────────────

        [Header("State Layer Opacities (0–1)")]

        [Tooltip("Opacity of the state layer on Hover.\nM3 default: 0.08 (8%)")]
        [Range(0f, 0.3f)]
        public float stateHoverOpacity = 0.08f;

        [Tooltip("Opacity of the state layer on Focus or Press.\nM3 default: 0.12 (12%)")]
        [Range(0f, 0.3f)]
        public float statePressOpacity = 0.12f;

        [Tooltip("Opacity of disabled component overlay.\nM3 default: 0.12 (12%)")]
        [Range(0f, 0.3f)]
        public float stateDisabledContainerOpacity = 0.12f;

        [Tooltip("Opacity of text/icon on disabled components.\nM3 default: 0.38 (38%)")]
        [Range(0f, 1f)]
        public float stateDisabledContentOpacity = 0.38f;

        // ────────────────────────────────────────────────────
        // SPATIAL / XR SETTINGS
        // Based on: https://developer.android.com/design/ui/xr/guides/spatial-ui
        //            https://developer.android.com/design/ui/xr/guides/visual-design
        // ────────────────────────────────────────────────────

        [Header("Spatial Settings (Android XR)")]

        [Tooltip("Spawn distance from user in meters.\nAndroid XR default: 1.75m. Do not go below 0.75m (system minimum).")]
        [Range(0.75f, 5.0f)]
        public float panelSpawnDepth = 1.75f;

        [Tooltip("Panel corner radius in dp.\nAndroid XR default: 32dp. You can override this per-panel.")]
        [Range(0f, 64f)]
        public float panelCornerRadius = 32f;

        [Tooltip("Vertical offset from eye level in degrees (negative = below eye level).\nAndroid XR guideline: -5 degrees for optimal comfort.")]
        [Range(-30f, 10f)]
        public float panelVerticalOffsetDegrees = -5f;

        [Tooltip("Target size in dp for XR interactive elements.\nAndroid XR recommends 56dp x 56dp. M3 minimum is 48dp.")]
        [Range(40f, 96f)]
        public float xrTargetSizeDp = 56f;

        // ────────────────────────────────────────────────────
        // SPATIAL ELEVATION Z-DEPTHS
        // Per: https://developer.android.com/design/ui/xr/guides/spatial-ui#spatial-elevation
        // Level 0 = on panel surface
        // Level 1 = Orbiter (16dp)
        // Level 3 = SpatialPopup (32dp)
        // Level 5 = SpatialDialog (56dp)
        // ────────────────────────────────────────────────────

        [Header("Spatial Elevation Z-Depths (dp)")]

        [Tooltip("Level 0 — On panel surface. Default: 0.1dp")]
        public float spatialElevation0 = 0.1f;

        [Tooltip("Level 1 — Orbiter. Android XR default: 16dp")]
        public float spatialElevation1 = 16f;

        [Tooltip("Level 3 — SpatialPopup. Android XR default: 32dp")]
        public float spatialElevation3 = 32f;

        [Tooltip("Level 5 — SpatialDialog. Android XR default: 56dp")]
        public float spatialElevation5 = 56f;

        // ────────────────────────────────────────────────────
        // TYPOGRAPHY SETTINGS
        // Per: https://developer.android.com/design/ui/xr/guides/visual-design#xr-typography
        // Min 14dp, normal weight+, sentence case.
        // ────────────────────────────────────────────────────

        [Header("Typography (Android XR)")]

        [Tooltip("Label Large font size (buttons, tabs). Min 14dp per Android XR guidelines.")]
        [Range(14f, 24f)]
        public float labelLargeFontSize = 14f;

        [Tooltip("Body font size for text fields, search, supporting text.")]
        [Range(14f, 20f)]
        public float bodyFontSize = 16f;

        [Tooltip("TMP font asset for all UI components. Assign Roboto or your brand font.")]
        public TMPro.TMP_FontAsset fontAsset;

        // ────────────────────────────────────────────────────
        // SHADOWS / ELEVATION  (uGUI screen-space shadow)
        // Android XR uses subtle dual-layer drop shadows.
        // ────────────────────────────────────────────────────

        [Header("Component Shadow (M3 Elevation)")]

        [Tooltip("Elevation Level 1 shadow color (outer, soft). Alpha = shadow strength.")]
        public Color shadowLevel1Outer = new Color(0, 0, 0, 0.15f);

        [Tooltip("Elevation Level 1 shadow color (inner, tighter). Alpha = shadow strength.")]
        public Color shadowLevel1Inner = new Color(0, 0, 0, 0.30f);

        [Tooltip("Elevation Level 2 shadow color (outer, wider). Alpha = shadow strength.")]
        public Color shadowLevel2Outer = new Color(0, 0, 0, 0.15f);

        // ────────────────────────────────────────────────────
        // HELPER: Derive state-layer color from a base color
        // ────────────────────────────────────────────────────

        /// <summary>Returns a state-layer color at hover opacity.</summary>
        public Color HoverLayer(Color baseColor)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b, stateHoverOpacity);
        }

        /// <summary>Returns a state-layer color at press/focus opacity.</summary>
        public Color PressLayer(Color baseColor)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b, statePressOpacity);
        }

        /// <summary>Returns a color at the disabled content opacity.</summary>
        public Color DisabledContent(Color baseColor)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b,
                             baseColor.a * stateDisabledContentOpacity);
        }

        /// <summary>Returns a color at the disabled container opacity.</summary>
        public Color DisabledContainer(Color baseColor)
        {
            return new Color(baseColor.r, baseColor.g, baseColor.b,
                             baseColor.a * stateDisabledContainerOpacity);
        }

        // ────────────────────────────────────────────────────
        // HELPER: HEX string → Color (editor utility)
        // ────────────────────────────────────────────────────

        public static Color HexColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString("#" + hex.TrimStart('#'), out Color c))
                return c;
            return Color.magenta; // fallback — magenta = "something went wrong"
        }
    }
}
