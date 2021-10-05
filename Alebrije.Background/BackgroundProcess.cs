using System;
using System.Threading.Tasks;
using Alebrije.Abstractions.Background;
using Alebrije.Abstractions.Enums;
using Alebrije.Exceptions;
using Alebrije.Extensions;
using Microsoft.Extensions.Logging;

namespace Alebrije.Background
{
    public abstract class BackgroundProcess<TEnv, TExec> : IBackgroundProcess<TEnv, TExec>
    {
        private static class ProcessStepName
        {
            public const string SetUp = "Environment Setup";
            public const string Execution = "Work Execution";
            public const string Teardown = "Execution Teardown";
        }

        private readonly ILogger _logger;
        public ProcessState CurrentState { get; private set; }

        protected BackgroundProcess(ILogger logger)
        {
            _logger = logger;
        }

        protected abstract void OnCreateInitialEnvironment(TEnv environmentConfig);
        protected abstract void OnExecute(TExec workConfig);
        protected abstract void OnCleaningExecution();

        public Task SetupEnvironment(TEnv environmentConfig)
        {
            return ExecutionWrapper(ProcessStepName.SetUp,
                ProcessState.NotInitialized,
                ProcessState.Loading,
                ProcessState.Idle,
                () => { OnCreateInitialEnvironment(environmentConfig); });
        }

        public Task Execute(TExec workConfig)
        {
            return ExecutionWrapper(ProcessStepName.Execution,
                ProcessState.Idle,
                ProcessState.Working,
                ProcessState.WorkFinished,
                () => { OnExecute(workConfig); });
        }

        public Task ExecuteAndClean(TExec workConfig)
        {
            var exec = ExecutionWrapper(ProcessStepName.Execution,
                ProcessState.Idle,
                ProcessState.Working,
                ProcessState.WorkFinished,
                () => { OnExecute(workConfig); });

            if (!exec.IsCompleted || CurrentState != ProcessState.WorkFinished)
            {
                return Task.FromException(new AlebrijeInvalidProcessStateException(CurrentState.ToString()));
            }

            var td = TeardownExecution();
            return td.IsCompleted && CurrentState == ProcessState.Idle
                ? Task.CompletedTask
                : Task.FromException(new AlebrijeInvalidProcessStateException(CurrentState.ToString(), null));
        }

        public Task TeardownExecution()
        {
            return ExecutionWrapper(ProcessStepName.Teardown,
                ProcessState.WorkFinished,
                ProcessState.CleaningEnvironment,
                ProcessState.Idle, OnCleaningExecution);
        }

        private Task ExecutionWrapper(string step, ProcessState expected, ProcessState startOn,
            ProcessState finishOn, Action workItem)
        {
            _logger.Debug($"{step} started");
            if (CurrentState != expected)
            {
                _logger.Warning($"Cannot continue the {step}: Invalid state {expected} - expected {CurrentState}");
                return Task.FromException(new AlebrijeInvalidProcessStateException(CurrentState.ToString()));
            }

            try
            {
                CurrentState = startOn;
                _logger.Debug($"Process state: {CurrentState}");
                workItem();
                CurrentState = finishOn;
                _logger.Debug($"Process state: {CurrentState}");
                return Task.CompletedTask;
            }
            catch (Exception e)
            {
                CurrentState = ProcessState.Dead;
                Console.WriteLine(e);
                _logger.Fatal(exception: e, message: "Fatal Error");
                return Task.FromException(e);
            }
        }
    }
}