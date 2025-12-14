using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class LootUIManager : MonoBehaviour
{
    public static LootUIManager Instance;

    [Header("UI Components")]
    public GameObject lootPanel;
    public Transform lootContainer;
    public GameObject itemButtonPrefab;

    // Lưu lại cái rương đang mở để biết mà xóa đồ bên trong nó
    private LootableObject currentLootSource;
    private List<GameObject> currentButtons = new List<GameObject>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        HideLootUI();
    }

    // Sửa hàm này để nhận vào cả CÁI RƯƠNG (LootableObject) thay vì chỉ list đồ
    public void ShowLootForObject(LootableObject lootObj)
    {
        currentLootSource = lootObj; // Ghi nhớ cái rương đang mở
        UpdateLootList();
    }

    public void UpdateLootList()
    {
        // Nếu rương rỗng hoặc null -> Ẩn bảng
        if (currentLootSource == null || currentLootSource.itemsToLoot.Count == 0)
        {
            HideLootUI();
            return;
        }

        lootPanel.SetActive(true);

        // Xóa nút cũ
        foreach (var btn in currentButtons) Destroy(btn);
        currentButtons.Clear();

        // Tạo nút mới và GÁN SỰ KIỆN CLICK (Đây là đoạn ông thiếu lúc nãy)
        for (int i = 0; i < currentLootSource.itemsToLoot.Count; i++)
        {
            int index = i; // Biến tạm để dùng trong lambda (quan trọng)
            var item = currentLootSource.itemsToLoot[i];

            GameObject newBtn = Instantiate(itemButtonPrefab, lootContainer);

            // 1. Hiển thị tên
            TextMeshProUGUI tmpText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null) tmpText.text = item.itemName;
            else
            {
                Text legacyText = newBtn.GetComponentInChildren<Text>();
                if (legacyText != null) legacyText.text = item.itemName;
            }

            // 2. GÁN SỰ KIỆN CLICK (Bấm vào thì nhặt)
            Button btnComp = newBtn.GetComponent<Button>();
            if (btnComp != null)
            {
                btnComp.onClick.AddListener(() => OnItemClicked(index));
            }

            currentButtons.Add(newBtn);
        }
    }

    // Hàm xử lý khi bấm nút
    void OnItemClicked(int index)
    {
        if (currentLootSource == null) return;
        if (index >= currentLootSource.itemsToLoot.Count) return;

        // 1. Lấy thông tin món đồ
        var itemToLoot = currentLootSource.itemsToLoot[index];

        // 2. Thêm vào túi đồ (InventoryManager)
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.AddItem(itemToLoot.itemName);
        }

        // 3. Xóa món đó khỏi rương
        currentLootSource.itemsToLoot.RemoveAt(index);

        // 4. Nếu rương hết đồ thì hủy rương, còn không thì cập nhật lại danh sách nút
        if (currentLootSource.itemsToLoot.Count == 0)
        {
            currentLootSource.DestroyAfterLoot(); // Hủy rương
            HideLootUI(); // Ẩn bảng
        }
        else
        {
            UpdateLootList(); // Vẽ lại danh sách (để mất cái nút vừa bấm)
        }
    }

    public void HideLootUI()
    {
        if (lootPanel != null) lootPanel.SetActive(false);
        currentLootSource = null;
    }
}