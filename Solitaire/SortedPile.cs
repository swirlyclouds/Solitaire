
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    internal class SortedPile
    {
        private Stack<Card> cards;
        public SortedPile()
        {
            cards = new Stack<Card>();
        }

        public Card topCard()
        {
            return cards.Last();
        }

        public void addCard(Card c)
        {
            cards.Push(c);
        }
        public int count() { return cards.Count(); }
    }
}
