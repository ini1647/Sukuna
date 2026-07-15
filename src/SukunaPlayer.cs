using System;
using System.Collections.Generic;
using UnityEngine;

namespace SukunaMod
{
    public class SukunaPlayer
    {
        public byte PlayerId { get; private set; }
        public bool IsSukuna { get; private set; }
        public bool InDomain { get; private set; }
        public float DomainTimeRemaining { get; private set; }

        private Dictionary<string, float> abilityCooldowns;
        private Dictionary<string, float> abilityCooldownMax;

        public SukunaPlayer(byte playerId, float cleaveCd, float slashCd, float domainCd)
        {
            PlayerId = playerId;
            IsSukuna = false;
            InDomain = false;
            DomainTimeRemaining = 0f;

            abilityCooldowns = new Dictionary<string, float>
            {
                { "Cleave", 0f },
                { "Slash", 0f },
                { "Domain", 0f }
            };

            abilityCooldownMax = new Dictionary<string, float>
            {
                { "Cleave", cleaveCd },
                { "Slash", slashCd },
                { "Domain", domainCd }
            };
        }

        public void ToggleTransformation()
        {
            IsSukuna = !IsSukuna;
        }

        public void UpdateCooldowns(float deltaTime)
        {
            // Update ability cooldowns
            foreach (var ability in new List<string> { "Cleave", "Slash", "Domain" })
            {
                if (abilityCooldowns[ability] > 0)
                {
                    float cooldownReduction = InDomain ? deltaTime * 2f : deltaTime; // 2x reduction in domain
                    abilityCooldowns[ability] -= cooldownReduction;
                    if (abilityCooldowns[ability] < 0)
                        abilityCooldowns[ability] = 0f;
                }
            }

            // Update domain timer
            if (InDomain)
            {
                DomainTimeRemaining -= deltaTime;
                if (DomainTimeRemaining <= 0)
                {
                    InDomain = false;
                    DomainTimeRemaining = 0f;
                }
            }
        }

        public bool CanUseAbility(string abilityName)
        {
            if (!abilityCooldowns.ContainsKey(abilityName))
                return false;

            return abilityCooldowns[abilityName] <= 0f;
        }

        public void StartAbilityCooldown(string abilityName)
        {
            if (abilityCooldowns.ContainsKey(abilityName))
            {
                abilityCooldowns[abilityName] = abilityCooldownMax[abilityName];
            }
        }

        public float GetAbilityCooldown(string abilityName)
        {
            if (abilityCooldowns.ContainsKey(abilityName))
                return abilityCooldowns[abilityName];
            return 0f;
        }

        public void ActivateDomain(float duration, float radius)
        {
            InDomain = true;
            DomainTimeRemaining = duration;
        }

        public Dictionary<string, float> GetAllCooldowns()
        {
            return new Dictionary<string, float>(abilityCooldowns);
        }
    }
}
