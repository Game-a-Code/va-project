using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { Start, Busy, PlayerTurn, EnemyTurn, Won, Lost }

public class BattleSystem : MonoBehaviour
{
  public GameObject playerPrefab;

  public Transform playerBattleStation;
  public Transform enemyBattleStation;

  BattleUnit playerUnit;
  BattleUnit enemyUnit;

  public Text dialogueText;

  public BattleHud playerHUD;
  public BattleHud enemyHUD;
  
  public BattleState state;
  
  public event Action<bool> OnBattleOver;

  public void StartBattle(EnemyController enemyCon)
  {
    state = BattleState.Start;
    StartCoroutine(SetupBattle(enemyCon));
  }

  IEnumerator SetupBattle(EnemyController enemyCon)
  {
    GameObject playerGo = Instantiate(playerPrefab, playerBattleStation);
    playerUnit = playerGo.GetComponent<BattleUnit>();
    playerUnit.PlayEnterAnimation();

    GameObject enemyGo = Instantiate(enemyCon.EnemyPrefab, enemyBattleStation);
    enemyUnit = enemyGo.GetComponent<BattleUnit>();
    enemyUnit.PlayEnterAnimation();

    dialogueText.text = "A wild " + enemyUnit.unitName + " appproaches...";
    
    playerHUD.SetHUD(playerUnit);
    enemyHUD.SetHUD(enemyUnit);

    yield return new WaitForSeconds(2f);
    
    state = BattleState.PlayerTurn;
    PlayerTurn();
  }

  IEnumerator PlayerAttack()
  {
    state = BattleState.Busy;
    
    playerUnit.PlayAttackAnimation();
    enemyUnit.PlayHitAnimation();
    
    bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
    
    enemyHUD.SetHP(enemyUnit.currentHP);
    dialogueText.text = "The attack is successful!";

    yield return new WaitForSeconds(2f);

    if (isDead)
    {
      enemyUnit.PlayFaintAnimation();
      state = BattleState.Won;
      EndBattle(true);
    }
    else
    {
      state = BattleState.EnemyTurn;
      StartCoroutine(EnemyTurn());
    }
  }

  IEnumerator EnemyTurn()
  {
    state = BattleState.Busy;
    dialogueText.text = enemyUnit.unitName + " attacks!";
    
    enemyUnit.PlayAttackAnimation();
    playerUnit.PlayHitAnimation();

    yield return new WaitForSeconds(1f);

    bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
    
    playerHUD.SetHP(playerUnit.currentHP);
    
    yield return new WaitForSeconds(1f);

    if (isDead)
    {
      playerUnit.PlayFaintAnimation();
      state = BattleState.Lost;
      EndBattle(false);
    }
    else
    {
      state = BattleState.PlayerTurn;
      PlayerTurn();
    }
  }

  void EndBattle(bool won)
  {
    if (state == BattleState.Won)
    {
      dialogueText.text = "You won the battle!";
      playerUnit.PlayExitAnimation();
      OnBattleOver(won);
    }
    else if (state == BattleState.Lost)
    {
      dialogueText.text = "You were defeated.";
      enemyUnit.PlayExitAnimation();
      OnBattleOver(won);
    }
  }
  
  void PlayerTurn()
  {
    dialogueText.text = "Choose an action:";
  }

  IEnumerator PlayerHeal()
  {
    playerUnit.Heal();
    
    playerHUD.SetHP(playerUnit.currentHP);
    dialogueText.text = "You feel renewed strength!";

    yield return new WaitForSeconds(2f);

    state = BattleState.EnemyTurn;
    StartCoroutine(EnemyTurn());
  }

  public void OnAttackButton()
  {
    if (state !=BattleState.PlayerTurn)
      return;

    StartCoroutine(PlayerAttack());
  }
  
  public void OnHealButton()
  {
    if (state !=BattleState.PlayerTurn)
      return;

    StartCoroutine(PlayerHeal());
  }
}