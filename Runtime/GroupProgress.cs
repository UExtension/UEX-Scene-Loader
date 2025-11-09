using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UExtension.SceneLoader
{
    public class GroupProgress : IProgress<AsyncOperation>
    {
        private readonly int _initialCapacity;
        private readonly IProgress<float> _onProgress;
        private List<AsyncOperation> Operations { get; } = new();

        public GroupProgress(int initialCapacity, IProgress<float> onProgress)
        {
            _initialCapacity = initialCapacity;
            _onProgress = onProgress;
        }

        public void Report(AsyncOperation operation)
        {
            Operations.Add(operation);
            _onProgress.Report(CalculateGroupProgress());
        }

        private float CalculateGroupProgress()
        {
            return Operations.Sum(op => op.progress) / Math.Max(_initialCapacity, Operations.Count);
        }
    }
}