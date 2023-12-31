using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Rendering;

public class UiManager : MonoBehaviour
{
    [SerializeField] private Material basedMaterial;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private main mainSkript;
    [SerializeField] private GameObject gridLayoutTrain; 
    [SerializeField] private GameObject gridLayoutTest; 
    [SerializeField] private GameObject gridLayoutMyFiles; 
    [SerializeField] private GameObject imagesToTrainOn;
    [SerializeField] private GameObject imagesToTestOn;
    [SerializeField] private GameObject myFiles;
    [SerializeField] private GameObject warning;
    [Header("Buttons")]
    [SerializeField] private Button openImagesToTrainButton;
    [SerializeField] private Button openImagesToTestButton;
    [SerializeField] private Button openMyFilesButton;
    [SerializeField] private Button closeImagesToTrainButton;
    [SerializeField] private Button closeImagesToTestButton;
    [SerializeField] private Button closeMyFilesButton;
    [SerializeField] private Button addNewTrainVectorButton;
    [SerializeField] private Button clearButton;
    [SerializeField] private Button checkCurrentImageButton;
    [SerializeField] private Button showOnQuadButton;
    [SerializeField] private Button convertSpritesButton;
    [SerializeField] private Button currentTextureToVectorButton;
    [SerializeField] private Button deleteTrainingVectorsInTrainingVectors;
    [SerializeField] private Button deleteTrainingVectorsInMyFiles;
    [SerializeField] private Button loadCDJM;
    [SerializeField] private Button closeWarning;
    void Start()
    {
        openImagesToTrainButton.onClick.AddListener(() => imagesToTrainOn.SetActive(true)); 
        openImagesToTestButton.onClick.AddListener(() => imagesToTestOn.SetActive(true)); 
        openMyFilesButton.onClick.AddListener(() => myFiles.SetActive(true)); 
        closeImagesToTrainButton.onClick.AddListener(() => imagesToTrainOn.SetActive(false));
        closeImagesToTestButton.onClick.AddListener(() => imagesToTestOn.SetActive(false));
        closeMyFilesButton.onClick.AddListener(() => myFiles.SetActive(false));
        addNewTrainVectorButton.onClick.AddListener(() => AddNewTrainVector(playerController.GetTexture()));
        clearButton.onClick.AddListener(playerController.FillTextureWithWhite);
        checkCurrentImageButton.onClick.AddListener(CheckCurrentImage);
        showOnQuadButton.onClick.AddListener(ShowLastVector);
        convertSpritesButton.onClick.AddListener(() => ShowTrainInScrolRect(mainSkript.GetVectors()));
        currentTextureToVectorButton.onClick.AddListener(ShowCurrentTextureToVector);

        deleteTrainingVectorsInMyFiles.onClick.AddListener(DeleteTraingVectors);
        deleteTrainingVectorsInTrainingVectors.onClick.AddListener(DeleteTraingVectors);
        loadCDJM.onClick.AddListener(LoadCDJM);

        closeWarning.onClick.AddListener(() => warning.SetActive(false));

        playerController.FillTextureWithWhite();
        ShowTrainInScrolRect(mainSkript.GetVectors());
        ShowTestInScrolRect(mainSkript.GetVectorsTest());
        ShowMyFilesInScrolRect(mainSkript.GetMyFilesVectors());
        CheckCurrentImage();
        playerController.FillTextureWithWhite();
    }
    private void DeleteTraingVectors()
    {
        mainSkript.ClearVectors();
        if (gridLayoutTrain.transform.childCount > 0)
        {
            // ���������� �� ���� �������� �������� � ������� ��
            foreach (Transform child in gridLayoutTrain.transform)
            {
                Destroy(child.gameObject);
            }
        }
    }
    private void LoadCDJM()
    {
        DeleteTraingVectors();
        mainSkript.LoadImagesFromTrainMain();
        ShowTrainInScrolRect(mainSkript.GetVectors());
    }
    public void ShowCurrentTextureToVector()
    {
        Debug.Log("Current Texture to vector " + TextureToVector(playerController.GetTexture()));
    }
    public void ShowTestInScrolRect(Vector<double>[] vectors)
    {
        if (gridLayoutTest.transform.childCount > 0)
        {
            // ���������� �� ���� �������� �������� � ������� ��
            foreach (Transform child in gridLayoutTest.transform)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < vectors.Length; i++)
        {
            Texture2D newTexture = VectorToTexture(vectors[i], playerController.GetTextureSize(), playerController.GetTextureSize());

            // ������� ����� RawImage
            GameObject newRawImageObject = new GameObject("RawImage");
            RawImage rawImage = newRawImageObject.AddComponent<RawImage>();
            Button button = newRawImageObject.AddComponent<Button>();

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
            newRawImageObject.transform.SetParent(gridLayoutTest.transform, false);
            button.onClick.AddListener(() => playerController.SetTexture(copiedTexture));
            button.onClick.AddListener(() => imagesToTestOn.SetActive(false));

        }
        Debug.Log(mainSkript.GetVectorsTest().Length + "Train verors amount");
    }
    public void ShowMyFilesInScrolRect(Vector<double>[] vectors)
    {
        if (gridLayoutMyFiles.transform.childCount > 0)
        {
            // ���������� �� ���� �������� �������� � ������� ��
            foreach (Transform child in gridLayoutMyFiles.transform)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < vectors.Length; i++)
        {
            Texture2D newTexture = VectorToTexture(vectors[i], playerController.GetTextureSize(), playerController.GetTextureSize());

            // ������� ����� RawImage
            GameObject newRawImageObject = new GameObject("RawImage");
            RawImage rawImage = newRawImageObject.AddComponent<RawImage>();
            Button button = newRawImageObject.AddComponent<Button>();

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
            newRawImageObject.transform.SetParent(gridLayoutMyFiles.transform, false);
            button.onClick.AddListener(() => playerController.SetTexture(copiedTexture));
            button.onClick.AddListener(() => myFiles.SetActive(false));

        }
        Debug.Log(mainSkript.GetVectorsTest().Length + "Train verors amount");
    }
    public void ShowTrainInScrolRect(Vector<double>[] vectors)
    {
        if (gridLayoutTrain.transform.childCount > 0)
        {
            // ���������� �� ���� �������� �������� � ������� ��
            foreach (Transform child in gridLayoutTrain.transform)
            {
                Destroy(child.gameObject);
            }
        }
        for (int i = 0; i < vectors.Length; i++)
        {
            Texture2D newTexture = VectorToTexture(vectors[i], playerController.GetTextureSize(), playerController.GetTextureSize());

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
            newRawImageObject.transform.SetParent(gridLayoutTrain.transform, false);
        }
        Debug.Log(mainSkript.GetVectors().Length + "Train verors amount");
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
        if (mainSkript.GetVectors().Length > 0)
        {
            // �������� ������� ��������
            Texture2D newTexture = playerController.GetTexture();

            // ���� ����������� ��������
            Vector<double> newVector = TextureToVector(newTexture);
            Debug.Log("vector brfor hemming" + newVector);
            int size = playerController.GetTextureSize();
            Vector<double> hammingVector = mainSkript.HopfieldNeuralNetWork(newVector);
            Debug.Log("Vector After Hamming " + hammingVector);
            Texture2D showHVextor = VectorToTexture(hammingVector, size, size);
            playerController.SetTexture(showHVextor);
        }
        else
        {
            warning.SetActive(true);
        }

    }
    public void AddNewTrainVector(Texture2D texture)
    {
        // �������� ������� ��������
        Texture2D newTexture = texture;

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
        newRawImageObject.transform.SetParent(gridLayoutTrain.transform, false);

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
                // ����� ����������� -1
                resultVector[i] = 1.0;
            }
        }

        Debug.Log("TextureToVector " + resultVector);
        return resultVector;
    }
    public static Texture2D VectorToTexture(Vector<double> vector, int width, int height)
    {
        // ������� ����� �������� � ���������� ������� � �������
        Texture2D texture = new Texture2D(width, height);

        // ����������� ������ � ������ ��������
        double[] values = vector.ToArray();
        Debug.Log("AAAAAAAAAAAAAAA VALUES Length" + values.Length);
        Debug.Log("AAAAAAAAAAAAAAA vector " + vector.ToString());
        // �������� �� ������� ������� ��������
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                // �������� ������ �������� �������, ���������������� �������� �������
                int index = y * width + x;
                //int index = y + x;
                Debug.Log(index + "aaaaaaaaaaaaaaaaaaaa index");
                // ���� �������� ������� ����� 1, ������������� ������ ����, ����� � �����
                Color pixelColor = (values[index] >= 0.0) ? Color.black : Color.white;;// ���� ��� �������� �� ==

                Debug.Log("x = " + x + "y = " + y +"index" + index + "pixel color" + pixelColor);
                // ������������� ���� ������� � ��������
                texture.SetPixel(x, y, pixelColor);
                //if(x == 1 && y == 0) texture.SetPixel(x, y, Color.red);
            }
        }

        // ��������� ��������� � ��������
        texture.Apply();
        Debug.Log(vector + "TextureToVector");
        string vec = "vec= ";
        for(int i = 0; i < values.Length; i++)
        {
            vec += values[i];   
        }
        Debug.Log(vec + "values");
        return texture;
    }
}
