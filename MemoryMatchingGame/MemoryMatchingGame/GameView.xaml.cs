using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Gaming.Input;
using Windows.UI.Xaml.Media.Imaging;
using System.Diagnostics;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        Random rnd = new Random();
        while (n > 1)
        {
            int k = (rnd.Next(0, n) % n);
            n--;
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

namespace MemoryMatchingGame
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class GameView : Page
    {
        private DispatcherTimer timer;
        private Gamepad controller;
        //private Random randomCardGenerator;
        private List<CardButton> buttonList;
        //private Image[] images = new Image[53];
        int numOfRowCol = 4;
        int TimeLeftOnTimerInSeconds = 180;
        //int horizontal = 30;
        //int vertical = 30;

        private List<CardButton> SelectedCards = new List<CardButton>();


        private List<int> CardValues(int maxCards)
        {
            // Create an array of int numbers with duplicate values for matching cards 
            List<int> nums = new List<int>();
            for (int i = 0; i < maxCards; i++)
            {
                nums.Add(i + 1);
            }
            for (int i = 0; i < maxCards; i++)
            {
                nums.Add(i + 1);
            }
            return nums;
        }

        public GameView()
        {
            Windows.UI.ViewManagement.ApplicationView.GetForCurrentView().SetDesiredBoundsMode(Windows.UI.ViewManagement.ApplicationViewBoundsMode.UseCoreWindow);

            this.InitializeComponent();
            
            buttonList = new List<CardButton>();

            timer = new DispatcherTimer();
            timer.Tick += DispatcherTimer_Tick;
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();

            var cardValues = CardValues((numOfRowCol * numOfRowCol) / 2);
            cardValues.Shuffle();

            generateGameSheet(cardValues);
        }

        private void generateGameSheet(List<int> cardValues) //Creates the game sheet
        {
            int x = 300;
            int y = 300;
            buttonList.Clear(); // clear out

            for (int index = 0; index < numOfRowCol; index++)
            {
                for (int index2 = 0; index2 < numOfRowCol; index2++)
                {
                    var cardValue = cardValues[(index * numOfRowCol) + index2];

                    var button = new CardButton(cardValue);
                    button.Name = $"Card{index}{index2}";
                    // rectangle.Fill = GetRandomColor();
                    // rectangle.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    button.Margin = new Thickness(x, y, 0, 0);
                    button.Height = 100;
                    button.Width = 100;
                    button.HorizontalAlignment = HorizontalAlignment.Left;
                    button.VerticalAlignment = VerticalAlignment.Top;
                    buttonList.Add(button);
                    GameGrid.Children.Add(button);
                    Grid.SetRow(button, numOfRowCol);
                    x = x + 200;

                    // Assign events
                    button.Click += (s, e) => { CardButtonWasClicked((CardButton)s); };
                }
                x = 300;
                y = y + 150;
            }
        }

        private string TimeStringForSeconds(int seconds) // Converts time to MM:SS format for timer
        {
            int minutesLeft = seconds / 60;
            int secondsLeft = seconds % 60;
            return $"{minutesLeft}:{secondsLeft}";
        }

        private void DispatcherTimer_Tick(object sender, object e) // Tells program what to do every second
        {
            //Decrement timer seconds left
            TimeLeftOnTimerInSeconds--;
            TimeText.Text = TimeStringForSeconds(TimeLeftOnTimerInSeconds);
           
            if(TimeLeftOnTimerInSeconds <= 0)
            {
                timer.Stop();
                this.Frame.Navigate(typeof(LoseScreen), 1);
            }
        }

        void CardButtonWasClicked(CardButton sender)
        {
            Debug.WriteLine($"Clicked on: {sender.Value}");
            Debug.WriteLine($"Items selected: {SelectedCards.Count}");

            // Guard checks
            if (SelectedCards.Contains(sender))
            {
                // Selected already selected item -- should deselect it
                SelectedCards.Remove(sender);
                sender.FlipOver();
                return;
            }
            if (sender.IsAlreadyMatched || SelectedCards.Count >= 2) // Only allow 2 cards picked at once
            {
                return;
            }

            sender.FlipOver();
            SelectedCards.Add(sender);

            // Check if a match or not
            if (SelectedCards.Count > 1)
            {
                if (SelectedCards[0].Value == SelectedCards[1].Value)
                {
                    Debug.WriteLine($"Match with value: {sender.Value}");
                    // A match! -- mark as done in main array
                    foreach (var match in buttonList.Where(e => e.Value == SelectedCards[0].Value))
                    {
                        match.IsAlreadyMatched = true;
                    }

                    SelectedCards.Clear(); // free up
                }
            }
  

            // Check if it was a game winning move:
            if (AllCardsAreMatched())
            {
                GoToNextOrWinningScreen();
            }
        }

        private bool AllCardsAreMatched()
        {
            foreach (var cardButton in buttonList)
            {
                if (!cardButton.IsAlreadyMatched)
                {
                    return false;
                }
            }
            return true;
        } 


        private void ControllerLogic()
        {
            controller = Gamepad.Gamepads.First();
            var reading = controller.GetCurrentReading();

            if(reading.LeftThumbstickX < -.1)//Moves selected card left
            {

            }
            if(reading.LeftThumbstickX > 0.1)//Moves selected card right
            {

            }
            if(reading.LeftThumbstickY < -.1)//Moves selected card down
            {

            }
            if (reading.LeftThumbstickY > 0.1)//Moves selected card up
            {

            }
            if(reading.Buttons.HasFlag(GamepadButtons.A))//Flips the card over
            {
                //CardButtonWasClicked(reading.Buttons[0]);
            }
        }

        private void GoToNextOrWinningScreen()
        {
            //TODO: 
            TimeLeftOnTimerInSeconds = TimeLeftOnTimerInSeconds + 120;
            if (numOfRowCol > 8)
            {
                this.Frame.Navigate(typeof(WinScreen), 1);
            }
            else
            {
                numOfRowCol = numOfRowCol + 2;

                var cardValues = CardValues((numOfRowCol * numOfRowCol) / 2);
                cardValues.Shuffle();

                generateGameSheet(cardValues);
            }
        }

        /*private void allCardsMatched()
        {
            TimeLeftOnTimerInSeconds = TimeLeftOnTimerInSeconds + 120;
            if(numOfRowCol > 10)
            {
                this.Frame.Navigate(typeof(WinScreen), 1);
            }
            else
            {
                numOfRowCol = numOfRowCol + 2;

                var cardValues = CardValues(numOfRowCol);
                cardValues.Shuffle();

                generateGameSheet(cardValues);
            }
        }*/

        private void TimeText_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}









/* ALTERNATE WAY TO MAKE DYNAMIC ARRAY
        Button[] buttonArray = new Button[sizeOfArray];

        for(int index = 0; index <buttonArray.Length; index++)
        {
            buttonArray[index] = new Button();
            buttonArray[index].Size = new Size(30, 60);
            buttonArray[index].Location = new Point(horizontal, vertical);
            if ((index + 1) % 9 == 0)
            {
                horizontal += 80;
            }
            else
            {
                vertical += 30;
            }
            this.Controls.Add(buttonArray[index]);
        }
        for (int index2 = 0; index2 < buttonArray.Length; index2++)
        {
            buttonArray[index2] = new Button();
            buttonArray[index2].Size = new Size(30, 60);
            buttonArray[index2].Location = new Point(horizontal, vertical);
            if ((index2 + 1) % 9 == 0)
            {
                horizontal += 80;
            }
            else
            {
                vertical += 30;
            }
            this.Controls.Add(buttonArray[index2]);
        }
        */
