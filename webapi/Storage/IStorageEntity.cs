// Copyright (c) Microsoft. All rights reserved.

namespace ChatCopilot.WebApi.Storage;

public interface IStorageEntity
{
    string Id { get; set; }

    string Partition { get; }
}
