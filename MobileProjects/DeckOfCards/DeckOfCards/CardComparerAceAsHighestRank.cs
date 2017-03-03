using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeckOfCards
{

    public class CardComparerAceAsHighestRank : IComparer<Card>
    {
        public int Compare(Card x, Card y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    // If x is null and y is null, they're
                    // equal. 
                    return 0;
                }
                else
                {
                    // If x is null and y is not null, y
                    // is greater. 
                    return -1;
                }
            }
            else
            {
                // If x is not null...
                //
                if (y == null)
                // ...and y is null, x is greater.
                {
                    return 1;
                }
                else
                {
                    // ...and y is not null, compare the 
                    // Rank of both the cards.
                    //
                    int returnValue = 0;
                    if (x.Rank == Rank.Ace && y.Rank == Rank.Ace)
                    {
                        returnValue = 0;
                    }
                    else if (x.Rank == Rank.Ace)
                    {
                        returnValue = -1;
                    }
                    else if (y.Rank == Rank.Ace)
                    {
                        returnValue = 1;
                    }
                    else
                    {
                        returnValue = x.Rank.CompareTo(y.Rank);
                    }
                    return returnValue;
                }
            }
        }
    }
}
