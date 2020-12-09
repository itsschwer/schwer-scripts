﻿using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Schwer.ItemSystem.Demo {
    public class InventoryManager : MonoBehaviour {
        [SerializeField] private InventorySO _inventory = default;
        private Inventory inventory => _inventory.value;

        [Header("Item Display components")]
        [SerializeField] private Text nameDisplay = default;
        [SerializeField] private Text descriptionDisplay = default;

        private List<ItemSlot> itemSlots = new List<ItemSlot>();

        private void OnEnable() => inventory.OnContentsChanged += UpdateSlots;
        private void OnDisable() => inventory.OnContentsChanged -= UpdateSlots;

        private void Awake() {
            GetComponentsInChildren<ItemSlot>(itemSlots);

            foreach (var slot in itemSlots) {
                slot.manager = this;
            }

            UpdateDisplay(null);
            if (inventory != null) {
                UpdateSlots();
            }
        }

        private void UpdateSlots() => UpdateSlots(null, 0);
        private void UpdateSlots(Item item, int count) {
            for (int i = 0; i < itemSlots.Count; i++) {
                if (i < inventory.Count) {
                    var entry = inventory.ElementAt(i);
                    itemSlots[i].SetItem(entry.Key, entry.Value);
                }
                else {
                    itemSlots[i].Clear();
                }
            }

            var current = EventSystem.current?.currentSelectedGameObject?.GetComponent<ItemSlot>();
            if (current != null) {
                UpdateDisplay(current.item);
            }
        }

        public void UpdateDisplay(Item item) {
            if (item != null) {
                nameDisplay.text = item.name;
                descriptionDisplay.text = item.description;
            }
            else {
                nameDisplay.text = "";
                descriptionDisplay.text = "";
            }
        }
    }
}
