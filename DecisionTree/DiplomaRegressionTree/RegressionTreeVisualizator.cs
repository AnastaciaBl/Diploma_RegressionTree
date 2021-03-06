﻿using System.Collections.Generic;
using System.Linq;
using DecisionTree;
using System.Windows.Forms.DataVisualization.Charting;

namespace DiplomaRegressionTree
{
    class RegressionTreeVisualizator
    {
        public RegressionTree Tree { get; private set; }
        private Chart RegressionChart;

        public RegressionTreeVisualizator(RegressionTree tree, Chart chart)
        {
            Tree = tree;
            RegressionChart = chart;
        }

        public static void DrawCorrelationField(Data[] dataSet, Chart chart)
        {
            chart.Series["CorrelationField"].Points.Clear();
            for (int i = 0; i < dataSet.Length; i++)
                chart.Series["CorrelationField"].Points.AddXY(dataSet[i].Arguments[0], dataSet[i].Y);
        }

        public void DrawRegressionLine(string series)
        {
            RegressionChart.Series[series].Points.Clear();
            var averageY = new List<double>();
            var minX = new List<double>();
            var maxX = new List<double>();
            findArgumentsOfRegressionLine(averageY, minX, maxX);
            for (int i = 0; i < averageY.Count; i++)
            {
                RegressionChart.Series[series].Points.AddXY(minX[i], averageY[i]);
                RegressionChart.Series[series].Points.AddXY(maxX[i], averageY[i]);
            }
        }

        private void SortMaxMinX(List<double> minX, List<double> maxX)
        {
            var d = new Dictionary<double, double>();
            for (int i = 0; i < minX.Count; i++)
                d.Add(minX[i], maxX[i]);
            //d.OrderBy(key => key.Value);
            int counter = 0;
            foreach (var pair in d.OrderBy(key => key.Value))
            {
                minX[counter] = pair.Key;
                maxX[counter] = pair.Value;
                counter++;
            }
        }

        private void findArgumentsOfRegressionLine(List<double> averageY, List<double> minX, List<double> maxX)
        {
            var qu = new Queue<DecisionTreeNode>();
            qu.Enqueue(Tree.Head);
            while (qu.Count != 0)
            {
                DecisionTreeNode tempNode = qu.Dequeue();
                if (tempNode.IsLeaf)
                {
                    //averageY.Add(countAverageYInLeaf(tempNode));
                    minX.Add(findMinXInLeaf(tempNode));
                    maxX.Add(findMaxXInLeaf(tempNode));
                }
                else
                {
                    qu.Enqueue(tempNode.LeftChild);
                    qu.Enqueue(tempNode.RightChild);
                }
            }
            SortMaxMinX(minX, maxX);
            for (int i = 0; i < minX.Count; i++)
                averageY.Add(Tree.Deside(minX[i]));
        }

        private double countAverageYInLeaf(DecisionTreeNode node)
        {
            double answer = 0;
            for (int i = 0; i < node.Elements.Length; i++)
                answer += node.Elements[i].Y;
            return answer / node.Elements.Length;
        }

        private double findMinXInLeaf(DecisionTreeNode node)
        {
            double answer = 1000000;
            for (int i = 0; i < node.Elements.Length; i++)
            {
                if (node.Elements[i].Arguments[0] < answer)
                    answer = node.Elements[i].Arguments[0];
            }
            return answer;
        }

        private double findMaxXInLeaf(DecisionTreeNode node)
        {
            double answer = -1000000;
            for (int i = 0; i < node.Elements.Length; i++)
            {
                if (node.Elements[i].Arguments[0] > answer)
                    answer = node.Elements[i].Arguments[0];
            }
            return answer;
        }

        public static void ClearChart(Chart chart)
        {
            chart.Series["CorrelationField"].Points.Clear();
            switch (chart.Name)
            {
                case "RegressionChart":
                    chart.Series["RegressionLine"].Points.Clear();
                    break;
                case "RandomForestChart":
                    for (int i = chart.Series.Count - 1; i >= 1; i--)
                        chart.Series.RemoveAt(i);
                    break;
            }
        }
    }
}
