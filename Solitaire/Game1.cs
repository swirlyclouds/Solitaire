using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;

namespace Solitaire
{
    public class Game1 : Game
    {
        Texture2D cards;
        Texture2D empty;
        Texture2D back;


        Card[] deck = new Card[52];
        int cn = 0;

        Stack<Card> undrawnPile = new Stack<Card>();
        Stack<Card> drawnPile = new Stack<Card>();
        List<Card> hand;
        Dictionary<char, SortedPile> sortedPiles;
        Column[] columns = new Column[7];

        private bool unclicked;
        private bool isHolding;
        private char origin;

        private Point lastPosition;
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private MouseState mouseState, prevMouseState;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            sortedPiles = new Dictionary<char, SortedPile>();
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            unclicked = true;
            this.isHolding = false;
            this.hand = new List<Card>();

            sortedPiles.Add('H', new SortedPile());
            sortedPiles.Add('C', new SortedPile());
            sortedPiles.Add('S', new SortedPile());
            sortedPiles.Add('D', new SortedPile());

            for (int i = 0; i < 7; i++)
                columns[i] = new Column(new Rectangle(10 + 90 * i, 150, 70, 95));


        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            // create cards


            XmlDocument doc = new XmlDocument();
            doc.Load("playingCards.xml");

            XmlNode root = doc.FirstChild;

            //load cards into deck from XML
            if (root.HasChildNodes)
            {
                for (int i = 0; i < root.ChildNodes.Count; i++)
                {

                    string filename = root.ChildNodes[i].Attributes["name"].Value;
                    string value = Regex.Match(filename, @"((\d)+|(\S))[.](.{3})\s*$").Value;
                    value = value.Substring(0, value.Length - 4);
                    Console.WriteLine(value);

                    char suit = filename[4];
                    if (value == "K")
                        value = "13";
                    else if (value == "Q")
                        value = "12";
                    else if (value == "J")
                        value = "11";
                    else if (value == "A")
                        value = "1";

                    deck[cn] = new Card(value, suit, root.ChildNodes[i].Attributes["x"].Value, root.ChildNodes[i].Attributes["y"].Value);
                    cn++;
                }
            }

            cn = 0;

            shuffleDeck();

            foreach (Card card in deck)
            {
                undrawnPile.Push(card);
            }
            for(int i = 0; i < 3; i++)
            columns[0].Add(undrawnPile.Pop());
            base.Initialize();
        }

        protected override void LoadContent()
        {


            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here

            cards = Content.Load<Texture2D>("playingCards");
            back = Content.Load<Texture2D>("cardBack_green5");
            empty = Content.Load<Texture2D>("cardBack_blue1");


        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            if (GamePad.GetState(PlayerIndex.One).Buttons.A == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Left))
            {
                if(undrawnPile.Count > 0)
                    drawnPile.Push(undrawnPile.Pop());
                else
                {
                    undrawnPile = new Stack<Card>(drawnPile);
                    drawnPile.Clear();
                }
                Console.WriteLine(deck[cn].getCardPath());
            }

            foreach (Column col in columns)
                col.update();

            this.mouseState = Mouse.GetState();

            if(this.mouseState.LeftButton == ButtonState.Pressed)
            {
                if ( this.mouseState.Position.Y >= 10 && this.mouseState.Position.Y <= 105)
                {
                    if (this.mouseState.Position.X >= 720 && this.mouseState.Position.X <= 790 && unclicked)
                    {
                        unclicked = false;
                        if (undrawnPile.Count > 0)
                            drawnPile.Push(undrawnPile.Pop());
                        else
                        {
                            undrawnPile = new Stack<Card>(drawnPile);
                            drawnPile.Clear();
                        }
                    }

                    if (this.mouseState.Position.X >= 630 && this.mouseState.Position.X <= 700)
                    {

                        if (this.mouseState.Position != this.prevMouseState.Position)
                        {
                            if (drawnPile.Count > 0 && !this.isHolding)
                            {
                                hand.Add(drawnPile.Pop());
                                this.origin = 'H';
                            }
                            this.isHolding = true;
                        }
                        else
                        {

                        }
                    }
                }
            }
            if(this.mouseState.LeftButton == ButtonState.Released)
            {
                unclicked = true;
                this.prevMouseState = mouseState;

                if (this.isHolding)
                {
                    if (columns[0].position.Y <= mouseState.Position.Y)
                    {
                        foreach (Column col in columns)
                        {

                            if (mouseState.Position.X >= col.position.X && mouseState.Position.X <= col.position.X + 70)
                            {
                                if (col.GetCards().Count > 0)
                                {
                                    Card c = col.GetCards().Last();
                                    Console.WriteLine(col.GetCards().Last().getSuit());
                                    if (((c.getSuit() == 'C' || c.getSuit() == 'S') && (hand[0].getSuit() == 'H' || hand[0].getSuit() == 'D'))
                                        ||
                                        (c.getSuit() == 'H' || c.getSuit() == 'D') && (hand[0].getSuit() == 'C' || hand[0].getSuit() == 'S')
                                        )
                                    {
                                        Console.WriteLine("add to column");
                                        col.GetCards().AddRange(hand);
                                        hand.Clear();
                                        break;
                                    }
                                }
                                else
                                {
                                    col.GetCards().AddRange(hand);
                                    hand.Clear();
                                    break;
                                }
                            }
                        }
                        
                    }
                    else
                    {
                        switch (this.origin)
                        {
                            case 'H':
                                drawnPile.Push(hand[0]);
                                hand.Clear();
                                break;
                        }
                    }

                }
                this.isHolding = false;
            }
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);


            _spriteBatch.Begin();

            // Deck and drawn cards
            if(drawnPile.Count > 0)
                _spriteBatch.Draw(cards, new Rectangle(630, 10, 70, 95), new Rectangle(drawnPile.Peek().getX(), drawnPile.Peek().getY(), 140, 190), Color.White);
            if(undrawnPile.Count > 0)
                _spriteBatch.Draw(back, new Rectangle(720, 10, 70, 95), new Rectangle(0, 0, back.Width, back.Height), Color.White);

            int n = 0;
            // 4 stacks
            foreach (SortedPile sp in sortedPiles.Values)
            {
                if (sp.count() > 0)
                    _spriteBatch.Draw(cards, new Rectangle(10 + n * 90, 10, 70, 95), new Rectangle(sp.topCard().getX(), sp.topCard().getY(), 140, 190), Color.White);
                else
                    _spriteBatch.Draw(back, new Rectangle(10 + n * 90, 10, 70, 95), new Rectangle(0, 0, back.Width, back.Height), Color.White);
                n++;
            }
            n = 0;
            // 7 columns
            foreach (Column col in columns)
            {
                int i;
                _spriteBatch.Draw(empty, col.position, new Rectangle(0, 0, empty.Width, empty.Height), Color.White);
                for (i = 0; i < col.getSizeofCoveredStack(); i++)
                    _spriteBatch.Draw(back, new Rectangle(col.position.X, col.position.Y + 30 * i, 70, 95), new Rectangle(0, 0, back.Width, back.Height), Color.White);
                foreach (Card card in col.GetCards())
                {
                    _spriteBatch.Draw(cards, new Rectangle(col.position.X, col.position.Y + 30 * i, 70, 95), new Rectangle(card.getX(), card.getY(), 140, 190), Color.White);
                    i++;
                }
                n++;
            }

            if(hand.Count > 0)
                _spriteBatch.Draw(cards, new Rectangle(this.mouseState.X-28, this.mouseState.Y-38, 56, 76), new Rectangle(hand[0].getX(), hand[0].getY(), 140, 190), Color.White);


            _spriteBatch.End();

            base.Draw(gameTime);
        }

        //Fisher-Yates Shuffle
        protected void shuffleDeck()
        {
            Random cardIndex = new Random();
            for (int i = 0; i < 52; i++)
            {
                int j = cardIndex.Next(52);
                Card temp = deck[i];
                deck[j] = deck[i];
                deck[i] = temp;
            }
        }
    }
}