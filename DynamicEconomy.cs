using System.Collections.Generic;
using HarmonyLib;
using System.Reflection;
using System.Reflection.Emit;
#if (Il2Cpp)
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.ProductManagerApp;
using ScheduleOne.Product;
using ScheduleOne.UI.Handover;
using ScheduleOne.ItemFramework;
#elif (Mono)
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.ProductManagerApp;
using ScheduleOne.Product;
using ScheduleOne.UI.Handover;
using ScheduleOne.ItemFramework;
using ScheduleOne.Quests;
#endif
using System.Diagnostics;
using MelonLoader;
using DynamicEconomy;

namespace MyMod
{
    // Patch for Customer.ProcessHandover with prefix and postfix
    [HarmonyPatch(typeof(Customer), "ProcessHandover")]
    public static class ProcessHandoverPatch
    {
        // Prefix: runs before ProcessHandover.
        static bool Prefix(HandoverScreen.EHandoverOutcome outcome, Contract contract, List<ItemInstance> items, bool handoverByPlayer, bool giveBonuses)
        {
            MelonLogger.Msg("[MyMod] Prefix - ProcessHandover called. Outcome: " + outcome); // Use 'Log' from UnityEngine.Debug

            return true;
        }

        // Postfix: runs after ProcessHandover.
        static void Postfix(HandoverScreen.EHandoverOutcome outcome, Contract contract, List<ItemInstance> items, bool handoverByPlayer, bool giveBonuses)
        {
            MelonLogger.Msg("[MyMod] Postfix - ProcessHandover finished."); // Use 'Log' from UnityEngine.Debug
            // Go through items and for each item, add to DynamicEconomySaveData.Instance._ProductSaveData[item.ItemDefinition.Name]+=item.Quantity;
            foreach (var item in items)
            {
                var itemDef = item.Definition;
                int quantity = item.Quantity;
                if (itemDef is ProductDefinition productDefinition)
                {
                    string productName = productDefinition.ID;
                    // Check if the product name exists in the dictionary
                    if (DynamicEconomySaveData.Instance._ProductSaveData.ContainsKey(productName))
                    {
                        // Update the quantity
                        DynamicEconomySaveData.Instance._ProductSaveData[productName] += quantity;
                    }
                    else
                    {
                        // Add new entry to the dictionary
                        DynamicEconomySaveData.Instance._ProductSaveData[productName] = quantity;
                    }
                    var properties = productDefinition.Properties;
                    // Iterate through properties and add quantity to _AffinitySaveData or create it with quantity if it does not exist
                    for (int i=0;i< properties.Count; i++)
                    {
                        var property = properties[i];
                        string propertyName = property.Name;
                        // Check if the property name exists in the dictionary
                        if (DynamicEconomySaveData.Instance._AffinitySaveData.ContainsKey(propertyName))
                        {
                            // Update the quantity
                            DynamicEconomySaveData.Instance._AffinitySaveData[propertyName] += quantity;
                        }
                        else
                        {
                            // Add new entry to the dictionary
                            DynamicEconomySaveData.Instance._AffinitySaveData[propertyName] = quantity;
                        }
                    }
                }
                else
                {
                    continue;
                }
                if (itemDef is WeedDefinition)
                {
                    DynamicEconomySaveData.Instance._ProductTypeSaveData["weed"] += quantity;
                }
                else if (itemDef is MethDefinition)
                {
                    DynamicEconomySaveData.Instance._ProductTypeSaveData["meth"] += quantity;
                }
                else if (itemDef is CocaineDefinition)
                {
                    DynamicEconomySaveData.Instance._ProductTypeSaveData["cocaine"] += quantity;
                }
                else
                {
                    continue;
                }
                

            }

        }
    }

    // Patch for ProductAppDetailPanel.SetActiveProduct (from previous patch)
    [HarmonyPatch(typeof(ProductAppDetailPanel), "SetActiveProduct")]
    public static class SetActiveProductPatch
    {
        // Postfix: runs after SetActiveProduct.
        static void Postfix(ProductDefinition productDefinition, ProductAppDetailPanel __instance)
        {
            string productName = (productDefinition.Name != null) ? productDefinition.Name : "null";
            MelonLogger.Msg("[MyMod] Postfix - SetActiveProduct called with product: " + productName); // Use 'Log' from UnityEngine.Debug
        }
    }

    // Main mod entry point to apply all patches
    public  class MainMod : MelonMod
    {
        public override void OnInitializeMelon()
        {
            MelonLogger.Msg("DynamicEconomy-Mono initialized.");
            try
            {
                var harmony = new HarmonyLib.Harmony("com.Aracor.DynamicEconomy");
                harmony.PatchAll();
                MelonLogger.Msg("Harmony patches applied for CustomerProcessHandoverPatch.");
                new DynamicEconomySaveData();

            }
            catch (Exception ex)
            {
                MelonLogger.Error($"Failed to apply Harmony patches: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}