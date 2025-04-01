using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardStyleManager : MonoBehaviour
{
    //public MeshRenderer[] meshRenderer;
    public Material Board;
    public Material WhiteChesspiece;
    public Material BlackChesspiece;

    public Texture[] BoardTextures;
    public Texture[] WhiteChessTextures;
    public Texture[] BlackChessTextures;
    void Start()
    {
        ChangeBoardTexture(0);
    }
    public void ChangeBoardTexture(int index)
    {
        Board.mainTexture = BoardTextures[index];
        WhiteChesspiece.mainTexture = WhiteChessTextures[index];
        BlackChesspiece.mainTexture = BlackChessTextures[index];
    }
}
