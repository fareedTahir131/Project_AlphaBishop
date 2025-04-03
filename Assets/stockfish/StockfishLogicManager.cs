using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class StockfishLogicManager : MonoBehaviour
{
    private Process stockfishProcess;
    private string stockfishPath;
    private Queue<string> stockfishOutputQueue = new Queue<string>();

    public bool isWhiteTurn = true; // Track turn
    public string currentFEN = "startpos"; // FEN position

    public enum DifficultyLevel { Easy, Medium, Hard }
    public DifficultyLevel currentDifficulty = DifficultyLevel.Medium;

    private void Start()
    {
        stockfishPath = Path.Combine(Application.streamingAssetsPath, "stockfish"); // Mac executable
        StartStockfish();
        SetDifficulty(currentDifficulty);
    }

    private void StartStockfish()
    {
        stockfishProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = stockfishPath,
                RedirectStandardInput = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        stockfishProcess.OutputDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                stockfishOutputQueue.Enqueue(args.Data);
        };
        stockfishProcess.ErrorDataReceived += (sender, args) =>
        {
            if (!string.IsNullOrEmpty(args.Data))
                UnityEngine.Debug.LogError("Stockfish Error: " + args.Data);
        };

        stockfishProcess.Start();
        stockfishProcess.BeginOutputReadLine();
        stockfishProcess.BeginErrorReadLine();
    }

    private void SendCommand(string command)
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishProcess.StandardInput.WriteLine(command);
            stockfishProcess.StandardInput.Flush();
            UnityEngine.Debug.Log("Sent to Stockfish: " + command);
        }
    }

    public void SetDifficulty(DifficultyLevel level)
    {
        currentDifficulty = level;
        int skillLevel = level switch
        {
            DifficultyLevel.Easy => 1,
            DifficultyLevel.Medium => 10,
            DifficultyLevel.Hard => 20,
            _ => 10
        };
        SetSkillLevel(skillLevel);
    }

    private void SetSkillLevel(int level)
    {
        SendCommand("setoption name Skill Level value " + level);
    }

    public void SetFEN(string fen)
    {
        currentFEN = fen;
        SendCommand("position fen " + fen);
    }

    public void MakeMove(string move)
    {
        currentFEN += " " + move;
        SendCommand("position fen " + currentFEN);
        isWhiteTurn = !isWhiteTurn;
    }

    public IEnumerator GetBestMove(Action<string> callback)
    {
        SendCommand("go depth 15");
        yield return new WaitForSeconds(1);

        while (stockfishOutputQueue.Count > 0)
        {
            string response = stockfishOutputQueue.Dequeue();
            UnityEngine.Debug.Log("Stockfish Response: " + response);
            if (response.StartsWith("bestmove"))
            {
                string[] parts = response.Split(' ');
                if (parts.Length >= 2)
                {
                    string bestMove = parts[1];
                    callback?.Invoke(bestMove);
                    yield break;
                }
            }
        }
    }

    private void OnApplicationQuit()
    {
        if (stockfishProcess != null && !stockfishProcess.HasExited)
        {
            stockfishProcess.Kill();
        }
    }
}
