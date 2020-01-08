using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObjects/DeckSettings", fileName = "NewDeck")]
public class DeckSettings : ScriptableObject {
    public int[] baseValues;
    public int suitCount;
}