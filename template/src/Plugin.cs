using System.IO;
using System.Reflection;
using HarmonyLib;
using MGSC;
using UnityEngine;

namespace QM_Template
{
    public static class Plugin
    {
        public static string ModAssemblyName => Assembly.GetExecutingAssembly().GetName().Name;
        public static string ConfigPath => Path.Combine(Application.persistentDataPath, ModAssemblyName, "config.json");
        public static string ModPersistenceFolder => Path.Combine(Application.persistentDataPath, ModAssemblyName);
        public static ModConfig Config { get; private set; }

        [Hook(ModHookType.AfterConfigsLoaded)]
        public static void AfterConfig(IModContext context)
        {
            Directory.CreateDirectory(ModPersistenceFolder);
            Config = ModConfig.LoadConfig(ConfigPath);
            new Harmony("$UserName$_" + ModAssemblyName).PatchAll();

            foreach (BasePickupItemRecord record in Data.Items.Records)
            {
                AmmoRecord ammoRecord = (record as CompositeItemRecord)?.GetRecord<AmmoRecord>();
                TrashRecord trashRecord = (record as CompositeItemRecord)?.GetRecord<TrashRecord>();
                ConsumableRecord consumableRecord = (record as CompositeItemRecord)?.GetRecord<ConsumableRecord>();
                PlaceableDeviceRecord PlaceableDeviceRecord = (record as CompositeItemRecord)?.GetRecord<PlaceableDeviceRecord>();
                FixationMedicineRecord FixationMedicineRecord = (record as CompositeItemRecord)?.GetRecord<FixationMedicineRecord>();
                DeviceRecord DeviceRecord = (record as CompositeItemRecord)?.GetRecord<DeviceRecord>();
                GrenadeRecord grenadeRecord = (record as CompositeItemRecord)?.GetRecord<GrenadeRecord>();
                RepairRecord repairRecord = (record as CompositeItemRecord)?.GetRecord<RepairRecord>();
                
                if (ammoRecord != null) ammoRecord.MaxStack = Config.AmmoStackSize;
                if (trashRecord != null) trashRecord.MaxStack = Config.TrashStackSize;
                if (consumableRecord != null) consumableRecord.MaxStack = Config.ConsumableStackSize;
                if (PlaceableDeviceRecord != null) PlaceableDeviceRecord.MaxStack = Config.PlaceableStackSize;
                if (FixationMedicineRecord != null) FixationMedicineRecord.MaxStack = Config.FixationStackSize;
                if (DeviceRecord != null) DeviceRecord.MaxStack = Config.DeviceStackSize;
                if (grenadeRecord != null) grenadeRecord.MaxStack = Config.GrenadeStackSize;
                if (repairRecord != null) repairRecord.MaxStack = Config.RepairStackSize;
            }
        }
    }
}