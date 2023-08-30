﻿// Copyright (c) Microsoft. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using ChatCopilot.WebApi.Models.Response;
using ChatCopilot.WebApi.Storage;

namespace ChatCopilot.WebApi.Models.Storage;

/// <summary>
/// Information about a single chat message.
/// </summary>
public class ChatMessage : IStorageEntity
{
    private static readonly JsonSerializerOptions SerializerSettings = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    /// <summary>
    /// Role of the author of a chat message.
    /// </summary>
    public enum AuthorRoles
    {
        /// <summary>
        /// The current user of the chat.
        /// </summary>
        User = 0,

        /// <summary>
        /// The bot.
        /// </summary>
        Bot,

        /// <summary>
        /// The participant who is not the current user nor the bot of the chat.
        /// </summary>
        Participant
    }

    /// <summary>
    /// Type of the chat message.
    /// </summary>
    public enum ChatMessageType
    {
        /// <summary>
        /// A standard message
        /// </summary>
        Message,

        /// <summary>
        /// A message for a Plan
        /// </summary>
        Plan,

        /// <summary>
        /// An uploaded document notification
        /// </summary>
        Document,
    }

    /// <summary>
    /// Timestamp of the message.
    /// </summary>
    public DateTimeOffset Timestamp { get; set; }

    /// <summary>
    /// Id of the user who sent this message.
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// Name of the user who sent this message.
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// Id of the chat this message belongs to.
    /// </summary>
    public string ChatId { get; set; }

    /// <summary>
    /// Content of the message.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// Id of the message.
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Role of the author of the message.
    /// </summary>
    public AuthorRoles AuthorRole { get; set; }

    /// <summary>
    /// Prompt used to generate the message.
    /// Will be empty if the message is not generated by a prompt.
    /// </summary>
    public string Prompt { get; set; } = string.Empty;

    /// <summary>
    /// Type of the message.
    /// </summary>
    public ChatMessageType Type { get; set; }

    /// <summary>
    /// Counts of total token usage used to generate bot response.
    /// </summary>
    public Dictionary<string, int>? TokenUsage { get; set; }

    /// <summary>
    /// The partition key for the source.
    /// </summary>
    [JsonIgnore]
    public string Partition => this.ChatId;

    /// <summary>
    /// Create a new chat message. Timestamp is automatically generated.
    /// </summary>
    /// <param name="userId">Id of the user who sent this message</param>
    /// <param name="userName">Name of the user who sent this message</param>
    /// <param name="chatId">The chat ID that this message belongs to</param>
    /// <param name="content">The message</param>
    /// <param name="prompt">The prompt used to generate the message</param>
    /// <param name="authorRole">Role of the author</param>
    /// <param name="type">Type of the message</param>
    /// <param name="tokenUsage">Total token usages used to generate bot response</param>
    public ChatMessage(
        string userId,
        string userName,
        string chatId,
        string content,
        string? prompt = null,
        AuthorRoles authorRole = AuthorRoles.User,
        ChatMessageType type = ChatMessageType.Message,
        Dictionary<string, int>? tokenUsage = null)
    {
        this.Timestamp = DateTimeOffset.Now;
        this.UserId = userId;
        this.UserName = userName;
        this.ChatId = chatId;
        this.Content = content;
        this.Id = Guid.NewGuid().ToString();
        this.Prompt = prompt ?? string.Empty;
        this.AuthorRole = authorRole;
        this.Type = type;
        this.TokenUsage = tokenUsage;
    }

    /// <summary>
    /// Create a new chat message for the bot response.
    /// </summary>
    /// <param name="chatId">The chat ID that this message belongs to</param>
    /// <param name="content">The message</param>
    /// <param name="prompt">The prompt used to generate the message</param>
    /// <param name="tokenUsage">Total token usage of response completion</param>
    public static ChatMessage CreateBotResponseMessage(string chatId, string content, string prompt, Dictionary<string, int>? tokenUsage = null)
    {
        return new ChatMessage("bot", "bot", chatId, content, prompt, AuthorRoles.Bot, IsPlan(content) ? ChatMessageType.Plan : ChatMessageType.Message, tokenUsage);
    }

    /// <summary>
    /// Create a new chat message for a document upload.
    /// </summary>
    /// <param name="userId">The user ID that uploaded the document</param>
    /// <param name="userName">The user name that uploaded the document</param>
    /// <param name="chatId">The chat ID that this message belongs to</param>
    /// <param name="documentMessageContent">The document message content</param>
    public static ChatMessage CreateDocumentMessage(string userId, string userName, string chatId, DocumentMessageContent documentMessageContent)
    {
        return new ChatMessage(userId, userName, chatId, documentMessageContent.ToString(), string.Empty, AuthorRoles.User, ChatMessageType.Document);
    }

    /// <summary>
    /// Serialize the object to a formatted string.
    /// </summary>
    /// <returns>A formatted string</returns>
    public string ToFormattedString()
    {
        var content = this.Content;
        if (this.Type == ChatMessageType.Document)
        {
            var documentMessageContent = DocumentMessageContent.FromString(content);
            content = (documentMessageContent != null) ? documentMessageContent.ToFormattedString() : "Uploaded documents";
        }

        return $"[{this.Timestamp.ToString("G", CultureInfo.CurrentCulture)}] {this.UserName}: {content}";
    }

    /// <summary>
    /// Serialize the object to a JSON string.
    /// </summary>
    /// <returns>A serialized json string</returns>
    public override string ToString()
    {
        return JsonSerializer.Serialize(this, SerializerSettings);
    }

    /// <summary>
    /// Deserialize a JSON string to a ChatMessage object.
    /// </summary>
    /// <param name="json">A json string</param>
    /// <returns>A ChatMessage object</returns>
    public static ChatMessage? FromString(string json)
    {
        return JsonSerializer.Deserialize<ChatMessage>(json, SerializerSettings);
    }

    /// <summary>
    /// Check if the response is a Plan.
    /// This is a copy of the `isPlan` function on the frontend.
    /// </summary>
    /// <param name="response">The response from the bot.</param>
    /// <returns>True if the response represents  Plan, false otherwise.</returns>
    private static bool IsPlan(string response)
    {
        var planPrefix = "proposedPlan\":";
        return response.IndexOf(planPrefix, StringComparison.Ordinal) != -1;
    }
}
