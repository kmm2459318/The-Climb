using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Boss_20_StatusObjectScript", menuName = "Scriptable Objects/Boss_20_StatusObjectScript")]
public class Boss_20_StatusObjectScript : ScriptableObject
{
    public string NAME;    //“G‚Ì–¼‘O
    public int HP;@@@   //“G‚ÌHP
    public int Speed;      //“G‚Ì‘¬‚³
    public int Blow_away;  //‚Á”ò‚Î‚µ—Í
    public int Lateral;    //‰¡ˆÚ“®
    public int Vertical;   //cˆÚ“®
}
