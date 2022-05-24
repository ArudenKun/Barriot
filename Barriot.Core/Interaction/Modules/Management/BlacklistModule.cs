using Barriot.Interaction.Attributes;

namespace Barriot.Interaction.Modules
{
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
                text: ":white_check_mark: **Succesfully blacklisted user.** This user is now unable to interact with Barriot.");
        }

        [DoUserCheck]
        [ComponentInteraction("whitelist:*,*")]
        public async Task WhitelistAsync(ulong _, ulong targetId)
        {
            var user = await UserEntity.GetAsync(targetId);

            user.IsBlacklisted = false;

            await UpdateAsync(
                text: ":white_check_mark: **Whitelisted user.** This user is now able to interact with Barriot again.");
        }
    }
}
