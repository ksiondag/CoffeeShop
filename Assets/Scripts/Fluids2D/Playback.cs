﻿
using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

public class Playback : UdonSharpBehaviour
{
    private float[] history;
    private Pourer pourer;
    private Fluids2D fluids2d;
    private int row = 0;
    private float triggerInputTime;
    private float currentTime;


    private int DT = 0;
    private int COORD_X = 1;
    private int COORD_Y = 2;
    private int PREV_COORD_X = 3;
    private int PREV_COORD_Y = 4;
    private int DELTA_X = 5;
    private int DELTA_Y = 6;
    private int ROW_SIZE = 7;

    void Start()
    {
        pourer = GetComponent<Pourer>();
        fluids2d = GetComponent<Fluids2D>();

        // dt, coordX, coordY, prevCoordX, prevCoordY, deltaX, deltaY
        // all in one-dimensional array because VRChat doesn't support 2D arrays
        history = new float[] {
0f, 0.5045045045045045f, 0.9829443447037702f, 0.5045045045045045f, 0.9883303411131059f, 0f, -0.0053859964093356805f,
0.016f, 0.5045045045045045f, 0.973967684021544f, 0.5045045045045045f, 0.9829443447037702f, 0f, -0.008976660682226245f,
0.016f, 0.5045045045045045f, 0.9578096947935368f, 0.5045045045045045f, 0.973967684021544f, 0f, -0.016157989228007152f,
0.017f, 0.5054054054054054f, 0.9371633752244165f, 0.5045045045045045f, 0.9578096947935368f, 0.000897666068222612f, -0.02064631956912033f,
0.017f, 0.5054054054054054f, 0.9030520646319569f, 0.5054054054054054f, 0.9371633752244165f, 0f, -0.03411131059245964f,
0.016f, 0.5045045045045045f, 0.8626570915619389f, 0.5054054054054054f, 0.9030520646319569f, -0.000897666068222612f, -0.04039497307001794f,
0.017f, 0.5036036036036036f, 0.8150807899461401f, 0.5045045045045045f, 0.8626570915619389f, -0.000897666068222612f, -0.047576301615798844f,
0.017f, 0.5027027027027027f, 0.7594254937163375f, 0.5036036036036036f, 0.8150807899461401f, -0.000897666068222612f, -0.05565529622980259f,
0.016f, 0.5f, 0.699281867145422f, 0.5027027027027027f, 0.7594254937163375f, -0.0026929982046678363f, -0.06014362657091554f,
0.017f, 0.4990990990990991f, 0.6481149012567324f, 0.5f, 0.699281867145422f, -0.000897666068222612f, -0.05116696588868952f,
0.016f, 0.4981981981981982f, 0.5969479353680431f, 0.4990990990990991f, 0.6481149012567324f, -0.000897666068222612f, -0.0511669658886893f,
0.017f, 0.5f, 0.5529622980251346f, 0.4981981981981982f, 0.5969479353680431f, 0.001795332136445224f, -0.0439856373429085f,
0.017f, 0.5027027027027027f, 0.518850987432675f, 0.5f, 0.5529622980251346f, 0.0026929982046678363f, -0.03411131059245964f,
0.016f, 0.5054054054054054f, 0.4982046678635548f, 0.5027027027027027f, 0.518850987432675f, 0.0026929982046678363f, -0.02064631956912022f,
0.017f, 0.5081081081081081f, 0.4883303411131059f, 0.5054054054054054f, 0.4982046678635548f, 0.002692998204667947f, -0.009874326750448859f,
0.016f, 0.5108108108108108f, 0.48473967684021546f, 0.5081081081081081f, 0.4883303411131059f, 0.0026929982046678363f, -0.0035906642728904536f,
0.018f, 0.5135135135135135f, 0.4820466786355476f, 0.5108108108108108f, 0.48473967684021546f, 0.0026929982046678363f, -0.0026929982046678402f,
0.016f, 0.5153153153153153f, 0.4802513464991023f, 0.5135135135135135f, 0.4820466786355476f, 0.001795332136445224f, -0.0017953321364453378f,
0.017f, 0.5153153153153153f, 0.47935368043087967f, 0.5153153153153153f, 0.4802513464991023f, 0f, -0.0008976660682226134f,
0.016f, 0.5162162162162162f, 0.47845601436265706f, 0.5153153153153153f, 0.47935368043087967f, 0.000897666068222612f, -0.0008976660682226134f,
0.018f, 0.5162162162162162f, 0.47755834829443444f, 0.5162162162162162f, 0.47845601436265706f, 0f, -0.0008976660682226134f,
0.016f, 0.5171171171171172f, 0.47666068222621183f, 0.5162162162162162f, 0.47755834829443444f, 0.0008976660682227227f, -0.0008976660682226134f,
0.017f, 0.5171171171171172f, 0.4757630161579892f, 0.5171171171171172f, 0.47666068222621183f, 0f, -0.0008976660682226134f,
0.016f, 0.5180180180180181f, 0.4748653500897666f, 0.5171171171171172f, 0.4757630161579892f, 0.000897666068222612f, -0.0008976660682226134f,
0.033f, 0.5180180180180181f, 0.473967684021544f, 0.5180180180180181f, 0.4748653500897666f, 0f, -0.0008976660682226134f
};

        row = 0;
        currentTime = 0f;
        triggerInputTime = history[row * ROW_SIZE + DT];
    }

    void Update () {
        float dt = Time.deltaTime;
        currentTime += dt;

        if (currentTime > triggerInputTime) {
            pourer.texcoord = new Vector2(history[row * ROW_SIZE + COORD_X], history[row * ROW_SIZE + COORD_Y]);
            pourer.prevTexcoord = new Vector2(history[row * ROW_SIZE + PREV_COORD_X], history[row * ROW_SIZE + PREV_COORD_Y]);
            pourer.delta = new Vector2(history[row * ROW_SIZE + DELTA_X], history[row * ROW_SIZE + DELTA_Y]);
            fluids2d.SplatPourer();

            row = (row + 1) % (history.Length / ROW_SIZE);
            float nextTriggerInputTime = history[row * ROW_SIZE + DT];
            triggerInputTime += nextTriggerInputTime;
        }
    }
}
