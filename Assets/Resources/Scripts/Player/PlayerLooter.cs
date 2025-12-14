using UnityEngine;

public class PlayerLooter : MonoBehaviour
{
    private LootableObject currentLootable = null;

   

    private void OnTriggerEnter2D(Collider2D other)
    {
        LootableObject loot = other.GetComponent<LootableObject>();
        if (loot != null)
        {
            currentLootable = loot;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        LootableObject loot = other.GetComponent<LootableObject>();
        if (loot != null && currentLootable == loot)
        {
            currentLootable = null;
        }
    }

    // --- SỬA LẠI ĐOẠN NÀY ---
    private void Loot(LootableObject lootSource)
    {
        // Debug.Log("Đang loot từ " + lootSource.gameObject.name);

        // Gọi InventoryManager
        if (InventoryManager.Instance != null)
        {
            // THAY ĐỔI Ở ĐÂY:
            // Truyền trực tiếp "lootSource" (cả cái rương) vào
            // Thay vì truyền "items" (danh sách) như cũ.
            InventoryManager.Instance.AddItemsFromLoot(lootSource);
        }

        // Hủy đối tượng sau khi loot
        lootSource.DestroyAfterLoot();
        currentLootable = null;
    }
}