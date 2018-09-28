﻿using System;
using System.Threading.Tasks;
using System.Timers;
using Discord;
using Discord.WebSocket;
using Dota_Geek.Modules;

namespace Dota_Geek
{
    internal class Program
    {
        private static DiscordSocketClient _client;
        private CommandHandler _handler;

        private static void Main()
        {
            try
            {
                Global.Interval = 60 * 60 * 1000;
                var timer = new Timer(Global.Interval) {Enabled = true};
                timer.Elapsed += Timer_Elapsed;

                new Program().StartAsync().GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(exception);
                Console.ResetColor();
            }
        }

        private static async void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            try
            {
                foreach (var pair in TrackedAccounts.TrackDictionary)
                {
                    var final = Dota.LastMatch($"[U:1:{pair.Key}]", out var lastHour);
                    if (pair.Value is null)
                    {
                        continue;
                    }

                    if (lastHour)
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("POST " + final.Split('\n')[0]);
                        Console.ResetColor();

                        foreach (var data in pair.Value)
                        {
                            var myChannel =
                                _client.GetGuild(data.GuildId)?.GetChannel(data.ChannelId) as ITextChannel;
                            if (myChannel is null) continue;

                            await myChannel.SendMessageAsync(final);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("SKIP " + final.Split('\n')[0]);
                        Console.ResetColor();
                    }
                }
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine(exception);
                Console.ResetColor();
            }
        }

        private async Task StartAsync()
        {
            if (string.IsNullOrEmpty(Config.Bot.Token)) return;

            _client = new DiscordSocketClient(new DiscordSocketConfig
            {
                LogLevel = LogSeverity.Verbose
            });
            
            _handler = new CommandHandler(_client); // Add LavaLinkManager
            await _handler.InitializeAsync();

            await _client.LoginAsync(TokenType.Bot, Config.Bot.Token);
            await _client.StartAsync();
            await _client.SetGameAsync("Dota 2", null, ActivityType.Watching);

            await Task.Delay(-1);
        }
    }
}