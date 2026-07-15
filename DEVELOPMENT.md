# Development Guide for Sukuna Mod

## Prerequisites

- Visual Studio 2022 or Rider IDE
- .NET 6.0 SDK
- Among Us game files (for references)
- BepInEx source code understanding

## Project Structure

```
SukunaMod/
├── src/
│   ├── SukunaMod.cs          # Main mod entry point
│   ├── SukunaPlayer.cs       # Player state & transformation logic
│   ├── Abilities/
│   │   ├── CleaveAbility.cs  # AOE damage ability
│   │   ├── SlashAbility.cs   # Single target ability
│   │   └── DomainExpansion.cs # Zone ability
│   └── Networking/
│       └── SukunaRPC.cs      # Network synchronization
├── SukunaMod.csproj
└── README.md
```

## Building

```bash
dotnet build -c Release
```

Output: `bin/Release/SukunaMod.dll`

## Code Architecture

### Core Module: `SukunaMod.cs`

Initializes the mod and hooks into Among Us events:

```csharp
[BepInPlugin("com.ini1647.sukuna", "Sukuna Mod", "1.0.0")]
public class SukunaMod : BaseUnityPlugin
{
    public static SukunaMod Instance { get; private set; }
    
    private void Awake() { /* Setup */ }
    private void Update() { /* Input polling */ }
}
```

### Player State: `SukunaPlayer.cs`

Tracks player transformation and ability cooldowns:

```csharp
public class SukunaPlayer
{
    public bool IsSukuna { get; set; }
    public Dictionary<string, float> AbilityCooldowns { get; set; }
    public void Transform();
    public void UseAbility(string abilityName);
}
```

### Ability System

Each ability is a separate class implementing `IAbility`:

```csharp
public interface IAbility
{
    void Execute(PlayerControl player, Vector3 target);
    float Cooldown { get; }
    string Name { get; }
}
```

### Networking: `SukunaRPC.cs`

Uses Among Us RPC system to sync across players:

- `RPC_Transform`: Broadcast transformation to all players
- `RPC_CleaveAttack`: Sync AOE damage
- `RPC_SlashAttack`: Sync targeted attack
- `RPC_DomainExpansion`: Sync domain zone creation

## Adding New Abilities

1. Create `NewAbility.cs` in `src/Abilities/`:

```csharp
public class NewAbility : IAbility
{
    public string Name => "New Ability";
    public float Cooldown => 30f;
    
    public void Execute(PlayerControl player, Vector3 target)
    {
        // Your ability logic here
        SukunaRPC.SendAbilityRPC("NewAbility", player.PlayerId, target);
    }
}
```

2. Register in `SukunaMod.cs`:

```csharp
private void Awake()
{
    abilities.Add("newability", new NewAbility());
}
```

3. Add keybind in config and input handling in `Update()`.

## Testing Locally

1. Build the mod
2. Copy `SukunaMod.dll` to `BepInEx/plugins/`
3. Launch Among Us
4. Open BepInEx console: Press `F5`
5. Check for errors in console
6. Create a local game and test

## Debugging

Enable logging:

```csharp
Logger.LogInfo("Message here");
Logger.LogError("Error message");
Logger.LogDebug("Debug message");
```

View logs in: `Among Us\BepInEx\LogOutput.log`

## Common Issues

**Among Us version mismatch** - Check your Among Us version and update BepInEx references accordingly.

**RPC not syncing** - Verify RPC ID doesn't conflict with other mods. Use a high ID (e.g., 150+).

**Ability not executing** - Check keybind config and ensure player is actually the Impostor.
