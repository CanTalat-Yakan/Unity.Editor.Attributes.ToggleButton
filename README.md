# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# Toggle Button Attribute

> Quick overview: Render boolean fields as clickable toggle buttons with optional icons. Group related toggles on one line by name.

Replace checkbox UI with compact buttons. Use `[ToggleButton("iconName")]` on a `bool` to draw a toolbar‑style button; use a shared `groupName` to render multiple bools as a labeled row of buttons. Icons can be referenced by name or via the `EditorIconNames` enum.

![screenshot](Documentation/Screenshot.png)

## Features
- Button UI for boolean fields (on/off state)
- Optional icon per button; tooltip comes from the field’s tooltip
- Grouping: render multiple toggles in a labeled row via a shared `groupName`
- Keyboard activation support (Enter/Space) when focused
- Editor‑only; zero runtime overhead

## Requirements
- Unity Editor 6000.0+ (Editor‑only; attribute lives in Runtime for convenience)
- Inspector Hooks module (for property iteration and keyboard focus helper)
- Editor Icons module (for `EditorIconUtilities` and `EditorIconNames` icon references)

Tip: To show a helpful tooltip on hover, add a `[Tooltip("...")]` to your field.

## Usage
Single toggle with icon by name

```csharp
using UnityEngine;
using UnityEssentials;

public class Example : MonoBehaviour
{
    [Tooltip("Enable post effects")] 
    [ToggleButton("scenevis_visible_hover@2x")]
    public bool postFX;
}
```

Single toggle with icon enum

```csharp
public class Example2 : MonoBehaviour
{
    [ToggleButton(EditorIconNames.ToggleIcon)]
    public bool active;
}
```

Grouped toggles on one row

```csharp
public class RenderingModes : MonoBehaviour
{
    [ToggleButton(EditorIconNames.ShaderIcon, groupName: "Rendering Modes")] public bool opaque;
    [ToggleButton(EditorIconNames.ShaderGraphIcon, groupName: "Rendering Modes")] public bool cutout;
    [ToggleButton(EditorIconNames.ShaderVariantCollectionIcon, groupName: "Rendering Modes")] public bool transparent;
}
```

## How It Works
- Drawer supports `bool` fields only; others show an inline error
- Single: draws an optional label, then a 32×20 button showing the icon; clicking toggles the bool
- Grouped: finds all fields in the inspector with `[ToggleButton]` sharing the same `groupName`; draws them once as a labeled row of buttons
- Icons are loaded via `EditorGUIUtility.IconContent(iconName)`; enum overload resolves to a string using `EditorIconUtilities`
- Tooltip is taken from the field’s tooltip (if provided)
- Keyboard activation uses the Inspector Hooks focus helper to simulate a click when focused

## Notes and Limitations
- Supported field type: `bool` only
- Group rendering occurs only once (on the first field in the group) to avoid duplicates; this is automatic
- Icon references must exist; invalid names result in an empty button content
- Labels
  - Single button: uses the field label
  - Grouped row: uses the `groupName` as the row label
- Multi‑object editing supported; state toggles per inspected target
- Editor‑only: no effect at runtime

## Files in This Package
- `Runtime/ToggleButtonAttribute.cs` – Attribute with icon reference (string or `EditorIconNames`) and optional `groupName`
- `Editor/ToggleButtonDrawer.cs` – PropertyDrawer (single/grouped layout, icon/tooltip, keyboard, apply)
- `Runtime/UnityEssentials.ToggleButtonAttribute.asmdef` – Runtime assembly definition
- `Editor/UnityEssentials.ToggleButtonAttribute.Editor.asmdef` – Editor assembly definition

## Tags
unity, unity-editor, attribute, propertydrawer, toggle, button, toolbar, icon, group, inspector, ui, tools, workflow
