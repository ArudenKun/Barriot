using Barriot.Application.Interactions.Attributes;

namespace Barriot.Application.Interactions.Modules
{
    // TODO, rework SEND
    [IgnoreBlacklistedUsers]
    public class BlacklistModule : BarriotModuleBase
    {
        [DoUserCheck]
        [ComponentInteraction("blacklist:*,*")]
        public async Task BlacklistAsync(ulong _, ulong targetId)
        {
            var user = await UserEntity.GetAsync(targetId);

            user.IsBlacklisted = true;

            await UpdateAsync(
                format: MessageFormat.NotAllowed,
                header: "Succesfully blacklisted user.",
                context: "This user is now unable to interact with Barriot.");
        }

        [DoUserCheck]
        [ComponentInteraction("whitelist:*,*")]
        public async Task WhitelistAsync(ulong _, ulong targetId)
        {
            var user = await UserEntity.GetAsync(targetId);

            user.IsBlacklisted = false;

            await UpdateAsync(
                format: MessageFormat.Success,
                header: "Whitelisted user.",
                context: "This user is now able to interact with Barriot again.");
        }
    }
}
