using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    private Button beginGameButton;
    // Start is called before the first frame update
    void Start()
    {
        beginGameButton = this.GetComponent<Button>();
        beginGameButton.onClick.AddListener(BeginGameButtonOnClick);
    }

    private void BeginGameButtonOnClick()
    {
        SceneManager.LoadScene("Game");
    }
}
