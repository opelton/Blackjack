using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public struct Card {
    public int suitIndex { get; private set; }
    public int value { get; private set; }
    public Card(int pointValue, int suit) {
        value = pointValue;
        suitIndex = suit;
    }
}

public class Deck {
    // actual cards remain fixed, list of indices is changed
    private Card[] allCards;
    private List<int> cardIndices;

    public int StartingCount { get { return allCards.Count(); } }
    public int Count { get { return cardIndices.Count(); } }

    public Deck(DeckSettings settings) {
        LoadDeck(settings);
    }

    public Card Draw() {
        if (cardIndices.IsNullOrEmpty()) {
            Debug.Log("if I were spending more time on this, I wouldn't just give you a valid card worth 0 to represent an error unless I hated you");
            return new Card(0, 0);
        }

        if(Count < 1) {
            ResetDeck();
            Shuffle();
        }

        var cardIndex = cardIndices.First();
        cardIndices.RemoveAt(0);

        return allCards[cardIndex];
    }

    // deck back in starting order, discarded cards are returned
    public void ResetDeck() {
        cardIndices = Enumerable.Range(0, allCards.Length).ToList();
    }

    void LoadDeck(DeckSettings settings) {
        var newCards = new List<Card>();

        // clone base card values for each suit
        for (int suitId = 0; suitId < settings.suitCount; ++suitId) {
            foreach (var baseValue in settings.baseValues) {
                newCards.Add(new Card(baseValue, suitId));
            }
        }

        LoadDeck(newCards);
    }

    public void LoadDeck(IEnumerable<Card> cards) {
        allCards = cards.ToArray();
        ResetDeck();
    }

    // randomize without returning drawn/discarded cards
    public void Shuffle() {
        cardIndices = cardIndices.Randomize().ToList();
    }
}