using MGSC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace MoreItemsInStack
{
    public class ModConfig
    {
        public int DefaultStackSize { get; set; }
        public int AmmoStackSize { get; set; }
        public int TrashStackSize { get; set; }
        public int GrenadeStackSize { get; set; }
        public int ConsumableStackSize { get; set; }
        public int RepairStackSize { get; set; }
        public int PlaceableStackSize { get; set; }
        public int FixationStackSize { get; set; }
        public int DeviceStackSize { get; set; }

        [JsonIgnore]
        public Dictionary<string, int> OriginalSettings { get; set; }

        public ModConfig()
        {
            
        }

        public ModConfig(int defaultStackSize, int ammoStackSize, int trashStackSize, int grenadeStackSize, int consumableStackSize, int repairStackSize, int placeableStackSize, int fixationStackSize, int deviceStackSize)
        {
            DefaultStackSize = defaultStackSize;
            AmmoStackSize = ammoStackSize;
            TrashStackSize = trashStackSize;
            GrenadeStackSize = grenadeStackSize;
            ConsumableStackSize = consumableStackSize;
            RepairStackSize = repairStackSize;
            PlaceableStackSize = placeableStackSize;
            FixationStackSize = fixationStackSize;
            DeviceStackSize = deviceStackSize;
        }

        public void LoadConfigIni(string configPath)
        {
#if DEBUG
            Debug.Log($"Loading Stack Config from \"{configPath}\"");
#endif
            if (File.Exists(configPath))
            {
                var sourceLines = File.ReadAllLines(configPath);

                foreach (var line in sourceLines)
                {
                    string trimmedLine = line.Trim();

                    if (trimmedLine.Contains('='))
                    {
                        // Key-value pair
                        string[] keyValue = trimmedLine.Split(new[] { '=' }, 2);
                        string key = keyValue[0].Trim();
                        string value = keyValue[1].Trim();
                        var convertedValue = ConvertValue(value);
                        PropertyInfo propertyInfo = this.GetType().GetProperty(key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                        propertyInfo?.SetValue(this, convertedValue, null);
#if DEBUG
                        //Debug.Log($"Tried to set property of {propertyInfo?.Name} as {propertyInfo?.PropertyType} against {key} with value {convertedValue} of type {convertedValue.GetType()}");
#endif
                    }
                }
            }
        }

        public ModConfig LoadConfigJson(string configPath)
        {
            return File.Exists(configPath) ? JsonConvert.DeserializeObject<ModConfig>(File.ReadAllText(configPath)) : this;
        }

        public void SaveConfigJson(string configPath)
        {
            var data = JsonConvert.SerializeObject(this, Formatting.Indented);
            File.WriteAllText(configPath, data);
        }

        public void LoadConfig(Dictionary<string, object> propertiesDictionary)
        {
            foreach (var nameValuePair in propertiesDictionary)
            {
                PropertyInfo propertyInfo = this.GetType().GetProperty(nameValuePair.Key, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
                propertyInfo?.SetValue(this, Convert.ChangeType(nameValuePair.Value, propertyInfo.PropertyType), null);
            }
        }

        private object ConvertValue(string value)
        {
            if (int.TryParse(value, out int intValue))
            {
                return intValue;
            }

            if (bool.TryParse(value, out bool boolValue))
            {
                return boolValue;
            }

            if (ColorUtility.TryParseHtmlString(value.Replace("\"", string.Empty), out Color colorParsed))
            {
                return colorParsed;
            }

            return value;
        }
    }
}