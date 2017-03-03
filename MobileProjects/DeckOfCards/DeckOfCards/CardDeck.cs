using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DeckOfCards
{
    public class CardDeck
    {

        private Card[] deck;
        public static readonly int NUMBEROFCARDSINEACHSUIT = 13;
        public static readonly int NUMBEROFCARDSINDECK = 52;
        private int nextCardToDealIndex = 0;
        //normal card deck with normal rank
        public CardDeck()
        {
            deck = new Card[52];
            for (int i = 0; i < 4; i++)
            {
                for (int j= 0; j<13; j++)
                {
                    int index = (NUMBEROFCARDSINEACHSUIT * i) + j;
                    deck[index] = new Card((Suit)i+1, (Rank)j+1);
                }
            }
        }

        //Use this contructor if you want custom values for each rank in the deck. Each Rank could have multiple values in some games.(A in blackjack)
        public CardDeck(int[][] cardValues)
        {
            if(cardValues == null)
            {
                throw new ArgumentNullException("CardValue cannot be null");
            }
            if(cardValues.Length != 13)
            {
                throw new ArgumentException(String.Format("Passed in Card Values in invalid. Expecting a length of 13. Passed in length of array is={0}", cardValues.Length));
            }
            deck = new Card[52];
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    int index = (NUMBEROFCARDSINEACHSUIT * i) + j;
                    deck[index] = new Card((Suit)i, (Rank)j, cardValues[j]);
                }
            }
        }


        /// <summary>
        /// Shuffle the deck of cards.//refer knuth-fisher-yates
        /// </summary>
        public void Shuffle()
        {
           
            for (int i = 0; i < deck.Length; i++)
            {
                int swapIndex = CryptoRandom.Next(i, deck.Length - i);
                Swap(ref deck, i, i + swapIndex);
            }
        }

        //Swap the cards.
        void Swap(ref Card[] deck, int swapIndex1, int swapIndex2)
        {
            Card temp = deck[swapIndex1];
            deck[swapIndex1] = deck[swapIndex2];
            deck[swapIndex2] = temp;
        }
        
        public void ResetDeck()
        {
            nextCardToDealIndex = 0;
            Shuffle();
        }

        public Card[] Deal(int requestedNumberOfCards)
        {
            if(requestedNumberOfCards<1 
                || (requestedNumberOfCards > NUMBEROFCARDSINDECK) 
                ||(requestedNumberOfCards + nextCardToDealIndex) > NUMBEROFCARDSINDECK)
            {
                throw new ArgumentException(String.Format("{0} cards already dealt. Only {1} left to deal.", nextCardToDealIndex, (NUMBEROFCARDSINDECK - nextCardToDealIndex)));
            }
            Card[] dealtCards = new Card[nextCardToDealIndex];
            for (int i=0;i< requestedNumberOfCards; i++)
            {
                dealtCards[i] = deck[nextCardToDealIndex + i];
            }
            nextCardToDealIndex = nextCardToDealIndex + requestedNumberOfCards;
            return dealtCards;
        }

        //this method has to be commented out if used for a game since the order of cards are how it will be dealt.
        public List<Card> UnDealtCards()
        {
            int indexToCopyUndealtCards =  nextCardToDealIndex;
            List<Card> undealtCards = new List<Card>();
            for(int i= nextCardToDealIndex; i< NUMBEROFCARDSINDECK; i++)
            {
                undealtCards.Add(deck[i]);
            }
            return undealtCards;
        }
    }
}
