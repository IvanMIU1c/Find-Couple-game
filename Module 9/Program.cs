using System;
using SFML.Learning;
using SFML.System;
using SFML.Window;
using SFML.Graphics;
using SFML.Audio;

class FindCouple : Game
{
    static string[] iconsName;
    static int[,] cards;
    static int cardCount = 6;
    static int Score = 0;
    static int cardWidth = 100;
    static int cardHeight = 100;

    static int countPerLine = 6;
    static int space = 40;
    static int leftOffset = 40;
    static int topOffset = 40;

    static string BgMusic = LoadMusic("background_music.wav");
    static string CardSound = LoadSound("boop.wav");

    static int tmr = 120;
    static int YourTime = 0;
    static int num = 0;
    
    static void Count(object obj)
    {
        tmr--;
        YourTime++;
    }

    static void LoadIcons()
    {
        iconsName = new string[14];

        iconsName[0] = LoadTexture("icon_close.png");

        for (int i =1; i < iconsName.Length; i++)
        {
            iconsName[i] = LoadTexture("icon_" +i+ ".png");
        }
    }

    static void Shuffle(int[] arr)
    {
        Random rand = new Random();

        for(int i = arr.Length - 1; i >= 1; i--)
        {
            int j = rand.Next(1, i + 1);
            int tmp = arr[j];
            arr[j] = arr[i];
            arr[i] = tmp;
        }
    }

    static void Initcard()
    {
        Random rnd = new Random();
        cards = new int[cardCount, 6];

        int[] iconId = new int[cards.GetLength(0)];
        int id = 0;

        for (int i = 0; i < iconId.GetLength(0); i++)
        {
            if (i % 2 == 0)
            {
                id = rnd.Next(1, 13);
            }

            iconId[i] = id;
        }

        Shuffle(iconId);

        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = 0; //state 
            cards[i, 1] = (i%countPerLine)*(cardWidth+space) + leftOffset; //posX 
            cards[i, 2] = (i / countPerLine) * (cardHeight + space) + topOffset; ; //posY 
            cards[i, 3] = cardWidth; //width 
            cards[i, 4] = cardHeight; //height 
            cards[i, 5] = iconId[i]; //id
        }
    }

    static void SetStateToAllCards(int state)
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            cards[i, 0] = state;
        }
    }

    static void DrawCard()
    {
        for (int i = 0; i < cards.GetLength(0); i++)
        {
            if (cards[i, 0] == 1)
            {
                DrawSprite(iconsName[cards[i, 5]], cards[i, 1], cards[i, 2]);
            }

            if (cards[i, 0] == 0)
            {
                DrawSprite(iconsName[0], cards[i, 1], cards[i, 2]);
            }
        }
    }

    static int GetIndexByMousePosition()
    {
        for(int i =0;i<cards.GetLength(0); i++)
        {
            if (MouseX >= cards[i, 1] && MouseX <= cards[i, 1] + cards[i, 3] && MouseY >= cards[i, 2] && MouseY <= cards[i, 2] + cards[i, 4])
            {
                return i;
            }
        }

        return -1;
    }

    static void Main(string[] args)
    {

        int openCardAmount = 0;
        int firstOpenCardIndex = -1;
        int secondOpenCardIndex = -1;
        int remainingCards = cardCount;
        int winCOndition1 = 12;
        int winCOndition2 = 18;
        int winCOndition3 = 36;
        LoadIcons();
        SetFont("ARIALUNI.TTF");
        InitWindow(900, 900, "Find Couple");

        ClearWindow(30, 25, 70);
        DrawText(250, 100, "Выберите уровень сложности:", 32);
        DrawText(400, 140, "Легкий - [Q]",22);
        DrawText(400, 170, "Средний - [W]",22);
        DrawText(400, 200, "Сложный - [E]",22);
        DrawText(400, 230, "Невероятный - [R]",22);
        DisplayWindow();
        //Выбор сложности
        while (true) { 
             if (GetKey(Keyboard.Key.Q) == true) { 
                 cardCount = winCOndition1;
                remainingCards = winCOndition1;
                 break;
             }           
             if (GetKey(Keyboard.Key.W) == true) { 
                 cardCount = winCOndition2;
                remainingCards = winCOndition2;
                 break;
             }            
             if (GetKey(Keyboard.Key.E) == true) { 
                 cardCount = winCOndition3;
                remainingCards = winCOndition3;
                 break;
             }
            if (GetKey(Keyboard.Key.R) == true)
            {
                cardCount = winCOndition2;
                remainingCards = winCOndition2;
                tmr = 30;
                break;
            }
        }


        Initcard();
        SetStateToAllCards(1);

        ClearWindow(30, 25, 70);
        DrawCard();

        DisplayWindow();
        Delay(5000);

        SetStateToAllCards(0);

        for (int i=0;i<cards.GetLength(0); i++)
        {
            cards[i, 0] = 0;    
        }

        System.Threading.TimerCallback tm = new System.Threading.TimerCallback(Count);
        System.Threading.Timer timer = new System.Threading.Timer(tm, num, 0, 1000);

        while (true)
        {
            PlayMusic(BgMusic, 2);
            DispatchEvents();
            if (remainingCards == 0) break;
            if (tmr == 0) break;
            

            if (openCardAmount == 2)
            {
                if(cards[firstOpenCardIndex,5]== cards[secondOpenCardIndex, 5])
                {
                    cards[firstOpenCardIndex, 0] = -1;
                    cards[secondOpenCardIndex, 0] = -1;
                    Score++;
                    remainingCards -=2; 
                }
                else
                {
                    cards[firstOpenCardIndex, 0] = 0;
                    cards[secondOpenCardIndex, 0] = 0;
                }

                firstOpenCardIndex = -1;
                secondOpenCardIndex = -1;
                openCardAmount = 0;
                Delay(1000);
            }

            if (GetMouseButtonDown(0) == true)
            {
                int index = GetIndexByMousePosition();

                if(index != -1 && index != firstOpenCardIndex)
                {
                    cards[index, 0] = 1;
                    openCardAmount++;

                    if (openCardAmount == 1) firstOpenCardIndex = index;
                    if (openCardAmount == 2) secondOpenCardIndex = index;

                    PlaySound(CardSound,30);
                }
            }

            ClearWindow(30, 25, 70);
            DrawText(0, 0, $"Оставшееся время: {tmr} cекунд.", 24);
            DrawCard();

            DisplayWindow();

            Delay(1);
        }

        ClearWindow(30, 25, 70);

        SetFillColor(Color.White);

        if (Score == cardCount/2)
        {
            DrawText(300, 100, "Поздравляем!", 32);
            DrawText(300, 130, $"Вы выполнили задание за {YourTime} секунд.", 32);
        }

        else
        {
            DrawText(400, 100, "Время вышло!", 32);
        }
        DisplayWindow();

        Delay(5000);
    }
}