# How to Build the Sukuna Mod

## Prerequisites

Before building, install these:

1. **Visual Studio 2022** (Community Edition is free)
   - Download: https://visualstudio.microsoft.com/downloads/
   - When installing, select ".NET desktop development" workload

2. **.NET 6.0 SDK**
   - Download: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
   - Install the SDK (not just the runtime)

3. **Among Us** (to get game files for references)

---

## Step 1: Clone/Download the Repository

```bash
git clone https://github.com/ini1647/Sukuna.git
cd Sukuna
```

Or download as ZIP from GitHub and extract it.

---

## Step 2: Get Among Us References

1. Go to your Among Us folder: `C:\Program Files (x86)\Steam\steamapps\common\Among Us\`
2. Copy these files to a `lib/` folder in your Sukuna project:
   - `Assembly-CSharp.dll` (the main game DLL)
   - `UnityEngine.dll`
   - `UnityEngine.CoreModule.dll`

Your folder structure should look like:
```
Sukuna/
├── lib/
│   ├── Assembly-CSharp.dll
│   ├── UnityEngine.dll
│   └── UnityEngine.CoreModule.dll
├── src/
├── SukunaMod.csproj
└── ...
```

---

## Step 3: Build via Command Line (Easy)

Open **Command Prompt** or **PowerShell** in the Sukuna folder and run:

```bash
dotnet build -c Release
```

Wait for it to finish. You should see:
```
Build succeeded!
```

The compiled DLL will be at: `bin/Release/SukunaMod.dll`

---

## Step 4: Build via Visual Studio (GUI)

1. Open `SukunaMod.csproj` with Visual Studio
2. Go to **Build** menu → **Build Solution** (or press **Ctrl+Shift+B**)
3. Wait for the build to complete
4. Find your DLL in `bin/Release/SukunaMod.dll`

---

## Step 5: Copy to Among Us

1. Make sure **BepInEx is installed** in your Among Us folder (see BepInEx installation guide)
2. Copy the built DLL:
   ```
   SukunaMod.dll → Among Us/BepInEx/plugins/
   ```

3. Launch Among Us and enjoy!

---

## Troubleshooting

### ❌ "Can't find Assembly-CSharp.dll"
- Make sure you copied it to the `lib/` folder
- Check the file path in `SukunaMod.csproj` points to the right location
- Update the `.csproj` if needed:
  ```xml
  <ItemGroup>
    <Reference Include="Assembly-CSharp">
      <HintPath>lib/Assembly-CSharp.dll</HintPath>
    </Reference>
  </ItemGroup>
  ```

### ❌ ".NET 6.0 not found"
- Install .NET 6.0 SDK: https://dotnet.microsoft.com/en-us/download/dotnet/6.0
- Restart Visual Studio if already open

### ❌ Build errors with Reactor
- Make sure your `.csproj` has the Reactor NuGet package
- Run: `dotnet add package Reactor.Networking`

### ❌ "BepInEx not found" when running
- Install BepInEx first to your Among Us folder
- Press **F5** in-game to see the BepInEx console and check for errors

---

## Customizing Before Build

Edit `SukunaMod.cs` to customize:

```csharp
// Change ability cooldowns
cleaveCooldown = Config.Bind("Abilities", "CleaveCooldown", 25f, ...);
slashCooldown = Config.Bind("Abilities", "SlashCooldown", 15f, ...);
domainCooldown = Config.Bind("Abilities", "DomainCooldown", 60f, ...);

// Change keybinds
transformKey = Config.Bind("Keybinds", "TransformKey", KeyCode.S, ...);
cleaveKey = Config.Bind("Keybinds", "CleaveKey", KeyCode.C, ...);
```

Then rebuild with `dotnet build -c Release`

---

## Need Help?

If the build fails:
1. Check the **Error List** in Visual Studio (or console output)
2. Common issues:
   - Missing `.dll` references → copy game files to `lib/`
   - Wrong .NET version → install .NET 6.0
   - NuGet packages not restored → run `dotnet restore`

Check [DEVELOPMENT.md](DEVELOPMENT.md) for more details!
