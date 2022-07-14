using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class Grid : MonoBehaviour
{
    public static Grid instance { get; private set; }

    [SerializeField] private GameObject ScreenCharacterPrefab;

    public int ColCount { get; private set; } = 0;
    public int RowCount { get; private set; } = 0;

    private GridLayoutGroup _grid;
    private ScreenCharacter[,] _characterMatrix;

    private void Start()
    {
        if (instance != null)
            throw new System.Exception("Two grid instances on scene!");
        instance = this;

        _grid = GetComponent<GridLayoutGroup>();
    }

    public void GenerateField(int width, int height)
    {
        if (width <= 0 || height <= 0)
            return;

        ClearCharacterMatrix();

        RowCount = height;
        ColCount = width;
        ResizeGridElements();
        char[,] chars = GenerateCharMatrix();
        _characterMatrix = CreateCharLabels(chars);
    }
    private void ClearCharacterMatrix()
    {
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                Destroy(_characterMatrix[r, c].gameObject);
            }
        }

        _characterMatrix = null;
        RowCount = 0;
        ColCount = 0;
    }
    private void ResizeGridElements()
    {
        RectTransform rectTransform = _grid.GetComponent<RectTransform>();

        int count = ColCount < RowCount ? RowCount : ColCount;
        float width = rectTransform.rect.width;
        width -= _grid.padding.left + _grid.padding.right;
        width -= _grid.spacing.x * (count - 1);
        float size = width / count;
        _grid.cellSize = new Vector2(size, size);
    }
    private char[,] GenerateCharMatrix()
    {
        char[,] matrix = new char[RowCount, ColCount];
        for (int c = 0; c < ColCount; c++)
        {
            for (int r = 0; r < RowCount; r++)
            {
                matrix[r, c] = GetRandomCharacter();
            }
        }
        return matrix;
    }
    private char GetRandomCharacter()
    {
        const string alphabet = "àáâãäå¸æçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
        return alphabet[Random.Range(0, alphabet.Length - 1)];
    }
    private ScreenCharacter[,] CreateCharLabels(char[,] characterMatrix)
    {
        ScreenCharacter[,] result = new ScreenCharacter[RowCount, ColCount];

        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                GameObject newCharObject = Instantiate(ScreenCharacterPrefab, _grid.transform);
                ScreenCharacter characterScript = newCharObject.GetComponent<ScreenCharacter>();
                characterScript.SetData(characterMatrix[r, c]);
                result[r, c] = characterScript;
            }
        }

        return result;
    }

}
