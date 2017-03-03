using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards;

namespace OddsCalculator
{
    public enum HandHierarchy
    {
        HighCard = 1,
        OnePair = 2,
        TwoPair = 3,
        ThreeOfAKind = 4,
        Straigth = 5,
        Flush = 6,
        FullHouse = 7,
        FourOfAKind = 8,
        StraightFlush = 9,
        RoyalFlush = 10
    }

    public class BestHand
    {
        int[] top5CardsRanks;
        HandHierarchy handStrength;
    }

    public static class PokerHandHierarchy
    {
        public static BestHand ComputeBestHandPossible(List<Card> playerCards, List<Card> boardCards)
        {
            BestHand bestHand = new BestHand();

            return bestHand;
        }
    }
}
