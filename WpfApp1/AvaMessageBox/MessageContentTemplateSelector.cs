using Avalonia.Controls.Templates;
using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Metadata;

namespace AvaMessageBox
{
    internal class MessageContentTemplateSelector : IDataTemplate
    {
        [Content]
        // ReSharper disable once CollectionNeverUpdated.Global
        public Dictionary<string, IDataTemplate> AvailableTemplates { get; } = new();


        public Control? Build(object? param)
        {
            if (param is MessageBoxViewModel messageBoxViewModel)
            {
                return messageBoxViewModel.MessageBoxType switch
                       {
                           MessageBoxTypes.Waiting     => AvailableTemplates["WaitingMessageTemplate"].Build(param),
                           MessageBoxTypes.TextMessage => AvailableTemplates["TextMessageTemplate"].Build(param),
                           MessageBoxTypes.Customize   => AvailableTemplates["CustomizeTemplate"].Build(param),
                           _                           => throw new ArgumentOutOfRangeException()
                       };
            }
            else
            {
                throw new InvalidCastException(nameof(param));
            }
        }

        public bool Match(object? data)
        {
            return data is MessageBoxViewModel;
        }
    }
}