using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScreenCharacter : MonoBehaviour
{
    [SerializeField] private Text CharacterText;

    public char Character { get; private set; }

    public void SetData(char character)
    {
        Character = character;

        CharacterText.text = character.ToString();
    }
}
