using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Oxide.Core.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Oxide.Plugins
{
    [Info("Virtual Items", "WhiteThunder", "0.1.1")]
    [Description("Removes resource costs of specific ingredients for crafting and building.")]
    internal class VirtualItems : CovalencePlugin
    {
        #region Fields

        [PluginReference]
        Plugin BGrade, CraftSpamBlocker, FreeBuild, ItemPuller, Guardian, NoCraft, NoEscape, TimedProgression, VerificationGatekeeper, ZoneManager;

        private static VirtualItems _pluginInstance;
        private static Configuration _pluginConfig;

        private const string PermissionRulesetFormat = "virtualitems.ruleset.{0}";

        private const int FirstInvisibleItemSlot = 24;

        private readonly Dictionary<string, Item> _items = new Dictionary<string, Item>();

        #endregion

        #region Hooks

        private void Init()
        {
            _pluginInstance = this;

            foreach (var ruleset in _pluginConfig.Rulesets)
            {
                var perm = ruleset.GetPermission();
                if (perm != null)
                    permission.RegisterPermission(perm, this);
            }
        }

        private void Unload()
        {
            foreach (var item in _items.Values)
                item.Remove();

            _pluginConfig = null;
            _pluginInstance = null;
        }

        private void OnServerInitialized()
        {
            CreateItems();

            foreach (var player in BasePlayer.activePlayerList)
                player.Invoke(
                    () => player.inventory.SendUpdatedInventory(PlayerInventory.Type.Main, player.inventory.containerMain),
                    UnityEngine.Random.Range(0f, 1f)
                );
        }

        private void OnEntitySaved(BasePlayer player, BaseNetworkable.SaveInfo saveInfo)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return;

            AddItems(saveInfo.msg.basePlayer.inventory.invMain, ruleset);
        }

        private void OnInventoryNetworkUpdate(PlayerInventory inventory, ItemContainer container, ProtoBuf.UpdateItemContainer updatedItemContainer, PlayerInventory.Type inventoryType)
        {
            if (inventoryType != PlayerInventory.Type.Main)
                return;

            var ruleset = GetPlayerRuleset(inventory.baseEntity.UserIDString);
            if (ruleset == null)
                return;

            AddItems(updatedItemContainer.container[0], ruleset);
        }

        // Placing construction blocks.
        private object CanAffordToPlace(BasePlayer player, Planner planner, Construction component)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return null;

            var args = new object[] { player, planner, component };

            object otherPluginResult;
            if (PluginReturnedResult(VerificationGatekeeper, nameof(CanAffordToPlace), out otherPluginResult, args)
                || PluginReturnedResult(FreeBuild, nameof(CanAffordToPlace), out otherPluginResult, args)
                || PluginReturnedResult(ItemPuller, nameof(OnPayForPlacement), out otherPluginResult, args))
                return otherPluginResult;

            if (!ruleset.PlayerHasAmountList(player, component.defaultGrade.costToBuild))
                return false;

            // Return true to allow placement, skipping vanilla validation.
            return true;
        }

        // Placing construction blocks or deployables.
        private object OnPayForPlacement(BasePlayer player, Planner planner, Construction component)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return null;

            var args = new object[] { player, planner, component };

            object otherPluginResult;
            if (PluginReturnedResult(VerificationGatekeeper, nameof(OnPayForPlacement), out otherPluginResult, args)
                || PluginReturnedResult(FreeBuild, nameof(OnPayForPlacement), out otherPluginResult, args))
                return otherPluginResult;

            if (planner.isTypeDeployable)
                return null;

            ruleset.TakePlayerItemList(player, component.defaultGrade.costToBuild);

            if (PluginReturnedResult(BGrade, nameof(OnPayForPlacement), out otherPluginResult, args))
                return otherPluginResult;

            // Return non-null to force the placement, skipping vanilla charges.
            return false;
        }

        // Upgrading construction blocks.
        private object CanAffordUpgrade(BasePlayer player, BuildingBlock block, BuildingGrade.Enum iGrade)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return null;

            var args = new object[] { player, block, iGrade };

            object otherPluginResult;
            if (PluginReturnedResult(VerificationGatekeeper, nameof(OnPayForPlacement), out otherPluginResult, args))
                return otherPluginResult;

            if (!ruleset.PlayerHasAmountList(player, block.GetGrade(iGrade).costToBuild))
                return false;

            // Return true to allow placement, skipping vanilla validation.
            return true;
        }

        // Upgrading construction blocks.
        private object OnPayForUpgrade(BasePlayer player, BuildingBlock block, ConstructionGrade grade)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return null;

            var args = new object[] { player, block, grade };

            object otherPluginResult;
            if (PluginReturnedResult(FreeBuild, nameof(OnPayForPlacement), out otherPluginResult, args))
                return otherPluginResult;

            ruleset.TakePlayerItemList(player, grade.costToBuild);

            // Return non-null to force the placement, skipping vanilla charges.
            return false;
        }

        private object OnIngredientsCollect(ItemCrafter itemCrafter, ItemBlueprint blueprint, ItemCraftTask task, int amount, BasePlayer player)
        {
            var ruleset = GetPlayerRuleset(player.UserIDString);
            if (ruleset == null)
                return null;

            var collect = new List<Item>();
            ruleset.TakeFromContainers(itemCrafter.containers, blueprint.ingredients, amount, collect);

            // Re-implementing some vanilla logic.
            task.potentialOwners = new List<ulong>();
            foreach (var item in collect)
            {
                item.CollectedForCrafting(player);
                if (!task.potentialOwners.Contains(player.userID))
                {
                    task.potentialOwners.Add(player.userID);
                }
            }
            task.takenItems = collect;

            // Return non-null to skip vanilla ingredient collection.
            return false;
        }

        private object CanCraft(ItemCrafter itemCrafter, ItemBlueprint blueprint, int craftAmount, bool free)
        {
            var ruleset = GetPlayerRuleset(itemCrafter.baseEntity.UserIDString);
            if (ruleset == null)
                return null;

            var args = new object[] { itemCrafter, blueprint, craftAmount, free };

            object otherPluginResult;
            if (PluginReturnedResult(CraftSpamBlocker, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(Guardian, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(ItemPuller, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(NoCraft, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(NoEscape, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(TimedProgression, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(VerificationGatekeeper, nameof(CanCraft), out otherPluginResult, args)
                || PluginReturnedResult(ZoneManager, nameof(CanCraft), out otherPluginResult, args))
                return otherPluginResult;

            foreach (var itemAmount in blueprint.ingredients)
            {
                var playerAmount = ruleset.GetFreeAmount(itemAmount);

                foreach (var container in itemCrafter.containers)
                    playerAmount += container.GetAmount(itemAmount.itemid, onlyUsableAmounts: true);

                if (playerAmount < itemAmount.amount * craftAmount)
                    return false;
            }

            // Return non-null to force craft, skipping vanilla validation.
            return true;
        }

        #endregion

        #region API

        private Dictionary<string, int> API_GetItemAmounts(string userId) =>
            GetPlayerRuleset(userId)?.ItemAmounts;

        #endregion

        #region Helper Methods

        private int Clamp(int x, int min, int max) => Math.Max(min, Math.Min(x, max));

        private bool PluginReturnedResult(Plugin plugin, string hookName, out object result, params object[] args)
        {
            result = VerificationGatekeeper?.Call("CanAffordToPlace", args);
            return result != null;
        }

        private void CreateItems()
        {
            foreach (var ruleset in _pluginConfig.Rulesets)
            {
                foreach (var shortName in ruleset.ItemAmounts.Keys)
                {
                    if (_items.ContainsKey(shortName))
                        continue;

                    var itemDef = ItemManager.FindItemDefinition(shortName);
                    if (itemDef == null)
                    {
                        LogWarning($"Invalid item in config: {shortName}");
                        continue;
                    }

                    var item = ItemManager.Create(itemDef);

                    var heldEntity = item.GetHeldEntity();
                    if (heldEntity != null)
                        heldEntity.EnableSaving(false);

                    _items[shortName] = item;
                }
            }
        }

        private bool TryGetItem(string shortName, int amount, out Item item)
        {
            if (!_items.TryGetValue(shortName, out item))
                return false;

            item.amount = amount;
            return true;
        }

        private int HighestUsedSlot(ProtoBuf.ItemContainer containerData)
        {
            var highestUsedSlot = -1;

            foreach (var item in containerData.contents)
            {
                if (item.slot > highestUsedSlot)
                    highestUsedSlot = item.slot;
            }

            return highestUsedSlot;
        }

        private void AddItems(ProtoBuf.ItemContainer containerData, Ruleset ruleset)
        {
            if (containerData == null)
                return;

            // Note: Problems can arise if there are existing items spread out unevenly across the invisible slots.
            var slot = Math.Max(FirstInvisibleItemSlot, HighestUsedSlot(containerData) + 1);

            foreach (var entry in ruleset.ItemAmounts)
            {
                Item item;
                if (!TryGetItem(entry.Key, entry.Value, out item))
                    continue;

                item.position = slot++;
                containerData.contents.Add(item.Save());
            }

            containerData.slots = FirstInvisibleItemSlot + ruleset.ItemAmounts.Count;
        }

        #endregion

        #region Configuration

        private Ruleset GetPlayerRuleset(string userIdString)
        {
            if (_pluginConfig.Rulesets == null || _pluginConfig.Rulesets.Length == 0)
                return null;

            for (var i = 0; i < _pluginConfig.Rulesets.Length; i++)
            {
                var ruleset = _pluginConfig.Rulesets[i];
                var perm = ruleset.GetPermission();
                if (perm != null && permission.UserHasPermission(userIdString, perm))
                    return ruleset;
            }

            return null;
        }

        private class Configuration : SerializableConfiguration
        {
            [JsonProperty("Rulesets")]
            public Ruleset[] Rulesets = new Ruleset[]
            {
                new Ruleset()
                {
                    Name = "build",
                    ItemAmounts =
                    {
                        ["wood"] = 100000,
                        ["stones"] = 100000,
                        ["metal.fragments"] = 100000,
                        ["metal.refined"] = 100000,
                    }
                },
                new Ruleset()
                {
                    Name = "craft_most_items",
                    ItemAmounts =
                    {
                        ["bone.fragments"] = 100000,
                        ["can.tuna.empty"] = 100000,
                        ["cloth"] = 100000,
                        ["electric.rf.broadcaster"] = 100000,
                        ["electric.rf.receiver"] = 100000,
                        ["fat.animal"] = 100000,
                        ["gears"] = 100000,
                        ["ladder.wooden.wall"] = 100000,
                        ["leather"] = 100000,
                        ["lowgradefuel"] = 100000,
                        ["metal.fragments"] = 100000,
                        ["metal.refined"] = 100000,
                        ["metalblade"] = 100000,
                        ["metalpipe"] = 100000,
                        ["metalspring"] = 100000,
                        ["propanetank"] = 100000,
                        ["pumpkin"] = 100000,
                        ["riflebody"] = 100000,
                        ["roadsigns"] = 100000,
                        ["rope"] = 100000,
                        ["semibody"] = 100000,
                        ["sewingkit"] = 100000,
                        ["sheetmetal"] = 100000,
                        ["skull.wolf"] = 100000,
                        ["skull.human"] = 100000,
                        ["smgbody"] = 100000,
                        ["spear.wooden"] = 100000,
                        ["syringe.medical"] = 100000,
                        ["stash.small"] = 100000,
                        ["stones"] = 100000,
                        ["targeting.computer"] = 100000,
                        ["tarp"] = 100000,
                        ["wood"] = 100000,
                    }
                },
                new Ruleset()
                {
                    Name = "craft_all_items",
                    ItemAmounts =
                    {
                        ["bone.fragments"] = 100000,
                        ["can.tuna.empty"] = 100000,
                        ["cctv.camera"] = 100000,
                        ["charcoal"] = 100000,
                        ["cloth"] = 100000,
                        ["electric.rf.broadcaster"] = 100000,
                        ["electric.rf.receiver"] = 100000,
                        ["explosives"] = 100000,
                        ["fat.animal"] = 100000,
                        ["gears"] = 100000,
                        ["grenade.beancan"] = 100000,
                        ["gunpowder"] = 100000,
                        ["ladder.wooden.wall"] = 100000,
                        ["leather"] = 100000,
                        ["lowgradefuel"] = 100000,
                        ["metal.fragments"] = 100000,
                        ["metal.refined"] = 100000,
                        ["metalblade"] = 100000,
                        ["metalpipe"] = 100000,
                        ["metalspring"] = 100000,
                        ["propanetank"] = 100000,
                        ["pumpkin"] = 100000,
                        ["riflebody"] = 100000,
                        ["roadsigns"] = 100000,
                        ["rope"] = 100000,
                        ["semibody"] = 100000,
                        ["sewingkit"] = 100000,
                        ["sheetmetal"] = 100000,
                        ["skull.wolf"] = 100000,
                        ["skull.human"] = 100000,
                        ["smgbody"] = 100000,
                        ["spear.wooden"] = 100000,
                        ["syringe.medical"] = 100000,
                        ["stash.small"] = 100000,
                        ["stones"] = 100000,
                        ["sulfur"] = 100000,
                        ["scrap"] = 100000,
                        ["targeting.computer"] = 100000,
                        ["tarp"] = 100000,
                        ["techparts"] = 100000,
                        ["wood"] = 100000,
                    }
                }
            };
        }

        private class Ruleset
        {
            [JsonProperty("Name")]
            public string Name;

            [JsonProperty("ItemAmounts")]
            public Dictionary<string, int> ItemAmounts = new Dictionary<string, int>();

            private string _permissionName;
            public string GetPermission()
            {
                if (_permissionName == null && !string.IsNullOrWhiteSpace(Name))
                    _permissionName = string.Format(PermissionRulesetFormat, Name);

                return _permissionName;
            }

            public int GetFreeAmount(string shortname)
            {
                int freeAmount;
                return ItemAmounts.TryGetValue(shortname, out freeAmount)
                    ? freeAmount
                    : 0;
            }

            public int GetFreeAmount(ItemDefinition itemDefinition) =>
                GetFreeAmount(itemDefinition.shortname);

            public int GetFreeAmount(ItemAmount itemAmount) =>
                GetFreeAmount(itemAmount.itemDef);

            public int GetChargeAmount(ItemAmount itemAmount, int quantity = 1) =>
                (int)itemAmount.amount * quantity - GetFreeAmount(itemAmount);

            public bool PlayerHasAmount(BasePlayer player, ItemDefinition itemDefinition, int requiredAmount)
            {
                var freeAmount = GetFreeAmount(itemDefinition);
                if (freeAmount >= requiredAmount)
                    return true;

                return player.inventory.GetAmount(itemDefinition.itemid) + freeAmount >= requiredAmount;
            }

            public bool PlayerHasAmountList(BasePlayer player, IEnumerable<ItemAmount> itemAmountList, float costFraction = 1)
            {
                foreach (var itemAmount in itemAmountList)
                {
                    if (!PlayerHasAmount(player, itemAmount.itemDef, Mathf.CeilToInt(itemAmount.amount * costFraction)))
                        return false;
                }
                return true;
            }

            public void TakePlayerItem(List<Item> collect, BasePlayer player, ItemAmount itemAmount)
            {
                var chargeAmount = GetChargeAmount(itemAmount);
                if (chargeAmount <= 0)
                    return;

                player.inventory.Take(collect, itemAmount.itemDef.itemid, chargeAmount);
                player.Command("note.inv", itemAmount.itemDef.itemid, chargeAmount * -1);
            }

            public void TakePlayerItemList(BasePlayer player, IEnumerable<ItemAmount> itemAmountList)
            {
                var collect = Facepunch.Pool.GetList<Item>();

                foreach (var itemAmount in itemAmountList)
                    TakePlayerItem(collect, player, itemAmount);

                foreach (var item in collect)
                    item.Remove();

                Facepunch.Pool.FreeList(ref collect);
            }

            public void TakeFromContainers(IEnumerable<ItemContainer> containerList, IEnumerable<ItemAmount> itemAmountList, int craftAmount, List<Item> collect)
            {
                foreach (var itemAmount in itemAmountList)
                {
                    var chargeAmount = GetChargeAmount(itemAmount, craftAmount);
                    if (chargeAmount <= 0)
                        continue;

                    foreach (var container in containerList)
                    {
                        chargeAmount -= container.Take(collect, itemAmount.itemid, chargeAmount);
                        if (chargeAmount <= 0)
                            break;
                    }
                }
            }
        }

        private Configuration GetDefaultConfig() => new Configuration();

        #endregion

        #region Configuration Boilerplate

        private class SerializableConfiguration
        {
            public string ToJson() => JsonConvert.SerializeObject(this);

            public Dictionary<string, object> ToDictionary() => JsonHelper.Deserialize(ToJson()) as Dictionary<string, object>;
        }

        private static class JsonHelper
        {
            public static object Deserialize(string json) => ToObject(JToken.Parse(json));

            private static object ToObject(JToken token)
            {
                switch (token.Type)
                {
                    case JTokenType.Object:
                        return token.Children<JProperty>()
                                    .ToDictionary(prop => prop.Name,
                                                  prop => ToObject(prop.Value));

                    case JTokenType.Array:
                        return token.Select(ToObject).ToList();

                    default:
                        return ((JValue)token).Value;
                }
            }
        }

        private bool MaybeUpdateConfig(SerializableConfiguration config)
        {
            var currentWithDefaults = config.ToDictionary();
            var currentRaw = Config.ToDictionary(x => x.Key, x => x.Value);
            return MaybeUpdateConfigDict(currentWithDefaults, currentRaw);
        }

        private bool MaybeUpdateConfigDict(Dictionary<string, object> currentWithDefaults, Dictionary<string, object> currentRaw)
        {
            bool changed = false;

            foreach (var key in currentWithDefaults.Keys)
            {
                object currentRawValue;
                if (currentRaw.TryGetValue(key, out currentRawValue))
                {
                    var defaultDictValue = currentWithDefaults[key] as Dictionary<string, object>;
                    var currentDictValue = currentRawValue as Dictionary<string, object>;

                    if (defaultDictValue != null)
                    {
                        if (currentDictValue == null)
                        {
                            currentRaw[key] = currentWithDefaults[key];
                            changed = true;
                        }
                        else if (MaybeUpdateConfigDict(defaultDictValue, currentDictValue))
                            changed = true;
                    }
                }
                else
                {
                    currentRaw[key] = currentWithDefaults[key];
                    changed = true;
                }
            }

            return changed;
        }

        protected override void LoadDefaultConfig() => _pluginConfig = GetDefaultConfig();

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _pluginConfig = Config.ReadObject<Configuration>();
                if (_pluginConfig == null)
                {
                    throw new JsonException();
                }

                if (MaybeUpdateConfig(_pluginConfig))
                {
                    LogWarning("Configuration appears to be outdated; updating and saving");
                    SaveConfig();
                }
            }
            catch
            {
                LogWarning($"Configuration file {Name}.json is invalid; using defaults");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig()
        {
            Log($"Configuration changes saved to {Name}.json");
            Config.WriteObject(_pluginConfig, true);
        }

        #endregion
    }
}
