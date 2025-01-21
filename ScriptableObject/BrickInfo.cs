using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "BrickInfo", menuName = "Bricks/BrickInfo")]
public class BrickInfo : ScriptableObject
{
    public List<Sprite> SpriteForms = new List<Sprite>();
    public BrickType BrickType;
}
