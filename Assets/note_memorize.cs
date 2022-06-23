using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;



public class note_memorize : MonoBehaviour {
    
    //TODO
    //Add sharps and flats training


    //2.0
    //Dropdowns for string count
        //Dynamic board and button layout for this

    //Dropdowns for guitar tuning
        //populate the array of buttons with this information

    //Rewrite code to fit this new system

    //Timer

    //Fix UI scaleing


    public Button StartGameButton;

    public Toggle Toggle_D;
    public Toggle Toggle_A;
    public Toggle Toggle_E;
    public Toggle Toggle_B;

    public Button[] FretButtons;

    public Text PanelTitleText;
    public Text HeaderText;
    public Text DescriptionText;
    public Text PromptText;

    public Text ScoreRight;
    public Text ScoreWrong;

    public AudioClip rightClip;
    public AudioClip wrongClip;



    private string[] GuitarStrings = { "B", "E", "A", "D", "G", "B", "E" };

    private bool gameStarted;

    private string[][] shuffleBag;

    private int RandomString;
    private int RandomNote;

    private int CorrectAnswers;
    private int WrongAnswers;

    private int testStringIndex = 0;

    private AudioSource audio;



    void Awake() {
        // Setup Window
        PanelTitleText.text = "Fretboard Trainer";
        HeaderText.text = "Memorize the Notes of the Guitar";
        DescriptionText.text = "Click the note on the corresponding string to see if you gussed right or wrong, learn all natural notes one string at a time before moving on to another string, then mutiple strings, then finally all strings with sharps and flats.";

        ScoreRight.text = CorrectAnswers + " : RIGHT";
        ScoreWrong.text = WrongAnswers + " : WRONG";

        CorrectAnswers = 0;
        WrongAnswers = 0;

        audio = GetComponent<AudioSource>();

        gameStarted = false;
    }
    


    // Start Trainer
    public void StartTrainer() {
        SetupFretboardButtons_();
        GenerateRandomFret_();
    }



    // Clears the text on fretboard buttons, attaches
    void SetupFretboardButtons_() {
        if (!gameStarted) {
            for (int i=0; i < FretButtons.Length; i++) {
                Button TheButton = FretButtons[i];
                
                //Get button Text object
                Text buttonText = TheButton.GetComponentInChildren(typeof(Text)) as Text;

                //Attach event handler
                TheButton.onClick.AddListener(() => CheckFretButtonName(TheButton)); 

                //Set text labels to blank
                buttonText.text = "";
            }
        }
        
    }

    void CheckFretButtonName(Button button) {
        if (gameStarted) {
            
            // Correct Note
            if (button.name == shuffleBag[RandomString][RandomNote]) {

                //Correct String
                if (int.Parse(button.transform.parent.name) == testStringIndex) {
                    
                    StartCoroutine(CorrectGuess_(button));
                    
                // Wrong String
                } else {
                    StartCoroutine(WrongGuess_(button, "string"));
                }

            // Wrong Note
            } else {
                StartCoroutine(WrongGuess_(button, "note"));
            }
        }
    }

    void GenerateRandomFret_() {
        //Count Total Toggles Enabled
        //If a toggle is on incriment toggleCount
        int toggleCount = 0;
        if (Toggle_D.isOn) {toggleCount++;}
        if (Toggle_A.isOn) {toggleCount++;}
        if (Toggle_E.isOn) {toggleCount++;}
        if (Toggle_B.isOn) {toggleCount++;}
        //Debug.Log("toggleCount: " + toggleCount);

        //Handle each string (toggleCount)
        if (toggleCount > 0) {
            shuffleBag = new string[toggleCount][];
            
            int index = 0;
            if (Toggle_D.isOn) {shuffleBag[index] = new string[8] {"D", "E", "F", "G", "A", "B", "C", "D"}; index++;}
            if (Toggle_A.isOn) {shuffleBag[index] = new string[8] {"A", "B", "C", "D", "E", "F", "G", "A"}; index++;}
            if (Toggle_E.isOn) {shuffleBag[index] = new string[8] {"E", "F", "G", "A", "B", "C", "D", "E"}; index++;}
            if (Toggle_B.isOn) {shuffleBag[index] = new string[8] {"B", "C", "D", "E", "F", "G", "A", "B"}; index++;}

            RandomString = Random.Range(0,toggleCount);
            RandomNote = Random.Range(0,8);

            //i hate this fix
            //check shuffleBag[RandomString][0] to see what note it is, assume that will always be the string name, then set testStringIndex to the corresponding index in GuitarString
            //this is sloppy and i hate it, make this real code when you rewrite the program
            if        (shuffleBag[RandomString][0] == "B") {
                testStringIndex = 0;
            } else if (shuffleBag[RandomString][0] == "E") {
                testStringIndex = 1;
            } else if (shuffleBag[RandomString][0] == "A") {
                testStringIndex = 3;
            } else if (shuffleBag[RandomString][0] == "D") {
                testStringIndex = 4;
            }

            PromptText.text = "FIND THE " + shuffleBag[RandomString][RandomNote] + " FRET ON STRING " + GuitarStrings[testStringIndex] + ".";

            StartGameButton.gameObject.SetActive(false);

            gameStarted = true;

        } else {
            PromptText.text = "ERROR: YOU MUST PICK AT LEAST ONE STRING IN THE SELECT STRING(S) TOGGLE GROUP";
            //set color
        }
    }



    IEnumerator CorrectGuess_(Button button) {
        PromptText.text = "CORRECT!";
        CorrectAnswers++;
        ScoreRight.text = CorrectAnswers + " : RIGHT";

        button.GetComponent<Image>().color = Color.green;

        Text buttonText = button.GetComponentInChildren(typeof(Text)) as Text;
        buttonText.text = button.name;
        //shuffleBag[RandomString][RandomNote]

        audio.clip = rightClip;
        audio.Play();

        yield return new WaitForSeconds(2.5f);

        button.GetComponent<Image>().color = Color.white;
        buttonText.text = "";
        
        GenerateRandomFret_();
    }

    IEnumerator WrongGuess_(Button button, string failType) {
       
        if (failType == "string") {
            PromptText.text = "THAT'S THE RIGHT NOTE... BUT THE WRONG STRING!";

        } else if (failType == "note") {
            PromptText.text = "WRONG NOTE!";
        }

        WrongAnswers++;
        ScoreWrong.text = WrongAnswers + " : WRONG";

        button.GetComponent<Image>().color = Color.red;

        Text buttonText = button.GetComponentInChildren(typeof(Text)) as Text;
        buttonText.text = button.name;
        
        audio.clip = wrongClip;
        audio.Play();

        yield return new WaitForSeconds(2.5f);

        button.GetComponent<Image>().color = Color.white;
        buttonText.text = "";

        GenerateRandomFret_();
    }
}
