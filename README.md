# schwer-scripts
Open-source code for Unity.
## About
Hi, I'm Schwer, hobbyist game developer. As I am self-taught, I can't say for sure that the code I write is the cleanest. Despite that, rest assured that this repository contains only what I consider usable. Feel free to use this as a reference or a base for your own project(s). I'd love to improve, so if you have any suggestions to make, please do so.
## Contents
* Item System (using Scriptable Objects)
* TBA

# Item System
An skeleton implementation of items and inventories using Scriptable Objects.
## Features
* Items and inventories in Unity using Scriptable Objects
* Inventories that support saving and loading
* An Item Database that is generated and regenerated with the click of a button
* An Item Editor (custom editor window)
## Limitations
* Inventories use a Dictionary which Unity does not serialize to the Inspector by default, meaning that inventories cannot be easily be inspected via the Editor.
* Inventory assets do not maintain their value between Edit mode and Play mode, meaning that they can only be used at runtime and cannot easily have a starting value.
## Notes
* This implementation only provides the bare-bones of an item system.
* It is expected that you will extend the Item class in order for items to be usable in a game (e.g. consumables).
* The contents of the provided Demo folder is only intended for you to familiarise yourself with the system.
* You should implement your own system for displaying inventory to the UI, items that can be placed in the world, etc.
* As inventories use Scriptable Objects, in order for them to persist between scenes, ensure that there is a reference to the inventory in each scene you want it to persist to.
## Getting started
Download this repository and add the `SchwerScripts` folder to your Unity project.
Familiarise yourself with the system by dragging the prefab `Inventory` (`SchwerScripts/ItemSystem/Demo/UI/Prefabs/Inventory`) into a new Scene and following along with this section. 
#### Items
Create Item assets in a folder of your choice via `Create/Scriptable Object/Item System/Item`.
Edit them via the Inspector, or the Item Editor.
#### Item Editor
Can be opened in three ways:
* From the toolbar via `Item System/Open Item Editor`
* From the Inspector of an Item asset
* From double-clicking an Item asset

Dock this window appropriately for the best appearance.
#### Item Database
This system relies on an Item Database for the saving and loading of inventories. A Item Database can be created:
* From the toolbar via `Item System/Generate ItemDatabase`
* From the Item Editor (button at the bottom labelled `Generate ItemDatabase`)

This will create an Item Database in the currently selected folder if none exist.
Otherwise, it will update (regenerate) an existing Item Database.
To regenerate an Item Database after editing items, use either of the above methods for creating an Item Database or via the Inspector of the Item Database asset.
#### Inventory
Create an Inventory asset in a folder of your choice via `Create/Scriptable Objects/Item System/Inventory`.
Assign this asset to the `InventoryManager` script attached to the instance of the `Inventory` prefab in the Scene.
#### Play Mode
Enter Play mode and assign the `InventoryManager` to the appropriate field in the Inspector for the Inventory asset (you will need to do this every time you enter Play mode — again, this is only intended for familiarisation).

Experiment with the controls (Clear Inventory, Check, Set, Add, Remove, Clear) in the Inspector for the Inventory asset (make sure to assign an Item!).

## Usage guide
Non-editor scripts are in the namespace `Schwer.ItemSystem`, so remember to add that as a `using` when working with the Item System.
### Item Database
Generate or regenerate by:
* Menu item `Item System/Generate Item Database`
* Button in the Item Editor
* Inspector of an exisiting ItemDatabase asset

This process will first check for all ItemDatabase assets in your project. If none exist, a new one will be created in the selected folder/folder of selected item with the name `ItemDatabase`. If one already exists it will be regenerated. If multiple exist, an error will be logged to the Console. Make sure that your game object(s) refer to a single ItemDatabase and delete the extra assets before trying again.

Stores items in a list by item id in ascending order. The generation process is done alphabetically (by filename). Any items with an id matching that of an item that has already been added will be omitted from the ItemDatabase and a warning will be logged to the Console.

The ItemDatabase should only be referenced in the scene where saved inventories are loaded, as that is its only intended purpose.

Rather than trying to serialize each Item in an inventory, an inventory is first serialized into a form that stores the item id instead of the item itself (which will not work as the item is a nested Scriptable Object). Loading a `SerializedInventory` requires an ItemDatabase which is used to match ids in the `SerializedInventory` with the corresponding Item in the ItemDatabase. Because of this, ***item ids should not change once a build has been released***, since that will cause ids to be matched to the wrong Item.
### Item Editor
Accessible via:
* Menu item `Item System/Open Item Editor`
* Inspector of an Item asset
* Double-clicking on an Item asset

Serves as an additional way to edit existing Item assets. Items are ordered in the sidebar by their id for convenience.
### Item
This implementation provides a bare-bones script intended to be a base from which you will extend. It contains fields for: id, name, description, sprite, and stackable.

How these are used depends on you. If you do extend `Item`, then you will also need to edit `ItemEditor.cs` in order for your properties to be displayed in the Item Editor. You may choose to use the Inspector for editing the extended properties of classes derived from `Item` and use the Item Editor for the base properties if you wish not to manage an editor window.
### Inventory
The inventory Scriptable Object can be thought of as a container for an `Inventory` object. The `Inventory` uses a `Dictionary<Item, int>` which holds the items contained in the inventory and the associated amount of that item.
##### Code examples:
```csharp
[SerializeField] private InventorySO inventory = default;
// ...
// To check if the inventory has <count> of <item>:
    bool result = inventory.value.CheckItem(Item item, int count);
    
// To set the <count> of <item> in the inventory:
    inventory.value.SetItem(Item item, int count);
    
// To remove an <item> from the inventory:
    inventory.value.RemoveItem(Item item);
    
// To change the count of an <item> in the inventory by <amount>:
    inventory.value.ChangeItemCount(Item item, int amount);
    // Works with positive and negative values.
```
#### Saving and Loading
Saving and loading has successfully been done using a binary formatter approach. Ensure that the object you serialize uses `SerializedInventory` and not `Inventory`.
##### Example code:
```csharp
using Schwer.ItemSystem;

[System.Serializable]
public class SaveData {
    private SerializableInventory inventory;

    // Construct new save data.
    public SaveData() {
        inventory = new SerializableInventory();
    }

    // Construct save data from SOs (parameters).
    public SaveData(InventorySO inventory) {
        this.inventory = inventory.value.Serialize();
    }

    // Load save data into SOs (parameters).
    public void Load(InventorySO inventory, ItemDatabase itemDB) {
        inventory.value = this.inventory.Deserialize(itemDB);
    }
}
```
```csharp
private SaveData ReadSaveDataFile(string filePath) {
    SaveData result = null;
    BinaryFormatter formatter = new BinaryFormatter();
    using (FileStream stream = new FileStream(filePath, FileMode.Open)) {
        try {
            result = formatter.Deserialize(stream) as SaveData;
        }
        catch (System.Runtime.Serialization.SerializationException) {
            Debug.Log("File at: " + filePath + " is incompatible.");
        }
    }
    return result;
}

public void WriteSaveDataFile(SaveData saveData, string filePath) {
    BinaryFormatter formatter = new BinaryFormatter();
    using (FileStream stream = new FileStream(filePath, FileMode.Create)) {
        formatter.Serialize(stream, saveData);
    }
}
```
