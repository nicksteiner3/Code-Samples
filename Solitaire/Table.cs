using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire
{
    class Table
    {
        List<List<Card>> table;
        List<List<Card>> foundation = new List<List<Card>>();

        public Table (Deck deck)
        {
            table = new List<List<Card>>();

            for (int i = 0; i < 7; i++)
            {
                table.Add(new List<Card>());
                for (int j = 0; j <= i; j++) {
                    table[i].Add(deck.PopTopCard());
                    table[i][table[j].Count - 1].faceup = j==i;
                }
            }

            // Initialize Foundation
            for (int i = 0; i < 4; i++)
            {
                foundation.Add(new List<Card>());
            }

            // Top card faces up
            deck.FlipTopCard();
        }

        /// <summary>
        /// Print the table in the standard solitaire layout.
        /// </summary>
        public void PrintTable()
        {
            int row = 0;
            // Loop all rows
            while (true)
            {
                bool cardFoundInRow = false;
                // Loop all columns
                for (int column = 0; column < table.Count; column++)
                {
                    // Add some space before each column except for the first. 
                    if (column != 0)
                    {
                        Console.Write("   ");
                    }
                    // If there is nothing in a column at this row, write some empty space and move to the next column.
                    if (table[column].Count <= row)
                    {
                        Console.Write("    ");
                        continue;
                    }
                    // If the card is faceup, display the suit and value. 
                    if (table[column][row].faceup)
                    {
                        string message = "";
                        if (table[column][row].GetReadableValue().Length == 1)
                        {
                            message += "-";
                        }
                        message += table[column][row].GetReadableValue() + table[column][row].GetReadableSuit(true) + "-";

                        // Last parameter: appendEmptySpaceToEnd. Only true when iterating the last column in the table
                        ConsoleMessages.DisplayMessage(message, false, table[column][row].color == "black" ? ConsoleColor.Black : ConsoleColor.Red, ConsoleColor.White, (column == table.Count-1 && row == table[column].Count-1));
                    }
                    // If the card is facedown, display lines.
                    else
                    {
                        Console.Write("----");
                    }
                    cardFoundInRow = true;
                }
                Console.WriteLine();
                row++;

                // If we've reached a row with no cards (i.e. the 8th row in the initial game),
                // then we've finished displaying the cards. Break from the loop and the method. 
                if (!cardFoundInRow)
                {
                    break;
                }
            }
        }

        public int RemainingCardCount()
        {
            int cardCount = 0;
            foreach (List<Card> columns in table)
            {
                foreach (Card card in columns)
                {
                    cardCount++;
                }
            }
            return cardCount;
        }

        public void SetCard(string sourceCardName, Deck deck, ref int score, ref int moveCounter, bool drawThreeMode)
        {
            bool fromHand = false;
            (int sourceColumn, int sourceRow) = GetSourceIndices(sourceCardName);

            //could not find in table, check in hand
            if (sourceColumn == -1)
            {
                //check that the top of the hand is the source card, else print source card not found and return. 
                if (deck.GetTopOfHandName().ToLower() == sourceCardName)
                {
                    fromHand = true;
                }
                else
                {
                    ConsoleMessages.DisplayMessage("Source card " + sourceCardName.ToUpper() + " not available", true, ConsoleColor.Red);
                    return;
                }
            }
            if (!IsSetLegal(sourceColumn, sourceRow, fromHand ? deck.GetTopHandCard() : null))
            {
                return;
            }

            Card sourceCard;
            bool canMove = true;
            if (fromHand)
            {
                sourceCard = deck.RemoveTopCard();
                deck.deductPointsForCycle(drawThreeMode, ref score);
                deck.DecrementVisibleHandCount(drawThreeMode);
            }
            else
            {
                sourceCard = table[sourceColumn][sourceRow];
                table[sourceColumn].RemoveAt(sourceRow);
            }
                
            //flip new card if available
            if (!fromHand && sourceRow != 0 && table[sourceColumn][sourceRow-1].faceup == false)
            {
                table[sourceColumn][sourceRow - 1].faceup = true;
                score += 5;
            }

            if (foundation[sourceCard.suit].Count > 0)
            {
                Card topSuitCard = foundation[sourceCard.suit][foundation[sourceCard.suit].Count - 1];
                if (sourceCard.suit != topSuitCard.suit || topSuitCard.value + 1 != sourceCard.value)
                {
                    canMove = false;
                }
            }
            else if (sourceCard.value != 0)
            {
                canMove = false;
            }
            if (canMove)
            {
                foundation[sourceCard.suit].Add(sourceCard);
                score += 10;
                moveCounter++;
            }
            else
            {
                ConsoleMessages.DisplayMessage("Cannot set " + sourceCard.GetReadableCombined(), true, ConsoleColor.Red);
            }
        }

        public (bool cardFound, bool continueChecking) MoveCard(string sourceCardName, string destinationName, ref int score)
        {
            //find source and destination card indices in table. 
            (int sourceColumn, int sourceRow, int destinationColumn, int destinationRow) = GetSourceAndDestinationIndices(sourceCardName, destinationName);

            // source card not in deck. 
            if (sourceColumn == -1)
            {
                return (false, true);
            }

            //check if the move is legal
            (bool isMoveLegal, bool continueChecking) = IsMoveLegal(sourceColumn, sourceRow, destinationColumn, destinationRow);
            if (isMoveLegal)
            {
                //remove from list at current position
                while (table[sourceColumn].Count > sourceRow) {
                    Card sourceCard = table[sourceColumn][sourceRow];
                    table[sourceColumn].RemoveAt(sourceRow);

                    //move to new position
                    table[destinationColumn].Add(sourceCard);
                }

                //flip over newly revealed card
                if (table[sourceColumn].Count > 0)
                {
                    table[sourceColumn][sourceRow - 1].faceup = true;
                    score += 5;
                }

                return (true, false);
            }
            else
            {
                //TODO: illegal move
                return (false, continueChecking);
            }
        }

        public bool MoveFromHand(string sourceCardName, string destinationName, int suit, int value)
        {
            (int sourceColumn, int sourceRow, int destinationColumn, int destinationRow) = GetSourceAndDestinationIndices(sourceCardName, destinationName);

            if (IsMoveLegal(sourceColumn, sourceRow, destinationColumn, destinationRow, true).isMoveLegal)
            {
                table[destinationColumn].Add(new Card(suit, value, true));
                return true;
            }
            return false;
        }

        public bool MoveFromFoundation(string sourceCardName, string destinationName)
        {
            foreach(List<Card> suitPile in foundation)
            {
                if (suitPile.Count > 0 && suitPile[suitPile.Count-1].GetReadableCombined().ToLower() == sourceCardName)
                {
                    foreach(List<Card> column in table)
                    {
                        if (column[column.Count - 1].GetReadableCombined().ToLower() == destinationName)
                        {
                            Card movingCard = suitPile[suitPile.Count - 1];
                            column.Add(movingCard);
                            suitPile.Remove(movingCard);
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        private (int sourceColumn, int sourceRow, int destinationColumn, int destinationRow) GetSourceAndDestinationIndices(string sourceCard, string destination)
        {
            int sourceColumn = -1;
            int sourceRow = -1;
            int destinationColumn = -1;
            int destinationRow = -1;

            // if moving a king to an empty space
            if (destination == "empty")
            {
                for (int column = 0; column < table.Count; column++)
                {
                    if (table[column].Count == 0)
                    {
                        destinationColumn = column;
                        destinationRow = 0;
                        break;
                    }
                }
                //TODO: If there are no empty spaces
            }

            for (int column = 0; column < table.Count; column++)
            {
                for (int row = 0; row < table[column].Count; row++)
                {
                    if (sourceColumn == -1 && table[column][row].GetReadableCombined().ToLower() == sourceCard)
                    {
                        if (!table[column][row].faceup)
                        {
                            return (sourceColumn, sourceRow, destinationColumn, destinationRow);
                        }
                        sourceColumn = column;
                        sourceRow = row;
                        if (destinationColumn != -1)
                        {
                            break;
                        }
                    }
                    else if (destinationColumn == -1 && table[column][row].GetReadableCombined().ToLower() == destination)
                    {
                        destinationColumn = column;
                        destinationRow = row;
                        if (sourceColumn != -1)
                        {
                            break;
                        }
                    }
                }
                if (sourceColumn != -1 && destinationColumn != -1)
                {
                    break;
                }
            }

            return (sourceColumn, sourceRow, destinationColumn, destinationRow);
        }

        private (int sourceColumn, int sourceRow) GetSourceIndices(string sourceCard)
        {
            int sourceColumn = -1;
            int sourceRow = -1;
            for (int column = 0; column < table.Count; column++)
            {
                for (int row = 0; row < table[column].Count; row++)
                {
                    if (sourceColumn == -1 && table[column][row].GetReadableCombined().ToLower() == sourceCard && table[column][row].faceup)
                    {
                        sourceColumn = column;
                        sourceRow = row;
                        break;
                    }
                }
                if (sourceColumn != -1)
                {
                    break;
                }
            }

            return (sourceColumn, sourceRow);
        }

        private (bool isMoveLegal, bool continueSearcing) IsMoveLegal(int sourceColumn, int sourceRow, int destinationColumn, int destinationRow, bool fromHand = false)
        {
            //Card already in place. 
            if (sourceColumn == destinationColumn && sourceRow == destinationRow + 1)
            {
                ConsoleMessages.DisplayMessage("Error: Source card already in requested destination.", true, ConsoleColor.Red);
                return (false, false);
            }
            if (!fromHand && sourceColumn == -1)
            {
                ConsoleMessages.DisplayMessage("Error: Source card not available", true, ConsoleColor.Red);
                return (false, true);
            }
            if (destinationColumn == -1)
            {
                ConsoleMessages.DisplayMessage("Error: Destination card not available", true, ConsoleColor.Red);
                return (false, true);
            }
            return (true, true);
        }

        private bool IsSetLegal(int sourceColumn, int sourceRow, Card topOfHandCard)
        {
            Card sourceCard = topOfHandCard == null ? table[sourceColumn][sourceRow] : topOfHandCard;

            //in the foundation column of the same suit, is the value of the current card there 1 less than the source card
            if (sourceCard.value != 0)
            {
                int desiredColumnCount = foundation[sourceCard.suit].Count;
                if (desiredColumnCount == 0)
                {
                    ConsoleMessages.DisplayMessage($"Cannot set {sourceCard.GetReadableCombined(true)} onto the empty foundation column.", true, ConsoleColor.Red);
                    return false;
                }
                else if (desiredColumnCount != sourceCard.value)
                {
                    ConsoleMessages.DisplayMessage($"Cannot set {sourceCard.GetReadableCombined(true)} onto {foundation[sourceCard.suit][desiredColumnCount-1].GetReadableCombined(true)}", true, ConsoleColor.Red);
                    return false;
                }
            }

            // Attempting to set a card with other cards on top of it.
            if (sourceColumn >= 0 && sourceRow != 0 && table[sourceColumn].Count-1 > sourceRow)
            {
                ConsoleMessages.DisplayMessage("Cannot set a card with other cards on top of it.", true, ConsoleColor.Red);
                return false;
            }
            return true;
        }

        public void DisplayFoundation()
        {
            Console.Write("\nFoundation: ");
            int i = 0;
            foreach (List<Card> suitPile in foundation)
            {
                if (suitPile.Count == 0)
                {
                    Console.Write("---- ");
                }
                else
                {
                    string message = "";
                    string topCard = suitPile[suitPile.Count - 1].GetReadableCombined();
                    if (topCard.Length == 2)
                    {
                        message += "-";
                    }
                    message += suitPile[suitPile.Count - 1].GetReadableCombined(true) + "-";
                    ConsoleMessages.DisplayMessage(message, false, suitPile[0].color == "black" ? ConsoleColor.Black : ConsoleColor.Red, ConsoleColor.White);
                    Console.Write(" ");
                }
                i++;
            }
            Console.WriteLine();
        }

        public int FindFirstEmptySpace()
        {
            for (int i = 0; i < table.Count; i++)
            {
                if (table[0].Count == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public bool CheckForWin()
        {
            foreach (List<Card> column in foundation)
            {
                if (column.Count == 0 || column[column.Count-1].value != 12)
                {
                    return false;
                }
            }
            return true;
        }

        public bool AllCardsFlipped()
        {
            foreach (List<Card> column in table)
            {
                foreach (Card card in column)
                {
                    if (!card.faceup)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void Autocomplete(Deck deck)
        {
            //For each column in the table:
            foreach (List<Card> column in table)
            {
                if (column.Count == 0)
                {
                    continue;
                }
                Card topCardInColumn = column[column.Count - 1];
                List<Card> foundationPile = foundation[topCardInColumn.suit];
                Card topCardInFoundationPile = null;
                if (foundationPile.Count != 0)
                {
                    topCardInFoundationPile = foundationPile[foundationPile.Count - 1];
                }
                if (topCardInFoundationPile == null)
                {
                    if (topCardInColumn.value != 0)
                    {
                        continue;
                    }
                }
                else if (topCardInColumn.value != topCardInFoundationPile.value + 1)
                {
                    continue;
                }
                foundationPile.Add(topCardInColumn);
                column.Remove(topCardInColumn);
                return;
            }
            //For each card in the deck:
            foreach (Card card in deck.GetAllCards()) 
            {
                List<Card> foundationPile = foundation[card.suit];
                Card topCardInFoundationPile = null;
                if (foundationPile.Count != 0)
                {
                    topCardInFoundationPile = foundationPile[foundationPile.Count - 1];
                }
                if (topCardInFoundationPile == null)
                {
                    if (card.value != 0)
                    {
                        continue;
                    }
                }
                else if (card.value != topCardInFoundationPile.value + 1)
                {
                    continue;
                }
                foundationPile.Add(card);
                deck.cards.Remove(card);
                return;
            }
        }
    }
}