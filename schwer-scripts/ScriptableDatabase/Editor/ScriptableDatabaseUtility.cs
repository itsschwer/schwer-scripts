using UnityEditor;
using UnityEngine;

namespace SchwerEditor.Database {
    using Schwer.Database;

    public static class ScriptableDatabaseUtility<TDatabase, TElement>
        where TDatabase : ScriptableDatabase<TElement>
        where TElement : ScriptableObject {

        // [MenuItem("Generate ScriptableDatabase", false, -2), MenuItem("Assets/Create/ScriptableDatabase", false, -11)]
        public static void GenerateDatabase() {
            var db = GetDatabase();
            if (db == null) return;

            db.Initialise(AssetsUtility.FindAllAssets<TElement>());

            EditorUtility.SetDirty(db);
            AssetsUtility.SaveRefreshAndFocus();
            Selection.activeObject = db;
        }

        private static TDatabase GetDatabase() {
            var databases = AssetsUtility.FindAllAssets<TDatabase>();

            if (databases.Length < 1) {
                Debug.Log($"Creating a new {typeof(TDatabase).Name} since none exist.");
                return ScriptableObjectUtility.CreateAsset<TDatabase>();
            }
            else if (databases.Length > 1) {
                Debug.LogError($"Multiple `{typeof(TDatabase).Name}` exist. Please delete the extra(s) and try again.");
                return null;
            }
            else {
                return databases[0];
            }
        }
    }
}
