using AdventOfCode.Common;
using AdventOfCode.Core;
using System;
using System.Collections.Generic;
using System.ComponentModel;
namespace AdventOfCode.Y2023 {
    [Description("Camel Cards")]
    public class Puzzle07 : ASolver {
        private List<Hand> hands = new();

        public override void Setup() {
            string[] lines = Input.Split('\n');
            foreach (string line in lines) {
                hands.Add(new Hand(line));
            }
        }

        [Description("Find the rank of every hand in your set. What are the total winnings?")]
        public override string SolvePart1() {
            Hand.UseJokers = false;
            for (int i = 0; i < hands.Count; i++) {
                Hand hand = hands[i];
                hand.DetermineType();
            }
            hands.Sort();

            int total = 0;
            for (int i = 0; i < hands.Count; i++) {
                Hand hand = hands[i];
                total += hand.Amount * (i + 1);
            }
            return $"{total}";
        }

        [Description("Using the new joker rule, find the rank of every hand in your set. What are the new total winnings?")]
        public override string SolvePart2() {
            Hand.UseJokers = true;
            for (int i = 0; i < hands.Count; i++) {
                Hand hand = hands[i];
                hand.DetermineType();
            }
            hands.Sort();

            int total = 0;
            for (int i = 0; i < hands.Count; i++) {
                Hand hand = hands[i];
                total += hand.Amount * (i + 1);
            }
            return $"{total}";
        }

        private class Hand : IComparable<Hand> {
            public static bool UseJokers = false;
            public Card[] Cards = new Card[5];
            public int Amount;
            public HandType Type;

            public Hand(string hand) {
                for (int i = 0; i < 5; i++) {
                    char c = hand[i];
                    switch (c) {
                        case '2': Cards[i] = Card.Two; break;
                        case '3': Cards[i] = Card.Three; break;
                        case '4': Cards[i] = Card.Four; break;
                        case '5': Cards[i] = Card.Five; break;
                        case '6': Cards[i] = Card.Six; break;
                        case '7': Cards[i] = Card.Seven; break;
                        case '8': Cards[i] = Card.Eight; break;
                        case '9': Cards[i] = Card.Nine; break;
                        case 'T': Cards[i] = Card.Ten; break;
                        case 'J': Cards[i] = Card.Jack; break;
                        case 'Q': Cards[i] = Card.Queen; break;
                        case 'K': Cards[i] = Card.King; break;
                        case 'A': Cards[i] = Card.Ace; break;
                    }
                }

                Amount = hand.Substring(6).ToInt();
            }

            public int CompareTo(Hand other) {
                int comp = Type.CompareTo(other.Type);
                if (comp != 0) { return comp; }
                for (int i = 0; i < 5; i++) {
                    Card myCard = UseJokers && Cards[i] == Card.Jack ? Card.Joker : Cards[i];
                    Card otherCard = UseJokers && other.Cards[i] == Card.Jack ? Card.Joker : other.Cards[i];
                    comp = myCard.CompareTo(otherCard);
                    if (comp != 0) { return comp; }
                }
                return 0;
            }
            public void DetermineType() {
                int[] counts = new int[13];
                for (int i = 0; i < 5; i++) {
                    counts[(int)Cards[i]]++;
                }
                (int count, Card card)[] cardCounts = new (int, Card)[13];
                for (int i = 0; i < 13; i++) {
                    cardCounts[i] = (counts[i], (Card)i);
                }
                Array.Sort(cardCounts);

                int jokerCount = UseJokers ? counts[(int)Card.Jack] : 0;
                if (jokerCount >= 4) { Type = HandType.FiveOfAKind; return; }

                bool hasPair = false;
                bool hasThree = false;
                for (int i = 12; i >= 0; i--) {
                    if (cardCounts[i].count == 0 || (UseJokers && cardCounts[i].card == Card.Jack)) { continue; }

                    switch (cardCounts[i].count + jokerCount) {
                        case 5: Type = HandType.FiveOfAKind; return;
                        case 4: Type = HandType.FourOfAKind; return;
                        case 3:
                            if (hasPair) { Type = HandType.FullHouse; return; }
                            hasThree = true;
                            jokerCount = 0;
                            break;
                        case 2:
                            if (hasThree) { Type = HandType.FullHouse; return; }
                            if (hasPair) { Type = HandType.TwoPair; return; }
                            hasPair = true;
                            jokerCount = 0;
                            break;
                    }
                }

                if (hasPair) { Type = HandType.Pair; return; }
                if (hasThree) { Type = HandType.ThreeOfAKind; return; }
                Type = HandType.HighCard;
            }
            public override string ToString() {
                return $"{Cards[0]} {Cards[1]} {Cards[2]} {Cards[3]} {Cards[4]} = {Type}";
            }
        }
        private enum HandType {
            HighCard,
            Pair,
            TwoPair,
            ThreeOfAKind,
            FullHouse,
            FourOfAKind,
            FiveOfAKind
        }
        private enum Card {
            Joker = -1,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King,
            Ace
        }
    }
}