using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedDictionary : MonoBehaviour
{
    public static Dictionary<int, GameObject> s_SelectedDictionary = new Dictionary<int, GameObject>();

    public void AddSelected(GameObject _selected)
    {
        int id = _selected.GetInstanceID();

        if (!(s_SelectedDictionary.ContainsKey(id)))
        {
            s_SelectedDictionary.Add(id, _selected);
            _selected.AddComponent<SelectionComponent>();
        }
    }

    public void Deselect(int _id)
    {
        Destroy(s_SelectedDictionary[_id].GetComponent<SelectionComponent>());
        s_SelectedDictionary.Remove(_id);
    }
    public void DeselectAll()
    {
        foreach(KeyValuePair<int, GameObject> kvPair in s_SelectedDictionary)
        {
            if(kvPair.Value != null)
            {
                Destroy(s_SelectedDictionary[kvPair.Key].GetComponent<SelectionComponent>());
            }
        }
        s_SelectedDictionary.Clear();
    }
}
