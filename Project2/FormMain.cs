using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Media;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Project2
{
    public partial class FormMain : Form
    {
        // Single GameHelper instance used to make certain aspects of
        // Windows Forms game development easier. The main method here
        // will be the game_Update method which is really an event
        // handler--just like a button click only here the event
        // "fires" 30 times per second (by default) based on a timer.
        // This object also provides methods to make it easy to check
        // if a specific key is currently being pressed. For example:
        //
        //    game.IsPressed(Keys.A)
        //
        // will return true if A is currently pressed and false if not.
        // Usually you will use this in your game_Update method (or in
        // a method called from game_Update) to control your "player".
        private GameHelper game;
        List<PictureBox> good = new List<PictureBox>();
        List<PictureBox> bad = new List<PictureBox>();
        Random r = new Random();
        Random r2 = new Random();
        Random r3 = new Random();

        int lineSpeed = 10;
        int sendSpeed = 25;
        int updateSpeed = 25;
        int bonusreq = 0;
        bool hasbonus = true;
        int nchoice;
        int toysstole = 0;
        int screwups = 5;
        bool startdone = false;
        bool guidedone = false;
        bool firstitemsent = false;
        int spacecooldown;
        bool limbo = false;

        SoundPlayer s = new SoundPlayer(Properties.Resources.song);
        SoundPlayer l = new SoundPlayer(Properties.Resources.loser);
        bool playsong = true;
        


        int score = 0;

        public FormMain()
        {
            InitializeComponent();

            game = new GameHelper(this); // Initialize the GameHelper instance
            game.Update += game_Update;  // Set up the Update event handler method
            game.Start();                // Start the game timer--game_Update is 
            // now being called--you can use game.Stop()
            // to temporarily (or permanently) stop the
            // game timer if the game is over
            
        }

        // Almost all of your logic will happen in here. The method
        // runs 30 times per second. You will likely be doing a lot
        // of stuff in here, so you may want to organize the logic
        // into a number of other methods that this method will call.
        void game_Update(object sender, EventArgs e)
        {
            
            toggleSong();
            playerKeys();
            
            actualGame();




        }

        public void toggleSong()
        {


        }
        public void clearGame()
        {
            try
            {
                
                foreach (PictureBox b in bad)
                {
                    //bad.Remove(b);
                    this.Controls.Remove(b);
                   // bad.Clear();
                }
                foreach (PictureBox g in good)
                {
                    //good.Remove(g);
                    this.Controls.Remove(g);
                    //good.Clear();
                }
                bad.Clear();
                good.Clear();
                //game.Stop();
            }
            catch
            {
               // MessageBox.Show("error");
            }
        }
        public void endGame()
        {
            if (screwups == 0) //letting your screwups run out will get you fired
            {
                l.PlaySync();
                spacecooldown = 70;
                clearGame();
                limbo = true;
                firstitemsent = false;
                lineSpeed = 10;
                bonus1.Visible = false;
                bonus2.Visible = false;
                neglectLoss.Visible = true;
            }
            if (toysstole == 5) //stealing toys will get you fired
            {
                l.PlaySync();
                spacecooldown = 70;
                clearGame();
                limbo = true;
                firstitemsent = false;
                lineSpeed = 10;
                bonus1.Visible = false;
                bonus2.Visible = false;
                stoleLoss.Visible = true;
            }
            if (score >= 5000) //win but not as good as aaron
            {
                spacecooldown = 70;
                clearGame();
                limbo = true;
                firstitemsent = false;
                lineSpeed = 10;
                bonus1.Visible = false;
                bonus2.Visible = false;
                youWonpic.Visible = true;
            }
        }


        private void FormMain_Load(object sender, EventArgs e)
        {
           
        }
        public void actualGame()
        {
            if (limbo == false)
            {
                if (guidedone == true)
                {

                    if (startdone == true)
                    {
                        endGame();
                        conveyorLogic();
                        badDelivery();
                        goodDelivery();
                        scoreNspeedLogic();
                        productRemoval();

                        nchoice = r2.Next(1, 5);
                    }
                    else
                    {

                        if (start1.Visible == true && start2.Visible == false)
                        {
                            start1.Visible = false;
                            start2.Visible = true;
                        }
                        else if (start1.Visible == false && start2.Visible == true)
                        {
                            start1.Visible = true;
                            start2.Visible = false;
                        }
                        else if (start1.Visible == false && start2.Visible == false)
                        {
                            start1.Visible = true;
                        }
                    }
                }
                else
                {
                    guidepic.Visible = true;
                    if (guidecont1.Visible == true && guidecont2.Visible == false)
                    {
                        guidecont1.Visible = false;
                        guidecont2.Visible = true;
                    }
                    else if (guidecont1.Visible == false && guidecont2.Visible == true)
                    {
                        guidecont1.Visible = true;
                        guidecont2.Visible = false;
                    }
                    else if (guidecont1.Visible == false && guidecont2.Visible == false)
                    {
                        guidecont1.Visible = true;
                    }
                }
            }
        }


        public void scoreNspeedLogic()
        {
            toysleft.Text = "Toys Stolen: " + toysstole + "/5";
            screwupsleft.Text = "Screw Ups Left: " + screwups + "/5";
            thespot.SendToBack();
            bonusTimeleft.Text = "Items until next bonus: " + (bonusreq) + "!";
            machinespeedlabel.Text = "" + lineSpeed + " MPH"; //the "speed" of the conveyorbelt
            scorelabel.Text = "" + score;  //the user's score

        }
        public void conveyorLogic()
        {
            if (sendSpeed > 0)
            {
                sendSpeed--;
                if (sendSpeed == 0)
                {
                    sendItem(r.Next(1, 3)); //decides which type of item to send on conveyorbelt
                    sendSpeed = updateSpeed;

                    /* BONUS */
                    if (hasbonus == false && bonusreq == 0)
                    {
                        hasbonus = true;
                    }

                    if (score > 999 && hasbonus == true)
                    {

                        bonusTimeleft.Visible = false;

                        if (bonus1.Visible == true && bonus2.Visible == false)
                        {
                            bonus1.Visible = false;
                            bonus2.Visible = true;
                        }
                        else if (bonus1.Visible == false && bonus2.Visible == true)
                        {
                            bonus1.Visible = true;
                            bonus2.Visible = false;
                        }
                        else if (bonus1.Visible == false && bonus2.Visible == false)
                        {
                            bonus1.Visible = true;
                        }

                    }
                    else if (score > 999 && hasbonus == false)
                    {
                        bonusTimeleft.Visible = true;
                        bonus1.Visible = false;
                        bonus2.Visible = false;

                    }
                    else
                    {
                        bonusTimeleft.Visible = false;
                        bonus1.Visible = false;
                        bonus2.Visible = false;
                    }
                    /* END BONUS */

                    /* SPEED ACELERATION */
                    if (lineSpeed < 40)
                    {
                        lineSpeed++;
                    }
                    if (lineSpeed > 39 && lineSpeed < 56 && score > 500)
                    {
                        lineSpeed++;
                    }
                    if (lineSpeed > 54 && lineSpeed < 70 && score > 1000)
                    {
                        lineSpeed++;
                    }
                    if (lineSpeed > 69 && lineSpeed < 81 && score > 2000)
                    {
                        lineSpeed++;
                    }

                    /* END SPEED ACCELERATION */

                }
            }
        }
        public void sendItem(int type)
        {
            if (firstitemsent == false)
            {
                firstitemsent = true;
            }
            if (hasbonus == false)
            {
                bonusreq--;
            }

            if (type == 1)
            {
                //good items
                PictureBox gooditem = new PictureBox();
                gooditem.Width = 90;
                gooditem.Height = 70;
                gooditem.BackColor = System.Drawing.Color.Transparent;
                gooditem.BringToFront();
                gooditem.Parent = thespot;


                if (nchoice == 1)
                {
                    gooditem.Image = global::Project2.Properties.Resources.bear;
                    gooditem.Parent = thespot;
                }
                else if (nchoice == 2)
                {
                    gooditem.Image = global::Project2.Properties.Resources.car;
                    gooditem.Parent = thespot;
                }
                else if (nchoice == 3)
                {
                    gooditem.Image = global::Project2.Properties.Resources.lego;
                    gooditem.Parent = thespot;
                }
                else if (nchoice == 4)
                {
                    gooditem.Image = global::Project2.Properties.Resources.toy;
                    gooditem.Parent = thespot;
                }

                gooditem.Top = outtube.Top;
                gooditem.Left = outtube.Left - 90;
                this.Controls.Add(gooditem);
                good.Add(gooditem);


            }
            else
            {
                //bad items
                PictureBox baditem = new PictureBox();
                baditem.Width = 90;
                baditem.Height = 70;
                baditem.BackColor = System.Drawing.Color.Transparent;
                baditem.BringToFront();



                if (nchoice == 1)
                {
                    baditem.Image = global::Project2.Properties.Resources.nuke;
                }
                else if (nchoice == 2)
                {
                    baditem.Image = global::Project2.Properties.Resources.gun;
                }
                else
                {
                    baditem.Image = global::Project2.Properties.Resources.poison;
                }


                baditem.Top = outtube.Top;
                baditem.Left = outtube.Left - 90;
                this.Controls.Add(baditem);
                bad.Add(baditem);


            }
        }

        public void badDelivery()
        {
            try
            {
                foreach (PictureBox b in bad)
                {
                    b.Left -= lineSpeed;
                    if (b.IsInsideOfForm() == false)
                    {

                        bad.Remove(b);
                        this.Controls.Remove(b);
                        screwups--;
                        //MessageBox.Show("Lose");
                    }
                }
            }
            catch
            {
                //
            }

        }
        public void goodDelivery()
        {
            try
            {
                foreach (PictureBox g in good)
                {
                    g.Left -= lineSpeed;
                    if (g.IsInsideOfForm() == false)
                    {
                        good.Remove(g);
                        this.Controls.Remove(g);
                        score = score + 20;

                    }

                }

            }
            catch
            {
                //
            }

        }

        public void playerKeys()
        {
            if (spacecooldown > 0)
            {
                spacecooldown--;
            }

            if (game.IsPressed(Keys.Y) == true)
            {
                firstitemsent = false;
                score = 0;
                lineSpeed = 10;
                sendSpeed = 25;
                updateSpeed = 25;
                bonusreq = 0;
                hasbonus = true;
                toysstole = 0;
                screwups = 5;
                spacecooldown = 40;
                limbo = false; //order matters or else you will screw up the next game
                guidedone = false;
                startdone = false;
                
                
                
                highscorenreset.Visible = false;
                aaronScore.Visible = false;
                youScore.Visible = false;
                //game.Start();
                
            }
            if (game.IsPressed(Keys.N) == true)
            {
                try
                {
                    System.Environment.Exit(0);
                }
                catch
                {
                    System.Threading.Thread.Sleep(2000);
                    System.Environment.Exit(0);
                }
            }


            if (game.IsPressed(Keys.Up) == true)
            {
                if (hasbonus == true && score > 999)
                {
                    score = score - 1000;
                    hasbonus = false;
                    lineSpeed = 10;
                    bonusreq = 50;
                }
            }
            if (game.IsPressed(Keys.Space) == true)
            {
                if (limbo == true && spacecooldown < 15)
                {
                    s.Stop();
                    if (youWonpic.Visible == true)
                    {
                        youWonpic.Visible = false;
                        highscorenreset.Visible = true;
                        aaronScore.Visible = true;
                        youScore.Visible = true;
                        youScore.Text = Convert.ToString(score);
                        aaronScore.Text = Convert.ToString(score + 1);
                    }
                    else if (neglectLoss.Visible == true)
                    {
                        neglectLoss.Visible = false;
                        highscorenreset.Visible = true;
                        aaronScore.Visible = true;
                        youScore.Visible = true;
                        youScore.Text = Convert.ToString(score);
                        aaronScore.Text = Convert.ToString(score + 1);
                    }
                    else if (stoleLoss.Visible == true)
                    {
                        stoleLoss.Visible = false;
                        highscorenreset.Visible = true;
                        aaronScore.Visible = true;
                        youScore.Visible = true;
                        youScore.Text = Convert.ToString(score);
                        aaronScore.Text = Convert.ToString(score + 1);
                    }
                }
                else
                {
                    if (guidedone == false)
                    {
                        guidedone = true;
                        guidepic.Visible = false;
                        guidecont1.Visible = false;
                        guidecont2.Visible = false;
                        spacecooldown = 40;
                        
                        s.PlayLooping();
                    }
                    else
                    {
                        if (startdone == false && spacecooldown < 15)
                        {
                            startdone = true;
                            start1.Visible = false;
                            start2.Visible = false;
                            spacecooldown = 30;
                        }
                        else
                        {
                            if (spacecooldown == 0)
                            {
                                thespot.Location = new System.Drawing.Point(385, 310);
                            }
                        }
                    }
                }
            }
            else
            {
                thespot.Location = new System.Drawing.Point(385, 458);
            }
        }
        public void productRemoval()
        {
            try
            {
                foreach (PictureBox g in good)
                {
                    if (g.IsTouching(thespot))
                    {
                        good.Remove(g);
                        this.Controls.Remove(g);
                        toysstole++;

                    }

                }

            }
            catch
            {
                //
            }
            try
            {
                foreach (PictureBox b in bad)
                {
                    if (b.IsTouching(thespot))
                    {
                        
                        bad.Remove(b);
                        this.Controls.Remove(b);
                        score = score + 70;

                    }

                }

            }
            catch
            {
                //
            }
        }


    }
}
