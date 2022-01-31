using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
  [SerializeField] private bool isPlayerUnit;

  public string unitName;
  public int unitLevel;
  public int dificulty;

  public int damage;
  public int healing;

  public int maxHP;
  public int currentHP;
  
  
  public bool IsPlayerUnit => isPlayerUnit;

  private SpriteRenderer image;
  private Vector3 originalPos;
  private Color originalColor;

  public void Awake()
  {
    image = GetComponentInChildren<SpriteRenderer>();
    originalPos = image.transform.localPosition;
    originalColor = image.color;
  }

  public bool TakeDamage(int dmg)
  {
    currentHP -= dmg;
    
    if (currentHP <= 0)
      return true;
    else
      return false;
  }

  public void Heal()
  {
    currentHP += healing;
    if (currentHP > maxHP)
      currentHP = maxHP;
  }
  
  public void PlayEnterAnimation()
  {
    if (isPlayerUnit)
      image.transform.localPosition = new Vector3(-500, originalPos.y);
    else
      image.transform.localPosition = new Vector3(500f, originalPos.y);

    image.transform.DOLocalMoveX(originalPos.x, 1f);
  }
  
  public void PlayAttackAnimation()
  {
    var sequence = DOTween.Sequence();
    if (isPlayerUnit)
      sequence.Append(image.transform.DOLocalMoveX(originalPos.x + 2f, 0.2f));
    else
      sequence.Append(image.transform.DOLocalMoveX(originalPos.x - 2f, 0.2f));

    sequence.Append(image.transform.DOLocalMoveX(originalPos.x, 0.2f));
  }

  public void PlayHitAnimation()
  {
    var sequence = DOTween.Sequence();
    sequence.Append(image.DOColor(Color.gray, 0.1f));
    sequence.Append(image.DOColor(originalColor, 0.1f));
  }

  public void PlayFaintAnimation()
  {
    var sequence = DOTween.Sequence();
    sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
    sequence.Join(image.DOFade(0f, 0.5f));
  }
  
  public void PlayExitAnimation()
  {
    var sequence = DOTween.Sequence();
    sequence.Append(image.transform.DOLocalMoveY(originalPos.y - 150f, 0.5f));
    sequence.Join(image.DOFade(0f, 0.5f));
  }

}