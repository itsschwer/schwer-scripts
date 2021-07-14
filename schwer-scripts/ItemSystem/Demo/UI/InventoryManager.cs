﻿using System.Collections.Generic;
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

        private void OnEnable() {
            inventory.OnContentsChanged += UpdateSlots;

            var selected = EventSystem.current.currentSelectedGameObject;
            if (selected == null || !selected.transform.IsChildOf(this.transform)) {
                EventSystem.current.SetSelectedGameObject(itemSlots[0].gameObject);
            }

            Initialise();
        }

        private void OnDisable() => inventory.OnContentsChanged -= UpdateSlots;

        private void Awake() {
            GetComponentsInChildren<ItemSlot>(itemSlots);

            foreach (var slot in itemSlots) {
                slot.manager = this;
            }
        }

        private void Initialise() {
            UpdateDisplay(null);
            if (inventory != null) {
                UpdateSlots();
            }
        }

        public void UpdateSlots() => UpdateSlots(null, 0);
        private void UpdateSlots(Item item, int count) {
            var items = GetInventoryList();
            for (int i = 0; i < itemSlots.Count; i++) {
                if (i < items.Count) {
                    itemSlots[i].SetItem(items[i].Item1, items[i].Item2);
                }
                else {
                    itemSlots[i].Clear();
                }
            }

            var current = EventSystem.current.currentSelectedGameObject?.GetComponent<ItemSlot>();
            if (current != null) {
                UpdateDisplay(current.item);
            }
        }

        private List<(Item, int)> GetInventoryList() {
            var list = new List<(Item, int)>();

            foreach (var entry in inventory) {
                if (entry.Key.stackable) {
                    list.Add((entry.Key, entry.Value));
                }
                else {
                    for (int i = 0; i < entry.Value; i++) {
                        list.Add((entry.Key, 1));
                    }
                }
            }

            return list;
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
