using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public static float jumpPower = 30;
    public static float maxJump = 10;
    public static float maxJumpMulti = 4;
    public static float fallingMulti = 2;
    public static float lowJumpMulti = 4;
    public Text text;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        updateText();
    }

    public void PlayGame ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void setJumpPower(string input)
    {
        jumpPower = float.Parse(input);
        updateText();
    }

    public void setMaxJump(string input)
    {
        maxJump = float.Parse(input);
        updateText();
    }

    public void setMaxJumpMulti(string input)
    {
        maxJumpMulti = float.Parse(input);
        updateText();
    }

    public void setFallingMulti(string input)
    {
        fallingMulti = float.Parse(input);
        updateText();
    }

    public void setLowJumpMulti(string input)
    {
        lowJumpMulti = float.Parse(input);
        updateText();
    }

    public void updateText()
    {
        text.text = "  jumpPower:" + jumpPower + "  maxJump:" + maxJump + "  maxJumpMulti:" + maxJumpMulti + "  fallingMulti:" + fallingMulti + "  lowJumpMulti:" + lowJumpMulti;
    }
}
