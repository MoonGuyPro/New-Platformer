using UnityEngine;
using UnityEngine.UI; // Dla obsługi UI, takiego jak obrazy i przyciski
using UnityEngine.SceneManagement; // Dla zarządzania scenami

public class IntroSequence : MonoBehaviour
{
    public Sprite[] introImages; // Tablica obrazów do wyświetlenia
    public Image displayImage; // UI Image, które będzie wyświetlać obrazy

    private int currentImageIndex = 0;

    void Start()
    {
        if (introImages.Length > 0)
        {
            displayImage.sprite = introImages[currentImageIndex]; // Ustaw pierwszy obraz
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) // Sprawdź, czy naciśnięto lewy przycisk myszy
        {
            currentImageIndex++;
            if (currentImageIndex < introImages.Length)
            {
                displayImage.sprite = introImages[currentImageIndex];
            }
            else
            {
                StartGame(); // Wywołaj funkcję rozpoczynającą grę
            }
        }
    }

    void StartGame()
    {
        // Tutaj możesz na przykład załadować główną scenę gry
        SceneManager.LoadScene("SampleScene");
    }
}