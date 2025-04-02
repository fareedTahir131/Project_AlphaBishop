using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockFishDifficultySelection : MonoBehaviour
{
    public static StockFishDifficultySelection StockFishInstence;
    public GameUIScript gameUIScript;

    private void Awake()
    {
        if (StockFishInstence)
        {
            StockFishInstence = this;
        }
    }
    void Start()
    {
        
    }

    public void OnEasyDifficultyLevelSelection()
    {
        gameUIScript.StockfishDifficulty_level = "easy";
    }
    public void OnMediumDifficultyLevelSelection()
    {
        gameUIScript.StockfishDifficulty_level = "medium";
    }
    public void OnHardDifficultyLevelSelection()
    {
        gameUIScript.StockfishDifficulty_level = "hard";
    }
}
