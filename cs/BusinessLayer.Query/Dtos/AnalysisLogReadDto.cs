using System;
using System.Collections.Generic;
using System.Text;

namespace BusinessLayer.Query.Dtos
{
    public class AnalysisLogReadDto
    {
        public string Action { get; }
        public int Count { get; }
        public int Mean { get; }
        public int Median { get; }

        public AnalysisLogReadDto(string action, int count, int mean, int median)
        {
            Action = action;
            Count = count;
            Mean = mean;
            Median = median;
        }
    }
}
