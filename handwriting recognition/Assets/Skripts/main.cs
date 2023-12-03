using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;
using System.Linq;
using System;
using MathNet.Numerics;

public class main : MonoBehaviour
{
    private List<Vector<double>> vectors = new List<Vector<double>>();

    void Start()
    {
        Main();
    }
    public void AddVector(Vector<double> newVector)
    {
        vectors.Add(newVector);
    }
    public  Vector<double> GetLastVector()
    {
        return vectors[vectors.Count - 1];
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

        // ������� �� ���������� �������� ��� ������������
        weightMatrix = ZeroOutDiagonal(weightMatrix);
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
        // ��������, ��� ������� ����������
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
        // ��������� ������� �� ����������������� ������-�������
        Vector<double> resultVector = matrix.Multiply(vector.ToColumnMatrix().Column(0));

        // �������� ������� �� �������-�������
        Matrix<double> resultMatrix = Matrix<double>.Build.DenseOfColumnVectors(resultVector);

        return resultMatrix;
    }


    public Vector<double> MultiplyMatrixByVector2(Matrix<double> matrix, Vector<double> vector)
    {
        // ��������� ������� �� ������-�������
        Vector<double> resultVector = matrix.Multiply(vector);

        // ����� ����� ������������ �������� ���������:
        // Vector<double> resultVector = matrix * vector;

        return resultVector;
    }
    public Vector<double> SignedVectorElements(Vector<double> inputVector)
    {
        // ������� ����� �������, ����� �� �������� ��������
        Vector<double> modifiedVector = inputVector.Clone();

        // �������� �� ������� �������� ������� � ������ ��� � ������������ � ��������
        for (int i = 0; i < modifiedVector.Count; i++)
        {
            modifiedVector[i] = (modifiedVector[i] >= 0) ? 1 : -1;
        }

        return modifiedVector;
    }
    public int IdentifyRecognizedPattern(Vector<double> recognizedVector, Vector<double>[] trainingVectors)
    {
        for (int i = 0; i < trainingVectors.Length; i++)
        {
            // ��������� ������������� ������� � ����������
            if (recognizedVector.Equals(trainingVectors[i]))
            {
                return i; // ���������� ������ ������������� �������
            }
        }

        return -1; // ���������� -1, ���� �� ���� ������ �� ������
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
            //Debug.Log("hamming distance "+ hammingDistance + " index " + bestMatchIndex);
        }

        Debug.Log("min hamming distance " + minHammingDistance + " beast match index " + bestMatchIndex);
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
    public void HopfieldNeuralNetWork(Vector<double> inputVectorToRecognize)
    {
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
        inputVectorToRecognize = columnVector;
        // ����� ������� W
        Debug.Log("���������� ����� ������� " + numberOfRecognizedVector);
        Debug.Log(inputVectorToRecognize);
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
