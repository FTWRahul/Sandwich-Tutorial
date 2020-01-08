using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIngredient", menuName = "Ingredients", order = 1)]
public class IngredientSO : ScriptableObject
{
   public string ingredientName;
   public Color tempColor;
   public DG.Tweening.Ease ease;
   public GameObject model;
}
