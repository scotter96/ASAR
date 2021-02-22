using UnityEngine;

public class Product: MonoBehaviour
{
    public string code;
    public new string name;
    public string category_code;
    public string price;

    public override string ToString() {
      return $"[{code}] {name}";
    }
}
