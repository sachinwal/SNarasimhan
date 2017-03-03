using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeckOfCards;

namespace OddsCalculator
{
    public enum PokerHandHierarchy
    {
        HighCard = 0,
        OnePair = 1,
        TwoPair = 2,
        Trips = 3,
        Straight = 4,
        Flush = 5,
        FullHouse = 6,
        FourOfAKind = 7,
        StraightFlush = 8,
        RoyalFlush = 9
    }

    public static class PokerHandEvaluator
    {
        private static readonly int ACERANKFORPOKER = 14;
        public static IList<Player> DetermineWinningPlayers(IList<Player> players, IList<Card> boardCards)
        {
            //add validations if necesary
            List<Player> winningPlayers = new List<Player>();
            Player winningPlayerSoFar = players[0];
            int highestRankPossible = -1;
            int highestRankSoFar = -1;
            int lowRank = -1;
            Player winner = null;
            int rank1 = -1, rank2 = -1, rank3 = -1, rank4 = -1, rank5 = -1;
            var handValues = Enum.GetValues(typeof(PokerHandHierarchy)).Cast<PokerHandHierarchy>();

            foreach (PokerHandHierarchy handValue in handValues)
            {
                switch (handValue)
                {
                    case PokerHandHierarchy.RoyalFlush:
                        foreach (Player player in players)
                        {
                            //In omaha variants only one person can have royal flush
                            if (IsRoyalFlushPossible(player.PlayerCards, boardCards))
                            {
                                winningPlayers.Add(player);
                                return winningPlayers;
                            }
                        }
                        break;
                    case PokerHandHierarchy.StraightFlush:
                        highestRankPossible = -1;
                        winner = null;
                        foreach (Player player in players)
                        {
                            if ((highestRankPossible = HighestStraightFlushRankIfPossible(player.PlayerCards, boardCards)) != -1)
                            {
                                winner = player;
                            }
                        }
                        //Only one striaght flush possible
                        if(highestRankPossible != -1)
                        {
                            winningPlayers.Add(winner);
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.FourOfAKind:
                        highestRankPossible = -1;
                        winner = null;
                        int currIterHighestRankPossible = -1;
                        foreach (Player player in players)
                        {
                            //In omaha variants only one person can have royal flush
                            if ((currIterHighestRankPossible = HighestFourOfAkindIfPossible(player.PlayerCards, boardCards)) != -1)
                            {
                                //Ace (Rank = 1) is the highest and only one four of a kind possible
                                if (currIterHighestRankPossible == 1 || currIterHighestRankPossible > highestRankPossible)
                                {
                                    highestRankPossible = currIterHighestRankPossible;
                                    winner = player;
                                }
                            }
                        }
                        if (highestRankPossible != -1)
                        {
                            winningPlayers.Add(winner);
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.FullHouse:
                        int highRankSoFar = -1;
                        int lowRankSoFar = -1;
                        highestRankPossible = -1;
                        winner = null;
                        lowRank = -1;
                        foreach (Player player in players)
                        {
                            if (IsFullHousePossible(player.PlayerCards, boardCards, ref highestRankPossible, ref lowRank))
                            {
                                //treat Ace Rank as 14, makes it easier for calculation and does not affect any logic
                                if(highestRankPossible == 1)
                                {
                                    highestRankPossible = ACERANKFORPOKER;
                                }
                                if(lowRank == 1)
                                {
                                    lowRank = ACERANKFORPOKER;
                                }
                                if(winner == null)
                                {
                                    highRankSoFar = highestRankPossible;
                                    lowRankSoFar = lowRank;
                                    winner = player;
                                    winningPlayers.Clear();
                                    winningPlayers.Add(player);
                                }
                                else
                                {
                                   if(highestRankPossible > highRankSoFar)
                                   {
                                        highRankSoFar = highestRankPossible;
                                        lowRankSoFar = lowRank;
                                        winner = player;
                                        winningPlayers.Clear();
                                        winningPlayers.Add(player);
                                    }
                                   else if(highestRankPossible == highRankSoFar)
                                    {
                                        if(lowRank > lowRankSoFar)
                                        {
                                            lowRankSoFar = lowRank;
                                            winner = player;
                                            winningPlayers.Clear();
                                            winningPlayers.Add(player);
                                        }
                                        else if(lowRank == lowRankSoFar)
                                        {
                                            winner = player;
                                            //two players could have the same full house
                                            winningPlayers.Add(player);
                                        }
                                    }
                                }
                               
                            }
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.Flush:
                        rank1 = -1;
                        rank2 = -1;
                        rank3 = -1;
                        rank4 = -1;
                        rank5 = -1;
                        highestRankSoFar = -1;
                        winner = null;
                        foreach (Player player in players)
                        {
                            if (IsFlushPossible(player.PlayerCards, boardCards, ref rank1, ref rank2, ref rank3, ref rank4, ref rank5))
                            {
                                rank1 = (rank1 == 1) ? ACERANKFORPOKER : rank1;
                                int currIterRank = (rank1 * 10000) + (rank2 * 1000) + (rank3 * 100) + (rank4 * 10) + (rank5 * 1);
                                if (highestRankSoFar == -1)
                                {
                                    highestRankSoFar = currIterRank;
                                    winner = player;
                                }
                                else if(highestRankSoFar < currIterRank)
                                {
                                    highestRankSoFar = currIterRank;
                                    winner = player;
                                }
                                //In Omaha variant two players cannot have the same rank flush
                            }
                        }
                        if (winner != null)
                        {
                            winningPlayers.Add(winner);
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.Straight:
                        highestRankSoFar = -1;
                        highestRankPossible = -1;
                        winner = null;
                        ///make sure KQJ109 IS GREATER than A2345
                        foreach (Player player in players)
                        {
                            if ((highestRankPossible = HighestStraightPossible(player.PlayerCards, boardCards)) != -1)
                            {
                                if(highestRankPossible > highestRankSoFar)
                                {
                                    highestRankSoFar = highestRankPossible;
                                    winningPlayers.Clear();
                                    winningPlayers.Add(player);
                                }
                                else if (highestRankSoFar == highestRankPossible)
                                {
                                    winningPlayers.Add(player);
                                }
                                //In Omaha variant two players can have the same rank straight
                            }
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.Trips:
                        highestRankSoFar = -1;
                        highestRankPossible = -1;
                        winner = null;
                       
                        foreach (Player player in players)
                        {
                            //Make sure that the method returns the right ranks for Ace as well. or rewrite the method with the right signature
                            if ((highestRankPossible = HighestTripsPossible(player.PlayerCards, boardCards)) != -1)
                            {
                                if (highestRankPossible > highestRankSoFar)
                                {
                                    highestRankSoFar = highestRankPossible;
                                    winningPlayers.Clear();
                                    winningPlayers.Add(player);
                                }
                                else if (highestRankSoFar == highestRankPossible)
                                {
                                    winningPlayers.Add(player);
                                }
                                //In Omaha variant two players can have the same rank trips(because the board could have trips)
                            }
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.TwoPair:
                        highestRankSoFar = -1;
                        lowRankSoFar = -1;
                        winner = null;
                        int highRankCurrIter = -1;
                        int lowRankCurrIter = -1;
                        foreach (Player player in players)
                        {
                            if (IsTwoPairPossible(player.PlayerCards, boardCards, ref highRankCurrIter, ref lowRankCurrIter))
                            {
                                if (highRankCurrIter > highestRankSoFar)
                                {
                                    highestRankSoFar = highRankCurrIter;
                                    lowRankSoFar = lowRankCurrIter;
                                    winningPlayers.Clear();
                                    winningPlayers.Add(player);
                                }
                                else if (highestRankSoFar == highestRankPossible)
                                {
                                    if(lowRankCurrIter > lowRankSoFar)
                                    {
                                        lowRankSoFar = lowRankCurrIter;
                                        winningPlayers.Clear();
                                        winningPlayers.Add(player);
                                    }
                                    else if(lowRankCurrIter > lowRankSoFar)
                                    {
                                        winningPlayers.Add(player);
                                    }
                                }
                                //In Omaha variant two players can have the same rank trips(because the board could have trips)
                            }
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.OnePair:
                        highestRankSoFar = -1;
                        winner = null;
                        highestRankPossible = -1;
                        foreach (Player player in players)
                        {
                            if ((highestRankPossible = HighestOnePairPossible(player.PlayerCards, boardCards)) != -1)
                            {
                                if (highestRankPossible > highestRankSoFar)
                                {
                                    highestRankSoFar = highestRankPossible;
                                    winningPlayers.Clear();
                                    winningPlayers.Add(player);
                                }
                                else if (highestRankSoFar == highestRankPossible)
                                {
                                    winningPlayers.Add(player);
                                }
                                //In Omaha variant two players can have the same rank one pair with same side cards
                            }
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    case PokerHandHierarchy.HighCard:
                        rank1 = -1;
                        rank2 = -1;
                        rank3 = -1;
                        rank4 = -1;
                        rank5 = -1;
                        highestRankSoFar = -1;
                        winner = null;
                        highestRankPossible = -1;
                        foreach (Player player in players)
                        {
                            HighCardPossible(player.PlayerCards, boardCards, ref rank1, ref rank2, ref rank3, ref rank4, ref rank5);
                            rank1 = (rank1 == 1) ? ACERANKFORPOKER : rank1;
                            int currIterRank = (rank1 * 10000) + (rank2 * 1000) + (rank3 * 100) + (rank4 * 10) + (rank5 * 1);
                            if (currIterRank > highestRankSoFar)
                            {
                                highestRankSoFar = currIterRank;
                                winningPlayers.Clear();
                                winningPlayers.Add(player);
                            }
                            else if (highestRankSoFar == currIterRank)
                            {
                                winningPlayers.Add(player);
                            }
                                //In Omaha variant two players can have the same high cards
                            
                        }
                        if (winningPlayers.Count > 0)
                        {
                            return winningPlayers;
                        }
                        break;
                    default:
                        break;

                }


            }
            return winningPlayers;
        }

        /// <summary>
        /// Does board contains three or more cards of the same suit greater than equal to the rank provided. Ace (Rank 1) is treated specially since royal flush could 
        /// include Ace.
        /// </summary>
        /// <param name="boardCards"></param>
        /// <param name="gteRank"></param>
        /// <returns></returns>
        private static Suit BoardContainThreeOrMoreCardsOfSameSuitGTERank(IList<Card> boardCards, Rank gteRank)
        {
            int[] suitCount = new int[4] {0,0,0,0};
            
            foreach(Card card in boardCards)
            {
                if(card.Rank == Rank.Ace || card.Rank>= gteRank)
                {
                    int cardSuitIndex = (int)card.Suit - 1;
                    suitCount[cardSuitIndex]++;
                    if(suitCount[cardSuitIndex] >= 3)
                    {
                        return (Suit)cardSuitIndex+1;
                    }
                }
            }
            return Suit.None;
        }

        private static IList<Card> GetStraightFlushCardsOnBoard(IList<Card> boardCards)
        {
            //TODO: Make sure the boardcards are sorted by rank
            int[] suitCount = new int[4] { 0, 0, 0, 0 };
            int indexForStraightFlushSuit = -1;
            IList<Card> straightFlushCardsOnBoard = new List<Card>();
            foreach (Card card in boardCards)
            {
                int cardSuitIndex = (int)card.Suit - 1;
                suitCount[cardSuitIndex]++;
                if (suitCount[cardSuitIndex] >= 3)
                {
                    indexForStraightFlushSuit = cardSuitIndex;
                    break;
                }
            }
            if (indexForStraightFlushSuit != -1)
            {
                int prevRank = -1;
                foreach (Card card in boardCards)
                {
                    if ((int)card.Suit - 1 == indexForStraightFlushSuit)
                    {
                        if(prevRank == -1 || ((int)card.Rank - prevRank <= 2))
                        {
                            straightFlushCardsOnBoard.Add(card);
                        }
                        else if(((int)card.Rank - prevRank > 2) && straightFlushCardsOnBoard.Count <=2) //reset prevRank and clear straightFlushCardsOnBoard
                        {

                        }
                        prevRank = (int)card.Rank;
                    }
                }
            }
           
            return Suit.None;
        }

        private static bool PlayerCardsContainsTwoOrMoreCardOfTheGivenSuit(IList<Card> playerCards, Suit boardSuit, Rank gteRank)
        {
            int suitCount = 0;
            foreach (Card playerCard in playerCards)
            {
                if(playerCard.Suit == boardSuit && (playerCard.Rank == Rank.Ace || playerCard.Rank >= gteRank))
                {
                    suitCount++;
                }
                if(suitCount>= 2)
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// Check if royal flush possible for the given player
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static bool IsRoyalFlushPossible(Card[] playerCards, IList<Card> boardCards)
        {
            Suit boardSuitWithThreeOrMorecards;
            if((boardSuitWithThreeOrMorecards = BoardContainThreeOrMoreCardsOfSameSuitGTERank(boardCards, Rank.Ten)) != Suit.None)
            {
                return PlayerCardsContainsTwoOrMoreCardOfTheGivenSuit(playerCards.ToList(), boardSuitWithThreeOrMorecards, Rank.Ten);
            }
            return false;
        }

        /// <summary>
        /// Check if the straight flush is possible for the given player and return the highest starting rank if true, -1 otherwise
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static int HighestStraightFlushRankIfPossible(Card[] playerCards, IList<Card> boardCards)
        {
            int highestStraightFlushRankIfPossible = -1;
            List<Card> straightFlushCardsOnBoard = GetStraightFlushCardsOnBoard(boardCards);
            if (straightFlushCardsOnBoard.Count > 0)
            {

            }
                return highestStraightFlushRankIfPossible;
        }

        /// <summary>
        /// Check if Quads(Four of a kind) is possible for the given and return the rank if true, -1 otherwise
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static int HighestFourOfAkindIfPossible(Card[] playerCards, IList<Card> boardCards)
        {
            int highestFourOfAkindIfPossible = -1;
            return highestFourOfAkindIfPossible;
        }

        /// <summary>
        /// Check if the full house is possible for the given player and return the low rank and high rank in highrank and lowrank.
        /// Returns true or false, depending on Full House possible or not.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <param name="highRank"></param>
        /// <param name="lowRank"></param>
        /// <returns></returns>
        public static bool IsFullHousePossible(Card[] playerCards, IList<Card> boardCards, ref int highRank, ref int lowRank)
        {
            bool isFullHousePossible = false;
            return isFullHousePossible;
        }

        /// <summary>
        /// Check if flush is possible and return the highest five rank of the flush cards in Rank1(Highest) through Rank5 (lowest) rank as ref int. Return true or false depending on 
        /// if flush possible
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <param name="rank1"></param>
        /// <param name="rank2"></param>
        /// <param name="rank3"></param>
        /// <param name="rank4"></param>
        /// <param name="rank5"></param>
        /// <returns></returns>
        public static bool IsFlushPossible(Card[] playerCards, IList<Card> boardCards, ref int rank1, ref int rank2, ref int rank3, ref int rank4, ref int rank5)
        {
            bool isFlushPossible = false;
            return isFlushPossible;
        }

        /// <summary>
        /// Returns the rank of the highest straight card possible, -1 otherwise
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static int HighestStraightPossible(Card[] playerCards, IList<Card> boardCards)
        {
            int highestStraightPossible = -1;
            return highestStraightPossible;
        }

        /// <summary>
        /// Returns the rank of the highest trips possible, -1 otherwise
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static int HighestTripsPossible(Card[] playerCards, IList<Card> boardCards)
        {
            int highestTripsPossible = -1;
            return highestTripsPossible;
        }


        /// <summary>
        /// returns the ranks of two pairs if possible as ref ints, returns true if two pair possible, false otherwise.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <param name="highRank"></param>
        /// <param name="lowRank"></param>
        /// <returns></returns>
        public static bool IsTwoPairPossible(Card[] playerCards, IList<Card> boardCards, ref int highRank, ref int lowRank)
        {
            bool isTwoPairPossible = false;
            return isTwoPairPossible;
        }

        /// <summary>
        /// Return the rank of the highest one pair possible. -1 if not possible
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <returns></returns>
        public static int HighestOnePairPossible(Card[] playerCards, IList<Card> boardCards)
        {
            int highestOnePairPossible = -1;
            return highestOnePairPossible;
        }

        /// <summary>
        /// return the top 5 rank of the high card hand possible. There will always be a high card hand no matter the hands.
        /// </summary>
        /// <param name="playerCards"></param>
        /// <param name="boardCards"></param>
        /// <param name="rank1"></param>
        /// <param name="rank2"></param>
        /// <param name="rank3"></param>
        /// <param name="rank4"></param>
        /// <param name="rank5"></param>
        public static void HighCardPossible(Card[] playerCards, IList<Card> boardCards, ref int rank1, ref int rank2, ref int rank3, ref int rank4, ref int rank5)
        {
           
        }
    }
}
