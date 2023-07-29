using System.Collections.Generic;
using MonoMod.Utils;
using TowerFall;

namespace VariantsPickupMod
{
    public class VariantPickupManager
    {
        private static Dictionary<int, List<VariantPickup>> loseOnDeath = new Dictionary<int, List<VariantPickup>>();
        
        private static Dictionary<int, List<VariantPickup>> loseOnRoundEnd = new Dictionary<int, List<VariantPickup>>();

        private static List<VariantPickup> AllVariantPickup = new List<VariantPickup>();
        
        private static MatchVariants originalMatchVariants;
        private static Dictionary<string, Variant> originalCustomMatchVariants = new Dictionary<string, Variant>();
        public static Level level { get; set; }

        public static void BeginSessionVariantPickup(Level l)
        {
            level = l;
            originalMatchVariants = new MatchVariants();

            for (var index = 0; index < level.Session.MatchSettings.Variants.Variants.Length; index++)
            {
                var variantsVariant = level.Session.MatchSettings.Variants.Variants[index];
                originalMatchVariants.Variants[index].Value = variantsVariant.Value;
            }
            
            originalCustomMatchVariants.Clear();
            foreach (var customVariant in level.Session.MatchSettings.Variants.CustomVariants)
            {
                originalCustomMatchVariants.Add(customVariant.Key, customVariant.Value);
            }
        }

        public static void EndRoundVariantPickupRevert()
        {
            // if (!loseOnRoundEnd.TryGetValue(player.PlayerIndex, out var variantPickups)) return;
            foreach (var onRound in loseOnRoundEnd)
            {
                foreach (var variantPickup in onRound.Value)
                {
                    for (var index = 0; index < originalMatchVariants.Variants.Length; index++)
                    {
                        if (variantPickup.Variant.Title != originalMatchVariants.Variants[index].Title) continue;
                        level.Session.MatchSettings.Variants.Variants[index].Value
                            = originalMatchVariants.Variants[index].Value;
                    }
                }
            }
        }
        
        public static void OnDeathVariantPickupRevert(Player player)
        {
            if (!loseOnDeath.TryGetValue(player.PlayerIndex, out var variantPickups)) return;
            foreach (var onDeath in variantPickups)
            {
                for (var index = 0; index < originalMatchVariants.Variants.Length; index++)
                {
                    if (onDeath.Variant.Title != originalMatchVariants.Variants[index].Title) continue;
                    player.Level.Session.MatchSettings.Variants.Variants[index].Value
                        = originalMatchVariants.Variants[index].Value;
                        
                    RevertBigHead(player, onDeath.Variant);
                }
            }
        }
        
        public static void AddVariantPickup(int playerIndex, VariantPickup variantPickup)
        {
            if (variantPickup.IsLoseOnDeath)
            {
                if (!loseOnDeath.ContainsKey(playerIndex))
                {
                    loseOnDeath.Add(playerIndex, new List<VariantPickup>());
                }
                loseOnDeath[playerIndex].Add(variantPickup);
            }
            
            if (variantPickup.IsLoseOnEndRound)
            {
                if (!loseOnRoundEnd.ContainsKey(playerIndex))
                {
                    loseOnRoundEnd.Add(playerIndex, new List<VariantPickup>());
                }
                loseOnRoundEnd[playerIndex].Add(variantPickup);
            }

            AllVariantPickup.Add(variantPickup);
        }

        public static void CleanAll()
        {
            CleanAll(level);
        }

        public static void CleanAll(Level currentLevel)
        {
            RevertPickupVariants(AllVariantPickup, currentLevel);
            
            loseOnDeath.Clear();
            loseOnRoundEnd.Clear();
            AllVariantPickup.Clear();
        }

        public static void RevertPickupVariants(List<VariantPickup> variantPickups, Level level)
        {
            foreach (var onDeath in variantPickups)
            {
                for (var index = 0; index < originalMatchVariants.Variants.Length; index++)
                {
                    if (onDeath.Variant.Title != originalMatchVariants.Variants[index].Title) continue;
                    level.Session.MatchSettings.Variants.Variants[index].Value
                        = originalMatchVariants.Variants[index].Value;
                }
            }
        }
        
        public static void HandleBigHead(Player player, Variant variant)
        {
            if (variant.Title != "BIG HEADS") return;
            DynamicData.For(player).Set("baseHeadScale", 2f);
            var headYOrigins = (int[]) DynamicData.For(player).Get("headYOrigins");
            for (var i = 0; i < headYOrigins.Length; i++)
            {
                headYOrigins[i] -= 5;
            }

            DynamicData.For(player).Set("headYOrigins", headYOrigins);
        }
        
        public static void RevertBigHead(Player player, Variant variant)
        {
            if (variant.Title != "BIG HEADS") return;
            DynamicData.For(player).Set("baseHeadScale", 1f);
            var headYOrigins = (int[]) DynamicData.For(player).Get("headYOrigins");
            for (var i = 0; i < headYOrigins.Length; i++)
            {
                headYOrigins[i] += 5;
            }

            DynamicData.For(player).Set("headYOrigins", headYOrigins);
        }
    }
}