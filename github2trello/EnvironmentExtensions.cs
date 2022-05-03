﻿namespace github2trello;

public static class EnvironmentExtensions
{
    public static string GetOrThrow(string id)
    {
        return Environment.GetEnvironmentVariable(id) ??
               throw new NullReferenceException($"Environment variable {id} has not been set");
    }
}