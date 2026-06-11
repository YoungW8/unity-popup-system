# Unity Popup System

A lightweight, self-contained popup management system for Unity UI, battle-tested in a published mobile game. No third-party dependencies.

## Features

- **Centralized popup management** — a single `PopupController` owns all popups, the fade overlay and the show queue.
- **Lazy instantiation** — popup prefabs are loaded from `Resources` and instantiated on first use, then cached and reused.
- **Per-popup close callbacks** — pass an `Action` to `ShowPopup` and it is invoked when that popup closes.
- **Popup queue** — enqueue several popups (`AddPopupInQueue`), each with its own close callback; they are shown one after another as the previous one closes.
- **Per-popup configuration** — whether a popup needs the fade overlay and the gold counter is configured on the popup prefab itself (serialized fields of `Popup`), not passed on every call.
- **Template-method base class** — concrete popups inherit from `Popup` and implement a small set of hooks (`ShowRequirement`, `ChangeValues`, `ShowEvent`, `CloseEvent`, button handlers), while the base class wires up the buttons and handles open/close state.

## Structure

```
PopupController.cs   # Controller: show/queue/close logic, fade overlay, gold counter
Popup.cs             # Abstract base class for all popups
```

## Setup

1. Copy the scripts anywhere under `Assets/`.
2. Create a scene object for the controller:
   - Add a `Canvas` (Screen Space – Camera or Overlay) with a child layout `Transform` that will hold popup instances.
   - Add `PopupController` to the root object and assign: the canvas, the fade overlay object, the gold counter object and the popup layout transform.
   - Set **Popup Prefabs Path** — a path inside a `Resources` folder where popup prefabs live (e.g. `Popups/`).
3. The controller marks itself `DontDestroyOnLoad`. How you access it is up to you — store the reference wherever it fits your architecture (an inspector field, your bootstrapper, a DI container), or expose it as a static instance yourself if that suits your project better.
4. If the canvas is in camera space, call `SetCamera(camera)` after each scene load.

## Creating a popup

1. Create a class inheriting from `Popup` and implement the abstract members:

```csharp
public class MyPopup : Popup
{
    public override bool ShowRequirement() => true; // condition to allow showing

    protected override void ChangeValues() { }      // refresh displayed data
    protected override void ShowEvent() { }         // on opened
    protected override void CloseEvent() { }        // on closed

    protected override void CoreBtn() { Close(); }       // primary button
    protected override void AdditionalBtn() { }          // secondary button
    protected override void CloseBtn() { Close(); }      // close button
}
```

2. Make a prefab with this component, wire up the buttons (`_coreBtn`, `_additionalBtn`, `_closeBtn` — each is optional) and tick **Fade** / **Gold Counter** as needed.
3. Place the prefab into `Resources/<Popup Prefabs Path>/`. The prefab name is the popup's id.

If the popup needs a close animation, start it inside `CloseCoroutine` (see the comment in `Popup.cs`) — the popup is deactivated only after the animation coroutine finishes.

## Usage

```csharp
[SerializeField] private PopupController _popupController;

// Show immediately (popup name = prefab name in Resources)
_popupController.ShowPopup("MyPopup");

// Show with a callback fired when the popup closes
_popupController.ShowPopup("MyPopup", () => Debug.Log("closed"));

// Enqueue several popups — each with its own close callback —
// and show them one by one
_popupController.AddPopupInQueue(new PopupShowParams
{
    name = "DailyBonus",
    onCloseAction = GiveDailyReward
});
_popupController.AddPopupInQueue(new PopupShowParams { name = "NoAds" });
_popupController.ShowQueuePopups();
```

Opening a popup automatically closes any other popup that is currently open. The fade overlay and gold counter are hidden when the last open popup closes.

## Requirements

- Unity 2021.3+ (uses only `UnityEngine` and `UnityEngine.UI`).
- No third-party dependencies.
