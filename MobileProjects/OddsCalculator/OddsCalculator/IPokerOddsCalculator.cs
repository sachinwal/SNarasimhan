using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards;

namespace OddsCalculator
{
    public interface IPokerOddsCalculator
    {

        void CalculateOddsForLow(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards);
        void CalculateOddsForHigh(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards);
        void OverallEquity(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards);

    }
}
