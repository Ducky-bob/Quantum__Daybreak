using UnityEngine;

public class PlayerLooter : MonoBehaviour
{
    private LootableObject currentLootable = null;

    private void Update()
    {
        // (Giữ nguyên hoặc xóa đoạn nhấn E nếu ông muốn chỉ dùng chuột bấm UI)
        if (Input.GetKeyDown(KeyCode.E))
        {
            // Logic phím E: Nhặt món đầu tiên hoặc tất cả (tùy ông)
            // Ở đây tôi để trống để ông tập trung vào việc bấm chuột
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        LootableObject loot = other.GetComponent<LootableObject>();
        if (loot != null)
        {
            currentLootable = loot;
            // GỌI HÀM MỚI CỦA UI MANAGER
            if (LootUIManager.Instance != null)
            {
                LootUIManager.Instance.ShowLootForObject(loot);
            }
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LootableObject loot = other.GetComponent<LootableObject>();
        if (loot != null && currentLootable == loot)
        {
            currentLootable = null;
            if (LootUIManager.Instance != null)
            {
                LootUIManager.Instance.HideLootUI();
            }
        }
    }
}