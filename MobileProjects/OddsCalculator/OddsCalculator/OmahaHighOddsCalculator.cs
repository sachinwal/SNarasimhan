using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards;

namespace OddsCalculator
{
    public class OmahaHighOddsCalculator : PokerOddsCalculator
    {
        public OmahaHighOddsCalculator()
            : base(false)
        {

        }

        private void Validate(List<Player> players, OddsCalculationTime calulcateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            foreach(Player player in players)
            {
                if(player.PlayerCards.Count() != 4)
                {
                    throw new ArgumentException("Each player should have four cards. Invalid data for player " + player.PlayerNo);
                }
            }
            int totalPlayerCards = players.Count * 4;
            if (totalPlayerCards + boardCards.Count + undealtCards.Count != CardDeck.NUMBEROFCARDSINDECK)
            {
                throw new ArgumentException("Invalid number of cards. Total cards do not add upto " + CardDeck.NUMBEROFCARDSINDECK);
            }
           
        }

        public override void CalculateOddsForLow(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            throw new NotSupportedException("Calculating low is not allowed for Omaha High games");
        }


        public override void CalculateOddsForHigh(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            Validate(players, calculateAt, boardCards, undealtCards);
            base.CalculateOddsForHigh(ref players, calculateAt, boardCards, undealtCards);
        }

        public override void OverallEquity(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            Validate(players, calculateAt, boardCards, undealtCards);
            base.CalculateOddsForHigh(ref players, calculateAt, boardCards, undealtCards);
        }
    }
}
