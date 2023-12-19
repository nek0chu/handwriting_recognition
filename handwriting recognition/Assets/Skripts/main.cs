using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System;
using MathNet.Numerics;
using JetBrains.Annotations;
using System.IO;
using TMPro;

public class main : MonoBehaviour
{
    [SerializeField] private int minimalHammingDistance;
    [SerializeField] private Texture2D[] trainingImages;
    [SerializeField] private Texture2D[] testImages;
    private List<Vector<double>> vectors = new List<Vector<double>>();
    private List<Vector<double>> vectorsTest = new List<Vector<double>>();
    private List<Vector<double>> myFiles = new List<Vector<double>>();
    private int hammingDistance;
    public string folderName = "My Files";

    void Awake()
    {
        LoadImagesFromTrainMain();
        LoadImagesFromTestMain();
        LoadMyFiles();
    }

    void LoadMyFiles()
    {
        string folderPath = Path.Combine(Application.dataPath, folderName);

        if (Directory.Exists(folderPath))
        {
            string[] txtPaths = Directory.GetFiles(folderPath, "*.txt"); 

            foreach (string txtPath in txtPaths)
            {
                //������������ ��� �����
                Texture2D texture = ConvertTxtToTexture2D(txtPath);

                Vector<double> imageVector = ConvertImageToVectorMain(texture);
                myFiles.Add(imageVector);
                Debug.Log("MyFile" + imageVector);

            }
        }
        else
        {
            Debug.LogError("Folder not found: " + folderPath);
        }
    }
    Texture2D ConvertTxtToTexture2D(string filePath)
    {
        string[] lines = File.ReadAllLines(filePath);

        if (lines.Length == 0)
        {
            Debug.LogError("File is empty: " + filePath);
            return null;
        }
        int width = lines[0].Length;
        int height = lines.Length;

        Texture2D texture = new Texture2D(width, height);

        for (int y = 0; y < height; y++)
        {
            char[] chars = lines[y].ToCharArray();

            int reversedY = height - 1 - y;

            for (int x = 0; x < width; x++)
            {
                Color pixelColor = (chars[x] == '0') ? Color.white : Color.black;
                texture.SetPixel(x, reversedY, pixelColor);
            }
        }

        texture.Apply();
        return texture;
    }
    public void LoadImagesFromTrainMain()
    {
        if (vectors != null)
        {
            vectors.Clear();
        }
        foreach (var texture in trainingImages)
        {
            Vector<double> imageVector = ConvertImageToVectorMain(texture);
            vectors.Add(imageVector);
            Debug.Log("Train vector" + imageVector);
        }
        Debug.Log(vectors.Count + "Train vectors amount");
    }
    public void LoadImagesFromTestMain()
    {
        if (vectorsTest != null)
        {
            vectorsTest.Clear();
        }
        foreach (var texture in testImages)
        {
            Vector<double> imageVector = ConvertImageToVectorMain(texture);
            vectorsTest.Add(imageVector);
            Debug.Log("Test Vector" + imageVector);
        }
        Debug.Log(vectorsTest.Count + "Test vectors amount");
    }
    Vector<double> ConvertImageToVectorMain(Texture2D newTexture)
    {
        // ��������� ����������� � ��������
        Texture2D texture = newTexture;

        if (texture == null)
        {
            Debug.LogError("Failed to load the image.");
            return null;
        }

        int width = texture.width;
        int height = texture.height;
        Debug.Log($"Inconvert wi{width} and he{height} vect size {width * height}");

        Vector<double> vector = Vector<double>.Build.Dense(width * height);

        // �������� ����� �������� �� ��������
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            double grayscaleValue = pixels[i].grayscale; // �������� ������� (�� 0 �� 1)
            double normalizedValue = (grayscaleValue - 0.5) * 2.0; // ����������� � ��������� �� -1 �� 1
            if (grayscaleValue > 0.2f)
            {
                normalizedValue = -1;
            }
            else
            {
                normalizedValue = 1;
            }
            vector[i] = normalizedValue;
        }

        return vector;
    }

    Vector<double> ConvertImageToVector(string path)
    {
        // ��������� ����������� � ��������
        Texture2D texture = LoadTexture(path);

        if (texture == null)
        {
            Debug.LogError("Failed to load the image.");
            return null;
        }

        int width = texture.width;
        int height = texture.height;

        Vector<double> vector = Vector<double>.Build.Dense(width * height);

        // �������� ����� �������� �� ��������
        Color[] pixels = texture.GetPixels();

        for (int i = 0; i < pixels.Length; i++)
        {
            double grayscaleValue = pixels[i].grayscale; // �������� ������� (�� 0 �� 1)
            double normalizedValue = (grayscaleValue - 0.5) * 2.0; // ����������� � ��������� �� -1 �� 1
            if (grayscaleValue > 0.2f)
            {
                normalizedValue = -1;
            }
            else
            {
                normalizedValue = 1;
            }
            vector[i] = normalizedValue;
        }

        return vector;
    }

    Texture2D LoadTexture(string path)
    {
        byte[] fileData = System.IO.File.ReadAllBytes(path);
        Texture2D texture = new Texture2D(2, 2);
        texture.LoadImage(fileData); 

        return texture;
    }
    public Vector<double>[] GetVectors()
    {
        return vectors.ToArray();
    }
    public void SetVectors(Vector<double>[] newVectors)
    {
        vectors.Clear();
        vectors = newVectors.ToList();
    }
    public void ClearVectors()
    {
        vectors.Clear();
    }
    public Vector<double>[] GetVectorsTest()
    {
        return vectorsTest.ToArray();
    }
    public Vector<double>[] GetMyFilesVectors()
    {
        return myFiles.ToArray();
    }
    public void AddVector(Vector<double> newVector)
    {
        vectors.Add(newVector);
    }
    public  Vector<double> GetLastVector()
    {
        return vectors[vectors.Count - 1];
    }
    public string GetHammingDistance()
    {
        return hammingDistance.ToString();
    }
    // ������� ��� ������� ������� W
    public Matrix<double> CalculateWeightMatrix(Vector<double>[] trainingVectors)
    {
        // �������� ������� ���� �� ������ �������
        if (trainingVectors.Length == 0)
        {
            throw new ArgumentException("���������� ������������ ���� �� ���� ������ ��� ��������.");
        }

        // ��������, ��� ��� ������� ����� ���������� �����
        int vectorSize = trainingVectors[0].Count;
        if (trainingVectors.Any(v => v.Count != vectorSize))
        {
            throw new ArgumentException("��� ������� ������ ����� ���������� �����.");
        }

        // �������� ������� W ����������� vectorSize x vectorSize, ����������� ������
        Matrix<double> weightMatrix = Matrix<double>.Build.Dense(vectorSize, vectorSize);

        // ������ ������� W
        foreach (var vector in trainingVectors)
        {
            // �������������� ������� � �������-�������
            var columnMatrix = vector.ToColumnMatrix();

            // ���������� ������� W
            weightMatrix = weightMatrix.Add(columnMatrix * columnMatrix.Transpose());
        }

        //��������� ���������
        weightMatrix = ZeroOutDiagonal(weightMatrix);
        // ������� �� ���������� �������� ��� ������������
        weightMatrix = MultiplyMatrixByScalar(weightMatrix, 1 / (double)vectorSize);

        return weightMatrix;
    }
    public Matrix<double> MultiplyMatrixByScalar(Matrix<double> matrix, double scalar)
    {
        // ����������� ������� ��� ��������� ��������� ���������
        Matrix<double> resultMatrix = matrix.Clone();

        // ��������� ������� �������� ������� �� �����
        resultMatrix = resultMatrix.Multiply(scalar);

        return resultMatrix;
    }
    public Matrix<double> ZeroOutDiagonal(Matrix<double> matrix)
    {
        if (matrix.RowCount != matrix.ColumnCount)
        {
            throw new ArgumentException("������� ������ ���� ���������� ��� ��������� ���������.");
        }

        // ����������� ������� ��� ��������� ��������� ���������
        Matrix<double> resultMatrix = matrix.Clone();

        // ��������� ��������� �� ���������
        for (int i = 0; i < matrix.RowCount; i++)
        {
            resultMatrix[i, i] = 0.0;
        }

        return resultMatrix;
    }
    public Matrix<double> MultiplyMatrixByVector(Matrix<double> matrix, Vector<double> vector)
    {
        Matrix<double> resultMatrix = matrix * vector.ToColumnMatrix();

        return resultMatrix;
    }
    public Vector<double> SignedVectorElements(Vector<double> inputVector)
    {
        // ������� ����� �������, ����� �� �������� ��������
        Vector<double> modifiedVector = inputVector.Clone();

        for (int i = 0; i < modifiedVector.Count; i++)
        {
            modifiedVector[i] = (modifiedVector[i] >= 0) ? 1 : -1;
        }

        return modifiedVector;
    }

    // ������� ��� ������ ������� ���������� ������������
    private int FindBestMatchIndex(Vector<double> targetVector, Vector<double>[] candidates)
    {
        double minHammingDistance = double.MaxValue;
        int bestMatchIndex = -1;

        for (int i = 0; i < candidates.Length; i++)
        {
            double hammingDistance = HammingDistance(targetVector, candidates[i]);
            if (hammingDistance < minHammingDistance)
            {
                minHammingDistance = hammingDistance;
                bestMatchIndex = i;
            }
        }
        Debug.Log("min hamming distance " + minHammingDistance + " beast match index " + bestMatchIndex);
        hammingDistance = (int)minHammingDistance;
        if (minHammingDistance > minimalHammingDistance)
        {
            bestMatchIndex = -1;
            Debug.Log("Didn't recognize!");
        }
        return bestMatchIndex;
    }

    // ������� ��� ���������� ���������� �������� ����� ���������
    private double HammingDistance(Vector<double> vector1, Vector<double> vector2)
    {
        int differingBits = 0;

        for (int i = 0; i < vector1.Count; i++)
        {
            if (vector1[i] != vector2[i])
            {
                differingBits++;
            }
        }

        return differingBits;
    }
    // ������ �������������
    public Vector<double> HopfieldNeuralNetWork(Vector<double> inputVectorToRecognize)
    {
        // �� ����� � ��������� ��� ���������� ������ ������
        Vector<double>[] trainingVectors = vectors.ToArray();
        // ������� ������� W � ���������� ����������� � ��������������
        Matrix<double> weightMatrix = CalculateWeightMatrix(trainingVectors);
        // �������� �� ���������������� ������ ������� ����� ����������
        weightMatrix = MultiplyMatrixByVector(weightMatrix, inputVectorToRecognize);

        //������ �� ������� ������
        double[] columnMajorArray = weightMatrix.ToColumnMajorArray();
        Vector<double> columnVector = Vector<double>.Build.Dense(columnMajorArray);

        //��� ������ ��������� ����� ������� sign
        columnVector = SignedVectorElements(columnVector);
        //��������� � ��������� �� ������� ������� 
        int numberOfRecognizedVector = FindBestMatchIndex(columnVector, trainingVectors);

        // ����� ������� W
        Debug.Log("���������� ����� ������� " + numberOfRecognizedVector);
        Debug.Log(inputVectorToRecognize);
        Debug.Log(vectors.Count);

        return columnVector;
    }
    public void Main()
    {
        // ������ ��������� ��������
        Vector<double>[] trainingVectors = new[]
        {
            Vector<double>.Build.DenseOfArray(new double[] { 1, 1, 1, 1, -1, 1, 1, -1, 1}),
            Vector<double>.Build.DenseOfArray(new double[] { 1, 1, 1, 1, -1, -1, 1, -1, -1}),
            Vector<double>.Build.DenseOfArray(new double[] { 1, -1, -1, 1, -1, -1, 1, 1, 1}),
            // �������� ������ ������� �� ���� �������������
        };
        Vector<double> inputVectorToRecognize = Vector<double>.Build.DenseOfArray(new double[] { -1, -1, -1, 1, -1, -1, 1, -1, -1 });
    }
}
