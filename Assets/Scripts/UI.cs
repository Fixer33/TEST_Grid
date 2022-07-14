using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private InputField WidthInput;
    [SerializeField] private InputField HeightInput;

    public void GenerateButton_Press()
    {
        string widthRaw = WidthInput.text;
        string heightRaw = HeightInput.text;

        if (int.TryParse(widthRaw, out int width) && int.TryParse(heightRaw, out int height))
        {
            Grid.instance.GenerateGrid(width, height);
        }
    }

    public void ShuffleButton_Press()
    {
        Grid.instance.ShuffleElements();
    }
}
