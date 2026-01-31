// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace DefineWordExtension;

internal sealed partial class DefineWordExtensionPage : DynamicListPage
{
    private readonly List<IListItem> _items;
    private static readonly HttpClient _httpClient = new();
    private CancellationTokenSource _cts = new();
    private string _currentSearch = string.Empty;

    public DefineWordExtensionPage()
    {
        Icon = new("📚");
        Title = "Define Word";
        Name = "Open";

        _items = [
            new ListItem(new NoOpCommand()) {
                Title = "Define Word",
                Subtitle = "Type a word to get its definition."
            }
        ];
    }

    public override IListItem[] GetItems() => [.. _items];

    public override async void UpdateSearchText(string oldSearch, string newSearch)
    {
        if (_currentSearch != oldSearch)
        {
            _cts.Cancel();
            _cts = new CancellationTokenSource();
            _currentSearch = string.Empty;
            _items.Clear();
            _items.Add(new ListItem(new NoOpCommand())
            {
                Title = "Define Word",
                Subtitle = "Type a word to get its definition."
            });
        }

        if (newSearch.Trim().Length == 0)
        {
            _items.Clear();
            _items.Add(new ListItem(new NoOpCommand())
            {
                Title = "Define Word",
                Subtitle = "Type a word to get its definition."
            });
            RaiseItemsChanged(0);
            return;
        }

        try
        {
            var response = await _httpClient.GetAsync($"https://api.dictionaryapi.dev/api/v2/entries/en/{newSearch}", _cts.Token);

            if (response.IsSuccessStatusCode)
            {
                string json = await response.Content.ReadAsStringAsync();
                JsonDocument doc = JsonDocument.Parse(json);
                JsonElement root = doc.RootElement;
                _items.Clear();

                foreach (JsonElement entry in root.EnumerateArray())
                {
                    string word = entry.GetProperty("word").GetString() ?? newSearch;
                    JsonElement meanings = entry.GetProperty("meanings");

                    foreach (JsonElement meaning in meanings.EnumerateArray())
                    {
                        string partOfSpeech = meaning.GetProperty("partOfSpeech").GetString() ?? "unknown";
                        foreach (JsonElement definition in meaning.GetProperty("definitions").EnumerateArray())
                        {
                            string defText = definition.GetProperty("definition").GetString() ?? "No definition available.";
                            _items.Add(new ListItem(new NoOpCommand())
                            {
                                Title = $"{word} ({partOfSpeech})",
                                Subtitle = defText
                            });
                        }
                    }
                }
                _currentSearch = newSearch;
            }
            else
            {
                _items.Clear();
                _items.Add(new ListItem(new NoOpCommand())
                {
                    Title = "No definition found."
                });
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.WriteLine($"Request error: {ex.Message}");
            _items.Clear();
            _items.Add(new ListItem(new NoOpCommand())
            {
                Title = "Error fetching definition."
            });
        }
        catch (TaskCanceledException)
        {
            return;
        }

        RaiseItemsChanged(0);
    }
}