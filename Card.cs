using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sltr
{
    public enum suits
    {
        Hearts,
        Clubs,
        Diamonds,
        Spades
    }

    internal class Card
    {
        public int value;
        public suits suit;
        public bool isRed;

        public bool revealed;

        public Card(int value, suits suit)
        {
            this.value = value;
            this.suit = suit;

            if (suit == suits.Hearts || suit == suits.Diamonds) 
            {
                this.isRed = true;
            }
            else
            {
                this.isRed = false;
            }
        }

        public string ViewCard()
        {
            string output;

            output = val2nick(value) + suit2nick(suit);

            return output;
        }

        string val2nick(int val)
        {
            switch(val)
            {
                case 1:
                    return "A";
                case 11:
                    return "J";
                case 12:
                    return "Q";
                case 13:
                    return "K";
                default:
                    return val.ToString();
            }
        }

        public string suit2nick(suits val)
        {
            switch (val)
            {
                case suits.Hearts:
                    return "♥";
                case suits.Clubs:
                    return "♤";
                case suits.Diamonds:
                    return "♦";
                case suits.Spades:
                    return "♧";
                default:
                    return "█";
            }
        }
    }
}
