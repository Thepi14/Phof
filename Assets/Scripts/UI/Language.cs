using System;
using System.Collections;
using System.Collections.Generic;
using HabilitySystem;
using UnityEngine;
using System.IO;
using Unity.VisualScripting;

namespace LangSystem
{
    [CreateAssetMenu(fileName = "Language", menuName = "New Language", order = 3)]
    public class Language : ScriptableObject
    {
        public static Language currentLanguage;
        public static List<Language> languages;
        public string LanguageName => name;

        [Header("General")]
        public string language;
        public string enterText;
        public string enterNumber;
        public string exit;
        public string loading;
        public string ok;
        public string Return;

        [Header("Main menu")]
        public string playNewGame;
        public string continueGame;
        public string credits;

        [Header("New game world")]
        public string generateNewWorld;
        public string generate;
        public string mapSize;
        public string seed;
        public string generatingWorld;
        public string chooseYourClass;
        public string wizard;
        public string warrior;
        public string archer;

        [Header("Game")]
        public string level;
        public string currentLevel;
        public string availablePoints;
        public string cantLevelUpAttribute;
        public string strength;
        public string resistance;
        public string intelligence;
        public string defense;
        public string speed;
        public string chooseCard;
        public string habilityIsNotReloaded;

        [Header("Options")]
        public string options;

        public string configurations;

        public string volume;
        public string masterVolume;
        public string musicVolume;
        public string soundEffectVolume;
        public string UIVolume;

        public string keybind;
        public string exitOptionsMenu;
        public string returnToMainMenu;
        public string exitGame;

        /// <summary>
        /// \\ == hability name 
        /// </summary>
        public string HabilityIsNotReloadedPlusName(string name) => habilityIsNotReloaded.Replace("\\", name);

        [Header("Habilities")]
        public Dictionary<string, HabilityInfo> habilityInfos;
        [SerializeField]
        private List<HabilityInfoSerializable> habilityInfosList;

        public static List<Language> SetLanguageList()
        {
            languages = new List<Language>();
            foreach (var language in Resources.LoadAll<Language>("Lang"))
            {
                languages.Add(language);
            }
            return languages;
        }
        public static Language GetLanguage()
        {
            SetLanguageList();
            currentLanguage = Resources.Load<Language>("Lang/" + PlayerPrefs.GetString("CURRENT_LANGUAGE", "Português"));
            return currentLanguage;
        }
        public static void SetLanguage(string langName)
        {
            PlayerPrefs.SetString("CURRENT_LANGUAGE", langName);
            currentLanguage = Resources.Load<Language>("Lang/" + PlayerPrefs.GetString("CURRENT_LANGUAGE", "Português"));
        }
        public void OnValidate()
        {
            SetLanguageDescsLists();
        }
        public void SetLanguageDescsLists()
        {
            habilityInfos = new Dictionary<string, HabilityInfo>();
            habilityInfos.Clear();
            foreach (var info in habilityInfosList)
            {
                habilityInfos.Add(info.ID, info);
                //Debug.Log(habilityInfos[info.ID].TrueDescription);
            }
        }
    }
}
