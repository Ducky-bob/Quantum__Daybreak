using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class InventoryManager : MonoBehaviour
{
    public static InventoryManager Instance;

    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;
        public string storedData;
    }

    [System.Serializable]
    public class InventoryDataWrapper { public List<InventoryItem> items; }

    [Header("Cài đặt")]
    public List<GameObject> itemPrefabs;
    public Transform playerTransform;

    [Header("Dữ liệu Kho đồ")]
    public List<InventoryItem> currentInventory = new List<InventoryItem>();

    [Header("Giao diện UI")]
    public GameObject uiPanel;
    public Text inventoryDisplay;

    private void Awake()
    {
        if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        LoadInventory();
        if (uiPanel != null) uiPanel.SetActive(false);

        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();
        if (Input.GetKeyDown(KeyCode.X) && currentInventory.Count > 0) DropItem(currentInventory[0]);
    }

    // --- HÀM MỚI: NHẶT 1 MÓN ĐỒ LẺ (Dùng cho nút bấm UI) ---
    public void AddItem(string name)
    {
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = name;

        currentInventory.Add(newItem);
        Debug.Log($"Đã nhặt: {name}");

        UpdateInventoryUI();
        SaveInventory();
    }

    // Hàm cũ (Nhặt cả rương) - Vẫn giữ để dùng nếu cần
    public void AddItemsFromLoot(LootableObject lootSource)
    {
        InventoryItem newItem = new InventoryItem();
        newItem.itemName = lootSource.gameObject.name.Replace("(Clone)", "").Trim();

        // Code cũ lưu JSON...
        if (lootSource.itemsToLoot.Count > 0)
        {
            // (Đoạn này giữ nguyên logic cũ của ông nếu muốn lưu cả rương)
        }
        currentInventory.Add(newItem);
        UpdateInventoryUI();
        SaveInventory();
    }

    public void DropItem(InventoryItem itemToDrop)
    {
        GameObject prefabToSpawn = itemPrefabs.Find(p => p.name == itemToDrop.itemName);

        if (prefabToSpawn != null && playerTransform != null)
        {
            Vector3 dropPos = playerTransform.position + new Vector3(1.5f, 0f, 0f);
            GameObject newObj = Instantiate(prefabToSpawn, dropPos, Quaternion.identity);
            newObj.name = itemToDrop.itemName;

            currentInventory.Remove(itemToDrop);
            UpdateInventoryUI();
            SaveInventory();
        }
        else
        {
            Debug.LogError($"LỖI: Không tìm thấy Prefab tên '{itemToDrop.itemName}'!");
        }
    }

    private void UpdateInventoryUI()
    {
        if (inventoryDisplay == null) return;
        string display = "--- TÚI ĐỒ ---\n\n";
        foreach (var item in currentInventory) display += $"- {item.itemName}\n";
        inventoryDisplay.text = display;
    }

    public void ToggleInventory()
    {
        if (uiPanel != null) { uiPanel.SetActive(!uiPanel.activeSelf); if (uiPanel.activeSelf) UpdateInventoryUI(); }
    }

    private void SaveInventory()
    {
        InventoryDataWrapper wrapper = new InventoryDataWrapper { items = currentInventory };
        PlayerPrefs.SetString("SaveData_V2", JsonUtility.ToJson(wrapper));
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("SaveData_V2"))
        {
            InventoryDataWrapper wrapper = JsonUtility.FromJson<InventoryDataWrapper>(PlayerPrefs.GetString("SaveData_V2"));
            if (wrapper != null) currentInventory = wrapper.items;
        }
    }
}