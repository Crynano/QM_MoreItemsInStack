using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

namespace QM_Template
{
    public class ModConfig
    {
        public short AmmoStackSize = 100;
        public short TrashStackSize = 100;
        public short GrenadeStackSize = 100;
        public short ConsumableStackSize = 100;
        public short RepairStackSize = 100;
        public short PlaceableStackSize = 100;
        public short FixationStackSize = 100;
        public short DeviceStackSize = 100;
        public static ModConfig LoadConfig(string configPath)
        {
            ModConfig config;

            JsonSerializerSettings serializerSettings = new JsonSerializerSettings()
            {
                Formatting = Formatting.Indented,
            };

            if (File.Exists(configPath))
            {
                try
                {
                    string sourceJson = File.ReadAllText(configPath);

                    config = JsonConvert.DeserializeObject<ModConfig>(sourceJson, serializerSettings);

                    //Add any new elements that have been added since the last mod version the user had.
                    string upgradeConfig = JsonConvert.SerializeObject(config, serializerSettings);

                    if (upgradeConfig != sourceJson)
                    {
                        Debug.Log("Updating config with missing elements");
                        Debug.Log(upgradeConfig);
                        //re-write
                        File.WriteAllText(configPath, upgradeConfig);
                    }


                    return config;
                }
                catch (Exception ex)
                {
                    Debug.LogError("Error parsing configuration.  Ignoring config file and using defaults");
                    Debug.LogException(ex);

                    //Not overwriting in case the user just made a typo.
                    config = new ModConfig();
                    return config;
                }
            }
            else
            {
                config = new ModConfig();

                string json = JsonConvert.SerializeObject(config, serializerSettings);
                File.WriteAllText(configPath, json);

                return config;
            }
        }
    }
}