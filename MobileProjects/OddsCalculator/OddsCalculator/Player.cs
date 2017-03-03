using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards;

namespace OddsCalculator
{
    public class Player
    {
        int playerNo;
        Card[] playerCards;
        Decimal oddsOfWinningHigh = 0.0M;
        Decimal oddsOfWinningLow = 0.0M;
        Decimal oddsOfTieingHigh = 0.0M;
        Decimal oddsOfTieingLow = 0.0M;
        Decimal overallEquity = 0.0M;
        long numberOfLowWinningHands = 0;
        long numberOfHighWinningHands = 0;
        // TODO:number of tieing hands might be the wrong way to compute equityu for more than 2 tied players
        long numberOfLowTieingHands = 0;
        long numberOfHighTieingHands = 0;


        public Player(int playerNo, Card[] playerCards)
        {
            this.playerNo = playerNo;
            this.playerCards = playerCards;
        }

        public int PlayerNo { get { return playerNo; } set { PlayerNo = value; } }

        public Card[] PlayerCards { get { return playerCards; } set { playerCards = value; } }

        public Decimal OddsOfWinningHigh { get { return oddsOfWinningHigh; } set { oddsOfWinningHigh = value; } }

        public Decimal OddsOfWinningLow { get { return oddsOfWinningLow; } set { oddsOfWinningLow = value; } }

        public Decimal OddsOfTieingHigh { get { return oddsOfTieingHigh; } set { oddsOfTieingHigh = value; } }

        public Decimal OddsOfTieingLow { get { return oddsOfTieingLow; } set { oddsOfTieingLow = value; } }


        public Decimal OverallEquity { get { return overallEquity; } set { overallEquity = value; } }

        public long NumberOfLowWinningHands { get { return numberOfLowWinningHands; } set { numberOfLowWinningHands = value; } }

        public long NumberOfHighWinningHands { get { return numberOfHighWinningHands; } set { numberOfHighWinningHands = value; } }

        public long NumberOfLowTieingHands { get { return numberOfLowTieingHands; } set { numberOfLowTieingHands = value; } }

        public long NumberOfHighTieingHands { get { return numberOfHighTieingHands; } set { numberOfHighTieingHands = value; } }

    }
}
