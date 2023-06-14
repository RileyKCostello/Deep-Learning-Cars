using System.Collections;
using System.Collections.Generic;

public class Matrix
{
    public int numColumns, numRows;
    public float[] entries;
    public Matrix(int numRows, int numColumns)
    {
        this.numRows = numRows;
        this.numColumns = numColumns;
        entries = new float[numRows * numColumns];
    }

    //Multiply Matrices
    public static Matrix operator *(Matrix m1, Matrix m2)
    {
        Matrix outputMatrix = new Matrix(m1.numRows, m2.numColumns);
        float newEntry;
        float[] newRow = new float[m2.numRows];
        float[] row = new float[m1.numColumns];
        float[] column = new float[m2.numRows];
        //Iterate through rows of first matrix
        for (int i = 1; i <= m1.numRows; i++)
        {
            row = m1.GetRow(i);
            //Iterate through columns of second matrix
            for (int j = 1; j <= m2.numColumns; j++)
            {
                column = m2.GetColumn(j);
                newEntry = Msum(row, column);
                newRow[j-1] = newEntry;
            }
            outputMatrix.FillRow(newRow, i);

        }
        return outputMatrix;
    }

    //Matrix Creation and Adjustment Functions

    public void FillRow(float[] rowList, int rowNumber)
    {
        //Gives the index where the row starts
        int offset = numColumns * (rowNumber - 1);
        for(int i = 0; i < numColumns; i++)
        {
            entries[i + offset] = rowList[i];
        }
    }

    public void AdjustEntry(int row, int column, float newEntry)
    {
        //Finds the index for a given entry
        int index = (row - 1) * numColumns + column - 1;
        entries[index] = newEntry;
    }

    //Matrix Getter Functions

    public float this[int row, int column]
    {
        get => entries[(row - 1) * numColumns + column - 1];
        set => entries[(row - 1) * numColumns + column - 1] = value;
    }

    public float[] GetRow(int rowNumber)
    {
        float[] output = new float[numColumns];
        int offset = numColumns * (rowNumber - 1);
        for (int i = 0; i < numColumns; i++)
        {
            output[i] = entries[i + offset];
        }
        return output;
    }

    //The first entry in the returned array is the highest entry of the column
    public float[] GetColumn(int columnNum)
    {
        float[] output = new float[numRows];
        int index;
        for (int i = 0; i < numRows; i++)
        {
            index = i * numColumns + columnNum - 1;
            output[i] = entries[index];
        }
        return output;
    }

    //Matrix math functions

    static float Msum(float[] r, float[] c)
    {
        int size = r.Length;
        if (size != c.Length)
        {
            return -1f;
        }
        float total = 0f;
        for (int i = 0; i < size; i++)
        {
            total += r[i] * c[i];
        }
        return total;
    }

    //Converts matrix to multi-line string

    public override string ToString()
    {
        string output = "";
        float[] row = new float[numColumns];
        for (int i = 0; i < numRows; i++)
        {
            row = GetRow(i + 1);
            output = output + "|";
            foreach(float x in row)
            {
                output = output + " " + x.ToString() + " |";
            }
            output = output + "\n";
        }
        return output;
    }
}
