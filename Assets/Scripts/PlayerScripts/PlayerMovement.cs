using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public CharacterController2D controller;
    public Animator animator;
    public PlayerCombat playerCombat;

    public float runSpeed = 40f;

    private float horizontalMove = 0f;
    public bool isJumping = false;

    //Do tworzenia mapy cieplnej 
    public static int width = 100; // Szerokoœæ mapy cieplnej
    public static int height = 100; // Wysokoœæ mapy cieplnej
    public Texture2D heatMapTexture;

    private void Awake()
    {
        heatMapTexture = new Texture2D(width, height);
    }

    public void UpdateHeatMapAtPosition(Vector2 pos)
    {
        int x = (int)(((pos.x + 10) / 40.0f) * width);
        int y = (int)(((pos.y + 10) / 40.0f) * height);
        // Upewnij siê, ¿e wspó³rzêdne nie wychodz¹ poza teksturê
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);
        int radius = 2; // Mo¿esz dostosowaæ rozmiar promienia

        for (int dx = -radius; dx <= radius; dx++)
        {
            for (int dy = -radius; dy <= radius; dy++)
            {
                if (x + dx >= 0 && y + dy >= 0 && x + dx < width && y + dy < height)
                {
                    Color existingColor = heatMapTexture.GetPixel(x + dx, y + dy);
                    existingColor += new Color(0.08f, 0, 0, 1); // Stopniowo zwiêkszaj intensywnoœæ czerwonego kana³u
                    existingColor.r = Mathf.Min(existingColor.r, 1); // Ogranicz wartoœæ czerwonego kana³u do 1
                    heatMapTexture.SetPixel(x + dx, y + dy, existingColor);
                }
            }
        }

        heatMapTexture.Apply();
    }


    // Update is called once per frame
    void Update()
    {
        horizontalMove = Input.GetAxisRaw("Horizontal") * runSpeed;
        animator.SetFloat("Speed", Mathf.Abs(horizontalMove));
        
        if (Input.GetButtonDown("Jump") && !playerCombat.isCastingSpell)
        {
            isJumping = true;
            animator.SetBool("IsJumping", true);
        }

        if (Time.frameCount % 50 == 0)
        {
            UpdateHeatMapAtPosition(transform.position);
        }
    }

    private void FixedUpdate()
    {
        controller.Move(horizontalMove * Time.fixedDeltaTime, false, isJumping);
        

    }

    public void OnLanding()
    {
        animator.SetBool("IsJumping", false);
        isJumping = false;
    }

    public void SaveHeatMapToFile(Texture2D heatMapTexture, string filePath)
    {
        byte[] bytes = heatMapTexture.EncodeToPNG();
        File.WriteAllBytes(filePath, bytes);
    }
}
