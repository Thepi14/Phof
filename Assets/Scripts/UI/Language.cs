using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LangSystem
{
    [CreateAssetMenu(fileName = "Language", menuName = "New Language", order = 3)]
    public class Language : ScriptableObject
    {
        public static Language currentLanguage;

        public string LanguageName => name;

        //Main menu
        public string playNewGame;

        //general
        public string language;
        public string enterText;
        public string enterNumber;
        public string exit;
        public string exitGame;

        //New game world
        public string generateNewWorld;
        public string generate;
        public string mapSize;
        public string seed;

        public static void GetLanguage()
        {
            currentLanguage = Resources.Load<Language>("Lang/" + PlayerPrefs.GetString("CURRENT_LANGUAGE", "Português"));
        }
        public static void SetLanguage(string langName)
        {
            PlayerPrefs.SetString("CURRENT_LANGUAGE", langName);
            currentLanguage = Resources.Load<Language>("Lang/" + PlayerPrefs.GetString("CURRENT_LANGUAGE", "Português"));
        }
    }
}
