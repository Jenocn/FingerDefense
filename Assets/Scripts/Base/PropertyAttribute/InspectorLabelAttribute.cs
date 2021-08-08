using UnityEngine;

public class InspectorLabelAttribute : PropertyAttribute {
	public string name { get; private set; }

	public InspectorLabelAttribute(string name) {
		this.name = name;
	}
}