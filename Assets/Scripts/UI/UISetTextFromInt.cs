using UnityEngine;
using UnityEngine.UI;

public class UISetTextFromInt : MonoBehaviour
{
    Text text;

    private void Awake()
    {
        text = GetComponent<Text>();
    }

    public void SetTextFromInt(int value)
    {
        text.text = value.ToString().PadLeft(2, '0');
    }
}
