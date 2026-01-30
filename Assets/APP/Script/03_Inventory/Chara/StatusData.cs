using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// キャラクターの「ステータス」定義
/// </summary>
[System.Serializable]
public struct StatusData
    {
        public int ATK;
        public int DEF;
        public int AGI;
        public int INT;
        public int RES;

        // StatusDataの足し算
        public static StatusData operator +(StatusData a, StatusData b)
        {
            return new StatusData
            {
                ATK = a.ATK + b.ATK,
                DEF = a.DEF + b.DEF,
                AGI = a.AGI + b.AGI,
                INT = a.INT + b.INT,
                RES = a.RES + b.RES
            };
        }

        // StatusDataの引き算
        public static StatusData operator -(StatusData a, StatusData b)
        {
            return new StatusData
            {
                ATK = a.ATK - b.ATK,
                DEF = a.DEF - b.DEF,
                AGI = a.AGI - b.AGI,
                INT = a.INT - b.INT,
                RES = a.RES - b.RES
            };
        }
    }
