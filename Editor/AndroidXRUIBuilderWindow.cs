// ============================================================
//  AndroidXRUIBuilderWindow.cs
//  Android XR UI Builder — Editor
//
//  Main editor window. Accessible via:
//    Unity top menu → Android XR UI Builder → Open Builder
//
//  Day 1 scope:
//    [x] Menu entry
//    [x] Theme section: assign/create ThemeData
//    [x] Brand color preview swatches (live from ThemeData)
//    [x] "Create Android XR Canvas" button
//    [ ] Component palette (Day 3–8)
// ============================================================

using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace AndroidXRUIBuilder.Editor
{
    public class AndroidXRUIBuilderWindow : EditorWindow
    {
        // ── Window state ──────────────────────────────────
        ThemeData _theme;
        SerializedObject _themeSO;
        Vector2 _scroll;
        bool _showThemeSection    = true;
        bool _showSpatialSection  = true;
        bool _showComponentSection = true;

        // ── Style cache ───────────────────────────────────
        GUIStyle _headerStyle;
        GUIStyle _sectionStyle;
        GUIStyle _subHeaderStyle;
        GUIStyle _warningStyle;
        bool _stylesBuilt;

        // ── Menu entry ────────────────────────────────────
        [MenuItem("Android XR UI Builder/Open Builder", priority = 0)]
        public static void OpenWindow()
        {
            var win = GetWindow<AndroidXRUIBuilderWindow>("XR UI Builder");
            win.minSize = new Vector2(320, 520);
            win.Show();
        }

        [MenuItem("Android XR UI Builder/Create Theme Data", priority = 1)]
        public static void CreateTheme()
        {
            var path = EditorUtility.SaveFilePanelInProject(
                "Create Theme Data", "XRThemeData", "asset",
                "Save your new ThemeData asset");
            if (string.IsNullOrEmpty(path)) return;

            var asset = CreateInstance<ThemeData>();
            AssetDatabase.CreateAsset(asset, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Selection.activeObject = asset;
            Debug.Log($"[AndroidXR UI Builder] Created ThemeData at {path}");
        }

        // ── Lifecycle ─────────────────────────────────────
        void OnEnable()
        {
            // Auto-find ThemeData in project on open
            if (_theme == null)
            {
                var guids = AssetDatabase.FindAssets("t:ThemeData");
                if (guids.Length > 0)
                {
                    var path = AssetDatabase.GUIDToAssetPath(guids[0]);
                    _theme = AssetDatabase.LoadAssetAtPath<ThemeData>(path);
                    RebuildSerializedObject();
                }
            }
        }

        void OnGUI()
        {
            BuildStyles();
            _scroll = EditorGUILayout.BeginScrollView(_scroll);

            DrawHeader();
            DrawThemeSection();
            DrawSpatialInfoSection();
            DrawComponentPaletteSection();

            EditorGUILayout.EndScrollView();

            // Apply changes back to the ScriptableObject
            if (_themeSO != null)
            {
                _themeSO.ApplyModifiedProperties();
                if (GUI.changed) EditorUtility.SetDirty(_theme);
            }
        }

        // ── Header ────────────────────────────────────────
        void DrawHeader()
        {
            EditorGUILayout.Space(8);
            var rect = EditorGUILayout.GetControlRect(false, 48);
            EditorGUI.DrawRect(rect, new Color(0.247f, 0.212f, 0.353f)); // #3F3659
            GUI.Label(new Rect(rect.x + 12, rect.y + 6, rect.width, 36),
                "Android XR UI Builder", _headerStyle);
            EditorGUILayout.Space(4);
        }

        // ── Theme Section ─────────────────────────────────
        void DrawThemeSection()
        {
            _showThemeSection = DrawFoldout(_showThemeSection, "🎨  Theme");
            if (!_showThemeSection) return;

            EditorGUI.indentLevel++;

            // ThemeData object field
            EditorGUI.BeginChangeCheck();
            var newTheme = (ThemeData)EditorGUILayout.ObjectField(
                "Theme Data", _theme, typeof(ThemeData), false);
            if (EditorGUI.EndChangeCheck())
            {
                _theme = newTheme;
                RebuildSerializedObject();
            }

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New Theme", GUILayout.Height(22)))
                CreateTheme();
            if (GUILayout.Button("Ping in Project", GUILayout.Height(22)) && _theme != null)
                EditorGUIUtility.PingObject(_theme);
            EditorGUILayout.EndHorizontal();

            if (_theme == null)
            {
                EditorGUILayout.HelpBox(
                    "No ThemeData assigned.\nCreate one above or drag an existing asset here.",
                    MessageType.Warning);
                EditorGUI.indentLevel--;
                return;
            }

            EditorGUILayout.Space(6);

            // ── Brand Color Swatches ──────────────────────
            EditorGUILayout.LabelField("Brand Colors", _subHeaderStyle);
            DrawColorRow("Primary", _theme.primary,
                "Main brand color. Used on Filled buttons, Outlined borders.\n" +
                "XR Tip: Avoid very dark colors — transparent on wired glasses.");
            DrawColorRow("On Primary", _theme.onPrimary,
                "Text/icon color on top of Primary.\nUse bright/white for XR readability.");
            DrawColorRow("Secondary Container", _theme.secondaryContainer,
                "Tonal button background. Should feel lighter than Primary.");
            DrawColorRow("On Secondary Container", _theme.onSecondaryContainer,
                "Text/icon on Tonal buttons. Must contrast secondaryContainer.");
            DrawColorRow("Surface Container Low", _theme.surfaceContainerLow,
                "Elevated button background. Light surface tone.");
            DrawColorRow("On Surface", _theme.onSurface,
                "Default text / icon color.");
            DrawColorRow("Outline", _theme.outline,
                "Border for Outlined buttons, TextFields.");

            // XR color warning for very dark primaries
            if (_theme != null && _theme.primary.grayscale < 0.2f)
            {
                EditorGUILayout.HelpBox(
                    "⚠️  Primary color is very dark. On wired XR glasses, dark colors " +
                    "render as transparent. Consider a brighter tint.",
                    MessageType.Warning);
            }

            EditorGUILayout.Space(4);

            // ── Inline theme editing ──────────────────────
            EditorGUILayout.LabelField("Edit Theme Values", _subHeaderStyle);
            if (_themeSO != null)
            {
                // Draw all serialized properties that belong to the brand colors group
                DrawThemeProp("primary",              "Primary");
                DrawThemeProp("onPrimary",            "On Primary");
                DrawThemeProp("secondaryContainer",   "Secondary Container");
                DrawThemeProp("onSecondaryContainer", "On Secondary Container");
                DrawThemeProp("surfaceContainerLow",  "Surface Low");
                DrawThemeProp("onSurface",            "On Surface");
                DrawThemeProp("outline",              "Outline");
                DrawThemeProp("surface",              "Surface");
                EditorGUILayout.Space(4);
                DrawThemeProp("fontAsset",            "Font Asset");
            }

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(8);
        }

        // ── Spatial Info Section ──────────────────────────
        void DrawSpatialInfoSection()
        {
            _showSpatialSection = DrawFoldout(_showSpatialSection, "🌐  Spatial Settings");
            if (!_showSpatialSection) return;

            EditorGUI.indentLevel++;

            if (_theme != null && _themeSO != null)
            {
                DrawThemeProp("panelSpawnDepth",          "Spawn Depth (m)");
                DrawThemeProp("panelCornerRadius",         "Panel Corner Radius (dp)");
                DrawThemeProp("panelVerticalOffsetDegrees","Vertical Offset (°)");
                DrawThemeProp("xrTargetSizeDp",           "Min Target Size (dp)");
                EditorGUILayout.Space(4);
                DrawThemeProp("spatialElevation0", "Elevation 0 — Surface (dp)");
                DrawThemeProp("spatialElevation1", "Elevation 1 — Orbiter (dp)");
                DrawThemeProp("spatialElevation3", "Elevation 3 — Popup (dp)");
                DrawThemeProp("spatialElevation5", "Elevation 5 — Dialog (dp)");
            }

            EditorGUILayout.HelpBox(
                "Android XR Guidelines:\n" +
                "• Spawn panels at 1.75m from user\n" +
                "• Center 5° below eye level for comfort\n" +
                "• Keep content in center 41° FOV\n" +
                "• Min target size: 56dp × 56dp\n" +
                "• Panel corners: 32dp default\n" +
                "• Dark colors = transparent on wired XR glasses",
                MessageType.Info);

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(8);
        }

        // ── Component Palette Section ─────────────────────
        void DrawComponentPaletteSection()
        {
            _showComponentSection = DrawFoldout(_showComponentSection, "🧩  Components");
            if (!_showComponentSection) return;

            EditorGUI.indentLevel++;

            // Canvas creation button
            EditorGUILayout.LabelField("Canvas", _subHeaderStyle);
            using (new EditorGUI.DisabledScope(_theme == null))
            {
                if (GUILayout.Button("➕  Create Android XR Canvas", GUILayout.Height(32)))
                    CreateAndroidXRCanvas();
            }
            if (_theme == null)
                EditorGUILayout.HelpBox("Assign a ThemeData first.", MessageType.Warning);

            EditorGUILayout.Space(8);

            // Component buttons — stubs, will be wired up Day 3–8
            EditorGUILayout.LabelField("Button", _subHeaderStyle);
            DrawComponentButton("Filled Button",   "Style=Filled, Enabled");
            DrawComponentButton("Outlined Button", "Style=Outlined, Enabled");
            DrawComponentButton("Text Button",     "Style=Text, Enabled");
            DrawComponentButton("Elevated Button", "Style=Elevated, Enabled");
            DrawComponentButton("Tonal Button",    "Style=Tonal, Enabled");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Text Field", _subHeaderStyle);
            DrawComponentButton("Filled Text Field",   "Variant=Filled");
            DrawComponentButton("Outlined Text Field", "Variant=Outlined");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Search Bar", _subHeaderStyle);
            DrawComponentButton("Search Bar", "Pill shape, Elevation 1");

            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField("Image View", _subHeaderStyle);
            DrawComponentButton("Image (No Mask)",      "MaskShape=None");
            DrawComponentButton("Image (Circle Mask)",  "MaskShape=Circle");
            DrawComponentButton("Image (Rounded Rect)", "MaskShape=RoundedRect");

            EditorGUI.indentLevel--;
            EditorGUILayout.Space(8);
        }

        // ── Canvas Creation ───────────────────────────────
        void CreateAndroidXRCanvas()
        {
            // Create root GameObject
            var canvasGO = new GameObject("AndroidXRCanvas");
            Undo.RegisterCreatedObjectUndo(canvasGO, "Create Android XR Canvas");

            // Canvas
            var canvas = canvasGO.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;

            // Canvas Scaler (1024×720 reference — 0.868 dp-to-dmm)
            var scaler = canvasGO.AddComponent<CanvasScaler>();
            scaler.uiScaleMode           = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution   = new Vector2(1024, 720);
            scaler.screenMatchMode       = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight    = 0.5f;

            canvasGO.AddComponent<GraphicRaycaster>();

            // XRCanvasController
            var controller = canvasGO.AddComponent<XRCanvasController>();
            controller.theme = _theme;

            // World Space sizing (1mm per Unity unit)
            var canvasRT = canvasGO.GetComponent<RectTransform>();
            canvasRT.sizeDelta    = new Vector2(1556f, 1094f); // ~1024dp × 720dp at 1.75m
            canvasRT.localScale   = Vector3.one * 0.001f;
            canvasRT.localPosition = new Vector3(0f, 0f, _theme.panelSpawnDepth);

            // Root Panel
            var panelGO = new GameObject("RootPanel");
            panelGO.transform.SetParent(canvasGO.transform, false);

            var panelRT = panelGO.AddComponent<RectTransform>();
            panelRT.anchorMin  = Vector2.zero;
            panelRT.anchorMax  = Vector2.one;
            panelRT.offsetMin  = Vector2.zero;
            panelRT.offsetMax  = Vector2.zero;

            var panelImage = panelGO.AddComponent<Image>();
            panelImage.color = _theme.surface;
            panelImage.type  = Image.Type.Sliced;

            panelGO.AddComponent<VerticalLayoutGroup>();

            // Place in scene
            Selection.activeGameObject = canvasGO;
            EditorGUIUtility.PingObject(canvasGO);

            Debug.Log("[AndroidXR UI Builder] Created AndroidXRCanvas with RootPanel. " +
                      $"Theme: {_theme.themeName}, Spawn depth: {_theme.panelSpawnDepth}m");
        }

        // ── Helpers ───────────────────────────────────────
        void DrawColorRow(string label, Color color, string tooltip)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent(label, tooltip),
                GUILayout.Width(160));
            var swatchRect = EditorGUILayout.GetControlRect(false, 18, GUILayout.Width(48));
            EditorGUI.DrawRect(swatchRect, color);
            // Hex display
            string hex = "#" + ColorUtility.ToHtmlStringRGBA(color);
            EditorGUILayout.LabelField(hex, GUILayout.Width(100));
            EditorGUILayout.EndHorizontal();
        }

        void DrawThemeProp(string propName, string label)
        {
            if (_themeSO == null) return;
            var prop = _themeSO.FindProperty(propName);
            if (prop != null)
                EditorGUILayout.PropertyField(prop, new GUIContent(label));
        }

        void DrawComponentButton(string label, string subtitle)
        {
            bool hasCanvas = FindObjectOfType<XRCanvasController>() != null;
            using (new EditorGUI.DisabledScope(!hasCanvas || _theme == null))
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button($"  {label}", GUILayout.Height(26)))
                    OnComponentClicked(label);
                EditorGUILayout.LabelField(subtitle,
                    EditorStyles.miniLabel, GUILayout.Width(160));
                EditorGUILayout.EndHorizontal();
            }
        }

        void OnComponentClicked(string label)
        {
            // Placeholder — ComponentSpawner wired up Day 8
            Debug.Log($"[AndroidXR UI Builder] TODO: Spawn '{label}' — wiring up Day 8.");
            EditorUtility.DisplayDialog("Coming Soon",
                $"'{label}' spawning will be implemented in Day 8 (ComponentSpawner).\n" +
                "Keep going — canvas and theme are ready now!",
                "OK");
        }

        bool DrawFoldout(bool state, string label)
        {
            var rect = EditorGUILayout.GetControlRect(false, 24);
            EditorGUI.DrawRect(rect, new Color(0.18f, 0.18f, 0.22f, 1f));
            state = EditorGUI.Foldout(
                new Rect(rect.x + 4, rect.y + 3, rect.width, rect.height),
                state, "  " + label, true, _sectionStyle);
            EditorGUILayout.Space(2);
            return state;
        }

        void RebuildSerializedObject()
        {
            _themeSO = _theme != null ? new SerializedObject(_theme) : null;
        }

        void BuildStyles()
        {
            if (_stylesBuilt) return;
            _stylesBuilt = true;

            _headerStyle = new GUIStyle(EditorStyles.boldLabel)
            {
                fontSize  = 15,
                normal    = { textColor = Color.white }
            };
            _sectionStyle = new GUIStyle(EditorStyles.foldout)
            {
                fontStyle = FontStyle.Bold,
                fontSize  = 12,
                normal    = { textColor = new Color(0.85f, 0.80f, 1.0f) },
                onNormal  = { textColor = Color.white }
            };
            _subHeaderStyle = new GUIStyle(EditorStyles.miniBoldLabel)
            {
                normal = { textColor = new Color(0.7f, 0.65f, 0.9f) }
            };
            _warningStyle = new GUIStyle(EditorStyles.helpBox)
            {
                fontSize = 11
            };
        }
    }
}
