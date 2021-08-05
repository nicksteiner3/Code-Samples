using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire
{
    class Deck
    {
        public List<Card> cards;
        List<Card> cycledHandCards;
        int cycledCount;
        int visibleHandCount;

        public Deck()
        {
            visibleHandCount = 3;
            cycledCount = 0;
            cards = new List<Card>();
            cycledHandCards = new List<Card>();
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 13; j++)
                {
                    cards.Add(new Card(i, j));
                }
            }
            Shuffle();
        }

        public int RemainingCardCount()
        {
            return cards.Count;
        }

        private void Shuffle ()
        {
            Random rnd = new Random();
            for (int i = 0; i < cards.Count; i++)
            {
                int newPos = rnd.Next(0, 51);
                while (i == newPos)
                {
                    newPos = rnd.Next(0, 51);
                }
                Card tempCard = cards[i];
                cards[i] = cards[newPos];
                cards[newPos] = tempCard;
            }
        }

        public Card PopTopCard()
        {
            Card card = cards[cards.Count-1];
            cards.Remove(card);
            return card;
        }

        public void FlipTopCard()
        {
            if (cards.Count == 0)
            {
                NextCardInHand();
            }
            //All cards in hand played. 
            if (cards.Count != 0)
            {
                cards[0].faceup = true;
            }
        }

        public void deductPointsForCycle(bool drawThreeMode, ref int score)
        {
            bool deduct = drawThreeMode ? cycledCount > 3 : cycledCount > 0;
            if (cards.Count == 0 && deduct && cycledHandCards.Count == 0)
            {
                score = drawThreeMode ? Math.Max(score - 20, 0) : Math.Max(score - 100, 0);
            }
        }

        public void FlipNextThreeCards()
        {
            if (cards.Count == 0)
            {
                NextCardInHand();
            }
            else
            {
                cards[0].faceup = true;
            }
            if (cards.Count > 1)
            {
                cards[1].faceup = true;
            }
            if (cards.Count > 2)
            {
                cards[2].faceup = true;
            }
        }

        public void DisplayHand(bool drawThreeMode)
        {
            Console.Write("Hand: ");
            if (drawThreeMode)
            {
                PrintFirstTwoVisibleCards();
            }
            Console.Write(GetTopOfHandName(true, true));
            Console.ResetColor();
            Console.Write(" ");
            Console.WriteLine();
            Console.WriteLine($"Remaining: {cards.Count}/{cycledHandCards.Count+cards.Count}");
        }

        public void DecrementVisibleHandCount(bool drawThreeMode)
        {
            if (visibleHandCount > 1)
            {
                visibleHandCount--;
            }
            else
            {
                visibleHandCount = Math.Min(cards.Count, 3);
                if (cards.Count == 0)
                {
                    cards = new List<Card>(cycledHandCards);
                    cycledHandCards.Clear();
                    FlipAllCardsDown();
                    ConsoleMessages.DisplayMessage("End of deck. All hand cards played", true, ConsoleColor.Green);
                    if (drawThreeMode)
                    {
                        visibleHandCount = 3;
                    }
                }
            }
        }

        private void PrintFirstTwoVisibleCards()
        {
            if (cards.Count > 2 && visibleHandCount > 2)
            {
                cards[2].faceup = true;
                ConsoleMessages.DisplayMessage(cards[2].GetReadableCombined(true), false, cards[2].color == "black" ? ConsoleColor.Black : ConsoleColor.Red, ConsoleColor.Gray);
                Console.Write(" ");
            }
            if (cards.Count > 1 && visibleHandCount > 1)
            {
                cards[1].faceup = true;
                ConsoleMessages.DisplayMessage(cards[1].GetReadableCombined(true), false, cards[1].color == "black" ? ConsoleColor.Black : ConsoleColor.Red, ConsoleColor.Gray);
                Console.Write(" ");
            }
        }

        public string GetTopOfHandName(bool displaySuits = false, bool showColor = false)
        {
            if (cards.Count != 0)
            {
                if (showColor)
                {
                    Console.ForegroundColor = cards[0].color == "black" ? ConsoleColor.Black : ConsoleColor.Red;
                    Console.BackgroundColor = ConsoleColor.White;
                }
                return cards[0].GetReadableCombined(displaySuits);
            }
            else
            {
                return "Empty hand";
            }
        }

        public Card GetTopHandCard()
        {
            if (cards.Count != 0)
            {
                return cards[0];
            }
            else return null;
        }

        public bool NextCardInHand(bool drawThreeMode = false)
        {
            bool dockPointsForCycle = false;
            int cardsToMove = 0;

            foreach (Card card in cards)
            {
                if (card.faceup)
                {
                    cardsToMove++;
                }
                else
                {
                    break;
                }
            }

            for (int i = 0; i < cardsToMove; i++)
            {
                if (cards.Count > 1)
                {
                    FlipTopCard();
                    cycledHandCards.Add(cards[0]);
                    cards.RemoveAt(0);
                    if (cards.Count > 0 && (i == cardsToMove - 1))
                    {
                        if (drawThreeMode)
                        {
                            FlipNextThreeCards();
                        }
                        else
                        {
                            FlipTopCard();
                        }
                    }
                }
                else if (cards.Count == 0)
                {
                    if (cycledHandCards.Count == 0)
                    {
                        ConsoleMessages.DisplayMessage("End of deck. All hand cards played", true, ConsoleColor.Green);
                    }
                    else
                    {
                        dockPointsForCycle = drawThreeMode ? cycledCount > 3 : cycledCount > 0;
                        cycledCount++;
                        ConsoleMessages.DisplayMessage("End of deck. Starting from beginning.", true, ConsoleColor.Green);
                        cards = new List<Card>(cycledHandCards);
                        FlipTopCard();
                        cycledHandCards.Clear();
                    }
                }
                else
                {
                    dockPointsForCycle = drawThreeMode ? cycledCount > 3 : cycledCount > 0;
                    cycledCount++;
                    ConsoleMessages.DisplayMessage("End of deck. Starting from beginning.", true, ConsoleColor.Green);
                    FlipTopCard();
                    Card lastCard = cards[0];
                    cards = new List<Card>(cycledHandCards);
                    cards.Add(lastCard);
                    FlipTopCard();
                    cycledHandCards.Clear();

                    if (drawThreeMode)
                    {
                        SetFaceupForCycledDeck();
                    }
                    else
                    {
                        FlipAllCardsDown();
                    }
                }
            }

            visibleHandCount = Math.Min(cards.Count, 3);

            return dockPointsForCycle;
        }

        private void FlipAllCardsDown()
        {
            for (int i = 1; i < cards.Count; i++)
            {
                cards[i].faceup = false;
            }
        }

        private void SetFaceupForCycledDeck() 
        { 
            if (cards.Count > 3)
            {
                for (int i = 3; i < cards.Count; i++)
                {
                    cards[i].faceup = false;
                }
            }
        }

        public Card RemoveTopCard()
        {
            Card topCard = cards[0];
            for (int i = 0; i < cards.Count-1; i++)
            {
                cards[i] = cards[i + 1];
            }
            PopTopCard();
            FlipTopCard();
            return topCard;
        }

        public List<Card> GetAllCards()
        {
            List<Card> allCards = new List<Card>();
            foreach (Card card in cards)
            {
                allCards.Add(card);
            }
            foreach (Card card in cycledHandCards)
            {
                allCards.Add(card);
            }
            return allCards;
        }
    }
}