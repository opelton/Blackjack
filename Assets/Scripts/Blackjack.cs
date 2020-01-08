using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// blackjack where the twist is I don't let you do some of the hard stuff and also you can change the deck to some definitely-illegal ones
public class Blackjack : MonoBehaviour {
    public DeckSettings[] allDeckSettings;
    int selectedSettingsIndex = 0;
    DeckSettings deckSettings { get { return allDeckSettings[selectedSettingsIndex]; } }

    // Info displays
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

    // User controls
    public Button hitButton;
    public Button standButton;
    public Button betButton;
    public Slider betSlider;

    // some shady business here
    public Dropdown deckSelector;

    Deck cardDeck;
    Player player;
    Player dealer;

    void Start() {
        RestartGame();
    }

    // update info text boxes
    void Update() {
        playerDisplay.text = player.HandToString();
        dealerDisplay.text = dealer.HandToString();
        currentPotDisplay.text = string.Format("Prize: ${0}", currentPot);
        betSliderDisplay.text = "$" + betSliderAmount;
    }

    void RestartGame() {
        cardDeck = new Deck(deckSettings);
        cardDeck.Shuffle();
        dealer = new Player("Dealer", 1000000000);
        player = new Player("Player", 10000);

        // slider settings
        betSlider.minValue = minBet;
        betSlider.maxValue = maxBet;
        betSlider.wholeNumbers = true;
        betSliderAmount = minBet;
        statusText.text = string.Empty;

        // subscribe to buttons and sliders
        hitButton.onClick.AddListener(OnPlayerHit);
        standButton.onClick.AddListener(OnPlayerStand);
        betButton.onClick.AddListener(OnPlayerBet);
        betSlider.onValueChanged.AddListener(OnPlayerBetChanged);
        deckSelector.onValueChanged.AddListener(OnDeckSelectionToggled);

        // insert poetic statement about ends being beginnings
        EndRound();
        statusText.text = "Place a bet to begin";
    }

    #region callbacks

    // the deck is swapped out but play is not interrupted
    void OnDeckSelectionToggled(int newIndex) {
        selectedSettingsIndex = newIndex;
        cardDeck = new Deck(deckSettings);
        cardDeck.Shuffle();
    }

    // ironically this is also the only way to bust, silly players
    void OnPlayerHit() {
        player.currentHand.AddToHand(cardDeck.Draw(), true);
        CheckBust();
    }

    // oh, you think you can win now?
    void OnPlayerStand() {
        CheckWinner();
    }

    void OnPlayerBetChanged(float newValue) {
        betSliderAmount = (int) newValue;
    }

    void OnPlayerBet() {
        AddBet(player, betSliderAmount);

        // deals new cards if a round hasn't started yet
        if (betweenRounds) {
            BeginRound();
        }
    }
    #endregion

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

        // I know what the rules are, but this is a really lame blackjack so I choose to at least let players win the tie
        if (player.currentHand.HandStrength >= dealer.currentHand.HandStrength) {
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
        if (betMaker.score < 0) {
            betAmount = Math.Min(betMaker.score + betAmount, betAmount);
            betMaker.score = 0;
        }

        currentPot += betAmount;
    }

    // the old ways are no longer necessary
    // void DisplayScore(Player playerScore) {
    //     Debug.Log(string.Format("{0} has hand strength {1}", playerScore.playerId, playerScore.currentHand.HandStrength));
    // }

    void EndRound() {
        currentPot = 0;
        betweenRounds = true;

        // standing or hitting someone after a round of cards would be rude, so I don't let you
        standButton.interactable = false;
        hitButton.interactable = false;
        betButton.interactable = true;

        if (player.score <= 0) {
            statusText.text = "game over!";
        }
    }
}