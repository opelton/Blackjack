using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class PlayerHand {
    public List<Card> hiddenCards;
    public List<Card> visibleCards;

    public PlayerHand() {
         Clear();
    }

    public void Clear() {
        hiddenCards = new List<Card>();
        visibleCards = new List<Card>();
    }

    public void AddToHand(Card card, bool faceUp = false) {
        if (faceUp) {
            visibleCards.Add(card);
        } else {
            hiddenCards.Add(card);
        }
    }

    public void RevealHand() {
        visibleCards.AddRange(hiddenCards);
        hiddenCards.Clear();
    }

    public int HandStrength {
        get {
            var score = 0;

            foreach (var card in hiddenCards) {
                score += card.value;
            }

            foreach (var card in visibleCards) {
                score += card.value;
            }

            return score;
        }
    }
}

public class Player {
    public PlayerHand currentHand;
    public int score;
    public string playerId;

    public Player(string id, int startingScore) {
        playerId = id;
        score = startingScore;
        currentHand = new PlayerHand();
    }

    public void RoundReset() { 
        currentHand.Clear();
    }

    public string HandToString() {
        var sb = new StringBuilder();
        sb.AppendLine(string.Format("{0} ${1}", playerId, score));

        foreach (var visible in currentHand.visibleCards) {
            sb.Append("[");
            sb.Append(visible.value);
            sb.Append("] ");
        }

        foreach(var hidden in currentHand.hiddenCards) { 
            sb.Append("[??] ");
        }

        if(currentHand.HandStrength > 21) { 
            sb.AppendLine("BUST!");
        }
        return sb.ToString();
    }
}