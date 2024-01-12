using System;
using UnityEngine;

namespace Assets.Scripts.Utils
{
    [Serializable]
    public class PropertyField2D<T>
    {
        [Serializable]
        public struct RowData
        {
            public T[] Row;

            public RowData(int size) {
                Row = new T[size];
                for (int i = 0; i < size; i++) {
                    Row[i] = default;
                }
            }
        }

        [SerializeField] int _columnLength;
        [SerializeField] int _rowLength;
        [SerializeField] RowData[] _column;

        [SerializeField] int _labelWidth;
        [SerializeField] int _labelHeight;

        public PropertyField2D(int columnSize, int rowSize, int layoutWidth = 20,int layoutHeight = 20) {
            _columnLength = columnSize;
            _rowLength = rowSize;
            _column = new RowData[_columnLength];

            _labelWidth = layoutWidth;
            _labelHeight = layoutHeight;

            for (int i = 0; i < _columnLength; i++) {
                _column[i] = new RowData(rowSize);
            }
        }

        public T GetValue(int row, int column) {
            if (row >= 0 && row < _rowLength && column >= 0 && column < _columnLength) {
                return _column[row].Row[column];
            }
            return default;
        }

        public void SetValue(int row, int column, T value) {
            if (row >= 0 && row < _rowLength && column >= 0 && column < _columnLength) {
                _column[row].Row[column] = value;
            }
        }

        public int GetLength(int index) {
            return index == 0 ? _columnLength : _rowLength;
        }
    }
}
