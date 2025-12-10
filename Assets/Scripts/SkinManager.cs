using UnityEngine;

public class SkinManager : MonoBehaviour
{
    public int chosenSkinId;
    public static SkinManager instance;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    public void SetSkinId(int id) => chosenSkinId = id;

    public int GetSkinId() => chosenSkinId;
}
