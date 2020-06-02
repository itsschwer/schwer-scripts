using UnityEngine;

namespace Schwer.ItemSystem {
    [CreateAssetMenu(menuName = "Scriptable Object/Inventory")]
    public class InventorySO : ScriptableObject {
        public Inventory value = new Inventory();
    }
}
