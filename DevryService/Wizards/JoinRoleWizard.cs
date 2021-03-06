﻿using DevryService.Core;
using DSharpPlus.CommandsNext;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using DevryService.Core.Util;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;

namespace DevryService.Wizards
{
    public class JoinRoleWizardConfig : WizardConfig
    {
        public string[] BlacklistedRoles = new string[0];
    }

    public class JoinRoleWizard : WizardBase<JoinRoleWizardConfig>
    {
        const string AUTHOR_NAME = "Sorting Hat";
        const string AUTHOR_ICON = "https://vignette.wikia.nocookie.net/harrypotter/images/6/62/Sorting_Hat.png/revision/latest?cb=20161120072849";
        const string REACTION_EMOJI = "";
        const string DESCRIPTION = "Allows a user to join their fellow classmates";

        public override JoinRoleWizardConfig DefaultSettings()
        {
            JoinRoleWizardConfig config = new JoinRoleWizardConfig();

            config.AuthorIcon = AUTHOR_ICON;
            config.AuthorName = AUTHOR_NAME;
            config.Headline = "Let's get you settled";
            config.Description = DESCRIPTION;

            config.BlacklistedRoles = new string[]
            {
                "Moderator",
                "Admin",
                "Hardware",
                "Networking",
                "Programmer",
                "Professor",
                "Database",
                "Pollmaster",
                "See-All-Channels",
                "Motivator",
                "Server Booster",
                "Devry Test Bot",
                "Devry-Challenge-Bot",
                "Devry-Service-Bot",
                "DeVry-SortingHat",
                "announcement permissions",
                "@everyone",
                "Content Contributor",
                "Tutors",
                "DeVry Alumni",
                "Senior Moderators",
                "Junior Moderators"
            };

            config.UsesCommand = new WizardToCommandLink
            {
                DiscordCommand = "join",
                CommandConfig = DefaultCommandConfig()
            };

            return config;
        }

        public override CommandConfig DefaultCommandConfig()
        {
            return new CommandConfig
            {
                AuthorName = AUTHOR_NAME,
                Description = DESCRIPTION,
                ReactionEmoji = REACTION_EMOJI,
                IgnoreHelpWizard = false
            };
        }

        public JoinRoleWizard(CommandContext commandContext) : base(commandContext)
        {
        }
        
        protected override async Task ExecuteAsync()
        {
            var lowercased = _options.BlacklistedRoles.Select(x => x.ToLower());

            await _context.TriggerTypingAsync();
            
            // Get all the roles that users are allowed to join
            var roles = _context.Guild.Roles
                .Where(x => !lowercased.Contains(x.Value.Name.ToLower()) && !x.Value.Name.Contains("^"))
                .OrderBy(x => x.Value.Name)
                .Select(x=>x.Value)
                .ToList();
            
            // We only care about the FIRST word in the role (we follow a CATEGORY | COURSE NUMBER | DESCRIPTION format)
            List<string> courseTypes = roles.Select(x =>x.Name
                    .Trim()
                    .Replace("-", " ")
                    .Split(" ")
                    .First())
                .Distinct()
                .ToList();

            string reply = string.Empty;

            StringBuilder textBuilder = new StringBuilder();

            var embed = EmbedBuilder()
                .WithFooter($"{CANCEL_MESSAGE}\n--{_context.Member.DisplayName}")
                .WithDescription(
                    $"Which course(s) are you currently attending/teaching? Below is a list of categories. \nPlease type in the number(s) associated with the course\n")
                .FormatAsMenu(courseTypes, true);
            
            _recentMessage = await WithReply(embed.Build(), replyHandler: (context) => ReplyHandlerAction(context, ref reply), true);

            string[] parameters = reply.Replace(",", " ").Split(" ");

            Dictionary<string, List<DiscordRole>> selectedGroups = new Dictionary<string, List<DiscordRole>>();
            Dictionary<int, DiscordRole> roleMap = new Dictionary<int, DiscordRole>();

            foreach(var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    if (index < 0 || index > courseTypes.Count)
                    {
                        Console.WriteLine($"Invalid input");
                    }
                    else
                        selectedGroups.Add(courseTypes[index], roles.Where(x => x.Name.ToLower().StartsWith(courseTypes[index].ToLower())).ToList());
                }
            }

            int current = 0;
            
            foreach(var key in selectedGroups.Keys)
            {
                var temp = EmbedBuilder()
                    .WithFooter($"{CANCEL_MESSAGE}\n--{_context.Member.DisplayName}")
                    .WithDescription($"Select the number associated with the class(es) you wish to join\n\n{key}:\n")
                    .FormatAsMenu(selectedGroups[key], ref roleMap, (x)=>x.Name, current, out int actuallyAdded);

                current += actuallyAdded;
                
                _recentMessage = await SimpleReply(temp.Build(), true, true);
            }

            reply = string.Empty;
            var response = await _context.Message.GetNextMessageAsync();

            if (response.TimedOut)
            {
                await SimpleReply($"{_options.AuthorName} Wizard timed out...", false, false);
                throw new StopWizardException(_options.AuthorName);
            }

            _messages.Add(response.Result);

            reply = response.Result.Content.Trim();

            try
            {
                parameters = reply.Replace(",", " ").Split(" ");
            }
            catch
            {
                await CleanupAsync();
                return;
            }

            List<string> appliedRoles = new List<string>();

            DiscordMember member = _context.Member;
            await _context.TriggerTypingAsync();
            
            foreach (var selection in parameters)
            {
                if(int.TryParse(selection, out int index))
                {
                    if (index < 0 || index >= roleMap.Count)
                        Console.WriteLine($"Invalid Input: {index + 1}");
                    else
                    {
                        await _originalMember.GrantRoleAsync(roleMap[index]);
                        appliedRoles.Add(roleMap[index].Name);
                        await Task.Delay(500);
                    }
                }
            }

            await _context.TriggerTypingAsync();
            await CleanupAsync();

            if (appliedRoles.Count > 0)
                await SimpleReply($"Hey, {_originalMember.DisplayName}, the following roles were applied: \n{string.Join(", ", appliedRoles)}", false, false);
            else
                await SimpleReply($"Hey, {_originalMember.DisplayName}, no changes were applied", false, false);
        }
    }
}
