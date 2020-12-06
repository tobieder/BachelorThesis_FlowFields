using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
    public static Dictionary<int, GameObject> selectedDictionary = new Dictionary<int, GameObject>();

    public void AddSelected(GameObject _selected)
    {
        int id = _selected.GetInstanceID();

        if (!(selectedDictionary.ContainsKey(id)))
        {
            selectedDictionary.Add(id, _selected);
            _selected.AddComponent<SelectionComponent>();
        }
    }

    public void Deselect(int _id)
    {
        Destroy(selectedDictionary[_id].GetComponent<SelectionComponent>());
        selectedDictionary.Remove(_id);
    }
    public void DeselectAll()
    {
        foreach(KeyValuePair<int, GameObject> kvPair in selectedDictionary)
        {
            if(kvPair.Value != null)
            {
                Destroy(selectedDictionary[kvPair.Key].GetComponent<SelectionComponent>());
            }
        }
        selectedDictionary.Clear();
    }
}
