using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CutsceneManager : MonoBehaviour
{
    public static CutsceneManager instance;

    public Animator dialogueAnim;
    public GameObject cutsceneWindow;
    public TextMeshProUGUI speakerTextbox;
    public TextMeshProUGUI cutsceneTextbox;
    private string currentCutscene;
    public bool inCutscene = false;
    public bool fadeValue = true;
    private static Cutscenes thisCutscene;
    public List<Sprite> cutscenePng = new List<Sprite>();

    public static string[] cutsceneNames = new string[] {

        "Intro",
        "Eden Defeated",
        "Unlikely Visitor",
        "Confrontation",
        "Dorian's Honesty",
        "DEPRECATED",
        "Ulea Defeated",
        "The Lovers",
        "Compendium Stabilized",
        "Izaak's Denial",
        "Ragna Defeated",
        "Acrid Defeated",
        "Dorian Defeated",
        "Lyra Defeated",
        "Izaak and Agnes",
        "Izaak's Revolt",
    };


    #region REGION CUTSCENE TEXTS, SPEAKERS, INDEXES, ANIMATOR NAMES, ANIMATOR VALUES

    private Dictionary<string, int[]> cutsceneValuePairs = new Dictionary<string, int[]>()
    {
        {"One_1", new int[]{ 0 } },
        {"One_2", new int[]{ 1 } },
        {"One_3", new int[]{ 2 } },
        {"One_4", new int[]{ 3 } },

        {"Two_1", new int[]{ 4, 5, 6 } },
        {"Two_2", new int[]{ 7, 8} },
 
        {"Three_1", new int[]{ 9, 10, 11 } },
        {"Three_2", new int[]{ 12, 13, 14, 15, 16, 17 } },

        {"Four_1", new int[]{ 18, 19, 20, 21, 22 } },
        {"Four_2", new int[]{ 23, 24, 25, 26, 27 } },

        {"Five_1", new int[]{ 28, 29, 30 } },

        {"Seven_1", new int[]{ 31 } },

        {"Eight_1", new int[]{ 32, 33, 34, 35, 36, 37, 38, 39, 40 } },
        {"Eight_2", new int[]{ 41 } },

        {"Nine_1", new int[]{ 42, 43, 44, 45 } },
        {"Nine_2", new int[]{ 46, 47 } },

        {"Ten_1", new int[]{ 48 } },
        {"Ten_2", new int[]{ 49 } },

        {"Eleven_1", new int[]{ 50, 51 } },
        {"Eleven_2", new int[]{ 52, 53, 54, 55 } },
        {"Eleven_3", new int[]{ 56, 57 } },

        {"Twelve_1", new int[]{ 58 } },
        {"Twelve_2", new int[]{ 59, 60, 61, 62 } },
        {"Twelve_3", new int[]{ 63, 64, 65} },

        {"Thirteen_1", new int[]{ 66, 67 } },
        {"Thirteen_2", new int[]{ 68, 69, 70, 71 } },
        {"Thirteen_3", new int[]{ 72, 73 } },

        {"Fourteen_1", new int[]{ 74 } },
        {"Fourteen_2", new int[]{ 75, 76, 77 } },
        {"Fourteen_3", new int[]{ 78 } },

        {"Fifteen_1", new int[]{ 79, 80, 81, 82, 83, 84, 85 } },
        {"Fifteen_2", new int[]{ 86, 87, 88 } },

        {"Sixteen_1", new int[]{ 89, 90, 91, 92, 93 } },


    };

    string[] cutsceneText = new string[] {

        //Cutscene one
        //0
        "The kingdom of Astra, which began as a small hub for trade and sea travel, flourished into a beacon for inventions, progress, and ingenuity when their scholars discovered a powerful energy source.",
        //1
        "Ether, is what Astrians called it, became the sole source of fuel throughout the kingdom. Under King Eden's reign, new ways of manipulating and harnessing Ether propelled Astra towards its industrial age.",
        //2
        "The kingdom's progress would not last long. When the queen fell ill, King Eden sought to find a cure. Having investigated all-known magic and exhausted the Ether mines, Eden's efforts proved futile. " +
        "As the years passed, King Eden fell into despair and isolated himself in his tower, never to be seen again. With no ruler and a depleting source of Ether, Astra fell into ruins.",
        //3
        "Then, one night, an unexplainable event, later called the Rain of Stars, reshaped the lives of all Astrians.",
        
        
        //Cutscene two
        //4
        "N…No. No! NO! NO!!",
        //5
        "If I can't have it,  NO ONE CAN!!",
        //6
        "Aghhh!!!",
        //7
        "What did you just do?",
        //8
        "It stopped.  The voices, the visions,  they're gone.",


        //Cutscene three
        //9
        "It's finally over.",
        //10
        "Ulea, I did it. I...",
        //11
        "I hate to ruin your celebration.",
        //12
        "...",
        //13
        "Listen, we need to talk...Is your name Betty, you look like a Betty.",
        //14
        "Get away from me.",
        //15
        "Saints, get it together.",
        //16
        "This is impossible, I watched you die.",
        //17
        "So did I, but you don't see me making a big deal out of it",


        //Cutscene four
        //18
        "So much chaos.",
        //19
        "Great! So what's the plan here.",
        //20
        "We need to contain the overflowing energy.",
        //21
        "Interesting! It's reacting to your presence.",
        //22
        "There's tiny bits of shards stuck in your skin, put your arm forward.",
        //23
        "No matter what you feel, don't stop.",
        //24
        "What? Why?",
        //25
        "It's...too much.",
        //26
        "It's working, don't stop!",
        //27
        "...can't...",




        //Cutscene Five
        //28
        "Just in time, I need you to hear this too.",
        //29
        "Brothers, it's time you heard the truth about the Rain of Stars.",
        //30
        "I realize I have been a coward, and I'm sorry. I have no right to ask you to fight alongside someone you cannot trust.",

        
        //Cutscene Seven
        //31
        "I'm sorry...I'm so sorry!",

        
        //Cutscene Eight
        //32
        "Oh no, not the sad face. It's a horrible look for you.",
        //33
        "You are the best thing to ever happen to me.",
        //34
        "There will be better things to come. Just enjoy the journey!",
        //35
        "You will do great things for Astra. My only regret is that I won't be there to see it.",
        //36
        "Promise me one thing. Promise me you won't do anything stupid.",
        //37
        "Two things actually. Don't let them take my portrait off the wall. I want your next wife to know I came first.",
        //38
        "I can only entertain one of those.",
        //39
        "Haha!",
        //40
        "Get some rest, I'll see you in the morning.",
        
        //41
        "I will find a way.",
       
        
        //Cutscene Nine
        //42
        "This is it, I can feel it.",
        //43
        "And you're sure we'll be able to help Ulea?",
        //44
        "Of course. We can both get what we want.",
        //45
        "Keep going!",

        //46
        "Finally. It's time.",
        //47
        "What are you doing?",
        
        
        //Cutscene Ten
        //48
        "It's gone. There has to be a way to get it back. I just need...",

        //49
        "I knew it, you just wanted the Compendium for yourself.",
       
     
            
        //Cutscene Eleven
        //panel 1 izaak and defated ragna 
        //Panel 2 a flashback panel with left side showing ragna kneeling in front of grave and right side kneeling in front of eden
        //50
        "Agh!",
        //51
        "We don't have to keep doing this.",
        
        //52
        "You don't know what it's like to lose someone who meant everything to you!",
        //53
        "I was drowning in a sea of despair...alone.",        
        //54
        "King Eden offered me a new purpose.",
        //55
        "I became the first Etherborn in the name of Astra!",
        
        //56
        "I'm sorry you feel this way but-",
        //57
        "I don't need your pity! Next time, you'll be the one to fall.",
        
        
        //Cutscene Twelve
        //panel 1 izaak and defated acrid
        //58
        "Your victory here means nothing!",

        //Acrid in his lab and eden lurking in the shadow
        //59
        "A life time of taking orders.",
        //60
        "Working to make others' dreams come true.",
        //61
        "I've worked in the shadows for far too long.",
        //62
        "Astra will know my name!",

        //63
        "Well, let me be the first to thank you for what I am now.",
        //64
        "I didn't predict a level 5 curse to manifest in the first generation.",
        //65
        "I've exceeded my own expectations.",

        //Cutscene Thirteen
        //panel 1 izaak and defated dorian
        //66
        "You've become a fine warrior.",
        //67
        "Not unlike Eden from when we trained together.",
        //Dorian standing by empty throne
        //68
        "I couldn't see how much he was hurting.",
        //69
        "I should've tried harder to stop him from using that darn book.",
        //70
        "He was meant to be the greatest king Astra had ever sceen.",
        //71
        "He's out of my reach now.",
        
        //72
        "May you succeed where I failed, young warrior.",
        //73
        "I intend to.",

        //Cutscene Fourteen
        //panel 1 izaak and defated lyra
        //74
        "Our goals are far beyond your understanding.",
        //Lyra standing next to the throne while old eden is on it with all the others bowing down in front of her.
        //75
        "My rule shall come to pass!",
        //76
        "Astra will be absolved of all its sins.",
        //77
        "As the Saints have ordained.",
        
        //78
        "Such a pity you can't see the truth. You could have been a valuable ally.",

        //Cutscene Fifteen
        //panel 1 backshot of izaak and agnes sitting on a rooftop. The sun is setting.
        //79
        "...And that about sums it up.",
        //80
        "So it's all gone. Acrid's research is all that's left.",
        //81
        "Even that might not be enough.",
        //82
        "Compendium aside, how are you doing?",
        //83
        "Mutations haven't been around long enough for us to know the long-term effects.",
        //84
        "Looks like I'll need your help monitoring it. Are you taking new patients?",
        //85
        "Ha, I can get you in for an appointment right away.",

        //panel 2, fade to black
        //86
        "We should do this more often.",
        //87
        "Yeah?",
        //88
        "Yes.",
        

        //Cutscene Sixteen
        //Panel 1. Izaak standing by the tree in the middle of town. he is surrounded by people. you can see agnes, shopkeeper, arlo and absalom in crowd, the wind is blowing piece of papers through the town. Top half is ulea and lyra and cult shadows
        //89
        "Astra will never be the same. A kingdom once famed for its ruined land, corrupt monarchy, and xenophobic populace has been moved by the wind of change.",
        //90
        "A revolution brews where it is least expected.",
        //91
        "A new order starts to take shape.",
        //92
        "A struggle for power finds renewed energy.",
        //93
        "The fate of the kingdom lies in the hands of victor.",
       
    };
    
    string[] cutsceneSpeaker = new string[] {

        //Cutscene one
        //0
        "",
        //1
        "",
        //2
        "",
        //3
        "",


        //Cutscene two
        //4
        "Eden",
        //5
        "Eden",
        //6
        "Izaak",
        //7
        "Izaak",
        //8
        "Eden",


        //Cutscene three
        //9
        "Izaak",
        //10
        "Izaak",
        //11
        "Eden",
        //12
        "Izaak",
        //13
        "Eden",
        //14
        "Izaak",
        //15
        "Eden",
        //16
        "Izaak",
        //17
        "Eden",


        //Cutscene four
        //18
        "Eden",
        //19
        "Izaak",
        //20
        "Eden",
        //21
        "Eden",
        //22
        "Eden",
        //23
        "Eden",
        //24
        "Izaak",
        //25
        "Izaak",
        //26
        "Eden",
        //27
        "Izaak",

         
        //Cutscene Five
        //28
        "Dorian",
        //29
        "Dorian",
        //30
        "Dorian",

        
        //Cutscene Seven
        //31
        "Izaak",


        //Cutscene Eight
        //32
        "Mirabelle",
        //33
        "Eden",
        //34
        "Mirabelle",
        //35
        "Mirabelle",
        //36
        "Mirabelle",
        //37
        "Mirabelle",
        //38
        "Eden",
        //39
        "Mirabelle",
        //40
        "Eden",
        //41
        "Eden",


        //Cutscene nine
        //42
        "Eden",
        //43
        "Izaak",
        //44
        "Eden",
        //45
        "Eden",
        //46
        "Eden",
        //47
        "Izaak",


        //Cutscene ten
        //48
        "Izaak",
        //49
        "Izaak",
    
        //Cutscene 11
        //50
        "Ragna",
        //51
        "Izaak",
        
        //52
        "Ragna",
        //53
        "Ragna",
        //54
        "Ragna",
        //55
        "Ragna",

        //56
        "Izaak",
        //57
        "Ragna",

    
        
        //Cutscene 12
        //58
        "Acrid",

        //59
        "Acrid",
        //60
        "Acrid",
        //61
        "Acrid",
        //62
        "Acrid",

        //63
        "Izaak",
        //64
        "Acrid",
        //65
        "Acrid",

        //Cutscene 13
        //66
        "Dorian",
        //67
        "Dorian",

        //68
        "Dorian",
        //69
        "Dorian",
        //70
        "Dorian",
        //71
        "Dorian",
        
        //72
        "Dorian",
        //73
        "Izaak",


        //Cutscene 14
        //74
        "Lyra",

        //75
        "Lyra",
        //76
        "Lyra",
        //77
        "Lyra",
        
        //78
        "Lyra",

        //Cutscene Fifteen
        //79
        "Izaak",
        //80
        "Agnes",
        //81
        "Izaak",
        //82
        "Agnes",
        //83
        "Agnes",
        //84
        "Izaak",
        //85
        "Agnes",

        //86
        "Agnes",
        //87
        "Izaak",
        //88
        "Agnes",
        

        //Cutscene Sixteen
        //89
        "",
        //90
        "",
        //91
        "",
        //92
        "",
        //93
        "",


    };

    private Dictionary<Cutscenes, string[]> cutsceneNameValuePairs = new Dictionary<Cutscenes, string[]>()
    {
        {Cutscenes.One, new string[]{ "One_1", "One_2", "One_3", "One_4" } },
        {Cutscenes.Two, new string[]{ "Two_1", "Two_2" } },
        {Cutscenes.Three, new string[]{ "Three_1", "Three_2" } },
        {Cutscenes.Four, new string[]{ "Four_1", "Four_2" } },
        {Cutscenes.Five, new string[]{ "Five_1"} },
        {Cutscenes.Seven, new string[]{ "Seven_1" } },
        {Cutscenes.Eight, new string[]{ "Eight_1", "Eight_2" } },
        {Cutscenes.Nine, new string[]{ "Nine_1", "Nine_2" } },
        {Cutscenes.Ten, new string[]{ "Ten_1", "Ten_2" } },
        {Cutscenes.Eleven, new string[]{ "Eleven_1", "Eleven_2", "Eleven_3" } },
        {Cutscenes.Twelve, new string[]{ "Twelve_1", "Twelve_2", "Twelve_3" } },
        {Cutscenes.Thirteen, new string[]{ "Thirteen_1", "Thirteen_2", "Thirteen_3" } },
        {Cutscenes.Fourteen, new string[]{ "Fourteen_1", "Fourteen_2", "Fourteen_3" } },
        {Cutscenes.Fifteen, new string[]{ "Fifteen_1", "Fifteen_2" } },
        {Cutscenes.Sixteen, new string[]{ "Sixteen_1" } },
    };

    private Dictionary<Cutscenes, bool[]> cutsceneFadeValuePairs = new Dictionary<Cutscenes, bool[]>()
    {
        {Cutscenes.One, new bool[]{ false, false, false, true } },
        {Cutscenes.Two, new bool[]{ false, false } },
        {Cutscenes.Three, new bool[]{ false, false } },
        {Cutscenes.Four, new bool[]{ false, false } },
        {Cutscenes.Five, new bool[]{ false, false } },
        {Cutscenes.Seven, new bool[]{ true, } },
        {Cutscenes.Eight, new bool[]{ false, false, } },
        {Cutscenes.Nine, new bool[]{ false, false, } },
        {Cutscenes.Ten, new bool[]{ true, false } },
        {Cutscenes.Eleven, new bool[]{ false, false, false  } },
        {Cutscenes.Twelve, new bool[]{ false, false, false  } },
        {Cutscenes.Thirteen, new bool[]{ false, false, false  } },
        {Cutscenes.Fourteen, new bool[]{ false, false, false } },
        {Cutscenes.Fifteen, new bool[]{ false, false } },
        {Cutscenes.Sixteen, new bool[]{ false,  } },
    };
    #endregion

    private int panelCount = 0;
    private int panelIndexCount = 0;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        dialogueAnim = GetComponent<Animator>();
        cutsceneWindow.SetActive(inCutscene);

    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public string GetCutsceneNames(int index)
    {
        return cutsceneNames[index];
    }
    // Update is called once per frame

    private IEnumerator coroutine;
    private bool skipping = false;
    void Update()
    {
        if (PauseController.instance.isGamePaused()) return;
        if (inCutscene && Input.GetKeyUp(KeyCode.Mouse0) && !skipping)
        {
            StopCoroutine(coroutine);
            coroutine = ClickedNext(fadeValue);
            StartCoroutine(coroutine);
        }

    }

    private IEnumerator ClickedNext(bool fadeVal)
    {
        AudioManager.instance.PlaySfxSound("Button_Click");
        if (fadeVal)
        {
            skipping = true;
            dialogueAnim.Play("Fade", 1, 0);
            yield return new WaitForSeconds(0.75f);
        }
        
        yield return new WaitForSeconds(0.25f);
        skipping = true;
        if (panelCount >= cutsceneNameValuePairs[thisCutscene].Length)
        {
            EndCutscene();
        }
        else
        {
            PlayNextPanel();
        }
        skipping = false;
    }

    public void EndCutscene()
    {
        if (inCutscene)
        {
            StopTrackForCutscene();
            inCutscene = false;
            currentCutscene = null;
            cutsceneWindow.SetActive(inCutscene);
            panelCount = 0;
            dialogueAnim.Play("End");
            StopCoroutine(coroutine);

        }
    }

    private void PlayNextPanel()
    {
        currentCutscene = cutsceneNameValuePairs[thisCutscene][panelCount];
        fadeValue = cutsceneFadeValuePairs[thisCutscene][panelCount];
        int textIndex = cutsceneValuePairs[currentCutscene].Length > 0 ? cutsceneValuePairs[currentCutscene][panelIndexCount] : -1;
        speakerTextbox.transform.parent.gameObject.SetActive(textIndex >= 0);
        string speakerName = textIndex >= 0 ? cutsceneSpeaker[textIndex] : "";
        string speakerText = textIndex >= 0 ? cutsceneText[textIndex] : "";

        speakerTextbox.text = speakerName;
        cutsceneTextbox.text = speakerText;

        dialogueAnim.Play(currentCutscene);
        panelIndexCount++;
        if (panelIndexCount >= cutsceneValuePairs[currentCutscene].Length)
        { 
            panelCount++;
            panelIndexCount = 0;
        }
        
    }

    public void Play(Cutscenes cutsceneName)
    {
        thisCutscene = cutsceneName;
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().cutscenesUnlocked.Add(thisCutscene.ToString());
        inCutscene = true;
        cutsceneWindow.SetActive(inCutscene);
        StartTrackForCutscene();
        coroutine = ClickedNext(true);
        PlayNextPanel(); 
    }

    //Cutscene music
    private void StartTrackForCutscene()
    {
        switch(thisCutscene)
        {
            case Cutscenes.One:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_1" });
                break;
            case Cutscenes.Two:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_2" });
                break;
            case Cutscenes.Three:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_3" });
                break;
            case Cutscenes.Four:
            case Cutscenes.Nine:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_4" });
                break;
            case Cutscenes.Seven:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_7" }); 
                break;
            case Cutscenes.Eight:
            case Cutscenes.Fifteen:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_8" }); //Eden and mirabelle - Izaak and agnes
                break;
            case Cutscenes.Ten:
            case Cutscenes.Sixteen:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Cutscene_10" });
                break;
            case Cutscenes.Eleven:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Ragna_story", }); //Izaak and ragna
                break;
            case Cutscenes.Twelve:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Acrid_story", }); //Izaak and acrid
                break;
            case Cutscenes.Five:
            case Cutscenes.Thirteen:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Dorian_story" }); //Izaak and dorian
                break;
            case Cutscenes.Fourteen:
                AudioManager.instance.PlayFloorMusic(new List<string>() { "Lyra_story", }); //Izaak and lyra
                break;
        }
    }

    private void StopTrackForCutscene()
    {
        if (!GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerManager>().inBattle)
        {
            AudioManager.instance.PlaySceneMusic("City_Theme");
            return;
        }
        switch(thisCutscene)
        {
            case Cutscenes.One:
            case Cutscenes.Two:
            case Cutscenes.Three:
            case Cutscenes.Four:
            case Cutscenes.Five:
            case Cutscenes.Seven:
            case Cutscenes.Eight:
            case Cutscenes.Ten:
            case Cutscenes.Eleven:
            case Cutscenes.Twelve:
            case Cutscenes.Thirteen:
            case Cutscenes.Fourteen:
            case Cutscenes.Fifteen:
            case Cutscenes.Sixteen:
                GameObject.Find("Floor Manager").GetComponent<FloorManager>().PlayFloorMusic();
                break;
            case Cutscenes.Nine:
                break;
        }
    }


}
