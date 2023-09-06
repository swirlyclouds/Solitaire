using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Solitaire
{
    internal class Column
    {
        public Rectangle position;
        Stack<Card> covered;
        List<Card> cards;
        public Column(Rectangle position)
        {
            covered = new Stack<Card>();
            cards = new List<Card>();
            this.position = position;
        }

        public void Add(Card card)
        {
            covered.Push(card);
        }

        public void update()
        {
            if (cards.Count == 0 && covered.Count > 0)
            {
                cards.Add(covered.Pop());
            }
        }

        public List<Card> GetCards() { return cards; }


        public int getSizeofCoveredStack()
        {
            return covered.Count;
        }
    }
}
