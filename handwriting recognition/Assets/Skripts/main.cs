using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MathNet.Numerics.LinearAlgebra;

public class main : MonoBehaviour
{
    void Start()
    {
        // ������� ������-�������
        Vector<double> vectorColumn = Vector<double>.Build.Dense(new double[] { 1, 2, 3 });

        // ������� ������-������
        Vector<double> vectorRow = Vector<double>.Build.Dense(new double[] { 1, 2, 3 });

        // ����������� ������-������� �� ������-������
        Matrix<double> resultMatrix = vectorColumn.ToColumnMatrix() * vectorRow.ToRowMatrix();

        // ������� ��������� � ������� Unity
        Debug.Log("��������� ������������:");
        Debug.Log(resultMatrix); 
    }
}
