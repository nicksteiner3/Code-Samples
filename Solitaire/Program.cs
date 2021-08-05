using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
//using System.Diagnostics;

//TODO: Add option to delete the high score and the best time.
//TODO: There is an empty space in front of "Do you want to start a new game" when finishing a game. 
//TODO: Setting (and probably moving) the last card in the hand deducts points for cycling the deck, but it shouldn't. 
//TODO: Moving or setting the last card (With still more cards) in the deck and it didn't deduct points for cycling the deck.
//TODO: FIXED (I think) test the breakpoint to see. Draw 3 mode, setting the last card in the hand recycles the deck but with only one card showing. 
//TODO: Draw3 mode autocomplete.
//TODO: Add a certain amount of time on autocomplete option. 
//TODO: Autocomplete doesn't provide points to score. 
//TODO: You can incorrectly move a card from the hand to the table if the desired destination card is facedown in one of the columns. 
//TODO: implement draw 3 option instead of draw 1 option.  
//TODO: give the option to show the whole hand in draw 1 mode (disables scoring)
//TODO: give the option to sort the hand in draw 1 mode (disables scoring)
//TODO: add comments everywhere and optimize wherever possible.  
//TODO: Move to Unity
namespace Solitaire
{
    class Program
    {
        static Table table;
        static Deck deck;
        static int moveCounter;
        static int score;
        static int recordedHighScore;
        static TimeSpan recordedBestTime;
        static DateTime startTime;
        static DateTime timeOfLastMove;
        static bool drawThreeMode;
        static bool autoCompleteOption;
        static string gameDataFilePath;
        const string draw1GameDataFilePath = "draw1Scores.txt";
        const string draw3GameDataFilePath = "draw3Scores.txt";

        static void Main(string[] args)
        {
            Console.SetWindowSize(70, 30);
            NewGame(true);
            Display();

            while (true)
            {
                CheckForAutoComplete();
                if (table.CheckForWin()) {
                    ConsoleMessages.DisplayMessage("Game Over.", true, ConsoleColor.Green);

                    HandleHighScoreAndBestTime();

                    if (!NewGame(false))
                    {
                        break;
                    }
                }
                GetUserCommand();
                Display();
            }
        }

        static bool NewGame(bool startupSequence)
        {
            autoCompleteOption = true;
            if (!startupSequence)
            {
                while (true)
                {
                    ConsoleMessages.DisplayMessage("Do you want to start a new game? y/n or yes/no", true);
                    string confirmationInput = Console.ReadLine();
                    if (confirmationInput == "n" || confirmationInput == "no")
                    {
                        return false;
                    }
                    else if (confirmationInput == "y" || confirmationInput == "yes")
                    {
                        break;
                    }
                    else
                    {
                        ConsoleMessages.DisplayMessage("Invalid input", true, ConsoleColor.Red);
                    }
                }
            }

            Console.WriteLine("Choose a mode: Draw 1 or Draw 3: (Type '1' or '3'):");
            while (true)
            {
                string input = Console.ReadLine();
                if (input == "1")
                {
                    drawThreeMode = false;
                    gameDataFilePath = draw1GameDataFilePath;
                    break;
                }
                else if (input == "3")
                {
                    drawThreeMode = true;
                    gameDataFilePath = draw3GameDataFilePath;
                    break;
                }
                else
                {
                    ConsoleMessages.DisplayMessage("Error: Please type '1' or '3' to choose the mode.", true, ConsoleColor.Red);
                }
            }

            Console.WriteLine();

            deck = new Deck();
            table = new Table(deck);
            startTime = DateTime.UtcNow;
            timeOfLastMove = startTime;

            moveCounter = 0;
            score = 0;
            return true;
        }

        static void Display()
        {
            table.PrintTable();
            deck.DisplayHand(drawThreeMode);
            table.DisplayFoundation();
            DisplayTimer();
            DisplayScore();
            DisplayMoveCount();
        }

        static void GetUserCommand()
        {
            Console.WriteLine("\n======================================================================\n");
            Console.WriteLine("Enter a command. Type 'Help' to see a list of possible commands.");

            string input = Console.ReadLine().ToLower();
            Console.WriteLine();

            if (!ValidateInput(input))
            {
                Console.WriteLine("\n======================================================================\n\n");
                return;
            }

            string[] inputStrings = input.Split(' ');
            string firstString = inputStrings[0];

            //Move command
            if (firstString == "move" || firstString == "m")
            {
                string sourceCard = inputStrings[1];
                string destination = inputStrings[3];

                if (!VerifyMove(sourceCard, destination))
                {
                    return;
                }

                // search in table
                (bool foundCard, bool continueLooking) = table.MoveCard(sourceCard, destination, ref score);
                if (foundCard && !continueLooking)
                {
                    moveCounter++;
                }
                else if (!foundCard && continueLooking)
                {
                    // not found in table, search in hand
                    if (deck.GetTopOfHandName().ToLower() == sourceCard)
                    {
                        Card topCard = deck.RemoveTopCard();
                        if (table.MoveFromHand(sourceCard, destination, topCard.suit, topCard.value))
                        {

                            deck.deductPointsForCycle(drawThreeMode, ref score);
                            deck.DecrementVisibleHandCount(drawThreeMode);
                            score += 5;
                            moveCounter++;
                        }
                    }
                    else
                    {
                        if (!table.MoveFromFoundation(sourceCard, destination))
                        {
                            ConsoleMessages.DisplayMessage("Error: " + sourceCard.ToUpper() + " not available.", true, ConsoleColor.Red);
                        }
                        else
                        {
                            score -= 15;
                            moveCounter++;
                        }
                    }
                }
            }
            //Set Command
            else if (firstString == "set" || firstString == "s")
            {
                string[] strings = input.Split(' ');
                string sourceCard = strings[1];
                table.SetCard(sourceCard, deck, ref score, ref moveCounter, drawThreeMode);
            }
            else if (input == "help" || input == "h")
            {
                ShowPossibleCommands();
            }
            else if (input == "next" || input == "n")
            {
                if (deck.NextCardInHand(drawThreeMode))
                {
                    score = drawThreeMode ? Math.Max(score - 20, 0) : Math.Max(score - 100, 0);
                }
            }
            else if (input == "new game" || input == "ng")
            {
                NewGame(false);
            }
            else if (input == "scoring")
            {
                ShowScoringRules();
            }
            else if (input == "show hand")
            {
                
            }
            else if (input == "undo" || input == "u")
            {
                Undo();
            }
            else if (input == "high score" || input == "hs")
            {
                ShowHighScore();
            }
            else if (input == "best time" || input == "bt")
            {
                ShowBestTime();
            }
            Console.WriteLine("\n======================================================================\n\n");
        }

        static bool VerifyMove(string source, string destination)
        {
            //TODO: Leading white space should be acceptable input. 
            string error = "";
            char sourceSuit = source.Length >= 2 ? source[source.Length - 1] : 'n';
            char destSuit = destination.Length >= 2 ? destination[destination.Length - 1] : 'n';
            string sourceValue = source.Substring(0, source.Length - 1);
            string destValue = destination != "empty" ? destination.Substring(0, destination.Length - 1) : destination;

            if (sourceValue == "k")
            {
                if (destination != "empty")
                {
                    error = "Error: Invalid move. Can only move a k to \"empty\"";
                }
            }
            else if (sourceSuit == 'n')
            {
                error = "Error: invalid source card.";
            }
            else if (destSuit == 'n')
            {
                error = "Error: invalid destination card.";
            }
            else if ((sourceSuit == 's' || sourceSuit == 'c') && (destSuit != 'd' && destSuit != 'h'))
            {
                error = "Error: Invalid move. Cannot move suit " + sourceSuit + " to suit " + destSuit;
            }
            else if ((sourceSuit == 'd' || sourceSuit == 'h') && (destSuit != 's' && destSuit != 'c'))
            {
                error = "Error: Invalid move. Cannot move suit " + sourceSuit + " to suit " + destSuit;
            }
            else if (sourceValue == "a")
            {
                if (destValue != "2")
                {
                    error = "Error: Invalid move. Cannot move an a to a " + destValue;
                }
            } 
            else if (sourceValue == "j")
            {
                if (destValue != "q")
                {
                    error = "Error: Invalid move. Cannot move a j to a " + destValue;
                }
            }
            else if (sourceValue == "q")
            {
                if (destValue != "k")
                {
                    error = "Error: Invalid move. Cannot move a q to a " + destValue;
                }
            }
            else if (sourceValue == "10")
            {
                if (destValue != "j")
                {
                    error = "Error: Invalid move. Cannot move a 10 to a " + destValue;
                }
            }
            //TODO: Typed 'move q0s to jh' accidentally and caused an exception here
            else if (Int32.Parse(sourceValue) != Int32.Parse(destValue) -1)
            {
                error = "Error: Invalid move. Cannot move a " + sourceValue + " to a " + destValue;
            }

            if (error != "")
            {
                ConsoleMessages.DisplayMessage(error, true, ConsoleColor.Red);
                Console.WriteLine("\n======================================================================\n\n");
                return false;
            }

            return true;
        }

        static void DisplayTimer()
        {
            // Deduct 2 points for each 10 seconds elapsed. 
            TimeSpan test = (DateTime.UtcNow - timeOfLastMove) / 10;
            if (test.Seconds >= 1)
            {
                timeOfLastMove = DateTime.UtcNow;
                int pointsToDeduct = (2 * test.Seconds);
                // Don't allow the points to drop below 0
                score = Math.Max(0, score - pointsToDeduct);
            }

            TimeSpan time = (DateTime.UtcNow - startTime);
            Console.WriteLine("\nTime: " + (GetTimeString(time)));
        }

        private static string GetTimeString(TimeSpan time)
        {
            string hours = time.Hours > 9 ? time.Hours.ToString() : "0" + time.Hours.ToString();
            string minutes = time.Minutes > 9 ? time.Minutes.ToString() : "0" + time.Minutes.ToString();
            string seconds = time.Seconds > 9 ? time.Seconds.ToString() : "0" + time.Seconds.ToString();
            return hours + ":" + minutes + ":" + seconds;
        }

        static void DisplayScore()
        {
            Console.WriteLine("Score: " + score);
        }

        static void DisplayMoveCount()
        {
            Console.WriteLine("Moves: " + moveCounter);
        }

        static void ShowPossibleCommands()
        {
            Console.WriteLine("Possible Commands: Most can be called with their initials. i.e. 'H' instead of 'Help' and 'NG' instead of 'New Game'");
            Console.WriteLine("- Help: \n    Show the list of possible commands.");
            Console.WriteLine("- Move [Card to move] to [Destination card]: \n    Example, Move AS to 2C. \n    To move king to empty type 'move [KS, KD, KC, or KH] to empty'");
            Console.WriteLine("- Set [Card to move]: \n    Moves card to foundation \n    Example, Set AS.");
            Console.WriteLine("- Next: \n    Show next card in hand.");
            Console.WriteLine("- New Game: \n    Reset and start a new game.");
            Console.WriteLine("- Scoring: \n    Show the scoring rules.");
            Console.WriteLine("- Show Hand: \n    Show the entire hand.");
            Console.WriteLine("- Undo: \n    Undo the last move.");
            Console.WriteLine("- High Score: \n    Shows the current high score for the mode you are in (draw one/draw three.");
            Console.WriteLine("- Best Time: \n    Shows the current best time for the mode you are in (draw one/draw three.");
        }

        static void ShowScoringRules()
        {
            Console.WriteLine("• 10 points for each card moved to a suit stack.");
            Console.WriteLine("• 5 points for each card moved from the deck to a row stack.");
            Console.WriteLine("• 5 points for each card turned face-up in a row stack.");
            Console.WriteLine("• -2 points for each 10 seconds elapsed during a timed game.");
            Console.WriteLine("• -15 points for each card moved from a suit stack to a row stack.");
            Console.WriteLine("• -20 points for each pass through the deck after four passes (Draw Three option).");
            Console.WriteLine("• -100 points for each pass through the deck after one pass (Draw One option).");
        }

        static bool ValidateInput(string input)
        {
            string[] strings = input.Split(' ');
            string error = "";

            if (strings.Length == 0)
            {
                error = "Error: Invalid input. Type 'help' to see acceptable input.";
                return false;
            }

            string firstWord = strings[0];
            if (firstWord == "move" || firstWord == "m")
            {
                //check input word count and second word is "to"
                if (strings.Length != 4 || strings[2] != "to")
                {
                    error = "Error: Invalid move input. Type 'help' to see acceptable input.";
                }
                //check source and destination length
                else if (strings[1].Length < 2 || strings[1].Length > 3 || strings[3].Length < 2 || (strings[3].Length > 3 && strings[3] != "empty"))
                {
                    error = "Error: Source card value must be 2 or 3 characters long. Example: AS or 10D";
                }
                //check valid source card value
                else if (strings[1][0] != 'a' && strings[1][0] != '2' && strings[1][0] != '3' && strings[1][0] != '4' && strings[1][0] != '5' && strings[1][0] != '6' && strings[1][0] != '7' &&
                        strings[1][0] != '8' && strings[1][0] != '9' && strings[1][0] != 'j' && strings[1][0] != 'q' && strings[1][0] != 'k' && (strings[1][0] == '1' && strings[1][1] != '0'))
                {
                    error = "Error: Invalid source card value. Must be A, J, Q, K, or a value between 2 and 10";
                }
                //check valid source card suit
                else if (strings[1][strings[1].Length - 1] != 's' && strings[1][strings[1].Length - 1] != 'd' && strings[1][strings[1].Length - 1] != 'c' && strings[1][strings[1].Length - 1] != 'h')
                {
                    error = "Error: Invalid source card suit. Must be S, D, C, or H. (spades, diamonds, clubs, or hearts)";
                }
                //check valid destination card value
                else if (strings[3] != "empty" && strings[3][0] != 'a' && strings[3][0] != '2' && strings[3][0] != '3' && strings[3][0] != '4' && strings[3][0] != '5' && strings[3][0] != '6' && strings[3][0] != '7' &&
                        strings[3][0] != '8' && strings[3][0] != '9' && strings[3][0] != 'j' && strings[3][0] != 'q' && strings[3][0] != 'k' && (strings[3][0] == '1' && strings[3][1] != '0'))
                {
                    error = "Error: Invalid destination card value. Must be A, J, Q, K, a value between 2 and 10, or 'empty'";
                }
                //check valid destination card suit
                else if (strings[3] != "empty" && strings[3][strings[3].Length - 1] != 's' && strings[3][strings[3].Length - 1] != 'd' && strings[3][strings[3].Length - 1] != 'c' && strings[3][strings[3].Length - 1] != 'h')
                {
                    error = "Error: Invalid destination card suit. Must be S, D, C, or H. (spades, diamonds, clubs, or hearts), destination may also simply be 'empty'";
                }
            }
            else if (firstWord == "set" || firstWord == "s")
            {
                if (strings.Length != 2)
                {
                    error = "Error: Invalid set input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "help" || firstWord == "h")
            {
                if (strings.Length != 1)
                {
                    error = "Error: Invalid help input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "next" || firstWord == "n")
            {
                if (strings.Length != 1)
                {
                    error = "Error: Invalid next input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "new" || firstWord == "ng")
            {
                if (firstWord == "new" && (strings.Length != 2 || strings[1] != "game"))
                {
                    error = "Error: Invalid input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "high" || firstWord == "hs")
            {
                if (firstWord == "high" && (strings.Length != 2 || strings[1] != "score"))
                {
                    error = "Error: Invalid input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "best" || firstWord == "bt")
            {
                if (firstWord == "best" && (strings.Length != 2 || strings[1] != "time"))
                {
                    error = "Error: Invalid input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "scoring")
            {
                if (strings.Length > 1)
                {
                    error = "Error: Invalid scoring input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "show")
            {
                if (strings.Length != 2 || strings[1] != "hand")
                {
                    error = "Error: Invalid input. Type 'help' to see acceptable input.";
                }
            }
            else if (firstWord == "undo" || firstWord == "u")
            {
                if (strings.Length > 1)
                {
                    error = "Error: Invalid undo input. Type 'help' to see acceptable input.";
                }
            }
            else
            {
                error = "Error: \"" + input + "\" is not a valid command. Type \"Help\" to see a list of valid commands.";
            }
            if (error != "")
            {
                ConsoleMessages.DisplayMessage(error, true, ConsoleColor.Red);
                return false;
            }
            return true;
        }

        //TODO: Undo
        static void Undo()
        {
            
        }

        /// <summary>
        /// Prints the best time and sets the new best time if it was faster than the current best time
        /// </summary>
        private static void BestTimeMessage()
        {
            TimeSpan? bestTime = GetBestTime();

            TimeSpan time = (DateTime.UtcNow - startTime);
            Console.WriteLine();

            //if there is no current best time or if time is better than best time:
            if (!bestTime.HasValue || time < bestTime.Value)
            {
                ConsoleMessages.DisplayMessage($"New Best Time!: {GetTimeString(time)}", true, ConsoleColor.Green);
                recordedBestTime = time;
            }
            else
            {
                //Print exisiting best time message
                ConsoleMessages.DisplayMessage($"Best Time: {GetTimeString(bestTime.Value)}", true);
                recordedBestTime = bestTime.Value;
            }
        }

        private static TimeSpan? GetBestTime()
        {
            //If the file exists:
            if (File.Exists(gameDataFilePath))
            {
                using (StreamReader sr = File.OpenText(gameDataFilePath))
                {
                    string s;
                    while (!sr.EndOfStream)
                    {
                        if ((s = sr.ReadLine()) != null)
                        {
                            string[] words = s.Split(' ');
                            if (words.Length == 3 && words[0] == "Best" && words[1] == "Time:")
                            {
                                if (Double.TryParse(words[2], out double milliseconds))
                                {
                                    return TimeSpan.FromMilliseconds(milliseconds);
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static void HighScoreMessage()
        {
            int? highScore = GetHighScore();

            Console.WriteLine();

            if (!highScore.HasValue || score > highScore.Value)
            {
                ConsoleMessages.DisplayMessage($"New High Score!: {score}", true, ConsoleColor.Green);
                recordedHighScore = score;
            }
            else
            {
                ConsoleMessages.DisplayMessage($"High Score: {highScore.Value}", true);
                recordedHighScore = highScore.Value;
            }
        }

        private static int? GetHighScore() 
        {
            //If the file exists:
            if (File.Exists(gameDataFilePath))
            {
                using (StreamReader sr = File.OpenText(gameDataFilePath))
                {
                    string s;
                    while (!sr.EndOfStream)
                    {
                        if ((s = sr.ReadLine()) != null)
                        {
                            string[] words = s.Split(' ');
                            if (words.Length == 3 && words[0] == "High" && words[1] == "Score:")
                            {
                                if (Int32.TryParse(words[2], out int highScore))
                                {
                                    return highScore;
                                }
                            }
                        }
                    }
                }
            }
            return null;
        }

        private static void HandleHighScoreAndBestTime()
        {
            BestTimeMessage();
            HighScoreMessage();
            SetBestTimeAndHighScore(recordedBestTime, recordedHighScore);
        }

        private static void SetBestTimeAndHighScore(TimeSpan bestTime, int highScore)
        {
            using (StreamWriter sw = File.CreateText(gameDataFilePath))
            {
                sw.WriteLine($"Best Time: {bestTime.TotalMilliseconds}");
                sw.WriteLine($"High Score: {highScore}");
            }
        }

        private static void ShowHighScore()
        {
            int? highScore = GetHighScore();

            if (highScore.HasValue)
            {
                ConsoleMessages.DisplayMessage($"Current high score: {highScore}.", true);
            }
            else
            {
                ConsoleMessages.DisplayMessage("No high score recorded.", true, ConsoleColor.Red);
            }
        }

        private static void ShowBestTime()
        {
            TimeSpan? bestTime = GetBestTime();

            if (bestTime.HasValue)
            {
                ConsoleMessages.DisplayMessage($"Current best time: {GetTimeString(bestTime.Value)}.", true);
            }
            else
            {
                ConsoleMessages.DisplayMessage("No best time recorded.", true, ConsoleColor.Red);
            }
        }

        private static void CheckForAutoComplete()
        {
            if (!autoCompleteOption)
            {
                return;
            }
            if (!drawThreeMode && table.AllCardsFlipped())
            {
                bool autocomplete = false;
                //Prompt user for autocomplete option:
                while (true)
                {
                    ConsoleMessages.DisplayMessage("\n Would you like this game to auto-complete? (yes (y) or no (n))", true);
                    string response = Console.ReadLine().ToLower();
                    if (response == "y" || response == "yes")
                    {
                        autocomplete = true;
                        break;
                    }
                    else if (response == "n" || response == "no")
                    {
                        break;
                    }
                    else
                    {
                        ConsoleMessages.DisplayMessage("Error, invalid input", true, ConsoleColor.Red);
                    }
                }
                //If user says yes: 
                if (autocomplete) 
                {
                    while (table.RemainingCardCount() != 0 || deck.RemainingCardCount() != 0)
                    {
                        table.Autocomplete(deck);
                        System.Threading.Thread.Sleep(500);
                        Console.WriteLine("\n======================================================================\n");
                        Display();
                    }
                }
                else
                {
                    //turn off autocomplete option flag
                    autoCompleteOption = false;
                }
            }
            //TODO: DrawThree mode autocomplete
        }
    }
}