using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharacterClone : MonoBehaviour
{
    public const float TIME_TO_REACH = 2f; //Секунды

    [SerializeField] private Text CharacterText;

    private Vector3 _target;

    public void SetCharacter(char c)
    {
        CharacterText.text = c.ToString();
    }

    public void StartMoving(Vector3 target)
    {
        _target = target;
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (transform.position != _target)
        {
            transform.position = Vector3.Lerp(transform.position, _target, Time.deltaTime * (TIME_TO_REACH + 1f)); // +1 для плавности анимации
            yield return null;
        }
    }
}
