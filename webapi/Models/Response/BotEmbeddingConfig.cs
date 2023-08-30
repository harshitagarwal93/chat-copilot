﻿// Copyright (c) Microsoft. All rights reserved.

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ChatCopilot.WebApi.Options;

namespace ChatCopilot.WebApi.Models.Response;

/// <summary>
/// The embedding configuration of a bot. Used in the Bot object for portability.
/// </summary>
public class BotEmbeddingConfig
{
    /// <summary>
    /// The AI service.
    /// </summary>
    [Required]
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public AIServiceOptions.AIServiceType AIService { get; set; } = AIServiceOptions.AIServiceType.AzureOpenAI;

    /// <summary>
    /// The deployment or the model id.
    /// </summary>
    public string DeploymentOrModelId { get; set; } = string.Empty;
}
