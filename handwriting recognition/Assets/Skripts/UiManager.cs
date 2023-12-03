using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Material basedMaterial;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private main mainSkript;
    [SerializeField] private GameObject GridLayout; 
    [SerializeField] private GameObject imagesToTrainOn;
    [Header("Buttons")]
    [SerializeField] private Button openImagesToTrainButton;
    [SerializeField] private Button closeImagesToTrainButton;
    [SerializeField] private Button addNewTrainVectorButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button checkCurrentImageButton;
    [SerializeField] private Button showOnQuadButton;
    void Start()
    {
        openImagesToTrainButton.onClick.AddListener(() => imagesToTrainOn.SetActive(true)); 
        closeImagesToTrainButton.onClick.AddListener(() => imagesToTrainOn.SetActive(false));
        addNewTrainVectorButton.onClick.AddListener(AddNewTrainVector);
        clearButton.onClick.AddListener(playerController.FillTextureWithWhite);
        checkCurrentImageButton.onClick.AddListener(CheckCurrentImage);
        showOnQuadButton.onClick.AddListener(ShowLastVector);
        playerController.FillTextureWithWhite();
    }
    public void ShowLastVector()
    {
        Texture2D newTexture = playerController.GetTexture();
        int size = playerController.GetTextureSize();
        newTexture = VectorToTexture(mainSkript.GetLastVector(), size, size);
        playerController.SetTexture(newTexture);
    }
    public void CheckCurrentImage()
    {
        // �������� ������� ��������
        Texture2D newTexture = playerController.GetTexture();

        // ���� ����������� ��������
        Vector<double> newVector = TextureToVector(newTexture);
        mainSkript.HopfieldNeuralNetWork(newVector);
    }
    public void AddNewTrainVector()
    {
        // �������� ������� ��������
        Texture2D newTexture = playerController.GetTexture();

        // ������� ����� RawImage
        GameObject newRawImageObject = new GameObject("RawImage");
        RawImage rawImage = newRawImageObject.AddComponent<RawImage>();

        // �������� ��������, ����� ��������� �� ����������� ��������
        Texture2D copiedTexture = new Texture2D(newTexture.width, newTexture.height);
        copiedTexture.SetPixels(newTexture.GetPixels());
        copiedTexture.filterMode = playerController.GetFilterMode();
        copiedTexture.wrapMode = playerController.GetWrapMode();
        copiedTexture.Apply();
        rawImage.material = basedMaterial;
        // ����������� �������� RawImage
        rawImage.texture = copiedTexture;

        // ������ ����� RawImage �������� ��� GridLayout
        newRawImageObject.transform.SetParent(GridLayout.transform, false);

        // ���� ����������� ��������
        Vector<double> newVector = TextureToVector(newTexture);
        mainSkript.AddVector(newVector);
        playerController.FillTextureWithWhite();
    }

    public static Vector<double> TextureToVector(Texture2D texture)
    {
        // �������� ������ ������ ��������
        Color[] pixels = texture.GetPixels();

        // ������� ������ ������ �����
        Vector<double> resultVector = Vector<double>.Build.Dense(pixels.Length);

        // �������� �� ������� �������
        for (int i = 0; i < pixels.Length; i++)
        {
            // ���� ���� ������� �����  (��� ������ � �������)
            if (pixels[i].r > 0.95f && pixels[i].g > 0.95f && pixels[i].b > 0.95f)
            {
                // ����������� 1
                resultVector[i] = -1.0;
            }
            else
            {
                // ����� ����������� 0
                resultVector[i] = 1.0;
            }
        }

        return resultVector;
    }
    public static Texture2D VectorToTexture(Vector<double> vector, int width, int height)
    {
        // ������� ����� �������� � ���������� ������� � �������
        Texture2D texture = new Texture2D(width, height);

        // ����������� ������ � ������ ��������
        double[] values = vector.ToArray();

        // �������� �� ������� ������� ��������
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // �������� ������ �������� �������, ���������������� �������� �������
                int index = y * width + x;

                // ���� �������� ������� ����� 1, ������������� ������ ����, ����� � �����
                Color pixelColor = (values[index] == 1.0) ? Color.black : Color.white;

                // ������������� ���� ������� � ��������
                texture.SetPixel(x, y, pixelColor);
            }
        }

        // ��������� ��������� � ��������
        texture.Apply();

        return texture;
    }
}
