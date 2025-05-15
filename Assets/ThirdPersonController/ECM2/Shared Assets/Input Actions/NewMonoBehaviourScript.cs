using UnityEngine;
using UnityEngine.InputSystem;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class CreateDashInputReference : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Create Dash InputActionReference")]
    public static void CreateDashReference()
    {
        var asset = AssetDatabase.LoadAssetAtPath<InputActionAsset>("Assets/InputSystem_Actions.inputactions");
        if (asset == null)
        {
            Debug.LogError("InputSystem_Actions.inputactions not found in Assets/!");
            return;
        }

        var action = asset.FindAction("Player/Dash");
        if (action == null)
        {
            Debug.LogError("Action 'Player/Dash' not found in InputSystem_Actions!");
            return;
        }

        var reference = InputActionReference.Create(action);
        AssetDatabase.CreateAsset(reference, "Assets/Dash.inputActionReference");
        AssetDatabase.SaveAssets();

        Debug.Log("Dash.inputActionReference created successfully!");
    }
#endif
}