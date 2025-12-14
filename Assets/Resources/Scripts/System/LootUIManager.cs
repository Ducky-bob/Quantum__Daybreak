using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro; // <--- QUAN TRỌNG: Thư viện cho chữ nét

public class LootUIManager : MonoBehaviour
{
    public static LootUIManager Instance;

    [Header("UI Components")]
    public GameObject lootPanel;
    public Transform lootContainer;
    public GameObject itemButtonPrefab;

    private List<GameObject> currentButtons = new List<GameObject>();

    private void Awake()
    {
        Instance = this;
        HideLootUI();
    }

    public void UpdateLootList(List<LootableObject.LootItem> items)
    {
        if (items == null || items.Count == 0)
        {
            HideLootUI();
            return;
        }

        lootPanel.SetActive(true);

        // Xóa nút cũ
        foreach (var btn in currentButtons) Destroy(btn);
        currentButtons.Clear();

        // Tạo nút mới
        foreach (var item in items)
        {
            GameObject newBtn = Instantiate(itemButtonPrefab, lootContainer);

            // --- SỬA ĐOẠN NÀY ĐỂ HỖ TRỢ CẢ TEXT THƯỜNG VÀ TMP ---

            // Thử tìm TextMeshPro trước (Ưu tiên)
            TextMeshProUGUI tmpText = newBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmpText != null)
            {
                tmpText.text = item.itemName;
            }
            else
            {
                // Nếu không thấy TMP thì tìm Text thường (Dự phòng)
                Text legacyText = newBtn.GetComponentInChildren<Text>();
                if (legacyText != null)
                {
                    legacyText.text = item.itemName;
                }
            }
            // ----------------------------------------------------

            currentButtons.Add(newBtn);
        }
    }

    public void HideLootUI()
    {
        if (lootPanel != null) lootPanel.SetActive(false);
    }
}