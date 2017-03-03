using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using DeckOfCards;

namespace OddsCalculator
{
    public enum WinType
    {
        High = 1,
        Low = 2,
        Both = 3
    }
    public enum OddsCalculationTime
    {
        PreFlop = 1,
        OnFlop = 2,
        OnTurn = 3,
        OnRiver = 4,
        AfterRiver = 5
    }

    public abstract class PokerOddsCalculator : IPokerOddsCalculator
    {
        private int numberOfPlayers;
        private int numberOfCardsPerPlayer;
        OddsCalculationTime calculateAt;
        bool calculateOddsForLow = false;
        private Int64 totalNumberOfSimulations = 0;
        private const int NUMBER_OF_CARDS_ON_FLOP = 3;
        private const int NUMBER_OF_CARDS_ON_TURN = 1;
        private const int NUMBER_OF_CARDS_ON_RIVER = 1;

        public PokerOddsCalculator()
        {

        }

        public PokerOddsCalculator(bool calculateOddsForLow)
        {
            this.calculateOddsForLow = calculateOddsForLow;
        }

        private void Validate(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards)
        {
            if (players == null || players.Count == 0)
            {
                throw new ArgumentNullException("Please pass information about players");
            }
            if (calculateAt != OddsCalculationTime.PreFlop && (boardCards == null || boardCards.Count == 0))
            {
                throw new ArgumentNullException("Pass board card(s) for calculating odds.");
            }
            if (players.Count == 0)
            {
                throw new ArgumentException("Atleast one player is required.");
            }
            if (players.Count > 0)
            {
                int cardsPerPlayer = -1;
                for (int i = 0; i < players.Count; i++)
                {
                    Player plr = players[i];

                    if (plr.PlayerCards == null)
                    {
                        throw new ArgumentNullException(String.Format("Players Cards cannot be empty.Check cards for {0}", plr.PlayerNo));
                    }
                    if (i == 0)
                    {
                        cardsPerPlayer = plr.PlayerCards.Length;
                    }
                    if (cardsPerPlayer != plr.PlayerCards.Length)
                    {
                        throw new ArgumentException(String.Format("All players must have the same number of cards. Check cards for Player{0}", plr.PlayerNo));
                    }
                    HashSet<Card> playerCardsUnique = new HashSet<Card>();
                    for (int z = 0;z<plr.PlayerCards.Length; z++)
                    {
                        if (playerCardsUnique.Contains(plr.PlayerCards[z]))
                        {
                            //show the card in the future??
                            throw new ArgumentException(string.Format("Please check the player {0}'s cards. Duplicates are not allowed.", z + 1));
                        }
                        else
                        {
                            playerCardsUnique.Add(plr.PlayerCards[z]);
                        }
                    }
                    for (int b=0;b<boardCards.Count; b++)
                    {
                        if (playerCardsUnique.Contains(boardCards[b]))
                        {
                            //show the card in the future??boardCards[b].Rank + boardCards[b].Suit
                            throw new ArgumentException("Please check the board cards. Duplicates are not allowed.");
                        }
                        else
                        {
                            playerCardsUnique.Add(boardCards[b]);
                        }
                    }
                }
            }
        }

        private int Nfactorial(int n)
        {
            if(n == 0)
            {
                return 1;
            }
            else
            {
                return Nfactorial(n - 1) * n;
            }
        }

        //Maybe change to Dictionary of cards instead of list
        public List<Card>[] GetCardCombinations(List<Card> undealtCards, int cardsHandled)
        {
            //number of combinations is cardHandled Choose 2, since we just add1 more card (undealt card) to the Combinations
            // to gather the new combinations
            int denm = 4 * Nfactorial(cardsHandled - 2);
            int numberOfCombinations = Nfactorial(cardsHandled) / denm;

            List<Card>[] combinations = new List<Card>[numberOfCombinations];
            //Add one more card and calculate the different combinations with that additional card.
            Card nextCardToBeHandled = GetNthCardOfList(undealtCards, (cardsHandled + 1));
            if(nextCardToBeHandled == null)
            {
                return combinations;
            }
            

            List<Card> cardsHandledList = undealtCards.Take(cardsHandled).ToList();

            IEnumerable<IEnumerable<Card>> NChoose2Combos = MathUtil.Combinations(cardsHandledList, 2);
            int i = 0;
            foreach(IEnumerable comboList in NChoose2Combos.ToList())
            {
                List<Card> finalList = (List<Card>)comboList;
                finalList.Add(nextCardToBeHandled);
                combinations[i] = finalList;
                i++;
            }
            return combinations;
        }

        private Card GetNthCardOfList(List<Card> undealtCards, int index)
        {
            int listIndex = 0;
            foreach (Card card in undealtCards)
            {
                listIndex++;
                if(listIndex == index)
                {
                    return card;
                }
            }
            return null;
        }

        private void DetermineWinnerOnFlop(ref List<Player> player, List<Card> boardCards, List<Card> undealtCards, WinType winType)
        {
            Int32 cardsHandled;
            int undealtCardsCount = undealtCards.Count;
            //BitArray handledCombinationCards = new BitArray(undealtCards.Count(), false);
            List<Card> boardCardCurrIteration = new List<Card>();
            boardCardCurrIteration.AddRange(undealtCards.GetRange(0, NUMBER_OF_CARDS_ON_FLOP));
            //int setHandledCardsBit = 11;//00000111
            //BitArray b = new BitArray(new int[] { setHandledCardsBit });
            //handledCombinationCards.Or(b);

            //initially NUMBER_OF_CARDS_ON_FLOP cards are handled(dealt on the flop)
            List<Card> mutableUndealtCards = new List<Card>();
            mutableUndealtCards.AddRange(undealtCards.GetRange(NUMBER_OF_CARDS_ON_FLOP, (undealtCardsCount - NUMBER_OF_CARDS_ON_FLOP)));
            cardsHandled = NUMBER_OF_CARDS_ON_FLOP;
            //while there are enough cards for both turn and river continue
            if (undealtCardsCount - cardsHandled > 1)
            {
                DetermineWinnerOnTurn(ref player, boardCardCurrIteration, mutableUndealtCards, winType);
                 //while there are enough cards for both turn and river loop through.
                while ((undealtCardsCount - cardsHandled) > 1)
                {
                    List<Card>[] differentCombinationOfCards = GetCardCombinations(undealtCards, cardsHandled);
                    mutableUndealtCards.RemoveAt(cardsHandled);
                    cardsHandled++;
                    foreach (List<Card> flopCombination in differentCombinationOfCards)
                    {
                        DetermineWinnerOnTurn(ref player, flopCombination, mutableUndealtCards, winType);
                    }
                }
            }
         
        }

        //ToDO check if there is a way to compute using a bit instead of cloning the cards everytime
        private void DetermineWinnerOnTurn(ref List<Player> player, List<Card> boardCards, List<Card> undealtCards, WinType winType)
        {
            List<Card> mutableUndealtCards = new List<Card>();
            mutableUndealtCards = undealtCards.ToList();
            List<Card> boardCardCurrIteration = new List<Card>();
            boardCardCurrIteration.AddRange(boardCards);
            foreach (Card undealtCard in mutableUndealtCards)
            {
                boardCardCurrIteration.Add(undealtCard);
                //All possible combination with this undealt card will be computed by DetermineWinnerOnRiver
                //Can safely remove the undealt card from the List. We want all the different combinations not permutations.
                mutableUndealtCards.Remove(undealtCard);
                if (mutableUndealtCards.Count > 0)
                {
                    DetermineWinnerOnRiver(ref player, boardCardCurrIteration, mutableUndealtCards, winType);
                }
            }
        }

        private void DetermineWinnerOnRiver(ref List<Player> player, List<Card> boardCards, List<Card> undealtCards, WinType winType)
        {
            //TODOCan multi thread each card calculation later.
            //Even if low not possible, we add to the number of simulations.
            //Each undealt card is a combination
            totalNumberOfSimulations = totalNumberOfSimulations + undealtCards.Count;
            switch(winType)
            {
                case WinType.Low:
                    bool[] lowPossibleForPlayer = new bool[player.Count()];
                    List<Card>[] lowCardListForEachPlayer = new List<Card>[player.Count];
                    List<Card> lowCardListForBoard = new List<Card>();
                    int numberOfUniqueCardsBelowEigth = 0;
                    Dictionary<int, int> cardsBelowCount = new Dictionary<int, int>();
                    int cardFreq = 0;
                    foreach (Card brdCard in boardCards)
                    {
                        if ((int)brdCard.Rank >= 8)
                        {
                            if (cardsBelowCount.TryGetValue((int)brdCard.Rank, out cardFreq))
                            {
                                cardFreq = cardFreq + 1;
                                cardsBelowCount[(int)brdCard.Rank] = cardFreq;
                            }
                            else
                            {
                                cardsBelowCount[(int)brdCard.Rank] = 1;
                                numberOfUniqueCardsBelowEigth++;
                                lowCardListForBoard.Add(brdCard);
                            }
                        }
                    }
                    //on turn only one low card. Low not possible.
                    if (numberOfUniqueCardsBelowEigth < 2)
                    {
                        return;
                    }
                    //ToDo some optimization possible here to remove some players (where no low possible).
                    // by cross referencing cards in the other hashset
                    for (int p = 0; p < player.Count; p++)
                    {
                        int numOfUniqueLowCards = 0;
                        foreach (Card plrCrd in player[p].PlayerCards)
                        {
                            HashSet<Card> uniqueLowCardForPlayer = new HashSet<Card>();
                            if ((int)plrCrd.Rank <= 8)
                            {
                                if (!uniqueLowCardForPlayer.Contains(plrCrd))
                                {
                                    numOfUniqueLowCards++;
                                    lowCardListForEachPlayer[p].Add(plrCrd);
                                    uniqueLowCardForPlayer.Add(plrCrd);
                                }
                                //else
                                //{
                                //    uniqueLowCardForPlayer.Add(plrCrd);
                                //}
                                /*if(!cardsBelowCount.TryGetValue((int)plrCrd.Rank, out cardFreq))
                                {

                                }**/
                            }
                            if (numOfUniqueLowCards == 2)
                            {
                                lowPossibleForPlayer[p] = true;
                            }

                        }
                    }
                    int cardsLeft = undealtCards.Count();
                    foreach (Card rvrCard in undealtCards)
                    {

                        if (lowCardListForBoard.Contains(rvrCard))
                        {
                            //river card has already occured on board. Low does not change. 
                            continue;
                        }
                        if (numberOfUniqueCardsBelowEigth == 2)
                        {
                            if ((int)rvrCard.Rank > 8)
                            {
                                continue;
                            }
                            else
                            {
                                if (cardsBelowCount.TryGetValue((int)rvrCard.Rank, out cardFreq))
                                {
                                    //low card has already occured on board. Low not possible.
                                    continue;
                                }
                            }
                        }

                        //low possible
                        //order each players low cards to determine who will win, only if low possible for the user
                        for (int p = 0; p < player.Count(); p++)
                        {
                            //calculate odds only if low possible for player.
                            if (lowPossibleForPlayer[p])
                            {
                                //lowCardListForEachPlayer and lowCardListForBoard has the required cards. Order them
                                OrderCardsAndCalculateLowForPlayers(player, lowCardListForEachPlayer, lowCardListForBoard);
                            }
                        }
                    }
                    break;
                case WinType.High:
                    foreach (Card rvrCard in undealtCards)
                    {
                        CalculateWinnerForHigh(player, boardCards);
                    }
                    break;
                default:
                    break;
            }
           
           
        }

        private void CalculateWinnerForHigh(List<Player> players, List<Card> boardCards)
        {
            IList<Player> winningPlayers = PokerHandEvaluator.DetermineWinningPlayers(players, boardCards);
            if(winningPlayers.Count > 1)
            {
                foreach(Player player in winningPlayers)
                {
                    player.NumberOfHighTieingHands++;
                }
            }
            else
            {
                foreach (Player player in winningPlayers)
                {
                    player.NumberOfHighWinningHands++;
                }
            }

        }

        private void OrderCardsAndCalculateLowForPlayers(List<Player> players, List<Card>[] lowCardListForEachPlayer, List<Card> lowCardListForBoard)
        {
            SortedSet<Card> bestLowHandSoFar = new SortedSet<Card> (new CardComparer());
            BitArray winningPlayers = new BitArray(players.Count(), false);
            bool doesPlayerTieOtherPlayers = false;
            for (int i=0; i < lowCardListForEachPlayer.Count(); i++)
            {
                SortedSet<Card> lowCardsForCurrentPlayer = new SortedSet<Card>(new CardComparer());
                foreach (Card lowCard in lowCardListForEachPlayer[i]){
                    if (!lowCardsForCurrentPlayer.Contains(lowCard))
                    {
                        lowCardsForCurrentPlayer.Add(lowCard);
                    }
                }
                foreach (Card lowCard in lowCardListForBoard)
                {
                    if (!lowCardsForCurrentPlayer.Contains(lowCard))
                    {
                        lowCardsForCurrentPlayer.Add(lowCard);
                    }
                }
                if(lowCardsForCurrentPlayer.Count() >= 5)
                {
                    if(bestLowHandSoFar.Count() > 0)
                    {
                        Card[] otherPlayersLowCards = new Card[bestLowHandSoFar.Count];
                        bestLowHandSoFar.CopyTo(otherPlayersLowCards);
                        Card[] currentPlayersLowCards = new Card[lowCardsForCurrentPlayer.Count];
                        lowCardsForCurrentPlayer.CopyTo(currentPlayersLowCards);
                        CardComparer crdCompare = new CardComparer();
                        bool isCurrentPlayerBest = false;
                        
                        //start comparing the fifth card in both the arrays, thats what we care about.
                        for (int j = 4; j >= 0; j--)
                        {
                            int compareValue = crdCompare.Compare(otherPlayersLowCards[j], currentPlayersLowCards[j]);
                            if (compareValue > 0)
                            {
                                //Current players low cards are better(lesser rank). So it becomes the best hand.
                                isCurrentPlayerBest = true;
                                break;
                            }
                           else if(compareValue < 0)
                            {
                                break;
                            }
                            //continue if 0
                            if (j == 0)
                            {
                                doesPlayerTieOtherPlayers = true;
                            }
                        }
                        if (doesPlayerTieOtherPlayers)
                        {
                            winningPlayers.Set(i, true);
                        }
                        else if(isCurrentPlayerBest)
                        {
                            winningPlayers.SetAll(false);
                            winningPlayers.Set(i, true);
                            doesPlayerTieOtherPlayers = false;
                        }
                    }
                    else
                    {
                        bestLowHandSoFar = lowCardsForCurrentPlayer;
                        winningPlayers.Set(i, true);
                        doesPlayerTieOtherPlayers = false;
                    }
                }
            }
            int k = 0;
            if(doesPlayerTieOtherPlayers)
            {
                foreach (bool winning in winningPlayers)
                {
                    if (winning)
                    {
                        players[k].NumberOfLowTieingHands++;
                    }
                    k++;
                }
            }
            else
            {
                foreach (bool winning in winningPlayers)
                {
                    if (winning)
                    {
                        players[k].NumberOfLowWinningHands++;
                    }
                    k++;
                }
            }

        }


        /// <summary>
        /// Calculate the odds for low for the given list of players.
        /// </summary>
        /// <param name="players"></param>
        /// <param name="calculateAt"></param>
        /// <param name="boardCards"></param>
        /// <param name="undealtCards"></param>
        public virtual void CalculateOddsForLow(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            if (calculateOddsForLow)
            {
                Validate(ref players, calculateAt, boardCards);
            }
            else
            {
                throw new NotSupportedException("The selected game does not support calculating odds for low.");
            }
            switch ((int)calculateAt)
            {
                case (int)OddsCalculationTime.OnFlop:
                    DetermineWinnerOnFlop(ref players, boardCards, undealtCards, WinType.Low);
                    break;
                case (int)OddsCalculationTime.OnTurn:
                    DetermineWinnerOnTurn(ref players, boardCards, undealtCards, WinType.Low);
                    break;
                case (int)OddsCalculationTime.OnRiver:
                    DetermineWinnerOnRiver(ref players, boardCards, undealtCards, WinType.Low);
                    break;
                default:
                    throw new ArgumentException("Invalid value for Calculation time.");
            }
            SetWinOddsForEachPlayers(players, WinType.Low);
        }

        private void SetWinOddsForEachPlayers(List<Player> players, WinType winType)
        {
            foreach (Player player in players) {
                switch (winType)
                {
                    case WinType.Low:
                        player.OddsOfWinningLow = (player.NumberOfLowWinningHands / totalNumberOfSimulations) * 100;
                        player.OddsOfTieingLow = (player.NumberOfLowTieingHands / totalNumberOfSimulations) *100;
                        break;
                    case WinType.High:
                        player.OddsOfWinningHigh = (player.NumberOfHighWinningHands / totalNumberOfSimulations) * 100;
                        player.OddsOfTieingHigh = (player.NumberOfHighTieingHands / totalNumberOfSimulations) * 100;
                        break;
                    case WinType.Both:
                        player.OddsOfWinningLow = (player.NumberOfLowWinningHands / totalNumberOfSimulations) * 100;
                        player.OddsOfTieingLow = (player.NumberOfLowTieingHands / totalNumberOfSimulations) * 100;
                        player.OddsOfWinningHigh = (player.NumberOfHighWinningHands / totalNumberOfSimulations) * 100;
                        player.OddsOfTieingHigh = (player.NumberOfHighTieingHands / totalNumberOfSimulations) * 100;
                        break;
                    default:
                        throw new ArgumentException("Illegel WinType passed in.");
                }
            }
        }

        public virtual void CalculateOddsForHigh(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            Validate(ref players, calculateAt, boardCards);
            switch ((int)calculateAt)
            {
                case (int)OddsCalculationTime.OnFlop:
                    DetermineWinnerOnFlop(ref players, boardCards, undealtCards, WinType.High);
                    break;
                case (int)OddsCalculationTime.OnTurn:
                    DetermineWinnerOnTurn(ref players, boardCards, undealtCards, WinType.High);
                    break;
                case (int)OddsCalculationTime.OnRiver:
                    DetermineWinnerOnRiver(ref players, boardCards, undealtCards, WinType.High);
                    break;
                default:
                    throw new ArgumentException("Invalid value for Calculation time.");
            }
        }

        public virtual void OverallEquity(ref List<Player> players, OddsCalculationTime calculateAt, List<Card> boardCards, List<Card> undealtCards)
        {
            Validate(ref players, calculateAt, boardCards);
        }


    }
}
