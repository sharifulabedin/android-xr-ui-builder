# Android XR UI Builder

A **Unity 6 LTS** Editor plugin for building **Android XR**-compliant UIs using the uGUI Canvas system. Implements the **Material Design 3** component library with brand-driven theming — no Figma tokens required.

> Based on [Android XR Design Guidelines](https://developer.android.com/design/ui/xr/guides/get-started)

---

## ✨ Features (v1.0 — In Progress)

- 🎨 **Brand-driven ThemeData** — set your own Primary, Secondary, Surface colors
- 🌐 **World Space Canvas** — spawns at 1.75m per Android XR spatial guidelines
- 📐 **M3 Components** — Button (Filled/Outlined/Text/Elevated/Tonal), Text Field, Search Bar, Image View
- 🔍 **Custom Inspector** — M3-style Style/State dropdowns with live OnValidate preview
- ⚠️ **XR Color Warnings** — alerts when dark colors will appear transparent on wired XR glasses

---

## 📦 Install via Unity Package Manager

### Option A — Install from Git URL (Recommended)

1. Open Unity 6 LTS
2. **Window → Package Manager**
3. Click **＋** → **Add package from git URL…**
4. Paste:

```
https://github.com/YOUR_USERNAME/android-xr-ui-builder.git
```

5. Click **Add**

To install a specific version/tag:
```
https://github.com/YOUR_USERNAME/android-xr-ui-builder.git#v1.0.0
```

### Option B — Add to `Packages/manifest.json` manually

Open `YourProject/Packages/manifest.json` and add to the `dependencies` block:

```json
{
  "dependencies": {
    "com.studio.android-xr-ui-builder": "https://github.com/YOUR_USERNAME/android-xr-ui-builder.git",
    ...
  }
}
```

---

## 🚀 Quick Start

1. After installation, open **Android XR UI Builder → Open Builder** from the Unity top menu
2. Click **New Theme** to create a `ThemeData` ScriptableObject
3. Set your brand's **Primary**, **Secondary**, and **Surface** colors
4. Click **➕ Create Android XR Canvas** — a World Space Canvas + Root Panel appears in the scene
5. Click any component button to add it to the Root Panel

---

## 📁 Package Structure

```
com.studio.android-xr-ui-builder/
├── package.json
├── README.md
├── Runtime/
│   ├── AndroidXRUIBuilder.Runtime.asmdef
│   ├── ThemeData.cs              ← Brand color ScriptableObject
│   └── XRCanvasController.cs    ← World Space canvas setup
├── Editor/
│   ├── AndroidXRUIBuilder.Editor.asmdef
│   └── AndroidXRUIBuilderWindow.cs  ← Main builder window
├── Prefabs/                     ← (Coming Day 5–12)
├── Shaders/                     ← (Coming Day 5)
└── Themes/                      ← (Coming Day 2)
```

---

## 🗺️ Sprint Roadmap

| Day | Feature |
|-----|---------|
| ✅ Day 1 | Package setup, ThemeData, Canvas, Builder Window |
| Day 2 | ThemeData editor UI polish, color harmony helpers |
| Day 3 | AndroidXRUIBuilderWindow full layout |
| Day 4 | XRCanvasController + Root Panel |
| Day 5–7 | XRButton (all 5 styles × 5 states) |
| Day 8 | ComponentSpawner + palette wiring |
| Day 9–10 | XRTextField (Filled + Outlined) |
| Day 11 | XRSearchBar |
| Day 12 | XRImageView with mask options |
| Day 13–14 | Polish, prefabs, export |

---

## 📋 Requirements

- Unity **6000.0** (Unity 6 LTS) or later
- TextMeshPro (included with Unity)

---

## 📄 License

MIT — see [LICENSE](LICENSE)
