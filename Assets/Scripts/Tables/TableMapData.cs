using System.Collections.Generic;
using Game.Systems;
using GCL.Serialization;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace Game.Tables {
	public class TableMapdatElementItem : DataObject {
		public int index = 0;
		public int id = 0;
		public Vector2 position = Vector2.zero;
	}
	public class TableMapdatElement : DataObject {
		public Vector2Int gird = Vector2Int.zero;
		public List<TableMapdatElementItem> items = new List<TableMapdatElementItem>();

	}
	public class TableMapdat : TableBase<TableMapdat, int, TableMapdatElement> {
		public override void Load() {

			var text = AssetSystem.Load<TextAsset>("mapdata", "normal_index")?.text;
			if (string.IsNullOrEmpty(text)) {
				return;
			}

			var jtoken = JSONTool.ParseToToken(text);
			if (jtoken == null) {
				return;
			}
			JTokenHelper.ListArrayValue(jtoken, (JToken item) => {
				var id = JTokenHelper.GetInt(item, "id", -1);
				if (id == -1) {
					return;
				}
				var file = JTokenHelper.GetString(item, "file", "");
				if (!string.IsNullOrEmpty(file)) {
					var element = LoadElement(file);
					if (element) {
						Emplace(id, element);
					}
				}
			});
		}

		public static TableMapdatElement LoadElement(string filename) {
			var text = AssetSystem.Load<TextAsset>("mapdata", filename)?.text;
			if (string.IsNullOrEmpty(text)) {
				return null;
			}
			var jtoken = JSONTool.ParseToToken(text);
			if (jtoken == null) {
				return null;
			}
			var element = new TableMapdatElement();
			var grid = JTokenHelper.GetToken(jtoken, "grid");
			if (grid != null) {
				element.gird.Set(
					JTokenHelper.GetInt(grid, "x", 0),
					JTokenHelper.GetInt(grid, "y", 0));
			}
			JTokenHelper.ListArrayValue(jtoken, "data", (JToken item) => {
				var elementItem = new TableMapdatElementItem();
				elementItem.index = JTokenHelper.GetInt(item, 0, 0);
				elementItem.id = JTokenHelper.GetInt(item, 1, 0);
				elementItem.position.Set(
					JTokenHelper.GetFloat(item, 2, 0),
					JTokenHelper.GetFloat(item, 3, 0));
				element.items.Add(elementItem);
			});
			return element;
		}
	}
}