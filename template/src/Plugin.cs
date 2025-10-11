using MGSC;
using ModConfigMenu;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using ModConfigMenu.Objects;
using UnityEngine;
using ModConfigMenu.Contracts;
using ModConfigMenu.Implementations;

namespace MoreItemsInStack
{
    public static class Plugin
    {
        private static string ModAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;

        private static string ModPersistenceFolder =>
            Path.Combine($"{Application.persistentDataPath}/../Quasimorph_ModConfigs", ModAssemblyName);

        private static string ConfigPath => Path.Combine(ModPersistenceFolder, "config.txt");
        private static ModConfig Config { get; set; }

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            AddLocalization();

            Directory.CreateDirectory(ModPersistenceFolder);

            // Create config for MCM
            Config = new ModConfig(-1, -1, -1, -1, -1, -1, -1, -1, -1);
            Config = Config.LoadConfigJson(ConfigPath);

            List<IConfigValue> modConfigs = new List<IConfigValue>
            {
                new StringConfig("Info1", "css.modconfig.info.info1", "Info"),
                new RangeConfig<int>(key: "DefaultStackSize", value: Config.DefaultStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Default Stack Size", tooltip: "Applied to items not under any category",
                    header: "Settings"),
                new RangeConfig<int>(key: "AmmoStackSize", value: Config.AmmoStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Ammo Stack Size", tooltip: "Affects all ammo types.", header: "Settings"),
                new RangeConfig<int>(key: "TrashStackSize", value: Config.TrashStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Trash Stack Size", tooltip: "Affects trash items such as plastic and wire.",
                    header: "Settings"),
                new RangeConfig<int>(key: "GrenadeStackSize", value: Config.GrenadeStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Explosives Stack Size", tooltip: "Affects grenades and other throwable items.",
                    header: "Settings"),
                new RangeConfig<int>(key: "ConsumableStackSize", value: Config.ConsumableStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Consumable Stack Size",
                    tooltip: "Affects healing items, food, and items that can be consumed.", header: "Settings"),
                new RangeConfig<int>(key: "RepairStackSize", value: Config.RepairStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Repair Items Stack Size", tooltip: "Affects scrap items used to repair other items.",
                    header: "Settings"),
                new RangeConfig<int>(key: "PlaceableStackSize", value: Config.PlaceableStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Placeable Stack Size", tooltip: "Affects turrets, mines and other placeable devices.",
                    header: "Settings"),
                new RangeConfig<int>(
                    key: "FixationStackSize",
                    value: Config.FixationStackSize,
                    defaultValue: -1,
                    min: -1, max: 1000,
                    label: "Fixation Stack Size",
                    tooltip: "Affects medicine that fixes wounds.",
                    header: "Settings"),
                new RangeConfig<int>(key: "DeviceStackSize", value: Config.DeviceStackSize, defaultValue: -1, min: -1, max: 1000,
                    label: "Devices Stack Size", tooltip: "Affects devices and other misc items.", header: "Settings"),
                new StringConfig(key: "About1", value: "Original Mod by <color=#ffffff>Konich</color>", header: "About"),
                new StringConfig(key: "About2", value: "Updated and mantained by <color=#ffff00>Crynano</color>", header: "About")
            };

            ModConfigMenuAPI.RegisterModConfig("Configurable Stack Sizes", modConfigs,
            (Dictionary<string, object> config, out string message) =>
            {
                try
                {
                    message = "All good";
                    Config.LoadConfig(config);
                    Config.SaveConfigJson(ConfigPath);
                    return true;
                }
                catch (Exception ex)
                {
                    message = ex.Message;
                    return false;
                }
            });
        }

        [Hook(ModHookType.BeforeSaveLoaded)]
        public static void LoadConfiguration()
        {
            // Apply configuration every time the user loads the game. This way, the game will have the default values when opening.
            // This way, we can store the values of default items in config? Or it will not be applied with -1 so it wont matter.
            ApplyConfig();
        }

        private static void ApplyConfig()
        {
            var now = DateTime.Now;
            foreach (BasePickupItemRecord record in Data.Items.Records)
            {
                if (!(record is CompositeItemRecord compositeItemRecord))
                {
                    continue;
                }

                AmmoRecord ammo = compositeItemRecord.GetRecord<AmmoRecord>();
                if (ammo != null && Config.AmmoStackSize > 0)
                {
                    ammo.MaxStack = (short)Config.AmmoStackSize;
                }

                TrashRecord trash = compositeItemRecord.GetRecord<TrashRecord>();
                if (trash != null && Config.TrashStackSize > 0)
                {
                    trash.MaxStack = (short)Config.TrashStackSize;
                }

                ConsumableRecord consumable = compositeItemRecord.GetRecord<ConsumableRecord>();
                if (consumable != null && Config.ConsumableStackSize > 0)
                {
                    consumable.MaxStack = (short)Config.ConsumableStackSize;
                }

                PlaceableDeviceRecord placeable = compositeItemRecord.GetRecord<PlaceableDeviceRecord>();
                if (placeable != null && Config.PlaceableStackSize > 0)
                {
                    placeable.MaxStack = (short)Config.PlaceableStackSize;
                }

                FixationMedicineRecord fixation = compositeItemRecord.GetRecord<FixationMedicineRecord>();
                if (fixation != null && Config.FixationStackSize > 0)
                {
                    fixation.MaxStack = (short)Config.FixationStackSize;
                }

                DeviceRecord device = compositeItemRecord.GetRecord<DeviceRecord>();
                if (device != null && Config.DeviceStackSize > 0)
                {
                    device.MaxStack = (short)Config.DeviceStackSize;
                }

                GrenadeRecord grenade = compositeItemRecord.GetRecord<GrenadeRecord>();
                if (grenade != null && Config.GrenadeStackSize > 0)
                {
                    grenade.MaxStack = (short)Config.GrenadeStackSize;
                }

                RepairRecord repair = compositeItemRecord.GetRecord<RepairRecord>();
                if (repair != null && Config.RepairStackSize > 0)
                {
                    repair.MaxStack = (short)Config.RepairStackSize;
                }
            }

            Debug.Log($"All stack sizes modified. Process took {(DateTime.Now - now).TotalSeconds:0.000} seconds.");
        }

        // private static string Base64Decode(string base64EncodedData)
        // {
        //     var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
        //     return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        // }

        private static void AddLocalization()
        {
            string key = "css.modconfig.info.info1";
            foreach (var languageDic in MGSC.Localization.Instance.db)
            {
                languageDic.Value[key] =
                    "To avoid a stack size from getting changed, set its value to <color=#50ff50>-1</color> or <color=#50ff50>0</color>.";
            }

            MGSC.Localization.Instance.db[Localization.Lang.Spanish][key] =
                "Para evitar que el tamaño del stack cambie, pon el valor a -1 o 0.";
        }
    }
}