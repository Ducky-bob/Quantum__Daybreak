using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI; // Cần thiết để dùng Text UI

[DefaultExecutionOrder(-10)]
public class InventoryManager : Singleton<InventoryManager>
{
    [Header("Dữ liệu Kho đồ")]
    private List<string> currentInventory = new List<string>();

    [Header("Giao diện UI")]
    // 1. Biến này nắm giữ cái khung túi đồ để Bật/Tắt
    public GameObject uiPanel;

    // 2. Biến này hiển thị chữ
    public Text inventoryDisplay;

    protected override void OnAwake()
    {
        if (currentInventory == null) currentInventory = new List<string>();

        // Mặc định khi vào game thì ẩn túi đồ đi cho gọn
        if (uiPanel != null)
        {
            uiPanel.SetActive(false);
        }
    }

    // --- Lắng nghe phím bấm ---
    private void Update()
    {
        // Nhấn phím I để Mở/Đóng túi
        if (Input.GetKeyDown(KeyCode.I))
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (uiPanel != null)
        {
            // Đảo ngược trạng thái: Đang mở -> Đóng, Đang đóng -> Mở
            bool isActive = uiPanel.activeSelf;
            uiPanel.SetActive(!isActive);

            // Nếu vừa mở lên thì cập nhật lại chữ cho mới nhất
            if (!isActive) UpdateInventoryUI();
        }
    }

    // --- Logic Thêm đồ (Giữ nguyên) ---
    public void AddItem(string itemName)
    {
        currentInventory.Add(itemName);
        Debug.Log($"[Inventory] Đã nhặt: {itemName}");

        // Cập nhật text ngay cả khi túi đang đóng (để mở ra là thấy ngay)
        UpdateInventoryUI();
    }

    public void AddItemsFromLoot(List<LootableObject.LootItem> items)
    {
        foreach (var item in items)
        {
            AddItem(item.itemName);
        }
    }

    // --- Cập nhật chữ trên UI ---
    private void UpdateInventoryUI()
    {
        if (inventoryDisplay == null)
        {
            Debug.LogError("LỖI: Chưa gắn Text vào Inventory Display!");
            return;
        }

        // --- KIỂM TRA DỮ LIỆU ---
        Debug.Log($"[CHECK 1] Tổng số món trong list: {currentInventory.Count}");

        string display = "--- TÚI ĐỒ CỦA TÔI ---\n\n";
        Dictionary<string, int> itemCounts = new Dictionary<string, int>();

        foreach (string item in currentInventory)
        {
            if (itemCounts.ContainsKey(item))
                itemCounts[item]++;
            else
                itemCounts.Add(item, 1);
        }

        foreach (var pair in itemCounts)
        {
            display += $"- {pair.Key}: {pair.Value}\n";
        }

        // --- KIỂM TRA CHUỖI SẮP HIỆN ---
        Debug.Log($"[CHECK 2] Nội dung sắp hiển thị lên UI:\n{display}");

        inventoryDisplay.text = display;
    }
}