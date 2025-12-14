using UnityEngine;
using System.Collections.Generic;

public class LootableObject : MonoBehaviour
{
    // --- 1. CẤU TRÚC DỮ LIỆU (Giữ nguyên) ---
    [System.Serializable]
    public struct LootItem
    {
        public string itemName; // Tên phải khớp với tên Prefab trong InventoryManager
        // public int quantity; // Mở rộng sau này nếu muốn stack số lượng
        // public Sprite icon;  // Mở rộng sau này nếu muốn hiện hình ảnh trên nút UI
    }

    // Danh sách các vật phẩm mà đối tượng này chứa
    public List<LootItem> itemsToLoot = new List<LootItem>();

    // --- 2. XÓA BỎ LOGIC TRIGGER CŨ ---
    // Chúng ta đã XÓA các hàm OnTriggerEnter/Exit và biến playerIsNearby
    // Lý do: PlayerLooter đã đảm nhận việc phát hiện va chạm rồi.
    // Script này giờ chỉ cần ngồi im giữ đồ thôi, không cần "hét" lên nữa.

    // --- 3. CÁC HÀM XỬ LÝ ---

    // Hàm trả về dữ liệu (InventoryManager dùng cái này để lấy danh sách)
    public List<LootItem> GetLoot()
    {
        return itemsToLoot;
    }

    // Hàm hủy đối tượng (LootUIManager gọi cái này sau khi người chơi click nút)
    public void DestroyAfterLoot()
    {
        // [Mở rộng] Bạn có thể thêm code sinh ra âm thanh hoặc hiệu ứng hạt (Particle) ở đây
        // Ví dụ: AudioSource.PlayClipAtPoint(soundEffect, transform.position);

        Destroy(gameObject);
    }

    // [Tùy chọn] Hàm vẽ Gizmos để dễ nhìn thấy vật phẩm trong Scene editor
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}