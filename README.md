# Unity Popup System

A lightweight, reusable popup management system for Unity UI, extracted from a published mobile solitaire game.

## Features

- **Centralized popup management** — a single `PopupController` owns all popups, their backgrounds, fade overlay and show queue.
- **Lazy instantiation** — popup prefabs are loaded from `Resources` and instantiated only on first use, then cached and reused.
- **Popup queue** — popups can be enqueued (`AddPopupInQueue`) and are shown one after another as the previous one closes.
- **Show callbacks** — pass an `Action` to `ShowPopup` to be invoked when the popup (or the whole queue) is closed.
- **Configurable chrome per popup** — fade overlay, gold counter and one of several background frames (`orange` / `yellow` / `blue`) are toggled per show call.
- **UI pause events** — `EventBus.OnUIPause` / `EventBus.OnUIResume` are raised when popups open/close, so gameplay can react (e.g. pause input under an open popup).
- **Template-method popup base class** — concrete popups inherit from `Popup` and implement a small set of hooks (`ShowRequierment`, `ShowEvent`, `CloseEvent`, button handlers), while the base class handles buttons, sounds, open/close state and the close animation.

## Structure

```
PopupSystem/
├── PopupController.cs      # Singleton controller: show/queue/close logic, fade & backgrounds
├── Popup.cs                # Abstract base class for all popups
├── UpMenuFadeAnimation.cs  # Optional slide animation for the top menu bar
└── Stubs/                  # Minimal stand-ins for external systems (see below)
    ├── EventBus.cs         # UI pause/resume events
    ├── GameSounds.cs       # Sound playback entry point
    ├── ShowAnim.cs         # Popup show/hide animation hook
    └── UIA_Bubble.cs       # Popup close ("bubble") animation hook
```

### About `Stubs/`

`PopupController` and `Popup` reference a few systems that live elsewhere in the original project (audio, global event bus, UI animations). The `Stubs` folder contains minimal compilable versions of them so the module works standalone.

When integrating into a project that already has its own `EventBus`, `GameSounds`, etc., **delete the corresponding stub files** and let the module use your implementations (the required API surface is documented inside each stub).

## Setup

1. Copy the `PopupSystem` folder anywhere under `Assets/`.
2. Create a scene object for the controller:
   - Add a `Canvas` (Screen Space – Camera or Overlay) with a child layout `Transform` that will hold popup instances.
   - Add `PopupController` to the root object and assign: the canvas, fade overlay object, background frame objects, gold counter object and the popup layout transform.
   - Set **Popup Prefabs Path** — a path inside a `Resources` folder where popup prefabs live (e.g. `Popups/`).
3. Call `Init()` once on startup (e.g. from your bootstrapper). The controller becomes a `DontDestroyOnLoad` singleton available via `PopupController.Instance`.
4. If the canvas is in camera space, call `SetCamera(camera)` after each scene load.

## Creating a popup

1. Create a class inheriting from `Popup` and implement the abstract members:

```csharp
public class MyPopup : Popup
{
    public override bool ShowRequierment() => true; // condition to allow showing

    protected override void ChangeValues() { }      // refresh displayed data
    protected override void ShowEvent() { }         // on opened
    protected override void CloseEvent() { }        // on closed

    protected override void CoreBtn() { Close(true); }       // primary button
    protected override void AdditionalBtn() { }              // secondary button
    protected override void CloseBtn() { Close(true); }      // close button
}
```

2. Make a prefab with this component, wire up the buttons (`coreBtn`, `additionalBtn`, `closeBtn`) and place the prefab into `Resources/<Popup Prefabs Path>/`. The prefab name is the popup's id.

## Usage

```csharp
// Show immediately (popup name = prefab name in Resources)
PopupController.Instance.ShowPopup("MyPopup", fade: true, goldCounter: false, backName: "blue");

// Show with a callback fired after the popup closes
PopupController.Instance.ShowPopup("MyPopup", true, false, () => Debug.Log("closed"), "");

// Enqueue several popups — they will be shown one by one
PopupController.Instance.AddPopupInQueue(new PopupShowParams { name = "DailyBonus", fade = true });
PopupController.Instance.AddPopupInQueue(new PopupShowParams { name = "NoAds", fade = true });
PopupController.Instance.ShowQueuePopups();

// React to UI pause while any popup is open
EventBus.SubscribeOnUIPause(() => inputEnabled = false);
EventBus.SubscribeOnUIResume(() => inputEnabled = true);
```

## Requirements

- Unity 2021.3+ (uses only `UnityEngine` and `UnityEngine.UI`).
- No third-party dependencies.
