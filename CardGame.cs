using System;
using System.Collections.Generic;
using System.Linq;

namespace CardGame
{
    class Program
    {
        static void Main(string[] args)
        {
            //Global Initialization
            int currentRound = 1;
            int currentPlayerTurn = 0;
            int handLimit = 5;
            int totalRounds = 13;
            int totalPlayers = 4;
            int deckSize = 52 * totalPlayers;
            Deck myDeck = new Deck(deckSize);
            Deck table = new Deck();

            myDeck.Shuffle(myDeck);

            List<Player> players = new List<Player>();

            Player myPlayer = new Player("Name");
            Player myBot1 = new Player();
            Player myBot2 = new Player();
            Player myBot3 = new Player();

            players.Add(myPlayer);
            players.Add(myBot1);
            players.Add(myBot2);
            players.Add(myBot3);

            for (int k = 0; k < handLimit; k++)
            {
                for (int i = 0; i < players.Count; i++)
                {
                    players[i].Draw(myDeck);
                }
            }

            Console.Write("Welcome to CardGame!\n\n");
            Console.Write("The goal of this game is to match the table's score value to the current round number by adding or subtracting the value of the cards in your hand. The first player to do so in the current round collects all the cards played that round. The player who has collected the most cards at the end of the game wins!\n\n");
            Console.Write("Press any key to continue.\n");
            Console.ReadLine();
            Console.Clear();

            //Game Loop
            while (currentRound < totalRounds && myDeck.deck.Count > 0)
            {

                while (currentPlayerTurn < players.Count)
                {
                    //draw the current player's hand up to the hand limit at the beginning of the turn
                    if (players[currentPlayerTurn].hand.Count < handLimit)
                    {
                        players[currentPlayerTurn].Draw(myDeck);
                    }

                    //Sort current player's hand by smallest to largest
                    players[currentPlayerTurn].hand.Sort();

                    if (players[currentPlayerTurn].isBot == true)
                    {
                        //take an AI turn
                        //if currentScore < currentRound, add the card that brings currentScore closest to currentRound
                        //else, subtract the card that will bring currentScore closest to currentRound
                        Console.Write("Taking AI #{0}'s Turn\n", currentPlayerTurn);

                        bool AIHasPlayed = false;

                        for (int j = 0; j < (players[currentPlayerTurn].hand.Count); j++)
                        {
                            //iterate through AI player's hand
                            if (players[currentPlayerTurn].hand[j].value + table.tableScore == currentRound) //if a card will win the round, play it
                            {
                                Console.Write("AI #{0} adds {1} of {2}\n", currentPlayerTurn, players[currentPlayerTurn].hand[j].value, players[currentPlayerTurn].hand[j].suit);
                                players[currentPlayerTurn].PlayCard(j, 1, table);
                                AIHasPlayed = true;
                                break;
                            }
                            else if (table.tableScore - players[currentPlayerTurn].hand[j].value == currentRound) //if a card will win the round, play it
                            {
                                Console.Write("AI #{0} subtracts {1} of {2}\n", currentPlayerTurn, players[currentPlayerTurn].hand[j].value, players[currentPlayerTurn].hand[j].suit);
                                players[currentPlayerTurn].PlayCard(j, 2, table);
                                AIHasPlayed = true;
                                break;
                            }
                        }

                        if(AIHasPlayed == false) //AI does not have a card that will win the round
                        {
                            if(table.tableScore < currentRound) //if the table's score is less than the round score, add the lowest value card
                            {
                                Console.Write("AI #{0} adds {1} of {2}\n", currentPlayerTurn, players[currentPlayerTurn].hand[0].value, players[currentPlayerTurn].hand[0].suit);
                                players[currentPlayerTurn].PlayCard(0, 1, table);
                            }
                            else //if the table's score is greater than the round score, subtract the lowest value card
                            {
                                Console.Write("AI #{0} subtracts {1} of {2}\n", currentPlayerTurn, players[currentPlayerTurn].hand[0].value, players[currentPlayerTurn].hand[0].suit);
                                players[currentPlayerTurn].PlayCard(0, 2, table);
                            }
                            
                            AIHasPlayed = true;
                        }
                    }
                    else //take a human turn
                    {
                        Console.Write("The current round value is: {0}\n", currentRound);
                        Console.Write("The table score value is: {0}\n", table.tableScore);

                        if (players[currentPlayerTurn].hand.Count < handLimit)
                        {
                            players[currentPlayerTurn].Draw(myDeck);
                        }

                        players[currentPlayerTurn].hand.Sort();
                        
                        for (int j = 0; j < players[currentPlayerTurn].hand.Count; j++)
                        {
                            Console.Write("{2}. {0} {1}\n", players[currentPlayerTurn].hand[j].suit, players[currentPlayerTurn].hand[j].value, j + 1);
                        }

                        Console.Write("Pick a card from your hand (1 - 5): \n");
                        int cardChoice = int.Parse(Console.ReadLine());

                        Console.Write("Add or Subtract (1 or 2)?\n");
                        int operationChoice = int.Parse(Console.ReadLine());

                        players[currentPlayerTurn].PlayCard(cardChoice - 1, operationChoice, table);
                    }

                    Console.Write("The table score is now: {0}\n", table.tableScore);
                    Console.Write("#####################################\n");
                    currentPlayerTurn++;

                    //check if round advancement condition has been reached, if so increment the round
                    if (table.tableScore == currentRound)
                    {
                        //current player has achieved round win condition
                        Console.Write("#####################################\n");
                        Console.Write("Round value reached, advancing round.\n");
                        Console.Write("#####################################\n");

                        players[currentPlayerTurn - 1].Collect(table);

                        currentRound++;

                        //fixes a strange condition where a player would get skipped
                        if (currentPlayerTurn == totalPlayers)
                        {
                            currentPlayerTurn = 0;
                        }
                        break;
                     }

                    if (currentPlayerTurn == totalPlayers)
                    {
                        currentPlayerTurn = 0;
                    }
                }
            }

            //End condition reached

            Console.Clear();

            if(myDeck.deck.Count == 0)
            {
                Console.Write("Deck is out of cards, you lose.\n");
            }
            else
            {
                int highScore = 0;
                int highScorePlayer = 0;

                for (int i = 0; i < players.Count; i++)
                {
                    if(players[i].pile.Count() > highScore)
                    {
                        highScore = players[i].pile.Count();
                        highScorePlayer = i;
                    }
                }

                Console.Write("The winner is Player {0} with {1} total cards acquired:\n", highScorePlayer + 1, highScore);

                for (int j = 0; j < players[highScorePlayer].pile.Count; j++)
                {
                    Console.Write("{0} {1}\n", players[highScorePlayer].pile[j].suit, players[highScorePlayer].pile[j].value);
                }
            }

            Console.Read();
        }
    }

    public class Player
    {
        public string playerName = "Player1";
        public List<Card> hand;
        public List<Card> pile;

        public int currentScore = 0;
        public bool isBot = false;

        public Player()
        {
            this.playerName = "Robot";
            this.hand = new List<Card>();
            this.pile = new List<Card>();
            this.currentScore = 0;
            this.isBot = true;
        }

        public Player(string name)
        {
            this.playerName = name;
            this.hand = new List<Card>();
            this.pile = new List<Card>();
            this.currentScore = 0;
            this.isBot = false;
        }

        public void Draw(Deck deck)
        {
            //take a card from the deck and add it to hand
            hand.Add(deck.deck[0]);
            deck.deck.RemoveAt(0);
        }

        public void Collect(Deck table)
        {
            //take all cards from the table and add it to pile
            while(table.deck.Count > 0)
            {
                pile.Add(table.deck[0]);
                table.deck.RemoveAt(0);
            }
            //reset tableScore to 0
            table.tableScore = 0;
        }

        public void PlayCard(int handIndex, int operation, Deck table)
        {
            if (handIndex < 5) //error handling
            {
                table.deck.Add(hand[handIndex]);

                if (operation == 1)
                {
                    table.tableScore = table.tableScore + hand[handIndex].value;
                }
                else if (operation == 2)
                {
                    table.tableScore = table.tableScore - hand[handIndex].value;
                }

                hand.RemoveAt(handIndex);
            }
        }
    }

    public class Card : IComparable<Card>
    {
        public string suit = "None";
        public int value = 0;

        //allow two cards to be compared based on their values
        public int CompareTo(Card other)
        {
            return this.value.CompareTo(other.value);
        }
    }

    public class Deck
    {
        public List<Card> deck;

        public String[] suits = new string[] { "Diamonds", "Hearts", "Clubs", "Spades" };

        public int suitIndex = 0;

        private static Random rng = new Random();

        public int tableScore = 0;

        public Deck()
        {
            deck = new List<Card>();
        }

        public Deck(int size)
        {
            deck = new List<Card>();

            for(int i = 0; i < size; i++)
            {
                Card newCard = new Card();

                newCard.value = (i % 13) + 1;

                if(i % 13 == 0 && suitIndex < 4)
                {
                    suitIndex++;
                }

                if(suitIndex == 4)
                {
                    suitIndex = 0;
                }

                newCard.suit = suits[suitIndex];

                deck.Add(newCard);
            }
        }

        public void Shuffle(Deck deck)
        {
            rng = new Random();
            int n = deck.deck.Count;
            while (n > 1)
            {
                int k = rng.Next(n);
                n--;
                Card temp = deck.deck[n];
                deck.deck[n] = deck.deck[k];
                deck.deck[k] = temp;
            }
        }
    }
}
