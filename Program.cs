using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace sltr
{
    internal class Program
    {
        //initital global objects
        static Deck deck = new Deck();
        static Stack[] rows = new Stack[7];
        static Stack drawPile = new Stack();

        //gameplay variables
        static string nextCommand = "Start of game!";
        static bool runGame = true;
        static void Main(string[] args)
        {
            //setting the console output to an encoding with card symbols
            Console.OutputEncoding = Encoding.UTF8;

            // SETUP
            //Populating the rows array

            for (int i = 0; i < rows.Length; i++)
            {
                rows[i] = new Stack();
            }

            //Setting up the main deck of cards
            deck.PopulateDeck();
            deck.ShuffleDeck();

            //Drawing cards from the deck into the rows
            for (int i = 7; i > 0; i--)
            {
                drawCards4Stack(rows[rows.Length - i], deck, i, true);
            }

            //MAIN GAME LOOP - think of this as the update function
            
            while (runGame) 
            { 
                //Draws all of the screen info and waits for new input
                WriteRound();

                string input = Console.ReadLine();

                input = input.ToUpper();

                switch (input)
                {
                    //Draws a card from the deck into the draw pile
                    case "D":
                        drawFromDeck();
                        break;
                    //Ends the game
                    case "K":
                        runGame = false;
                        break;
                    //Debug testing feature - reveals a whole row
                    case "FORTNITE":
                        int x = Int32.Parse(Console.ReadLine());
                        debugViewStack(rows[x], true);
                        break;
                    //Move cards from one stack to another
                    case "M":
                        MoveCards();
                        break;
                    default:
                        break;
                }
            }
        }

        //======STACK MANIPULATION FUNCTIONS========
        
        //draws Amount of cards from the deck into a stack, (while also turning them around if set to startingDraw)
        static void drawCards4Stack(Stack currentStack, Deck currentDeck, int amount, bool startingDraw = false)
        {
            for (int i = 0; i < amount; i++)
            {
                Card newCard = currentDeck.DrawCard();
                newCard.revealed = !startingDraw;
                currentStack.Add(newCard);
            }
        }

        //Debug function that prints out a specific stack (if revealAll true, will also turn around hidden cards)
        static void debugViewStack(Stack stack, bool revealAll = false)
        {
            if (stack.Count() > 0)
            {
                for (int i = 0; i < stack.Count(); i++)
                {
                    Console.WriteLine(stack.ViewCard(i, revealAll));
                }
            }
            else
            {
                Console.WriteLine(stack.ViewCard(0));
            }
        }

        //=======GAMEPLAY FUNCTIONS==========

        //D (draw) command
        //Draws a card from the deck into the drawPile revealing it,
        //if there are no more cards left in the deck it will put the drawPile back into the deck
        static void drawFromDeck()
        {
            if (deck.Count() > 0)
            {
                drawCards4Stack(drawPile, deck, 1);

                nextCommand = "Drawn Card from deck.";
            }
            else
            {
                while (drawPile.Count() > 0)
                {
                    Card drawnCard = drawPile.DrawCard();
                    deck.Add(drawnCard);
                }

                nextCommand = "No more cards to draw. Reset deck.";
            }
        }

        //M (move) command
        //Takes in 2 inputs one where from to take and the other where to put the taken cards
        //TODO: add the ability to select a specific card, better input handling
        static void MoveCards()
        {
            string input1 = Console.ReadLine();
            Console.WriteLine("to...");
            string input2 = Console.ReadLine();

            int fixedInput2 = Letter2Number(input2);

            CardPile takingPile = null;
            CardPile placingPile = null;
            int cardDepth = 0;

            //check if taking from pile
            if (input1.ToUpper() == "PILE")
            {
                takingPile = drawPile;
                cardDepth = 0;
            }
            else
            {
                int rowNum = InputCardPos(input1)[0];
                int depth = InputCardPos(input1)[1];

                takingPile = rows[rowNum];
                cardDepth = takingPile.Count() - depth;
            }

            placingPile = rows[fixedInput2];

            MoveCards(takingPile, placingPile, cardDepth);
        }

        static void MoveCards(CardPile takingPile, CardPile placingPile, int depth = 0)
        {
            List<Card> takenCards = new List<Card>();

            Card placeCard = placingPile.DrawCard(true);
            Card checkCard = takingPile.DrawCard(true, depth);

            if (CheckCorrectOrder(checkCard, placeCard)) 
            {
                for (int i = 0; i <= depth; i++)
                {
                    takenCards.Add(takingPile.DrawCard());
                }

                for (int i = 0; i < takenCards.Count; i++)
                {
                    placingPile.Add(takenCards[i]);
                }

                nextCommand = "Moved Cards.";
            }
            else
            {
                nextCommand = "Can't move here.";
            }
            
        }

        //=====UTILITY FUNCTIONS=======

        static int[] InputCardPos(string input)
        {
            char[] x = input.ToCharArray();

            int[] output = new int[2];

            output[0] = Letter2Number(x[0].ToString());
            output[1] = Int32.Parse(x[1].ToString());

            return output;
        }

        static int Letter2Number(string input)
        {
            int output;
            if (Int32.TryParse(input,out output))
            {
                return output + 1;
            }
            else
            {
                char delta = 'a';
                input = input.ToLower();

                char[] x = input.ToCharArray();

                output = x[0] - delta;
            }

            return output;
        }
        //Check if a card can go on top of another
        //It has to have a different color and be of a lower value by 1
        static bool CheckCorrectOrder(Card topCard, Card bottomCard)
        {
            if (topCard.isRed != bottomCard.isRed && topCard.value == bottomCard.value - 1)
            {
                return true;
            }
            else
            {
                DebugCheckOrder(topCard, bottomCard);
                return false;
            }
        }

        static void DebugCheckOrder(Card a, Card b)
        {
            nextCommand = "DEBUG: ";
            if (a.isRed != b.isRed)
            {
                nextCommand += "Colors match; ";
            }
            if (a.value == b.value - 1) 
            {
                nextCommand += "Wrong values; ";
            }
        }

        static int GetLargestListCount()
        {
            int max = 0;

            for (int i = 0; i < rows.Length; i++)
            {
                int x = rows[i].Count();

                if (x > max)
                {
                    max = x;
                }
            }

            return max;
        }

        //=====DRAWING FUNCTIONS=======

        //Draws out all of the info about the card states
        static void WriteRound()
        {
            Console.Clear();
            //write the previous move
            Console.WriteLine(nextCommand);
            //Draws the current top card ont the draw pile
            Console.Write("Draw Pile: " + drawPile.ViewCard(drawPile.Count() - 1));
            Console.WriteLine();

            //Write the rows
            Console.WriteLine();
            Console.WriteLine("   A.  B.  C.  D.  E.  F.  G.");
            WriteRows();
            Console.WriteLine();
            Console.WriteLine("==================================================================");
            Console.WriteLine();
            Console.WriteLine("Input Command: ");
        }

        //Draws the rows in the 
        static void WriteRows()
        {
            string[,] drawArray = new string[GetLargestListCount() , rows.Length];

            for(int i = 0; i < rows.Length; i++)
            {
                for(int j = 0; j < rows[i].Count();  j++)
                {
                    string currentCard = rows[i].ViewCard(j);

                    drawArray[j, i] = currentCard;
                }

            }

            for(int i = 0; i < GetLargestListCount(); i++)
            {
                Console.Write((i + 1) + ". ");
                for (int j = 0; j < rows.Length; j++)
                {
                    string currentCard = drawArray[i, j];

                    if (currentCard != null)
                    {
                        if (currentCard.Length > 2)
                        {
                            Console.Write(currentCard + " ");
                        }
                        else
                        {
                            Console.Write(currentCard + "  ");
                        }
                    }
                    else
                    {
                        Console.Write("    ");
                    }
                }
                Console.WriteLine();
            }
        }

        

        

    }
}