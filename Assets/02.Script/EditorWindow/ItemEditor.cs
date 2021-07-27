using System.Collections;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class ItemEditor : EditorWindow
{
	ItemList itemList;
	Vector2 scrollPos;

	[MenuItem("Custom/ItemEditor")]
	static void Init()
	{
		// Get existing open window or if none, make a new one:
		ItemEditor itemEditor = (ItemEditor)EditorWindow.GetWindow(typeof(ItemEditor));
		itemEditor.Show();
	}

    private void OnEnable()
    {
		LoadItemList();
    }

    private void OnGUI()
	{
		scrollPos =	EditorGUILayout.BeginScrollView(scrollPos);
		DisplayItemList();
		EditorGUILayout.EndScrollView();

		GUILayout.Space(10f);
		EditorGUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Add", GUILayout.Width(70f), GUILayout.Height(40f)))
		{
			AddItemToList();
		}
		if (GUILayout.Button("Save", GUILayout.Width(70f), GUILayout.Height(40f)))
		{
			SaveItemList();
		}
		EditorGUILayout.EndHorizontal();
		GUILayout.Space(10f);
	}

	private void AddItemToList()
    {
		itemList.itemList.Add(new Item(Item.ItemType.weapon, Item.ItemParts.none, -1, "defalut"));
		SortItemList();
		DisplayItemList();
	}

	private void RemoveItemFromList(Item item)
    {
		itemList.itemList.Remove(item);
		SortItemList();
		DisplayItemList();
	}

	private void SortItemList()
    {
		itemList.itemList = itemList.itemList.OrderBy(x => x.itemNo).ToList();
    }

	private void SaveItemList()
	{
		string fileName = "ItemList.json";
		string jsonPath = Application.dataPath + "/Resources/Data/";
		string jsonData = JsonUtility.ToJson(itemList, true);
		File.WriteAllText(jsonPath + fileName, jsonData);

	}

	private void LoadItemList()
    {
		string fileName = "ItemList";

		TextAsset jsonData = Resources.Load<TextAsset>("Data/" + fileName);
		itemList = JsonUtility.FromJson<ItemList>(jsonData.ToString());
	}

	private void DisplayItemList()
    {
		foreach(Item item in itemList.itemList)
        {
			item.itemNo = int.Parse(EditorGUILayout.TextField("No", item.itemNo.ToString()));
			item.itemName = EditorGUILayout.TextField("Name", item.itemName);

			item.itemType = (Item.ItemType)EditorGUILayout.EnumPopup("Type", item.itemType);
			item.itemParts = (Item.ItemParts)EditorGUILayout.EnumPopup("Parts", item.itemParts);

			item.damage = int.Parse(EditorGUILayout.TextField("Damage", item.damage.ToString()));
			item.criticalChance = float.Parse(EditorGUILayout.TextField("Critical Chance", item.criticalChance.ToString()));
			item.health = int.Parse(EditorGUILayout.TextField("Health", item.health.ToString()));
			item.mana = int.Parse(EditorGUILayout.TextField("Mana", item.mana.ToString()));
			item.manaRegeneration = float.Parse(EditorGUILayout.TextField("Mana Regeneration", item.manaRegeneration.ToString()));
			item.price = int.Parse(EditorGUILayout.TextField("Price", item.price.ToString()));
			item.itemDescription= EditorGUILayout.TextField("Description", item.itemDescription);

			GUILayout.Space(10f);
			EditorGUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Remove", GUILayout.Width(60f), GUILayout.Height(20f)))
			{
				RemoveItemFromList(item);
				return;
			}
			EditorGUILayout.EndHorizontal();
			GUILayout.Space(10f);

			EditorGUI.DrawRect(EditorGUILayout.GetControlRect(false, 0.7f), Color.white);
			GUILayout.Space(10f);
		}
    }
}
