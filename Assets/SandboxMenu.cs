using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SandboxMenu : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField m_WidthHeightInputField;

    public void SetMapSize(int _mapSize)
    {
        GameManager.instance.SetSelectedMapSize(_mapSize);
    }

    public void StartSandbox()
    {
        int mapSize;
        if (int.TryParse(m_WidthHeightInputField.text, out mapSize))
        {
            if (m_WidthHeightInputField.text == "")
            {
                StartCoroutine(FlashPreviewWarning());
            }
            else if (mapSize <= 0)
            {
                m_WidthHeightInputField.text = "";
                StartCoroutine(FlashPreviewWarning());
            }
            else
            {
                SetMapSize(mapSize);
                GameManager.instance.LoadSandbox();
            }
        }
        else
        {
            StartCoroutine(FlashPreviewWarning());
            Debug.LogError("Parsing the Input value was NOT successful.");
        }
    }

    public IEnumerator FlashPreviewWarning()
    {
        for(int i = 0; i < 3; i++)
        {
            m_WidthHeightInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "<color=red>Gültige Größe eingben!</color>";
            yield return new WaitForSeconds(0.5f);
            m_WidthHeightInputField.placeholder.GetComponent<TextMeshProUGUI>().text = "<color=grey>Gültige Größe eingben!</color>";
            yield return new WaitForSeconds(0.5f);
        }
    }
}
