using System.Collections;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FieldOfView))]
public class PlayerGUI : Editor
{
    private void OnSceneGUI()
    {
        FieldOfView fow = (FieldOfView)target;
        Handles.color = Color.white;
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        Vector3 viewNegativeAngle = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewPositiveAngle = fow.DirFromAngle(fow.viewAngle / 2, false);

        Handles.DrawLine(fow.transform.position, fow.transform.position + viewNegativeAngle * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewPositiveAngle * fow.viewRadius);


    }
}
