using System.Collections;
using System.Collections.Generic;
using Coord = RoomGenerator.Coord;
using UnityEngine;
using UnityEngine.UI;

public static class Utilities
{
    public static T[] ShuffleCoords<T>(T[] allTilesCoord)
    {
        for (int i=0;i<allTilesCoord.Length;i++)
        {
            int index =UnityEngine.Random.Range(i,allTilesCoord.Length);
            T temp = allTilesCoord[index];
            allTilesCoord[index] = allTilesCoord[i];
            allTilesCoord[i] = temp;
        }
        return allTilesCoord;
    }

    public static Vector2 GetRandomPointInCircle(float radius)
    {
        Vector2 value = new Vector2(radius,radius);

        float t = 2 * Mathf.PI * UnityEngine.Random.Range(0, 1f);
        float u = UnityEngine.Random.Range(0, 1f) + UnityEngine.Random.Range(0, 1f);

        float r;

        if (u > 1)
            r = 2 - u;
        else
            r = u;

        value.x += radius * r * Mathf.Cos(t);
        value.y += radius * r * Mathf.Sin(t);

        return value;
    }

    public static void UIFollowCharacter(RectTransform _ui,Vector3 _followPos,Vector2 _offsetPos)
    {
        if (_ui == null)
        {
            return;
        }
        _ui.position = _offsetPos + RectTransformUtility.WorldToScreenPoint(Camera.main,_followPos);
    }

    public static Vector3 GetBezierPoint(float _percent,Vector3 _startPos,Vector3 _endPos,float _height)
    {
        Vector3 center = (_startPos + _endPos) * 0.5f + Vector3.up * _height;
        return (1 - _percent) * (1 - _percent) * _startPos + 2 * _percent * (1 - _percent) * center + _percent * _percent * _endPos;
    }

}