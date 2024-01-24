using FluentAssertions;
using Unity.ParameterizedAutoFactory.Core.Caches;
using Xunit;

namespace ParameterizedAutoFactory.Core.Tests;

public class Adding_entry_to_LeastRecentlyUsedCache
{
    [Fact]
    public void Evicts_if_maximum_capacity_reached()
    {
        // Arrange
        const int capacity = 1000;
        var leastRecentlyUsedCache = new LeastRecentlyUsedCache<string, string>(capacity);

        // Act
        for (var i = 0; i < capacity * 2; ++i)
        {
            var entry = $"entry_{i}";
            leastRecentlyUsedCache.AddOrReplace(entry, entry);
        }

        // Assert
        leastRecentlyUsedCache.Count.Should().Be(capacity);
    }

    [Fact]
    public void Does_not_evict_if_maximum_capacity_not_reached()
    {
        // Arrange
        const int capacity = 1000;
        var leastRecentlyUsedCache = new LeastRecentlyUsedCache<string, string>(capacity);

        // Act
        for (var i = 0; i < capacity; ++i)
        {
            var entry = $"entry_{i}";
            leastRecentlyUsedCache.AddOrReplace(entry, entry);
        }

        // Assert
        leastRecentlyUsedCache.Count.Should().Be(capacity);
    }

    [Fact]
    public void Evicts_the_least_recently_used_entry()
    {
        // Arrange
        const string leastRecentEntry = "leastRecentEntry";
        const string mostRecentEntry = "mostRecentEntry";

        const int capacity = 1000;
        var leastRecentlyUsedCache = new LeastRecentlyUsedCache<string, string>(capacity);

        leastRecentlyUsedCache.AddOrReplace(leastRecentEntry, leastRecentEntry);
        for (var i = 0; i < capacity - 1; ++i)
        {
            var entry = $"entry_{i}";
            leastRecentlyUsedCache.AddOrReplace(entry, entry);
        }

        // Act
        leastRecentlyUsedCache.AddOrReplace(mostRecentEntry, mostRecentEntry);
        var cacheContainsLeastRecentEntry = leastRecentlyUsedCache
            .TryGetValue(leastRecentEntry, out _);

        // Assert
        cacheContainsLeastRecentEntry.Should().BeFalse();
    }
}
