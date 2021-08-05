using System;
using System.Collections.Generic;
using System.Text;

namespace Solitaire
{
    class Card
    {
        public int value;
        public int suit;
        public string color;
        public bool faceup;
        private string readableName;
        public Card(int suit, int value, bool faceup = false)
        {
            this.value = value;
            this.suit = suit;
            this.color = suit == 0 || suit == 2 ? "black" : "red";
            this.faceup = faceup;
            this.readableName = GetReadableCombined(true);
        }

        public string GetReadableValue()
        {
            string retValue;
            switch(value)
            {
                case 0:
                    retValue = "A";
                    break;
                case 10:
                    retValue = "J";
                    break;
                case 11:
                    retValue = "Q";
                    break;
                case 12:
                    retValue = "K";
                    break;
                default:
                    retValue = (value + 1).ToString();
                    break;
            }
            return retValue;
        }
        public string GetReadableSuit(bool displaySuits = false)
        {
            if (displaySuits)
            {
                return suit switch
                {
                    0 => "♠",
                    1 => "♦",
                    2 => "♣",
                    _ => "♥"
                };
            }
            else
            {
                return suit switch
                {
                    0 => "S",
                    1 => "D",
                    2 => "C",
                    _ => "H"
                };
            }
        }

        public string GetReadableCombined(bool displaySuits = false)
        {
            return GetReadableValue() + GetReadableSuit(displaySuits);
        }
    }
}