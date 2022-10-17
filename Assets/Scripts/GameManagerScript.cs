using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerScript : MonoBehaviour
{
    public BattleHUDScript allyHUD;
    public BattleHUDScript enemyHUD;
    public Image playerActions;
    public Image combatReadout;
    public Canvas MainMenu;
    public Canvas TeamSelectCanvas;
    public TeamSelectionScript teamSelectionScript;
    public Canvas monsterSelectCanvas;
    public Canvas battleCanvas;
    public BattleSystem battleSystem;
    public Canvas nameEntryCanvas;
    public AudioManager audioManager;
    public BGMManager BGMManager;
    public FadeToBlackManager fade;
    //public MainMenuScript mainMenuScript;
    public string gamePhaseCurrent;

    // Start is called before the first frame update
    // Game logic begins here
    void Start()
    {
        //Debug.Log("GameManagerScript's Start() ran.");
        gamePhaseChangeTo("mainMenu");
    }

    /// <summary>
    /// Validates target game phase and passes flow to creating that game phase.
    /// TODO: add param validation
    /// </summary>
    /// <param name="gamePhaseTarget"></param>
    public void gamePhaseChangeTo(string gamePhaseTarget)
    {
        Debug.Log("Attempting to change game phase to   \"" + gamePhaseTarget + "\"");
        switch (gamePhaseTarget)
        {
            case "mainMenu":
                this.gamePhaseCurrent = gamePhaseTarget;
                gamePhaseMainMenu();
                break;
            case "combatTrainer":
                this.gamePhaseCurrent = gamePhaseTarget;
                gamePhaseCombatTrainer();
                break;
            case "combatElite":
                this.gamePhaseCurrent = gamePhaseTarget;
                gamePhaseCombatElite();
                break;
            case "combatWild":
                this.gamePhaseCurrent = gamePhaseTarget;
                gamePhaseCombatWild();
                break;
        }
    }

    void gamePhaseCombatTrainer()
    {
        battleSystem.beginGame();
    }

    void gamePhaseCombatElite() { }
    void gamePhaseCombatWild() { }

    /// <summary>
    /// 
    /// </summary>
    void gamePhaseMainMenu()
    {
        reportFiredMethod(MethodBase.GetCurrentMethod());

        //fade.FadeBlackOutSquare(false);
        //StartCoroutine(fade.FadeBlackOutSquare(false));
        BGMManager.playMenuBGM();
        monsterSelectCanvas.gameObject.SetActive(false);
        battleCanvas.gameObject.SetActive(false);
        battleSystem.gameOverHUD.gameObject.SetActive(false);
        teamSelectionScript.ResetTeam();
        teamSelectionScript.UpdateTeamPreviews();

        if (battleSystem.allyGameObject)
        {
            Destroy(battleSystem.allyGameObject);
        }

        if (battleSystem.enemyGameObject)
        {
            Destroy(battleSystem.enemyGameObject);
        }

        //battleCanvas.gameOverHUD.gameObject.SetActive(false);
        MainMenu.gameObject.SetActive(true);
        //gameObject.SetActive(true);
    }
    public void reportFiredMethod(MethodBase methodBase)
    {
        Debug.Log("Method called: " + methodBase.Name + "()");
        //Debug.Log("Params: " + MethodBase.GetCurrentMethod().GetParameters());
    }

}
