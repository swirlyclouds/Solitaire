using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solitaire
{
    internal class Card
    {
        int width = 140;
        int height = 190;
        int value;
        char suit;
        String cardPath;
        int x, y;

        public Card(String _value, char _suit, String x, String y)
        {

            value = int.Parse(_value);
            suit = _suit;
            cardPath = $"card{suit}{value}";
            this.x = int.Parse(x);
            this.y = int.Parse(y);
        }

        public int getX()
        {
            return x;
        }

        public int getY()
        {
            return y;
        }

        public string getCardPath()
        {
            return cardPath;
        }

    }
}
