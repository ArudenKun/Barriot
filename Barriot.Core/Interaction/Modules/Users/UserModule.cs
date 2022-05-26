﻿using Barriot.Interaction.Attributes;
using Barriot.Interaction.Services;

namespace Barriot.Interaction.Modules
{
    [IgnoreBlacklistedUsers]
    public class UserModule : BarriotModuleBase
    {
        private readonly IConfiguration _configuration;

        private readonly UserService _service;

        public UserModule(IConfiguration config, UserService service)
        {
            _configuration = config;
            _service = service;
        }

        [SlashCommand("user-info", "Gets information about a user.")]
        public async Task SlashUserInfoAsync([Summary("user", "The user to see info about.")] RestUser? user = null)
            => await UserInfoAsync(user ?? Context.User);

        [UserCommand("Info")]
        public async Task UserInfoAsync(RestUser user)
        {
            var cb = new ComponentBuilder()
                .WithButton("View avatar", $"avatar:{Context.User.Id},{user.Id}", ButtonStyle.Primary);

            var rUser = await _service.GetOneAsync(user.Id);

            if (!string.IsNullOrEmpty(rUser.BannerId))
                cb.WithButton("View banner", $"banner:{Context.User.Id},{user.Id}", ButtonStyle.Primary);

            var eb = new EmbedBuilder()
                .AddField("Joined Discord on:", user.CreatedAt);

            if (user is RestGuildUser gUser)
            {
                eb.AddField("Joined this server on:", gUser.JoinedAt);

                if (gUser.RoleIds.Any(x => x != gUser.GuildId))
                    eb.AddField("Roles:", string.Join(", ", gUser.RoleIds.Where(x => x != gUser.GuildId).Select(x => $"<@&{x}>")));

                if (Context.Member.HasVoted())
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
                format: $"bust_in_silhouette",
                header: $"Information about {user.Username}#{user.Discriminator}",
                embed: eb,
                components: cb);
        }

        [DoUserCheck]
        [ComponentInteraction("avatar:*,*")]
        public async Task AvatarAsync(ulong _, ulong targetId)
        {
            var rUser = await _service.GetOneAsync(targetId);

            var eb = new EmbedBuilder()
                .WithImageUrl(rUser.GetAvatarUrl(ImageFormat.Auto, 256));

            await UpdateAsync(
                format: "selfie",
                header: $":selfie: <@{targetId}>'s avatar:",
                embed: eb);
        }

        [DoUserCheck]
        [ComponentInteraction("banner:*,*")]
        public async Task BannerAsync(ulong _, ulong targetId)
        {
            var rUser = await _service.GetOneAsync(targetId);

            var eb = new EmbedBuilder()
                .WithImageUrl(rUser.GetBannerUrl(ImageFormat.Auto, 256));

            await UpdateAsync(
                format: "sunrise_over_mountains",
                header: $"<@{targetId}>'s banner:",
                embed: eb);
        }
    }
}
