using UnityEngine;
using System.Collections.Generic;

public class LootableObject : MonoBehaviour
{
    [System.Serializable]
    public struct LootItem
    {
        public string itemName;
    }

    public List<LootItem> itemsToLoot = new List<LootItem>();
    private bool playerIsNearby = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerIsNearby = false;
        }
    }

    public bool CanBeLooted() => playerIsNearby;

    public void DestroyAfterLoot()
    {
        if (itemsToLoot.Count == 0) Destroy(gameObject); // Chỉ hủy rương nếu rỗng
    }
}