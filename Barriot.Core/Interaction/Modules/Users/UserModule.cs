﻿using Barriot.Interaction.Attributes;
using Barriot.Caching;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class UserModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        private readonly UserCache _users;

        public UserModule(IConfiguration config, UserCache users)
        {
            _configuration = config;
            _users = users;
        }

        [SlashCommand("user-info", "Gets information about a user.")]
        public async Task SlashUserInfoAsync([Summary("user", "The user to see info about.")] RestUser? user = null)
            => await UserInfoAsync(user ?? Context.User);

        [UserCommand("Info")]
        public async Task UserInfoAsync(RestUser user)
        {
            var cb = new ComponentBuilder()
                .WithButton("View avatar", $"avatar:{Context.User.Id},{user.Id}", ButtonStyle.Primary);

            var rUser = await _users.GetOneAsync(user.Id);

            if (!string.IsNullOrEmpty(rUser.BannerId))
                cb.WithButton("View banner", $"banner:{Context.User.Id},{user.Id}", ButtonStyle.Primary);

            var eb = new EmbedBuilder()
                .WithColor(Context.UserData.Color)
                .AddField("Joined Discord on:", user.CreatedAt);

            if (user is RestGuildUser gUser)
            {
                eb.AddField("Joined this server on:", gUser.JoinedAt);

                if (gUser.RoleIds.Any(x => x != gUser.GuildId))
                    eb.AddField("Roles:", string.Join(", ", gUser.RoleIds.Where(x => x != gUser.GuildId).Select(x => $"<@&{x}>")));

                if (Context.UserData.HasVoted())
                {
                    if (gUser.PremiumSince is not null)
                        eb.AddField("Boosting since:", gUser.PremiumSince);

                    if (gUser.PublicFlags.HasValue)
                        eb.AddField("User flags:", string.Join(", ", Enum.GetValues<UserProperties>()
                            .Where(x => gUser.PublicFlags.Value.HasFlag(x))
                            .Select(x => $"` {x} `")
                            .Where(x => !x.Contains("None"))));

                    if (gUser.TimedOutUntil is not null)
                        eb.AddField("Timed out until:", $"{gUser.TimedOutUntil}");
                }
                else
                {
                    eb.WithFooter("Get more information about this user and others by voting!");
                    cb.WithButton("Vote now!", style: ButtonStyle.Link, url: _configuration["Domain"] + "vote");
                }
            }
            await RespondAsync(
                text: $":bust_in_silhouette: **Information about {user.Username}#{user.Discriminator}**",
                embed: eb.Build(),
                components: cb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("avatar:*,*")]
        public async Task AvatarAsync(ulong _, ulong targetId)
        {
            var rUser = await _users.GetOneAsync(targetId);

            var eb = new EmbedBuilder()
                .WithColor(Context.UserData.Color)
                .WithImageUrl(rUser.GetAvatarUrl(ImageFormat.Auto, 256));

            await RespondAsync(
                text: $":selfie: **<@{targetId}>'s avatar:**",
                embed: eb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }

        [DoUserCheck]
        [ComponentInteraction("banner:*,*")]
        public async Task BannerAsync(ulong _, ulong targetId)
        {
            var rUser = await _users.GetOneAsync(targetId);

            var eb = new EmbedBuilder()
                .WithColor(Context.UserData.Color)
                .WithImageUrl(rUser.GetBannerUrl(ImageFormat.Auto, 256));

            await RespondAsync(
                text: $":sunrise_over_mountains: **<@{targetId}>'s banner:**",
                embed: eb.Build(),
                ephemeral: Context.UserData.DoEphemeral);
        }
    }
}