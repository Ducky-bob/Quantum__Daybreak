using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

[DefaultExecutionOrder(-10)]
public class InventoryManager : Singleton<InventoryManager>
{
    // --- 1. ĐỊNH NGHĨA CLASS InventoryItem (Bạn đang thiếu cái này) ---
    [System.Serializable]
    public class InventoryItem
    {
        public string itemName;      // Tên vật phẩm
        public string storedData;    // Dữ liệu JSON (để lưu đồ trong rương)
    }

    // --- 2. KHAI BÁO CÁC BIẾN (Bạn đang thiếu các dòng này) ---
    [Header("Cài đặt")]
    public List<GameObject> itemPrefabs; // List chứa các Prefab game 2D
    public Transform playerTransform;    // Vị trí người chơi

    [Header("Dữ liệu Kho đồ")]
    // Lưu ý: Dùng List<InventoryItem> chứ không phải List<string> nữa
    public List<InventoryItem> currentInventory = new List<InventoryItem>();

    [Header("Giao diện UI")]
    public GameObject uiPanel;
    public Text inventoryDisplay;


    // --- 3. CÁC HÀM XỬ LÝ (LOGIC) ---

    // Class phụ để lưu file save
    [System.Serializable]
    public class InventoryDataWrapper { public List<InventoryItem> items; }

    protected override void OnAwake()
    {
        LoadInventory();

        if (uiPanel != null) uiPanel.SetActive(false);

        // Tự tìm người chơi nếu bạn quên kéo thả vào Inspector
        if (playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) playerTransform = player.transform;
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) ToggleInventory();

        // Test: Nhấn X để vứt món đồ đầu tiên
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (currentInventory.Count > 0)
            {
                DropItem(currentInventory[0]);
            }
        }
    }

    // Hàm Nhặt đồ (Cập nhật mới)
    public void AddItemsFromLoot(LootableObject lootSource)
    {
        InventoryItem newItem = new InventoryItem();
        // Xóa chữ (Clone) nếu nhặt từ vật thể vừa sinh ra
        newItem.itemName = lootSource.gameObject.name.Replace("(Clone)", "").Trim();

        // Lưu dữ liệu bên trong rương (nếu có)
        if (lootSource.itemsToLoot.Count > 0)
        {
            LootWrapper wrapper = new LootWrapper { lootItems = lootSource.itemsToLoot };
            newItem.storedData = JsonUtility.ToJson(wrapper);
        }

        currentInventory.Add(newItem);
        Debug.Log($"Đã nhặt: {newItem.itemName}");

        UpdateInventoryUI();
        SaveInventory();
    }

    // Hàm Vứt đồ (DropItem) - Đã sửa cho 2D
    public void DropItem(InventoryItem itemToDrop)
    {
        // A. Tìm Prefab
        GameObject prefabToSpawn = itemPrefabs.Find(p => p.name == itemToDrop.itemName);

        if (prefabToSpawn != null && playerTransform != null)
        {
            // B. TÍNH VỊ TRÍ (2D)
            // Lấy vị trí người chơi, cộng thêm 1 đơn vị sang phải (X + 1)
            Vector3 dropPos = playerTransform.position + new Vector3(1f, 0f, 0f);

            // Sinh ra vật thể
            GameObject newObj = Instantiate(prefabToSpawn, dropPos, Quaternion.identity);
            newObj.name = itemToDrop.itemName; // Đặt tên lại cho đúng

            // C. KHÔI PHỤC DỮ LIỆU
            if (!string.IsNullOrEmpty(itemToDrop.storedData))
            {
                LootableObject lootScript = newObj.GetComponent<LootableObject>();
                if (lootScript != null)
                {
                    LootWrapper wrapper = JsonUtility.FromJson<LootWrapper>(itemToDrop.storedData);
                    lootScript.itemsToLoot = wrapper.lootItems;
                    Debug.Log("Đã khôi phục dữ liệu rương 2D!");
                }
            }

            // D. Xóa khỏi túi và Lưu
            currentInventory.Remove(itemToDrop);
            UpdateInventoryUI();
            SaveInventory();
        }
        else
        {
            Debug.LogError($"LỖI: Chưa có Prefab tên '{itemToDrop.itemName}' trong list ItemPrefabs, hoặc chưa gán PlayerTransform!");
        }
    }

    // Cập nhật giao diện chữ
    private void UpdateInventoryUI()
    {
        if (inventoryDisplay == null) return;
        string display = "--- TÚI ĐỒ (Nhấn X để vứt) ---\n\n";
        foreach (var item in currentInventory)
        {
            display += $"- {item.itemName}\n";
        }
        inventoryDisplay.text = display;
    }

    // --- HỆ THỐNG LƯU TRỮ ---
    private void SaveInventory()
    {
        InventoryDataWrapper wrapper = new InventoryDataWrapper { items = currentInventory };
        string json = JsonUtility.ToJson(wrapper);
        PlayerPrefs.SetString("SaveData_V2", json);
        PlayerPrefs.Save();
    }

    private void LoadInventory()
    {
        if (PlayerPrefs.HasKey("SaveData_V2"))
        {
            string json = PlayerPrefs.GetString("SaveData_V2");
            InventoryDataWrapper wrapper = JsonUtility.FromJson<InventoryDataWrapper>(json);
            if (wrapper != null) currentInventory = wrapper.items;
        }
        else
        {
            currentInventory = new List<InventoryItem>();
        }
    }

    public void ToggleInventory()
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
            if (uiPanel.activeSelf) UpdateInventoryUI();
        }
    }
}

// --- 4. CLASS PHỤ LOOTWRAPPER (Để ở ngoài cùng, sửa lỗi không tìm thấy LootWrapper) ---
[System.Serializable]
public class LootWrapper
{
    public List<LootableObject.LootItem> lootItems;
}