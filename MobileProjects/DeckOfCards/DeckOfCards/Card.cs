using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckOfCards
{
    public enum Suit
    {
        None = 0, //Maybe if we need a joker for some game in the future.
        Spades = 1,
        Hearts = 2,
        Diamonds = 3,
        Clubs = 4
    }

    public enum Rank
    {
        None = 0, //Maybe if we need a joker for some game in the future.
        Ace = 1,
        Two = 2,
        Three = 3,
        Four = 4,
        Five = 5,
        Six = 6,
        Seven = 7,
        Eight = 8,
        Nine = 9,
        Ten = 10,
        Jack = 11,
        Queen = 12,
        King = 13
    }

    public class Card 
    {
        private Suit suit;
        private Rank rank;
        //for some games, different cards have different values. in blackjack ace could be 1 or 11.
        private int[] cardValue;

        //constructor to use if you want to use the natural rank of cards, A-1, K 13,...
        public  Card(Suit suit, Rank rank)
        {
            this.suit = suit;
            this.rank = rank;
        }

        //constructor to use if you want custom values for cards, A-1 or 11, or Q 30 so on
        public Card(Suit suit, Rank rank, int[] cardValue)
        {
            this.suit = suit;
            this.rank = rank;
            this.cardValue = cardValue;
        }

        public Suit Suit { get { return suit;} set { suit = value; } }

        public Rank Rank { get { return rank; } set { rank = value; } }

        public int[] CardValue { get { return cardValue; } set { cardValue = value; } }

        //simplistic hashcode for now.
        public override int GetHashCode()
        {
            return (int)this.rank * (int)this.suit;
        }

        public override bool Equals(System.Object obj)
        {
            // If parameter is null return false.
            if (obj == null)
            {
                return false;
            }

            // If parameter cannot be cast to Point return false.
            Card compareToCard = obj as Card;
            if ((System.Object)compareToCard == null)
            {
                return false;
            }

            // Return true if the fields match:
            if (this.cardValue == null && compareToCard.CardValue == null)
            {
                return (this.suit == compareToCard.Suit) && (this.rank == compareToCard.Rank);
            }
            else if (this.cardValue == null && compareToCard.CardValue != null || this.cardValue != null && compareToCard.CardValue == null)
            {
                return false;
            }
            else
            {
                bool equals = false;
                equals = (this.suit == compareToCard.Suit) && (this.rank == compareToCard.Rank);
                if (equals)
                {
                    if(this.cardValue.Length == compareToCard.CardValue.Length)
                    {
                        for(int i = 0; i<cardValue.Length; i++)
                        {
                            if(this.cardValue[i] != compareToCard.CardValue[i])
                            {
                                return false;
                            }
                        }
                    }
                    else
                    {
                        return false;
                    }
                }
                return equals;
            }
        }

    }


    public static class CardExtension
    {
        public static Card Clone(this object obj)
        {
            if (obj == null)
                throw new ArgumentNullException("Passed in Card is null.");
            var card = obj as Card;
            Card clonedCard;
            if (card != null)
            {
                clonedCard = new Card(card.Suit, card.Rank);
            }
            else
            {
                throw new ArgumentNullException("Passed in Card is invalid.");
            }
            return clonedCard;

            throw new ArgumentException("Type of 'this' must have Clone method", "obj");
        }
    }
}

