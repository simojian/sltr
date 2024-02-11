using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sltr
{
    internal abstract class CardPile
    {
        public List<Card> Cards = new List<Card>();

        public int Count()
        {
            return (int)Cards.Count();
        }

        public void Add(Card input)
        {
            Cards.Add(input);
        }

        public Card DrawCard(bool nonDestructive = false, int depth = 0)
        {
            if (Cards.Count == 0) { return null; }

            Card currentCard = Cards[Cards.Count - 1 - depth];

            if (!nonDestructive) Cards.Remove(currentCard);
            return currentCard;
        } 
    }

    class Deck : CardPile
    {
        public void PopulateDeck()
        {
            for (int i = 1; i <= 13; i++)
            {
                Cards.Add(new Card(i, suits.Hearts));
                Cards.Add(new Card(i, suits.Clubs));
                Cards.Add(new Card(i, suits.Diamonds));
                Cards.Add(new Card(i, suits.Spades));
            }
        }

        public void ShuffleDeck()
        {
            var randomSeed = new Random();

            Cards = Cards.OrderBy(x => randomSeed.Next()).ToList();
        }
    }

    class Stack : CardPile
    {
        public string ViewCard(int number, bool showHidden = false)
        {
            if (Cards.Any() || number > Cards.Count() - 1)
            {
                if (number == Cards.Count() - 1 || showHidden || Cards[number].revealed)
                {
                    Cards[number].revealed = true;
                    return Cards[number].ViewCard();
                }
                else
                {
                    return "XX";
                }
            }
            else
            {
                return "..";
            }
        }
    }
}
