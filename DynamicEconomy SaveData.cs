using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using MelonLoader.Utils;
using MelonLoader;
using Newtonsoft.Json;
using S1API.Entities;
using S1API.Internal.Utils;
using S1API.Saveables;
#if (Il2Cpp)
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.ProductManagerApp;
using ScheduleOne.Product;
using ScheduleOne.UI.Handover;
using ScheduleOne.ItemFramework;
using UnityEngine;
#elif (Mono)
using ScheduleOne.Economy;
using ScheduleOne.UI.Phone.ProductManagerApp;
using ScheduleOne.Product;
using ScheduleOne.UI.Handover;
using ScheduleOne.ItemFramework;
using ScheduleOne.Quests;
using UnityEngine;
#endif

namespace DynamicEconomy
{
    public class DynamicEconomySaveData : NPC
    {
        //Switch to MelonPrefs
        public float productMultiple = 0.01f;
        public float productTypeMultiple = 0.005f;
        public float affinityMultiple = 0.001f;
        public float productMultipleMax = 0.1f;
        public float productTypeMultipleMax = 0.05f;
        public float affinityMultipleMax = 0.01f;

        public bool IsInitialised = false;
        public static DynamicEconomySaveData Instance { get;  set; }

        [SaveableField("ProductSaveData")]
        public Dictionary<string,int> _ProductSaveData = new Dictionary<string, int>();
        [SaveableField("ProductTypeSaveData")]
        public Dictionary<string, int> _ProductTypeSaveData = new Dictionary<string, int>();
        [SaveableField("AffinitySaveData")]
        public Dictionary<string, int> _AffinitySaveData = new Dictionary<string, int>();
        public DynamicEconomySaveData() : base(
            "DynamicEconomy SaveData",
            "DynamicEconomy",
            "SaveData")
        {
            
            MelonLogger.Msg($"✅ DynamicEconomy NPC updated");

        }
        protected override void OnLoaded()
        {
            MelonLoader.MelonLogger.Msg($"✅ DynamicEconomy NPC loaded");
            base.OnLoaded();
            IsInitialised = true;
            Instance = this;
            // If _ProductTypeSaveData is empty, add "weed", "meth", "cocaine" as 0
            if (_ProductTypeSaveData.Count == 0)
            {
                
                _ProductTypeSaveData.Add("weed", 0);
                _ProductTypeSaveData.Add("meth", 0);
                _ProductTypeSaveData.Add("cocaine", 0);
            }
        }
        protected override void OnCreated()
        {
            MelonLoader.MelonLogger.Msg($"✅ DynamicEconomy NPC created");
            base.OnCreated();
            IsInitialised = true;
            Instance = this;
            // If _ProductTypeSaveData is empty, add "weed", "meth", "cocaine" as 0
            if (_ProductTypeSaveData.Count == 0)
            {
                _ProductTypeSaveData.Add("weed", 0);
                _ProductTypeSaveData.Add("meth", 0);
                _ProductTypeSaveData.Add("cocaine", 0);
            }
        }

    }
    }
