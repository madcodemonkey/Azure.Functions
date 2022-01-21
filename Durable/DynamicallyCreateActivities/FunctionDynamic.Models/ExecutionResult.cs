using System;

namespace FunctionDynamic.Models
{
    public class ExecutionResult
    {
        private ExecutionStatus _status = ExecutionStatus.Success;

        public ExecutionStatus Status
        {
            get => _status;
            set
            {
                switch (value)
                {
                    case ExecutionStatus.Success:
                        TryMarkAsSuccess();
                        break;
                    case ExecutionStatus.Partial:
                        TryMarkAsPartial();
                        break;
                    case ExecutionStatus.Failure:
                        TryMarkAsFailure();
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(value), value, null);
                }
            }
        }

        public string LogText { get; set; }

        public void Merge(ExecutionResult result)
        {
            switch (result.Status)
            {
                case ExecutionStatus.Success:
                    TryMarkAsSuccess(result.LogText);
                    break;
                case ExecutionStatus.Partial:
                    TryMarkAsPartial(result.LogText);
                    break;
                case ExecutionStatus.Failure:
                    TryMarkAsFailure(result.LogText);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TryMarkAsFailure(string logText = null)
        {
            if (_status != ExecutionStatus.Failure)
            {
                LogText = null;
            }

            // If the failure doesn't have a message, don't overwrite the last message
            if (string.IsNullOrWhiteSpace(logText) == false)
            {
                LogText = logText;
            }

            _status = ExecutionStatus.Failure;
        }

        public void TryMarkAsPartial(string logText = null)
        {
            if (_status != ExecutionStatus.Failure)
            {
                if (_status != ExecutionStatus.Partial)
                {
                    LogText = null;
                }

                // If the partial doesn't have a message, don't overwrite the last message
                if (string.IsNullOrWhiteSpace(logText) == false)
                {
                    LogText = logText;
                }

                _status = ExecutionStatus.Partial;
            }
        }

        public void TryMarkAsSuccess(string logText = null)
        {
            if ((int)_status < 1)
            {
                _status = ExecutionStatus.Success;
                LogText = logText;
            }
        }
    }
}