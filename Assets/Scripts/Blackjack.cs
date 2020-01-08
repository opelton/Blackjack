using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blackjack : MonoBehaviour {
    public DeckSettings deckSettings;

    Deck cardDeck;
    Player player;
    Player dealer;

    public Text playerDisplay;
    public Text dealerDisplay;
    public Text currentPotDisplay;
    public Text betSliderDisplay;
    public Text statusText;

    public int minBet = 200;
    public int maxBet = 5000;
    int currentPot = 0;
    int betSliderAmount;
    bool betweenRounds = true;

    public Button hitButton;
    public Button standButton;
    public Button betButton;
    public Slider betSlider;

    void Start() {
        cardDeck = new Deck(deckSettings);
        cardDeck.Shuffle();
        dealer = new Player("Dealer", 10000);
        player = new Player("Player", 10000);

        betSlider.minValue = minBet;
        betSlider.maxValue = maxBet;
        betSlider.wholeNumbers = true;
        betSliderAmount = minBet;
        statusText.text = string.Empty;

        hitButton.onClick.AddListener(OnPlayerHit);
        standButton.onClick.AddListener(OnPlayerStand);
        betButton.onClick.AddListener(OnPlayerBet);
        betSlider.onValueChanged.AddListener(OnPlayerBetChanged);

        EndRound();
        statusText.text = "Place a bet to begin";
    }

    void Update() {

        playerDisplay.text = player.HandToString();
        dealerDisplay.text = dealer.HandToString();
        currentPotDisplay.text = string.Format("Prize: ${0}", currentPot);
        betSliderDisplay.text = "$" + betSliderAmount;
    }

    void OnPlayerHit() {
        player.currentHand.AddToHand(cardDeck.Draw(), true);
        CheckBust();
    }

    void OnPlayerStand() {
        CheckWinner();
    }

    void OnPlayerBet() {
        AddBet(player, betSliderAmount);

        if (betweenRounds) {
            BeginRound();
        }
    }
    void OnPlayerBetChanged(float newValue) { betSliderAmount = (int) newValue; }

    int roundCount = 0;

    void BeginRound() {
        betweenRounds = false;
        ++roundCount;
        player.RoundReset();
        dealer.RoundReset();
        statusText.text = "Round: " + roundCount;
        standButton.interactable = true;
        hitButton.interactable = true;
        betButton.interactable = false;

        // collect dealer's pitiful bet
        AddBet(dealer, minBet);

        // deal cards
        dealer.currentHand.AddToHand(cardDeck.Draw(), true);
        player.currentHand.AddToHand(cardDeck.Draw(), true);

        dealer.currentHand.AddToHand(cardDeck.Draw(), false);
        player.currentHand.AddToHand(cardDeck.Draw(), true);

        // wait for player to hit or stand
        CheckBust();
    }

    void CheckBust() {
        // dealer busts, player wins
        if (dealer.currentHand.HandStrength > 21) {
            player.score += currentPot;
            statusText.text = dealer.playerId + " busts!";
            EndRound();
        }

        // player busts, dealer wins
        if (player.currentHand.HandStrength > 21) {
            dealer.score += currentPot;
            statusText.text = player.playerId + " busts!";
            EndRound();
        }
    }

    void CheckWinner() {
        dealer.currentHand.RevealHand();

        if (player.currentHand.HandStrength > dealer.currentHand.HandStrength) {
            player.score += currentPot;
            statusText.text = player.playerId + " wins with " + player.currentHand.HandStrength + "!";
        } else {
            dealer.score += currentPot;
            statusText.text = dealer.playerId + " wins with " + dealer.currentHand.HandStrength + "!";
        }
        EndRound();
    }

    void AddBet(Player betMaker, int betAmount) {
        betMaker.score -= betAmount;
        currentPot += betAmount;
    }

    void DisplayScore(Player playerScore) {
        Debug.Log(string.Format("{0} has hand strength {1}", playerScore.playerId, playerScore.currentHand.HandStrength));
    }

    void EndRound() {
        currentPot = 0;
        betweenRounds = true;
        standButton.interactable = false;
        hitButton.interactable = false;
        betButton.interactable = true;
    }
}