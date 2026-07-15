using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using AmongUs.GameOptions;

namespace SukunaMod
{
    [BepInPlugin("com.ini1647.sukuna", "Sukuna Mod", "1.0.0")]
    [BepInDependency("me.eisbison.reactor", "0.0.0")]
    public class SukunaMod : BaseUnityPlugin
    {
        public static SukunaMod Instance { get; private set; }
        public new static ManualLogSource Logger { get; private set; }
        public Harmony Harmony { get; private set; }

        // Configuration
        private ConfigEntry<KeyCode> transformKey;
        private ConfigEntry<KeyCode> cleaveKey;
        private ConfigEntry<KeyCode> slashKey;
        private ConfigEntry<KeyCode> domainKey;
        private ConfigEntry<float> cleaveRadius;
        private ConfigEntry<float> cleaveCooldown;
        private ConfigEntry<float> slashCooldown;
        private ConfigEntry<float> domainDuration;
        private ConfigEntry<float> domainRadius;
        private ConfigEntry<float> domainCooldown;

        // Runtime
        private Dictionary<byte, SukunaPlayer> sukunaPlayers = new Dictionary<byte, SukunaPlayer>();
        private float lastAbilityCheckTime = 0f;

        private void Awake()
        {
            Instance = this;
            Logger = base.Logger;

            Logger.LogInfo("Sukuna Mod loaded!");

            // Load Configuration
            LoadConfig();

            // Apply Harmony patches
            Harmony = new Harmony("com.ini1647.sukuna");
            Harmony.PatchAll();

            Logger.LogInfo("Sukuna Mod initialized successfully!");
        }

        private void LoadConfig()
        {
            // Keybinds
            transformKey = Config.Bind("Keybinds", "TransformKey", KeyCode.S, "Key to toggle Sukuna transformation");
            cleaveKey = Config.Bind("Keybinds", "CleaveKey", KeyCode.C, "Key for Cleave ability");
            slashKey = Config.Bind("Keybinds", "SlashKey", KeyCode.V, "Key for Slash ability");
            domainKey = Config.Bind("Keybinds", "DomainKey", KeyCode.X, "Key for Domain Expansion ability");

            // Ability Settings
            cleaveRadius = Config.Bind("Abilities", "CleaveRadius", 3.0f, "Cleave damage radius in tiles");
            cleaveCooldown = Config.Bind("Abilities", "CleaveCooldown", 25f, "Cleave cooldown in seconds");
            slashCooldown = Config.Bind("Abilities", "SlashCooldown", 15f, "Slash cooldown in seconds");
            domainDuration = Config.Bind("Abilities", "DomainDuration", 10f, "Domain Expansion duration in seconds");
            domainRadius = Config.Bind("Abilities", "DomainRadius", 5.0f, "Domain Expansion radius in tiles");
            domainCooldown = Config.Bind("Abilities", "DomainCooldown", 60f, "Domain Expansion cooldown in seconds");
        }

        private void Update()
        {
            if (!GameData.Instance || !PlayerControl.LocalPlayer || PlayerControl.LocalPlayer.Data.IsDead)
                return;

            // Only host can use Sukuna powers
            if (!AmongUsClient.Instance.IsGameHost)
                return;

            // Must be Impostor
            if (!PlayerControl.LocalPlayer.Data.Role.IsImpostor)
                return;

            byte playerId = PlayerControl.LocalPlayer.PlayerId;

            // Initialize player if not exists
            if (!sukunaPlayers.ContainsKey(playerId))
                sukunaPlayers[playerId] = new SukunaPlayer(playerId, cleaveCooldown.Value, slashCooldown.Value, domainCooldown.Value);

            SukunaPlayer sukuna = sukunaPlayers[playerId];

            // Update cooldowns
            sukuna.UpdateCooldowns(Time.deltaTime);

            // Handle input
            if (Input.GetKeyDown(transformKey.Value))
            {
                sukuna.ToggleTransformation();
                SukunaRPC.SendTransformRPC(playerId, sukuna.IsSukuna);
                Logger.LogInfo($"Sukuna Transformation: {sukuna.IsSukuna}");
            }

            if (!sukuna.IsSukuna)
                return;

            // Ability inputs
            if (Input.GetKeyDown(cleaveKey.Value) && sukuna.CanUseAbility("Cleave"))
            {
                ExecuteCleave(sukuna);
            }

            if (Input.GetKeyDown(slashKey.Value) && sukuna.CanUseAbility("Slash"))
            {
                ExecuteSlash(sukuna);
            }

            if (Input.GetKeyDown(domainKey.Value) && sukuna.CanUseAbility("Domain"))
            {
                ExecuteDomain(sukuna);
            }
        }

        private void ExecuteCleave(SukunaPlayer sukuna)
        {
            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
            float radius = cleaveRadius.Value;

            sukuna.StartAbilityCooldown("Cleave");

            // Find all players within radius
            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer || player.Data.IsDead)
                    continue;

                float distance = Vector3.Distance(player.transform.position, playerPos);
                if (distance <= radius)
                {
                    // Kill the player
                    player.Exiled();
                    Logger.LogInfo($"Cleave hit {player.name}!");
                }
            }

            SukunaRPC.SendCleaveRPC(PlayerControl.LocalPlayer.PlayerId, playerPos, radius);
        }

        private void ExecuteSlash(SukunaPlayer sukuna)
        {
            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;

            // Find closest player within range
            PlayerControl closestPlayer = null;
            float closestDistance = 2.5f; // Attack range

            foreach (PlayerControl player in PlayerControl.AllPlayerControls)
            {
                if (player == PlayerControl.LocalPlayer || player.Data.IsDead)
                    continue;

                float distance = Vector3.Distance(player.transform.position, playerPos);
                if (distance < closestDistance)
                {
                    closestPlayer = player;
                    closestDistance = distance;
                }
            }

            if (closestPlayer != null)
            {
                sukuna.StartAbilityCooldown("Slash");
                closestPlayer.Exiled();
                SukunaRPC.SendSlashRPC(PlayerControl.LocalPlayer.PlayerId, closestPlayer.PlayerId);
                Logger.LogInfo($"Slash hit {closestPlayer.name}!");
            }
        }

        private void ExecuteDomain(SukunaPlayer sukuna)
        {
            sukuna.StartAbilityCooldown("Domain");
            sukuna.ActivateDomain(domainDuration.Value, domainRadius.Value);

            Vector3 playerPos = PlayerControl.LocalPlayer.transform.position;
            SukunaRPC.SendDomainRPC(PlayerControl.LocalPlayer.PlayerId, playerPos, domainRadius.Value, domainDuration.Value);

            Logger.LogInfo($"Domain Expansion activated!");
        }

        public SukunaPlayer GetSukunaPlayer(byte playerId)
        {
            if (sukunaPlayers.ContainsKey(playerId))
                return sukunaPlayers[playerId];
            return null;
        }
    }
}
