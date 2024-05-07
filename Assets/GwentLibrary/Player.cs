using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Dynamic;
using System.Net.WebSockets;
using System;
using Unity.VisualScripting;

public class Player
{
    public string PlayerName { get; private set; }      // nombre del jugador
    public string PlayerFaction { get; private set; }       // facción del jugador
    public Card PlayerLeader { get; private set; }      // carta líder de la facción del jugador
    public Hand PlayerHand { get; private set; }        // mano del jugador
    public PlayerBattlefield Battlefield { get; private set; }      // parte del campo de batalla perteneciente al jugador
    public bool HasPassed { get; private set; }     // booleano para indicar si el jugador ha pasado la ronda
    public int GamesWon { get; private set; }       // cantidad de rondas ganadas por el jugador
    public bool IsActive { get; private set; }      // booleano para indicar si es el turno del jugador

    /// <summary>
    /// Constructor de la clase Player.
    /// </summary>
    /// <param name="name">Nombre del jugador.</param>
    /// <param name="faction">Facción a la que pertenecen las cartas en el mazo.</param>
    /// <param name="initialDeck">Mazo elegido por el jugador.</param>
    public Player(string name, string faction, DeckCreator initialDeck)
    {
        PlayerName = name;
        PlayerFaction = faction;
        PlayerLeader = initialDeck.DeckLeader;
        PlayerHand = new Hand(initialDeck);
        Battlefield = new();
        HasPassed = false;
        GamesWon = 0;
        IsActive = false;
    }

    /// <summary>
    /// Este método activa el efecto del líder de la facción.
    /// </summary>
    /// <param name="activePlayer">El jugador activo al momento de la llamada del método.</param>
    /// <param name="rivalPlayer">El jugador rival al momento de la llamada del método.</param>
    public void UseLeaderSkill(Player activePlayer, Player rivalPlayer)
    {
        PlayerLeader.ActivateEffect(activePlayer, rivalPlayer, PlayerLeader);
        activePlayer.Battlefield.UpdateBattlefieldInfo();
        rivalPlayer.Battlefield.UpdateBattlefieldInfo();
    }

    /// <summary>
    /// Este método se llama cuando el jugador elige pasar turno.
    /// </summary>
    public void PassTurn()
    {
        HasPassed = true;
        IsActive = false;
    }

    /// <summary>
    /// Este método añade una victoria al jugador.
    /// </summary>
    public void AddVictory()
    {
        GamesWon = GamesWon + 1;
    }

    /// <summary>
    /// Este método se llama para iniciar el turno del jugador.
    /// </summary>
    public void StartTurn()
    {
        IsActive = true;
        Debug.Log($"El turno de {PlayerName} ha comenzado");
    }

    /// <summary>
    /// Este método se llama para finalizar el turno del jugador.
    /// </summary>
    public void FinishTurn()
    {
        IsActive = false;
        Debug.Log($"El turno de {PlayerName} ha finalizado");
    }

    /// <summary>
    /// Este método se llama para iniciar una nueva ronda.
    /// </summary>
    public void NewRound()
    {
        HasPassed = false;
    }

    /// <summary>
    /// Este método maneja el movimiento de una carta hacia un lugar del tablero.
    /// </summary>
    /// <param name="activePlayer">El jugador activo al momento de la llamada del método.</param>
    /// <param name="rivalPlayer">El jugador rival al momento de la llamada del método.</param>
    /// <param name="card">Carta que será manipulada.</param>
    /// <param name="Row">Lugar al que se moverá la carta.</param>
    public void DragAndDropMovement(Player activePlayer, Player rivalPlayer, Card card, string Row)
    {
        if (Row == "M" || Row == "R" || Row == "S")
        {
            if (card is UnityCard unityCard)
            {
                Battlefield.Battlefield[Battlefield.RowCorrespondency[Row]].Add(unityCard);
                PlayerHand.PlayerHand.Remove(unityCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();

                unityCard.ActivateEffect(activePlayer, rivalPlayer, unityCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();
            }

            if (card is IncreaseCard increaseCard)
            {
                Battlefield.IncreaseColumn[Battlefield.RowCorrespondency[Row]] = increaseCard;
                PlayerHand.PlayerHand.Remove(increaseCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();

                increaseCard.ActivateEffect(activePlayer, rivalPlayer, increaseCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();
            }

            if (card is WeatherCard weatherCard)
            {
                PlayerBattlefield.WeatherRow[Battlefield.RowCorrespondency[Row]] = weatherCard;
                PlayerHand.PlayerHand.Remove(weatherCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();

                weatherCard.ActivateEffect(activePlayer, rivalPlayer, weatherCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();
            }

            if (card is ClearanceCard clearanceCard)
            {
                PlayerHand.PlayerHand.Remove(clearanceCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();

                clearanceCard.ActivateEffect(activePlayer, rivalPlayer, clearanceCard);

                activePlayer.Battlefield.UpdateBattlefieldInfo();
                rivalPlayer.Battlefield.UpdateBattlefieldInfo();
            }

            Debug.Log($"El puntaje de la fila Melee de {activePlayer.PlayerName} es {activePlayer.Battlefield.MeleeRowScore}");
            Debug.Log($"El puntaje de la fila Ranged de {activePlayer.PlayerName} es {activePlayer.Battlefield.RangedRowScore}");
            Debug.Log($"El puntaje de la fila Sigee de {activePlayer.PlayerName} es {activePlayer.Battlefield.SigeeRowScore}");
            Debug.Log($"El puntaje total de {activePlayer.PlayerName} es {activePlayer.Battlefield.TotalScore}");

            Debug.Log($"El puntaje de la fila Melee de {rivalPlayer.PlayerName} es {rivalPlayer.Battlefield.MeleeRowScore}");
            Debug.Log($"El puntaje de la fila Ranged de {rivalPlayer.PlayerName} es {rivalPlayer.Battlefield.RangedRowScore}");
            Debug.Log($"El puntaje de la fila Sigee de {rivalPlayer.PlayerName} es {rivalPlayer.Battlefield.SigeeRowScore}");
            Debug.Log($"El puntaje total de {rivalPlayer.PlayerName} es {rivalPlayer.Battlefield.TotalScore}");
        }

        else
            throw new ArgumentException("Parámetro de entrada Row incorrecto");
    }

    /// <summary>
    /// Este método limpia todo el campo de batalla del jugador.
    /// </summary>
    public void GetNewBattlefield()
    {
        Battlefield.ClearBattelfield();
        Battlefield.UpdateBattlefieldInfo();
    }
}


