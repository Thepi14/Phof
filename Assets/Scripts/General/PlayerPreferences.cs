using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EntityDataSystem;
using static GamePlayer;
using ItemSystem;
using Unity.VisualScripting.Antlr3.Runtime.Misc;

public static class PlayerPreferences
{
    #region General
    public static string CurrentDifficulty => PlayerPrefs.GetString("DIFFICULTY", "Normal");
    public static float MasterVolume => PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float MusicVolume => PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f);
    public static float SoundEffectsVolume => PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f);
    public static float UIVolume => PlayerPrefs.GetFloat("UI_VOLUME", 1f);
    #endregion

    #region Scaled volume
    public static float MusicVolumeScaled => PlayerPrefs.GetFloat("MUSIC_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float SoundEffectsVolumeScaled => PlayerPrefs.GetFloat("SOUND_EFFECTS_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    public static float UIVolumeScaled => PlayerPrefs.GetFloat("UI_VOLUME", 1f) * PlayerPrefs.GetFloat("MASTER_VOLUME", 0.8f);
    #endregion

    #region Configurations
    public static bool ShowDamage => PlayerPrefs.GetInt("SHOW_DAMAGE", 1) == 1;
    public static bool Orthographic => PlayerPrefs.GetInt("ORTHOGRAPHIC", 0) == 1;
    public static bool ShowCoordinates => PlayerPrefs.GetInt("SHOW_COORD", 0) == 1;
    public static bool GameSaved => PlayerPrefs.GetInt("SAVED_GAME", 0) == 1;
    public static bool NewGame => PlayerPrefs.GetInt("NEW_GAME", 1) == 1;
    public static bool Died => PlayerPrefs.GetInt("DIED", 1) == 0;
    #endregion
    public static void SavePlayerData(this EntityData data)
    {
        PlayerPrefs.SetInt("PLAYER_LEVEL", data.level);
        PlayerPrefs.SetInt("STRENGTH", data.strength);
        PlayerPrefs.SetInt("RESISTANCE", data.resistance);
        PlayerPrefs.SetInt("INTELLIGENCE", data.intelligence);
        PlayerPrefs.SetInt("DEFENSE", data.defense);
        PlayerPrefs.SetFloat("SPEED", data.speed);
        PlayerPrefs.SetInt("CURRENT_HEALTH", data.currentHealth);

        var index = 0;
        Debug.Log($"SAVING");
        Debug.Log($"INVENTORY_ITEMS NOW");
        foreach (var slot in Inventory.Singleton.inventorySlots)
        {
            Debug.Log(slot.gameObject.name);
        }
        foreach (var slot in Inventory.Singleton.inventorySlots)
        {
            if (slot.myItem != null)
            {
                PlayerPrefs.SetString($"INVENTORY_ITEM_{index}", slot.myItem.myItem.ID);
            }
            else
                PlayerPrefs.SetString($"INVENTORY_ITEM_{index}", "NULL");
            Debug.Log($"INVENTORY_ITEM_{index}");
            index++;
        }
        index = 0;
        Debug.Log($"EQUIPMENT_ITEMS NOW");
        foreach (var slot in Inventory.Singleton.equipmentSlots)
        {
            Debug.Log(slot.gameObject.name);
        }
        foreach (var slot in Inventory.Singleton.equipmentSlots)
        {
            if (slot.myItem != null)
            {
                PlayerPrefs.SetString($"EQUIPMENT_ITEM_{index}", slot.myItem.myItem.ID);
            }
            else
                PlayerPrefs.SetString($"EQUIPMENT_ITEM_{index}", "NULL");
            Debug.Log($"EQUIPMENT_ITEM_{index}");
            index++;
        }
        for (int i = 0; i < 4; i++)
        {
            PlayerPrefs.SetString($"PLAYER_HABILITY_{i}", "NULL");
        }
        index = 0;
        foreach (var ID in player.GetCards())
        {
            PlayerPrefs.SetString($"PLAYER_HABILITY_{index}", ID);
            index++;
        }

        PlayerPrefs.SetInt("SAVED_GAME", 1);
        PlayerPrefs.Save();
    }
    public static void LoadPlayerData()
    {
        if (player != null)
        {
            player.EntityData.level = PlayerPrefs.GetInt("PLAYER_LEVEL", 1);
            player.EntityData.strength = PlayerPrefs.GetInt("STRENGTH", 1);
            player.EntityData.resistance = PlayerPrefs.GetInt("RESISTANCE", 1);
            player.EntityData.intelligence = PlayerPrefs.GetInt("INTELLIGENCE", 1);
            player.EntityData.defense = PlayerPrefs.GetInt("DEFENSE", 1);
            player.EntityData.speed = PlayerPrefs.GetFloat("SPEED", 1f);
            player.EntityData.currentHealth = PlayerPrefs.GetInt("CURRENT_HEALTH", 0);
        }
        else
            throw new System.Exception("Player is null");

        var index = 0;
        Debug.Log($"LOADING");
        Debug.Log($"INVENTORY_ITEMS NOW");
        foreach (var slot in Inventory.Singleton.inventorySlots)
        {
            Item item = null;
            var itemID = PlayerPrefs.GetString($"INVENTORY_ITEM_{index}", "NULL");

            if (itemID != "NULL")
                foreach (var listItem in Inventory.Singleton.items)
                    if (itemID == listItem.ID)
                    {
                        //Debug.Log($"INVENTORY_ITEM_{index}, {itemID}");
                        item = listItem;
                        break;
                    }
            if (item != null)
            {
                Inventory.Instantiate<InventoryItem>(Inventory.Singleton.itemPrefab, slot.transform).Initialize(slot, item);
                //Inventory.Singleton.SpawnInventoryItem(item);
                slot.SetItem(slot.myItem);
            }
            Debug.Log($"INVENTORY_ITEM_{index}");
            index++;
        }
        index = 0;
        Debug.Log($"EQUIPMENT_ITEMS NOW");
        foreach (var slot in Inventory.Singleton.equipmentSlots)
        {
            Item item = null;
            var itemID = PlayerPrefs.GetString($"EQUIPMENT_ITEM_{index}", "NULL");

            if (itemID != "NULL")
                foreach (var listItem in Inventory.Singleton.items)
                    if (itemID == listItem.ID)
                    {
                        //Debug.Log($"EQUIPMENT_ITEM_{index}, {itemID}");
                        item = listItem;
                        break;
                    }
            if (item != null)
            {
                Inventory.Instantiate<InventoryItem>(Inventory.Singleton.itemPrefab, slot.transform).Initialize(slot, item);
                //Inventory.Singleton.SpawnInventoryItem(item);
                slot.SetItem(slot.myItem);
            }
            Debug.Log($"EQUIPMENT_ITEM_{index}");
            index++;
        }
        index = 0;
        for (int i = 0; i < 4; i++)
        {
            var ID = PlayerPrefs.GetString($"PLAYER_HABILITY_{i}", "NULL");
            if (ID != "NULL")
                CardChoice.AddCard(ID);
        }

        PlayerPrefs.SetInt("SAVED_GAME", 0);
        PlayerPrefs.Save();
    }

    public static void Reset()
    {
        PlayerPrefs.SetFloat("MASTER_VOLUME", 0.8f);
        PlayerPrefs.SetFloat("MUSIC_VOLUME", 1f);
        PlayerPrefs.SetFloat("SOUND_EFFECTS_VOLUME", 1f);
        PlayerPrefs.SetFloat("UI_VOLUME", 1f);
        PlayerPrefs.SetInt("MAP_WIDTH", 100);
        PlayerPrefs.SetInt("MAP_HEIGHT", 100);
        PlayerPrefs.SetString("CLASS", "Warrior");
        PlayerPrefs.SetInt("SHOW_DAMAGE", 1);
        PlayerPrefs.SetInt("ORTHOGRAPHIC", 0);
        PlayerPrefs.SetInt("SHOW_COORD", 0);
        InputManagement.InputManager.ResetKeyBindToDefault();
    }
    public static Difficulty Difficulty =>
        CurrentDifficulty == "Easy" ? Difficulty.easy :
        CurrentDifficulty == "Normal" ? Difficulty.normal :
        CurrentDifficulty == "Hard" ? Difficulty.hard : Difficulty.lunatic;
}
public enum Difficulty : byte
{
    easy = 1,
    normal = 2,
    hard = 3,
    lunatic = 4
}