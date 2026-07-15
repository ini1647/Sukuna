# Sukuna Mod for Among Us - UI Button Guide

## Features with UI Buttons

Your mod now includes **on-screen buttons** for easy access to all Sukuna abilities!

### Button Layout (Top-Left Corner)

```
┌─────────────────┐
│  Transform [S]  │  <- Toggle Sukuna Form
├─────────────────┤
│  Cleave [C]     │  <- AOE Attack (25s cooldown)
├─────────────────┤
│  Slash [V]      │  <- Single Target (15s cooldown)
├─────────────────┤
│  Domain [X]     │  <- Domain Expansion (60s cooldown)
└─────────────────┘
```

---

## How to Use

### Enable/Disable Buttons

Edit the config file after first run:
```
Among Us\BepInEx\config\com.ini1647.sukuna.cfg
```

Find this line and change `true` to `false` to disable buttons:
```ini
[UI]
UseButtons = true
```

### Button Features

✅ **Visual Cooldown Indicators**
- Buttons show remaining cooldown time
- Button darkens when on cooldown
- Button becomes clickable when ready

✅ **Automatic Show/Hide**
- Buttons only appear when you're Sukuna
- Buttons hide when you transform back to normal
- Transform button always visible

✅ **Color Coded**
- **Transform**: Red
- **Cleave**: Blue
- **Slash**: Green
- **Domain**: Yellow

---

## Keyboard vs Button Mode

### Option 1: Buttons Only (Recommended for New Players)
```ini
[UI]
UseButtons = true
```
- Use on-screen buttons to activate abilities
- Keyboard shortcuts still work (S, C, V, X)
- Best for controller players

### Option 2: Keyboard Only
```ini
[UI]
UseButtons = false
```
- No buttons on screen
- Use keyboard keys: S, C, V, X
- Best for keyboard players who prefer minimal UI

### Option 3: Hybrid (Both Active)
- Keep `UseButtons = true`
- Use buttons OR keyboard shortcuts
- Full flexibility!

---

## Customizing Button Positions

Edit `src/UI/SukunaUIManager.cs` and change the `Vector2 position` values:

```csharp
// Current positions (top-left corner)
transformButton = CreateButton("Transform", new Vector2(50, 50), ...);
cleaveButton = CreateButton("Cleave [C]", new Vector2(50, 150), ...);
slashButton = CreateButton("Slash [V]", new Vector2(50, 250), ...);
domainButton = CreateButton("Domain [X]", new Vector2(50, 350), ...);

// Move to bottom-right instead (example):
transformButton = CreateButton("Transform", new Vector2(-70, -70), ...);
cleaveButton = CreateButton("Cleave [C]", new Vector2(-70, -150), ...);
// etc...
```

Then rebuild the mod: `dotnet build -c Release`

---

## Button Customization

### Change Button Colors

In `SukunaUIManager.cs`, modify these lines:

```csharp
// Red buttons
transformButton = CreateButton("Transform", new Vector2(50, 50), new Color(0.8f, 0.2f, 0.2f, 0.8f));

// Blue buttons
cleaveButton = CreateButton("Cleave [C]", new Vector2(50, 150), new Color(0.2f, 0.2f, 0.8f, 0.8f));

// Green buttons
slashButton = CreateButton("Slash [V]", new Vector2(50, 250), new Color(0.2f, 0.8f, 0.2f, 0.8f));

// Yellow buttons
domainButton = CreateButton("Domain [X]", new Vector2(50, 350), new Color(0.8f, 0.8f, 0.2f, 0.8f));
```

Colors use RGBA format (0-1 scale):
- `new Color(R, G, B, Alpha)`
- Example: `new Color(1f, 0f, 0f, 0.8f)` = Red with 80% opacity

### Change Button Size

Find this line in `CreateButton()`:
```csharp
rectTransform.sizeDelta = new Vector2(120, 80);  // Width, Height
```

Change to your preferred size (in pixels):
```csharp
rectTransform.sizeDelta = new Vector2(150, 100);  // Larger buttons
```

---

## Troubleshooting

### Buttons Don't Appear
- Check that `UseButtons = true` in config
- Make sure you're the Impostor
- Make sure you're the Host
- Check BepInEx console (F5) for errors

### Buttons Appear But Don't Work
- Verify Among Us window is focused
- Check that buttons aren't behind other UI
- Restart the game

### Buttons Cover Important UI
- Disable buttons: set `UseButtons = false`
- Or reposition them in `SukunaUIManager.cs`
- Rebuild and reinstall the mod

---

## Next Steps

1. **Build the mod** with buttons included: `dotnet build -c Release`
2. **Copy to BepInEx**: Place `SukunaMod.dll` in `BepInEx\plugins\`
3. **Launch Among Us** and test the buttons
4. **Customize** position/colors if needed

For full setup instructions, see [BUILD_GUIDE.md](BUILD_GUIDE.md)
