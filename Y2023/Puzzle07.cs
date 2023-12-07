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
            return $"{SortAndRankHands(false)}";
        }

        [Description("Using the new joker rule, find the rank of every hand in your set. What are the new total winnings?")]
        public override string SolvePart2() {
            return $"{SortAndRankHands(true)}";
        }

        private int SortAndRankHands(bool useJokers) {
            Hand.UseJokers = useJokers;
            for (int i = 0; i < hands.Count; i++) {
                hands[i].DetermineType();
            }
            hands.Sort();

            int total = 0;
            for (int i = 0; i < hands.Count; i++) {
                total += hands[i].Amount * (i + 1);
            }
            return total;
        }

        private class Hand : IComparable<Hand> {
            public static bool UseJokers = false;
            public Card[] Cards = new Card[5];
            public int Amount;
            public HandType Type;

            public Hand(string hand) {
                for (int i = 0; i < 5; i++) {
                    char c = hand[i];
                    Cards[i] = c switch {
                        '2' => Card.Two,
                        '3' => Card.Three,
                        '4' => Card.Four,
                        '5' => Card.Five,
                        '6' => Card.Six,
                        '7' => Card.Seven,
                        '8' => Card.Eight,
                        '9' => Card.Nine,
                        'T' => Card.Ten,
                        'J' => Card.Jack,
                        'Q' => Card.Queen,
                        'K' => Card.King,
                        'A' => Card.Ace,
                        _ => throw new NotSupportedException()
                    };
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
                Card[] cards = new Card[13];
                for (int i = 0; i < 5; i++) {
                    counts[(int)Cards[i]]++;
                    cards[(int)Cards[i]] = Cards[i];
                }

                int jokerCount = UseJokers ? counts[(int)Card.Jack] : 0;
                if (jokerCount >= 4) { Type = HandType.FiveOfAKind; return; }
                if (UseJokers) { Array.Sort(counts, cards); }

                bool hasPair = false;
                bool hasThree = false;
                for (int i = 12; i >= 0; i--) {
                    if (counts[i] == 0 || (UseJokers && cards[i] == Card.Jack)) { continue; }

                    switch (counts[i] + jokerCount) {
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