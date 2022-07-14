using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class Grid : MonoBehaviour
{
    public static Grid instance { get; private set; }

    [SerializeField] private GameObject ScreenCharacterPrefab;
    [SerializeField] private GameObject CharacterClonePrefab;
    [SerializeField] private Transform CharacterCloneParent;

    public int ColCount { get; private set; } = 0;
    public int RowCount { get; private set; } = 0;

    private GridLayoutGroup _grid;
    private ScreenCharacter[,] _characterMatrix;
    private bool _isShuffling = false;

    private void Start()
    {
        if (instance != null)
            throw new System.Exception("Two grid instances on scene!");
        instance = this;

        _grid = GetComponent<GridLayoutGroup>();
    }

    public void GenerateField(int width, int height)
    {
        if (width <= 0 || height <= 0 || _isShuffling)
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
        //const string alphabet = "àáâãäå¸æçèéêëìíîïðñòóôõö÷øùúûüýþÿ";
        const string alphabet = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
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

    public void ShuffleElements()
    {
        if (ColCount <= 0 || RowCount <= 0 || _isShuffling)
            return;

        _isShuffling = true;
        int[] newIndexes = GetNewLinearIndexes();
        CharacterClone[] clonesForAnimation = CreateAnimationClones();
        ChangeCharacters(newIndexes);
        HideAllOriginalCharacters();
        StartCloneMoving(clonesForAnimation, newIndexes);
        StartCoroutine(ShowOriginalCharactersAndDeleteClonesAfterMovingClones(clonesForAnimation));
    }
    private int[] GetNewLinearIndexes()
    {
        List<int> avaibleLinearIndexes = new List<int>();
        for (int i = 0; i < RowCount * ColCount; i++)
        {
            avaibleLinearIndexes.Add(i);
        }

        int[] newIndexes = new int[RowCount * ColCount];
        for (int i = 0; i < newIndexes.Length; i++)
        {
            int avaibleIndexPosition = Random.Range(0, avaibleLinearIndexes.Count);
            newIndexes[i] = avaibleLinearIndexes[avaibleIndexPosition];
            avaibleLinearIndexes.RemoveAt(avaibleIndexPosition);
        }
        return newIndexes;
    }
    private CharacterClone[] CreateAnimationClones()
    {
        CharacterClone[] result = new CharacterClone[RowCount * ColCount];
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                Vector3 position = _characterMatrix[r, c].transform.position;
                GameObject cloneObj = Instantiate(CharacterClonePrefab, position, Quaternion.identity, CharacterCloneParent);
                CharacterClone cloneScript = cloneObj.GetComponent<CharacterClone>();
                cloneScript.SetCharacter(_characterMatrix[r, c].Character);
                RectTransform rectTransform = cloneObj.GetComponent<RectTransform>();
                Vector2 size = _characterMatrix[r, c].GetComponent<RectTransform>().rect.size;
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
                rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);
                result[ColCount * r + c] = cloneScript;
            }
        }
        return result;
    }
    private void ChangeCharacters(int[] newIndexes)
    {
        char[] oldCharacters = new char[RowCount * ColCount];
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                oldCharacters[ColCount * r + c] = _characterMatrix[r, c].Character;
            }
        }

        for (int i = 0; i < newIndexes.Length; i++)
        {
            int index = newIndexes[i];
            _characterMatrix[index / ColCount, index - (ColCount * (index / ColCount))].SetData(oldCharacters[i]);
        }
    }
    private void HideAllOriginalCharacters()
    {
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                _characterMatrix[r, c].gameObject.SetActive(false);
            }
        }
    }
    private void StartCloneMoving(CharacterClone[] clones, int[] newIndexes)
    {
        for (int i = 0; i < clones.Length; i++)
        {
            int row = newIndexes[i] / ColCount;
            int col = newIndexes[i] - (ColCount * (newIndexes[i] / ColCount));
            clones[i].StartMoving(_characterMatrix[row, col].transform.position);
        }
    }
    private IEnumerator ShowOriginalCharactersAndDeleteClonesAfterMovingClones(CharacterClone[] clones)
    {
        yield return new WaitForSecondsRealtime(CharacterClone.TIME_TO_REACH);

        ShowAllOriginalCharacters();
        foreach (Transform child in CharacterCloneParent)
        {
            Destroy(child.gameObject);
        }
        _isShuffling = false;
    }
    private void ShowAllOriginalCharacters()
    {
        for (int r = 0; r < RowCount; r++)
        {
            for (int c = 0; c < ColCount; c++)
            {
                _characterMatrix[r, c].gameObject.SetActive(true);
            }
        }
    }
}
